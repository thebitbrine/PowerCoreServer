using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using TheBitBrine;

namespace PowerCoreServer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    new Program().Run();
                }
                catch { }
            }
        }

        private QuickMan API;
        public void Run()
        {
            API = new QuickMan();
            var Endpoints = new Dictionary<string, Action<HttpListenerContext>>();

            Endpoints.Add("stats", Stats);
            Endpoints.Add("watch", Watch);
            Endpoints.Add("get", Get);

            API.Start(Endpoints, 20);
        }


        public bool FrameLock = false;
        public static string CurrentFrame;
        public void Stats(HttpListenerContext Context)
        {
            try
            {
                while (FrameLock)
                    Thread.Sleep(11);
                FrameLock = true;
                CurrentFrame = new StreamReader(Context.Request.InputStream).ReadToEnd();
            }
            catch { }
            FrameLock = false;
            API.Respond("OK", Context);
        }


        public void Get(HttpListenerContext Context)
        {
            MemoryStream ms = new MemoryStream();
            {
                try
                {
                    while (FrameLock)
                        Thread.Sleep(11);
                    FrameLock = true;
                    var bytes = Convert.FromBase64String(CurrentFrame);
                    API.Respond(new MemoryStream(bytes), "image/jpeg", Context);
                }
                catch { }
                FrameLock = false;
            }
        }

        public void Watch(HttpListenerContext Context)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] {Context.Request.RemoteEndPoint} connected");
            API.Respond(File.ReadAllText("jsfs.html"), "text/html", Context);
        }

    }
}
