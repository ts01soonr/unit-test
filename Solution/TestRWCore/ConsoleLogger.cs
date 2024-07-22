using System;
using System.Text;

namespace TestRWCore
{
    public class ConsoleLogger : RWDetectCore.ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine(getTime() + message);
        }

        public void InfoException(string message, Exception exception)
        {
            Console.WriteLine(getTime() + message + exception.Message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(getTime() + message);
        }

        public void DebugException(string message, Exception exception)
        {
            Console.WriteLine(getTime() + message + ": " + exception.GetType() + ": " + exception.Message);
        }

        public void Error(string message)
        {
            Console.WriteLine(getTime() + message);
        }

        public void ErrorException(string message, Exception exception)
        {
            Console.WriteLine(getTime() + message + exception.Message);
        }

        private String getTime()
        {
            StringBuilder sp = new StringBuilder();
            DateTime dt = DateTime.Now;
            String sm = "" + dt.Millisecond;

            if (dt.Millisecond < 10)
                sm = "00" + sm;
            else if (dt.Millisecond < 100)
                sm = "0" + sm;

            sp.Append(dt.ToLocalTime()); sp.Append(":"); sp.Append(sm); sp.Append(" :");
            return sp.ToString();
        }
    }
}
