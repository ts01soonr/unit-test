using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketDemo
{
    
    public class ConnectionHandler
    {
        
        public static String PerformAction(String command)
        {
            if (command == null || command.Length == 0)
            {
                //sendMessage("no command given");
                return "No command given";
            }
            String[] tokens = command.Split(' ');
            String cmd = tokens[0];
            String path = tokens.Length > 1 ? tokens[1].Replace("\\*", " ") : "";
            String parm3 = tokens.Length > 2 ? tokens[2] : "";
            String parm4 = tokens.Length > 3 ? tokens[3] : "";
            String parm5 = tokens.Length > 4 ? tokens[4] : "";
            String parm6 = tokens.Length > 5 ? tokens[5] : "";
            String parm7 = tokens.Length > 6 ? tokens[6] : "";
            String parm8 = tokens.Length > 7 ? tokens[7] : "";
            String parm9 = tokens.Length > 8 ? tokens[8] : "";
            String parm10 = tokens.Length > 9 ? tokens[9] : "";
            String path2 = command.Substring(cmd.Length).Trim();
            String path3 = command.Substring((cmd + " " + path).Trim().Length).Trim();
            //String path4 = command.substring((cmd+" "+path+" "+path3).trim().length()).trim();
            String path4 = (parm4 + " " + parm5 + " " + parm6 + " " + parm7 + " " + parm8 + " " + parm9 + " " + parm10).Trim();
            if (cmd.Equals("bye") || cmd.Equals("quit"))
            {
                return "OK";
            }
            else if (cmd.Equals("stop"))
            {
                System.Environment.Exit(1);
                return "OK";
            }
            else if (cmd.Equals("aos"))
            {

                return "OK";
            }
            else if (cmd.Equals("aos2"))
            {

                return "OK";
            }
            else if (cmd.Equals("ios"))
            {

                return "OK";
            }
            else if (cmd.Equals("ios2"))
            {

                return "OK";
            }
            else if (cmd.Equals("run"))
            {
                //run system.shell command
                return "pending... OK";
            }
            else if (cmd.Equals("who"))
            {
                return "fang OK";
            }
            else if (cmd.Equals("help"))
            {
                return "aos|ios|help|who OK";
            }
            return "FAIL";
        }



    }

}
