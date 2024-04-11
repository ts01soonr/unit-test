﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using RWDetectCore;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TestRWCore
{
    class Program
    {
        public static RwDetectAPI rw;
        public static int OneSec = 1000;
        public static String[] watchPaths = { "C:\\AutoTest\\data" };
        public static String[] excl = { "$.td", "$WinREAgent", "log" };
        public static int delaySec = 90;
        public static Boolean debug = true; // extra log info
        public static String kill = "0";
        static RwDetectAPI.OStype getOStype()
        {
            string windir = Environment.GetEnvironmentVariable("windir");
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
            {
                Console.WriteLine("Current OS: Windows");
                return RwDetectAPI.OStype.Windows;
            }
            else if (File.Exists(@"/proc/sys/kernel/ostype"))
            {
                string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Current OS: Linux");
                    return RwDetectAPI.OStype.Linux;
                }
                
            }
            else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
            {
                Console.WriteLine("Current OS: Macx");
                return RwDetectAPI.OStype.OSX;
            }
            Console.WriteLine("Current OS: Unknown");
            return RwDetectAPI.OStype.Windows; //default

        }
        static Boolean isEncrypted(string filePath)
        {
            FileInfo tdi = new FileInfo(filePath);
            
            if ( /*p.Length  + tdi.Name.Length*/ tdi.FullName.Length + tdi.Name.Length >= 260) // too long pathname Gives exception!!!!!! double check for this
                return false;
            String ext = tdi.Extension == null ? "" : tdi.Extension;
            FileItem fi = new FileItem(tdi.Name, tdi.Length, ext);
            FileStream net = tdi.OpenRead();
            long fileSize = tdi.Length;
            long toRead = tdi.Length > FileItem.maxReadSize ? FileItem.maxReadSize : tdi.Length;

            byte[] buffer = new byte[FileItem.bufferReadSize];
            Int32 nBytesRead = 0; // number of bytes read on each cycle

            try
            {
                do
                {

                    /* READ A CHUNK */
                    nBytesRead = net.Read(buffer, 0, (int)FileItem.bufferReadSize); // await net.ReadAsync(buffer, offset, buffer.Length);
                    if (nBytesRead < FileItem.bufferReadSize && nBytesRead > 4 * 1024)
                    {
                        //This is the end of the file
                        nBytesRead -= 1024;//TEMP, 12-oct-2018
                    }

                }
                while (nBytesRead > 0 && fi.aggrReadFile(nBytesRead, buffer));
            }
            catch (IOException ex)
            {
                Console.WriteLine("Exception happen here");
            }

            finally
            {
                net.Close();
            }

            fi.computeFile();
            //Console.WriteLine(fi.isEncryptedFile);
            //Console.WriteLine(fi.isEncryptedHeader);
            return fi.isEncryptedFile;

        }
        static void getToken()
        {
            string fpath = "rmm.properties";
            if (!File.Exists("rmm.properties")) fpath = "../rmm.properties";
            if (File.Exists(fpath))
            {
                Properties config = new Properties(fpath);
                string apiUrl = config.get("apiUrl");
                string apiKey = config.get("apiKey");
                string apiSecretKey = config.get("apiSecretKey");
                // Request token
                var client = new HttpClient();
                var byteArray = new UTF8Encoding().GetBytes("public-client:public");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var form = new FormUrlEncodedContent(new Dictionary<string, string> { { "username", apiKey }, { "password", apiSecretKey }, { "grant_type", "password" } });
                var tokenMessage = client.PostAsync(apiUrl + "/auth/oauth/token", form);
                var result = tokenMessage.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }
            else          
                Console.WriteLine(fpath+" - missing");
        }
        static void AemApi(string apiMethod, string apiRequest)
        {
            if (apiRequest.Equals("token")) { getToken();  return; }
            string apiUrl = "[API URL]";
            string apiKey = "[API Key]"; 
            string apiSecretKey = "[API Secret Key]";
            string apiRequestBody = "[API Request Body]";   
            string apiAccessToken = "[API Secret Token]";
            string fpath = "rmm.properties";
            if (!File.Exists("rmm.properties")) fpath = "../rmm.properties";
            Properties config = new Properties(fpath);                
            apiUrl = config.get("apiUrl");
            apiKey = config.get("apiKey");
            apiSecretKey = config.get("apiSecretKey");
            apiAccessToken = config.get("apiAccessToken");
            
            var client = new HttpClient();
            HttpMethod CreateHttpMethod(string method)
            {
                switch (method.ToUpper())
                {
                    case "POST":
                        return HttpMethod.Post;

                    case "PUT":
                        return HttpMethod.Put;

                    case "DELETE":
                        return HttpMethod.Delete;

                    default:
                        return HttpMethod.Get;
                }
            }

            var requestType = CreateHttpMethod(apiMethod);

            // Build request
            var request = new HttpRequestMessage(requestType, apiUrl + "/api" + apiRequest);
            request.Headers.Add("authorization", $"Bearer {apiAccessToken}");

            // Add request body if present
            if (!string.IsNullOrEmpty(apiRequestBody))
            {
                request.Content = new StringContent(apiRequestBody, Encoding.UTF8, "application/json");
            }

            // Send request
            var requestMessage = client.SendAsync(request);
            requestMessage.Wait();
            var response = requestMessage.Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(response);
        }
        static void Playme()
        {
            //new Ptime("320748").CT();
            //String fpath = "rmm.properties";
            //Properties config = new Properties(fpath);
            //Console.WriteLine(config.get("apiAccessToken"));
            //getToken();
            //AemApi("GET" , "/v2/device/43178f1e-3e11-8488-35b0-8d705a3c8a58/alerts/open");

            //isEncrypted("C:/test2/density-test-encrypt/1238.copy");
            //isEncrypted("C:/AutoTest/soonr.jar");
            //args = "ise!C:/test2/density-test-encrypt/1238.copy".Split("!");
            //args = "C:\\AutoTest\\data!tmp!90!true".Split("!");
            //---check file----

        }

        static void LaunchRWCore(string[] args)
        {
            //---overwrite setting-----
            var os = Environment.OSVersion;
            Console.WriteLine("Current OS Information 1:");
            Console.WriteLine("Platform: {0:G}", os.Platform);
            Console.WriteLine("Version String: {0}", os.VersionString);
            String ostring = os.VersionString;
            if (!ostring.Contains("Windows"))
            {   //Overwrite for mac
                watchPaths = "~/AutoTest".Split("!");
                excl[1] = "Library";
            }
            if (args.Length > 0) //overwrite monitor path, split by "!"
                watchPaths = args[0].Split("!");
            if (args.Length > 1) //overwrite exclude path
                excl[2] = args[1];
            if (args.Length > 2) //overwrite delay
                delaySec = int.Parse(args[2]);
            if (args.Length > 3) //overwrite debug
                debug = Boolean.Parse(args[3]);
            if (args.Length > 4) //overwrite kill
                kill = args[4];
            //Monitor start
            RwDetectAPI.setValue("SamplingPeriod", delaySec * 1000);
            rw = new RwDetectAPI(debug, getOStype());
            ILogger consLogger = new ConsoleLogger();
            rw.setExternalLogger(consLogger);
            Console.WriteLine("RMM RansomWare Monitor Start");
            Console.WriteLine("Set SamplingPeriod = " + delaySec);
            Console.WriteLine("Set Debug = " + debug);

            rw.setWatchPaths(watchPaths);
            rw.PauseFileWatcher();
            rw.setExcludedFolders(excl);
            rw.ResumeFileWatcher();
            if (File.Exists(@"plist.txt")) File.Delete(@"plist.txt");
            if (File.Exists(@"alert.txt")) File.Delete(@"alert.txt");
            while (true)
            {
                Thread.Sleep(2 * OneSec);
                RwAlert.AlertState astate = rw.RwDetectAlert();
                if (astate == RwAlert.AlertState.FullAlert)
                {
                    File.WriteAllText(@"alert.txt", DateTime.Now.ToString("HH:mm:ss.fff")+ "DETECTION!!");
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") +" :GOT_ALERT:");
                    Thread.Sleep(10 * OneSec);
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " :AFTER 10s DELAY: ");

                    RwAlert ra = rw.getAlertInfo();

                    Console.WriteLine("DETECTION!!");     // but.Background = System.Windows.Media.Brushes.Red;

                    Console.WriteLine("Ransomware extension : " + ra.rwextension);
                    Console.WriteLine("First encrypted file : " + ra.firstFileTime.ToLocalTime() + ra.firstFileTime.Millisecond);
                    Console.WriteLine("MetaAlertTime: " + ra.metaAlertTime.ToLocalTime() + ra.metaAlertTime.Millisecond);
                    Console.WriteLine("AlertTime: " + ra.alertTime.ToLocalTime() + ra.alertTime.Millisecond);

                    // List detected files

                    foreach (String s in ra.possibleEncryptedFiles)
                    {
                        Console.WriteLine("Possibly encrypted :" + s);
                    }
                    foreach (String s in ra.deletedFiles)
                    {
                        Console.WriteLine("deleted file :" + s);
                    }
                    //Thread.Sleep(1 * OneSec);
                    Console.WriteLine("Possibly Process");
                    String pname = "";
                    foreach (Tuple<int, String> s in ra.proccesNames)
                    {
                        int pid = s.Item1;
                        Ptime.getCreate(pid);
                        Console.WriteLine(" "+ s.Item1 + s);
                        pname += s + "\r\n";
                    }
                    if (pname.Length > 0) File.WriteAllText(@"plist.txt", pname);
                    Console.WriteLine("DONE");
                    //Thread.Sleep(10 * OneSec);
                    Environment.Exit(-1);

                    break;

                }
                else Console.Write(".");


            }
        }
        static void Main(string[] args)
        {
            //Playme();
            //if (true) return;
            if (args.Length >1)
            {   if(args[0].Equals("ise")) {     // Check File encryption status for VFS
                    String path = String.Join(" ", args).Substring(3);
                    Console.WriteLine(isEncrypted(path.Trim())); 
                    return;
                }
                if (args[0].Equals("api"))      // Access RMM server via API
                {
                    if (args.Length == 3) AemApi(args[1], args[2]); else Console.WriteLine("incorrect-input");
                    return;
                }
                if (args[0].Equals("pt"))       // Get process createTime for Rollback 
                {
                    new Ptime(args[1]).CT();
                    return;
                }
            }

            //default launch rwcore runner
            LaunchRWCore(args);
           


        }
    }
}

