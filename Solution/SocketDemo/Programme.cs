// Asynchronous Server Socket Example
// http://msdn.microsoft.com/en-us/library/fx6588te.aspx
// remark by @fang - 2024

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
// State object for reading client data asynchronously
namespace SocketDemo
{
    public class Programme
    {
        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static int Main(String[] args)
        {
            Console.WriteLine("start");
            AsynchronousSocketListener.StartListening();
            return 0;
        }
    }
}