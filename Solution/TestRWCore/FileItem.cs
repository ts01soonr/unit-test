using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
//using System.Windows;

namespace TestRWCore
{
    public class FileItem
    {
      

        public   String fileName { get; set; }
        public long fileSize { get; set; }
        public String ext { get; set; }

        public double average { get; set; }
        public double entropy { get; set; }
        public double lowentropy { get; set; }
        public double highentropy { get; set; }
        public double averageentropy { get; set; }

        public String entropyRange { get; set; }
        private int entropycnt = 0;

        public double stdDeviation { get; set; }
        private double stdDevMin = 0;
        private double stdDevMax = 0;


        public double highLowProp { get; set; }
        public double piAprox { get; set; }
        public String encrypted { get; set; }

        public double headeraverage { get; set; }
        public double headerhighProp { get; set; }

        public String fileType { get; set; }
        public String filetypeResult { get; set; }

        public Boolean isEncryptedHeader { get; set; }
        public Boolean isEncryptedFile { get; set; }

        private long readcnt = 0;  // Aggregated count
        private long incircle = 0; // Aggregated value

        private long readcnt_header = 0;  // Aggregated count

        private int[] counts = new int[256]; // count spektrum, for all read bytes.
        private int[] count_header = new int[256]; // count spektrum, for header.

        private long lowentropyOffset = 0;

        public FileItem(String fileName, long fileSize, String extension)
            {
                this.fileName = fileName;
                this.fileSize = fileSize;

                if (extension.Length > 0)
                {
                    ext = extension.ToLower().Substring(1);
                }
                else
                    ext = "";

                lowentropy  = 8;
                highentropy = 0;
                fileType = "";
                headeraverage = 0;
                headerhighProp = 0;
                 isEncryptedHeader = false;
            readcnt = 0;
            filetypeResult = "";

            for (int i = 0; i < 256; i++)
            {
                counts[i] = 0;
                count_header[i] = 0;
            }

        }


        const double averageMin= 127;
        const double averageMax = 128;
        const double entropyMin = 6.5;

        const double averageMin_Small = 126.5;
        const double averageMax_Small = 128.5;
 


        const double fractMax = 1.9;
        const double piAproxMax = 0.1;

        const double fractMaxSmall = 1.9;
        const double piAproxMax_Small = 0.1;



        public const long maxReadSize       = 4* 64 * 1024 ;//* 1024*10 ;
        public const long bufferReadSize    = 64* 1024; //16*4*256;
        public const int HeaderSize  = 8 * 256; // assume this header size for analysis. If skipHeader is true, this header does not contribute.

        public void computeHeader(int cnt, byte[] buf)
        {
            readcnt_header += cnt;

           // incircle_header += AggrMonteCarloPiApproximation(cnt, buf);

            for (int i = 0; i < cnt; i++)
            {
                byte b = buf[i];
                count_header[b]++;
            }

            headeraverage = computeAverage(readcnt_header, count_header);


            fileType = HeaderType(buf);
            if (fileType.Length > 0)
            {
                if (!FileItem.checkExtension(fileType, ext))
                {
                    filetypeResult = "NO";
                }
            }




            headerhighProp = computeHighLowProp(cnt, count_header, false);
  
    
        }

        /* for each partial read - aggregates counts */
        public Boolean aggrReadFile(int cnt, byte[] buf)
        {
            if (readcnt >= maxReadSize)
                return false;
            if (readcnt + cnt > maxReadSize)
                cnt = (int) (maxReadSize - readcnt);

            if (readcnt == 0 && readcnt_header == 0)
            {   // Reading header
                int readHdr = HeaderSize <= cnt ? HeaderSize : cnt;
                computeHeader(readHdr, buf);
                int remCnt = cnt - readHdr;

                if (remCnt > 255) { // copy remaining buffer 
                    byte[] rembuffer = new byte[remCnt];
                    for (int i = 0; i < remCnt; i++)
                        rembuffer[i] = buf[i + HeaderSize];
                    return aggrReadFile(remCnt, rembuffer);
                }
                return false;
            }

  
            incircle += AggrMonteCarloPiApproximation(cnt, buf);
           

            for (int i = 0; i < cnt; i++)
            {
                byte b = buf[i];
                counts[b]++;
            }


            // Compute entropy for subrange of 256 chars
            for (int j =0; j < cnt/256; j++)
            {
                if (j * 256 + 255 > cnt)
                    break;
                int[] Avcount = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    byte b = buf[j*256 + i];
                    Avcount[b]++;
                }
                Double tmp = FileItem.computeEntropy(256, Avcount);
                if (tmp < lowentropy && tmp > 0)
                {
                    lowentropy = tmp;
                    lowentropyOffset = readcnt + j*256;
                }
                if (tmp > highentropy)
                    highentropy = tmp;

                averageentropy += tmp;
                entropycnt++;

                double stdtmp = computeStdDev(256, Avcount);
                if (stdtmp < stdDevMin || stdDevMin == 0)
                    stdDevMin = stdtmp;
                if (stdtmp > stdDevMax)
                    stdDevMax = stdtmp;

            }

         


            readcnt += cnt;
            return true;
        }


        public Boolean computeFile()
        {
            Boolean res = true;
            entropy = Math.Round(computeEntropy(),2);
            average = computeAverage();
            piAprox = computepiAprox();
            highLowProp  = computeHighLowProp();
            stdDeviation = computecomputeStdDev();
            entropyRange = "[ " + Math.Round(lowentropy, 2) + " : " + Math.Round(highentropy, 2) + " ] - "+ lowentropyOffset;
            averageentropy = Math.Round(entropycnt > 0 ? averageentropy / entropycnt : 0, 1);

            isEncryptedFile = setEncrypted();

            return res;
        }

        private double computeAverage()
        {
            return computeAverage(readcnt, counts);
        }

        public static double computeAverage(long cnt, int[] counts)
        {
            double a = 0;
            for (int i = 1; i < 256; i++)
            {
                a += i * counts[i];

            }
            return Math.Round(a / cnt, 2);
        }


        // Aggregate contribution
        // buf includes <count> values in the ramge 0..255
        // returns number inside circle, of count posible.
        public static long AggrMonteCarloPiApproximation(int cnt, byte[] buf)
        {
            long incircle = 0;
            double x, y = 0;
            int i = 0;

            while (i < cnt-1)
            {

                x = (double)(buf[i++]) / (double)255;
                y = (double)(buf[i++]) / (double)255;

                if ((Math.Sqrt(x * x + y * y) <= (double) 1.0))
                    incircle++;

            }
            return incircle;
        }

        private double computepiAprox()
        {
            return computepiAprox(readcnt, incircle);
        }

        public static double computepiAprox(long total, long inCircle)
        {
            total = total / 2; // 2 values used for each point
            double piApproximation = 4 * ((double)inCircle / (double)total);

            double percent = (piApproximation - 3.14159265358979323846D) * 100 / (double)3.14159265358979323846D;
            if (percent < 0)
                percent = -percent;
            return Math.Round(percent, 3);
        }
        private double computeEntropy()
        {
            return computeEntropy(readcnt, counts);
        }

        public static double computeEntropy(long totalCount, int[] counts)
        {
            double e = 0;

            double constLog = Math.Log(totalCount, 2);

            for (int i = 0; i < 256; i++)
            {
                if (counts[i] > 0)
                {
                    double p = ((double)counts[i] / (double)totalCount);
                    double eTmp = p * Math.Log(p, 2);

                    //double tmp = (Math.Log(counts[i], 2) - constLog) * counts[i] / fileSize;

                    e += eTmp;
                }
            }

            e = -e;

            return e;
        }

        private double computeHighLowProp()
        {
            return computeHighLowProp(readcnt, counts, true);

        }
        public static double computeHighLowProp(long totalcount, int[] counts, Boolean both)
        {
            //double hl;
            double h = 0;
            double l = 1;
            int hchar = 0;
            // double norm = 1 / 256;

            for (int i = 0; i < 256; i++)
            {
                double p = ((double)counts[i] / (double)totalcount);
                if (p > h)
                {
                    h = p;
                    hchar = i;
                }
                if (p < l)
                    l = p;

            }


            double fract = both ? (l > 0 ? h / l: 10000) : h*256;


            if (fract > 2 && !both)
            {
                // if (hchar == 0 || hchar == 32 /*|| hchar == 48*/)
                 //     return -999;  
            }
            return Math.Round(fract,1);
        }

        private double computecomputeStdDev()
        {
            return computeStdDev(readcnt, counts); 
        }

        public double computeStdDev(long fileSize, int[] counts)
        {
            double d = 0;
            double mp = 1 / 256; // Mean propability
            for (int i = 0; i < 256; i++)
            {
                double p = ((double)counts[i] / (double)fileSize);
                d += (p - mp) * (p - mp);

            }

            d = Math.Sqrt(d / 256)* 100000;

            return Math.Round(d, 2);
        }

        private Boolean isEncrypted()
        {

            if (averageentropy >= 7.2 && entropy == 8 && average >= 126.5 && average <= 128.5 && lowentropy >= 6.5)
                return true;

            if (piAprox < 0.1 && average >= averageMin && average <= averageMax && highLowProp < 2.0)
                return true;


            if (fileSize < HeaderSize + 255)
                return false;

            if (fileSize < 8 * 1024 + 128)
                return isEncryptedSmall();


            if (fileSize < 32 * 1024 + 128)
                return isEncryptedMedium();

            return isEncryptedLarge();

        }


        private Boolean isEncryptedLarge()
        {

            if (averageentropy >= 7.1 && lowentropy >= 6.95 && average >= 126 && average <= 129 && highLowProp < 2)
                return true;

            //if (averageentropy >= 7.2 && entropy == 8 && average >= 126.5 && average <= 128.5)
            //   return true;


           // if (piAproxMax > 1 && fileSize > 100 * 1024)
            //    return false;

            if (lowentropy < 6.50 || highLowProp > 2.0)
                return false;

            if (averageentropy >= 7.2 && lowentropy >= 7 && entropy >= 7.99  )
                return true;

        
            if (highLowProp < fractMax && piAprox < piAproxMax)
                return true;


                  // files > 32k
                if (average >= averageMin && average <= averageMax)
                {
                    if (highLowProp < fractMax || piAprox < piAproxMax)
                        return true;

                    if (average >= 127.4 && average <= 127.6 && (highLowProp < 2 || piAprox < 0.6)) // small files 
                        return true;
                }
                else
                {
                    if (highLowProp < fractMax && piAprox < piAproxMax)
                        return true;
                }
            

            return false;
        }

        private Boolean isEncryptedMedium()
        {
            if (averageentropy >= 7.1 && lowentropy >= 6.95 && average >= 126 && average <= 129 && highLowProp < 4)
                return true;


            if (averageentropy >= 7.2 && lowentropy > 7 && (entropy >= 7.99 || fileSize < 16 * 1024))
                return true;

            if (lowentropy < 6.9 || highLowProp > 3.0)
                return false;
           

        


     
                // files < 32k
                if (average >= averageMin_Small && average <= averageMax_Small)
                {
                    if (highLowProp <= 2.5 || piAprox < 0.8)
                        return true;

                    if (average >= 127 && average <= 128 && (highLowProp < 3 || piAprox < 1))
                        return true;

                    if (highLowProp < fractMax && piAprox < piAproxMax)
                        return true;

                    if (lowentropy > 6.95 && average >= 126 && average <= 129 && highLowProp < 3)
                        return true;
                }

 
            return false;
        }


        private Boolean isEncryptedSmall()
        {

            if (averageentropy >= 7.1 && lowentropy >= 6.95 && average >= 125 && average <= 130 )
                return true;


            if (lowentropy < entropyMin || ( average <= 124 && average >= 132 ))
                return false;

            if (averageentropy >= 7.2 && lowentropy > 7)
                return true;

            if (averageentropy >= 7.1 && lowentropy > 7 && entropy > 7.75 && average > 122 && average < 132)
                return true;





            if (highLowProp < fractMax && piAprox < piAproxMax)
                return true;



            // files < 32k
            if (average >= averageMin_Small && average <= averageMax_Small)
            {
                if (highLowProp <= 2.5 || piAprox < 0.8)
                    return true;

                if (average >= 127 && average <= 128 && (highLowProp < 3 || piAprox < 1))
                    return true;

                if (highLowProp < fractMax && piAprox < piAproxMax)
                    return true;

                if (lowentropy > 6.95 && average >= 126 && average <= 129 && highLowProp < 3)
                    return true;
            }
            return false;
        }


        // Determins if file is encrypted, based on computed values.
        private Boolean setEncrypted()
        {
            Boolean res = isEncrypted();
            encrypted = res ? "YES": "" ;
            return res;
        }

        public class filetype
        {
           public  byte[] sig;
            public String type;

            public filetype(byte[] s, String t)
            {
                sig = s;
                type = t;
            }
        }

        public class fileext
        {
      
            public String type;
            public String[] extlist;

            public fileext(String t, String[] e)
            {
                extlist = e;
                type = t;
            }

            public Boolean checkext(String ext)
            {
                foreach(String e in extlist)
                {
                    if (ext.ToLower().Equals(e.ToLower()))
                        return true;
                }
                return false;
            }
        }

        static fileext[] extlist =
        {
            new fileext( "ZIP-MS", new String[] { "docx", "xlsx", "pptx"}),
            new fileext( "ZIP-E", new String[] {"zip", "jar", "odt", "ods", "ott", "odp","docx", "xlsx", "pptx", "vsdx", "apk", "cpt", "xpi", "xps", "xpt" }),
            new fileext( "ZIP", new String[] {"zip", "zipx","jar", "odt", "ods", "ott", "odp","docx", "xlsx", "pptx", "vsdx", "apk", "cpt", "xpi", "xps", "xpt" }),
            new fileext( "PNG", new String[] {"png" }),
            new fileext( "class", new String[] {"class" }),
            new fileext( "ICO", new String[] {"ico" }),
            new fileext( "GIF", new String[] {"gif"}),
            new fileext( "TIF", new String[] {"tif", "tiff" }),
            new fileext( "JPEG", new String[] {"jpeg", "jpg", "jfif", "jpe", "spiff" }),
            new fileext( "EXE", new String[] {"exe"}),
            new fileext( "JP2", new String[] {"jp2"  }),
            new fileext( "LZIP", new String[] {"lz"}),
            new fileext( "RAR", new String[] {"rar"}),
            new fileext( "PDF", new String[] {"pdf" }),
            new fileext( "MP3", new String[] {"mp3" }),
            new fileext( "BMP", new String[] {"bmp" }),
            new fileext( "FLAC", new String[] {"flac" }),
            new fileext( "MSOffice", new String[] {"doc", "xls", "ppt", "msg", "dot", "pps", "xla", "wiz", "docx", "xlsx", "pptx" }), // docx if encrypted
            new fileext( "DEX", new String[] {"dex" }),
            new fileext( "appleworks-6", new String[] {"cwk" }), // ?? CWK – ClarisWorks-AppleWorks document
            new fileext( "XAR", new String[] {"xar" }), //Xara – Drawing
            new fileext( "GZIP", new String[] {"gz" }), // 
            new fileext( "7z", new String[] {"7z" }), 
            new fileext( "RTF", new String[] {"rtf" }), //Rich Text document
            new fileext( "PSD", new String[] {"psd" , "pdd"}), //Adobe Photoshop Drawing
            new fileext( "WOFF", new String[] {"woff" , "woff2"}),
            new fileext( "XML", new String[] {"xml"}),
            new fileext( "SWF", new String[] {"swf"}), // same as .CWS ?
            new fileext( "DWG", new String[] {"dwg"}), // Generic AutoCAD drawing

            new fileext( "CAB", new String[] {"cab"}), // Microsoft Cabinet file
            new fileext( "DOC", new String[] {"doc"}), // Microsoft word
             new fileext( "TAR", new String[] {"tar"}),  
            
            //-----Added recently           
             new fileext( "MP4", new String[] {"mp4"}),
             new fileext( "MOV", new String[] {"mov"}),
             new fileext( "WMF", new String[] {"wmf"}),
             new fileext( "AI", new String[] {"ai"}),
             new fileext( "MIDI", new String[] {"mid"}),
             new fileext( "AVI", new String[] {"avi"}),
             new fileext( "FLV", new String[] {"flv"}),
             new fileext( "WMVA", new String[] {"wmv", "wma"}),
             new fileext( "MSI", new String[] {"msi"}),
             new fileext( "OBJ", new String[] {"obj"}),
             new fileext( "DLL", new String[] {"dll"}),
             new fileext( "HLP", new String[] {"hlp"}),
             new fileext( "WMWARE", new String[] {"wmdk"}),
             new fileext( "OUTLOOK", new String[] {"pst"}),
             new fileext( "VISIO", new String[] {"vsd"}),
             new fileext( "MDB", new String[] {"mdb"}),
             new fileext( "PS", new String[] {"ps"}),
             new fileext( "MSG", new String[] {"msg"}),

        };


        static filetype[] list =
        {
           new filetype (new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x01, 0x00, 0x63, 0x00, 0x00, 0x00, 0x00}, "ZIP-E" ),  // zip, encrypted
           new filetype (new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06}, "ZIP-MS" ),  // ms office, pkzip
           new filetype (new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x08, 0x00, 0x08, 0x00}, "ZIP" ),  // jar, pkzip
           new filetype (new byte[] { 0x50, 0x4B, 0x03, 0x04 }, "ZIP" ),  // jar, pkzip  
           new filetype (new byte[] { 0x50, 0x4B, 0x05, 0x06 }, "ZIP" ),  // pkzip
           new filetype (new byte[] { 0x50, 0x4B, 0x07, 0x08 }, "ZIP" ),  // pkzip
           new filetype (new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 }, "ZIP" ),  // pksfx
           new filetype (new byte[] { 0x50, 0x4B, 0x4c, 0x49, 0x54, 0x45}, "ZIP" ), // pklite
           new filetype (new byte[] { 0x57, 0x69, 0x6e, 0x5a, 0x69, 0x70}, "ZIP" ), // winzip

 
           new filetype (new byte[] { 0xef, 0xbb, 0xbf}, "UTF8" ),
           new filetype (new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a}, "PNG" ),
           new filetype (new byte[] { 0xca, 0xfe, 0xba, 0xbe }, "class" ),

           new filetype (new byte[] { 0x00, 0x00, 0x01, 0x00 }, "ICO" ),
           new filetype (new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, "GIF" ),
           new filetype (new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, "GIF" ),
    
           new filetype (new byte[] { 0x49, 0x49, 0x2a, 0x00 }, "TIF" ),
           new filetype (new byte[] { 0x4d, 0x4d, 0x00, 0x2a }, "TIF" ), // big endian

           new filetype (new byte[] { 0xff, 0xd8, 0xff, 0xfb }, "JPEG" ),
           new filetype (new byte[] { 0xff, 0xd8, 0xff, 0xe0 }, "JPEG" ), // + nn nn 4a 46 49 46 00 01
           new filetype (new byte[] { 0xff, 0xd8, 0xff, 0xe1 }, "JPEG" ), // + nn nn 45 78 69 66 00
           new filetype (new byte[] { 0xff, 0xd8, 0xff, 0xe2 }, "JPEG" ), //  
           new filetype (new byte[] { 0xff, 0xd8, 0xff, 0xe8 }, "JPEG" ), //SPIFF,  + nn nn 53 50 49 46 46 00 

           new filetype (new byte[] { 0x00,0x00,0x00,0x0c,0x6a, 0x50,0x20,0x20}, "JP2" ),


           new filetype (new byte[] { 0x4c, 0x5a, 0x49, 0x50 }, "LZIP" ),
           new filetype (new byte[] { 0x4d, 0x5a}, "EXE" ),
           new filetype (new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x00 }, "RAR" ), // v4x
           new filetype (new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x01, 0x00 }, "RAR" ), // v5
           new filetype (new byte[] { 0x25, 0x50, 0x44, 0x46 }, "PDF" ),
           new filetype (new byte[] { 0xFF, 0xFB }, "MP3" ),
           new filetype (new byte[] { 0x49, 0x44, 0x33 }, "MP3" ),
           new filetype (new byte[] { 0x42, 0x4d }, "BMP" ),
           new filetype (new byte[] { 0x66, 0x4c, 0x61, 0x43 }, "FLAC" ),

           new filetype (new byte[] { 0xd0,0xcf,0x11,0xe0,0xa1,0xb1,0x1a,0xe1 }, "MSOffice" ), // doc, xls, ppt, msg

           new filetype (new byte[] { 0x64, 0x65, 0x78, 0x0a, 0x30, 0x33, 0x35, 0x00 }, "DEX" ), // Dalvik executable
           new filetype (new byte[] { 0x06,0x07,0xe1,0x00,0x42,0x4f,0x42,0x4f,0x06,0x07,0xe1,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01 }, "appleworks-6" ),
           new filetype (new byte[] { 0x78,0x61,0x72,0x21}, "XAR" ),

           new filetype (new byte[] { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c}, "7z" ),
           new filetype (new byte[] { 0x1f, 0x8b}, "GZIP" ),

           new filetype (new byte[] { 0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31}, "RTF" ),
           new filetype (new byte[] {0x38,0x42,0x50,0x53}, "PSD" ),
           new filetype (new byte[] {0x77,0x4f,0x46,0x46}, "WOFF" ),  //	WOFF File Format 1.0
           new filetype (new byte[] {0x77,0x4f,0x46,0x32}, "WOFF" ), //   WOFF File Format 2.0
           new filetype (new byte[] {0x3c,0x3f,0x78,0x6d,0x6c,0x20}, "XML" ),
           new filetype (new byte[] {0x43,0x57,0x53}, "SWF" ),  //Small Web Format. Flash
           new filetype (new byte[] {0x46,0x57,0x53}, "SWF" ),  //Small Web Format. Flash
           new filetype (new byte[] {0x41,0x43,0x31,0x30}, "DWG" ),  // Autocad

           new filetype (new byte[] {0x4d,0x53,0x43,0x46}, "CAB" ),
           new filetype (new byte[] {0xdb,0xa5, 0x2d, 0x00}, "DOC" ), // word 2.0
           new filetype (new byte[] {0x75, 0x73, 0x74, 0x61, 0x72}, "TAR" ), 
       
                  // Added recently    
            new filetype (new byte[] {0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34,0x32}, "MP4" ),
            new filetype (new byte[] {0x6D, 0x6F, 0x6F, 0x76}, "MOV" ), // MOV Video file
            new filetype (new byte[] {0xD7, 0xCD, 0xC6, 0x9A}, "WMF" ), // Window media File
            new filetype (new byte[] {0x25, 0x50, 0x44, 0x46}, "AI" ), // Adobe Illustrator
            new filetype (new byte[] {0x4D, 0x54, 0x68, 0x64}, "MIDI" ), // MIDI file
            new filetype (new byte[] {0x52, 0x49, 0x46, 0x46}, "AVI" ), // video file
            new filetype (new byte[] {0x46, 0x4C, 0x56}, "FLV" ), // Flash video
            new filetype (new byte[] {0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF}, "WMVA" ), // Windows video file, Windows adio file
            new filetype (new byte[] {0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1}, "MSI" ), // Microsoft installer
            new filetype (new byte[] {0x4C, 0x01}, "OBJ" ), // Object code file
            new filetype (new byte[] {0x4D, 0x5A}, "DLL" ), // Dynamic Library
            new filetype (new byte[] {0x3F, 0x5F, 0x03, 0x00}, "HLP" ), // Help file
            new filetype (new byte[] {0x4B, 0x44, 0x4D, 0x56}, "WMWARE" ), // WMWare Disk file
            new filetype (new byte[] {0x21, 0x42, 0x44, 0x4E, 0x42}, "OUTLOOK" ), // Outlook Post Office file
            new filetype (new byte[] {0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1}, "VISIO" ), // Visio document
            new filetype (new byte[] {0x53, 0x74, 0x61, 0x6E, 0x64, 0x61, 0x72, 0x64, 0x20, 0x4A, 0x65, 0x74}, "MDB" ), // Microsoft Database
            new filetype (new byte[] {0x25, 0x21}, "PS" ), // Postscript File
            new filetype (new byte[] {0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1}, "MSG" ), // Outlook message file
            new filetype (new byte[] { 0xff, 0xd8, 0xff, 0xe3 }, "JPEG" ), //  Samsung D500 jpeg

           //new filetype (new byte[] { 0x50, 0x4B, 0x03, 0x04 }, "ZIP" ),
   
                };

    //byte[] Zip = { 0x50, 0x4B, 0x03, 0x04 };

        public static Boolean checkType(byte[] t, byte[] h)
        {
            for (int i = 0; i < t.Length; i++)
                if (t[i] != h[i])
                    return false;
            return true;

        }

        public static Boolean checkExtension(String type, String extension) { 
       
            foreach(fileext e in extlist)
            {
                if ( e.type.Equals(type)) {
                    return e.checkext(extension);
                }
             }
            return false;
        }

        public static  String HeaderType( byte[] header)
        {
           foreach(filetype f in list)
            {
                if (checkType(f.sig, header))
                    return f.type;
            }

            return "";
        }


        //Added from fileentropy.java
        /** check if extension is a known extension (used when no filetype is available)
        * @param extension
        * @return true
        */
        public static Boolean knownExtension(String extension)
        {
            foreach (fileext e in extlist)
            {
                if (e.checkext(extension))
                    return true;
            }
            return false;
        }

        static String[] cornercaselist =
        {
             "7z","PNG","MP4","MOV","PDF"
         };

        public static Boolean knownCornercaseType(String type)
        {
            if (type.Length == 0)
                return false;

            foreach (String s in cornercaselist)
            {
                if (type.Equals(s))
                    return true;
            }

            return false;
        }



    }
}