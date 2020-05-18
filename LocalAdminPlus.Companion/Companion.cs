using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace LocalAdminPlus.Companion
{
    public class Companion
    {
        public static Companion Instance { get; } = new Companion();
        public ushort ConsolePort { get; private set; } = 10000;
        public TcpClient TcpClient { get; private set; }
        public HarmonyInstance Harmony { get; } = HarmonyInstance.Create("pl.js6pak.LocalAdminPlus.Companion");

        protected Companion() { }

        public void Enable()
        {
            Harmony.PatchAll();
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                const string consoleArg = "-console";

                if (arg.StartsWith(consoleArg))
                {
                    if (ushort.TryParse(arg.Substring(consoleArg.Length), out var consolePort))
                    {
                        ConsolePort = consolePort;
                    }
                }
            }

            ServerConsole.AddLog($"Starting TcpClient on port {ConsolePort}");

            TcpClient = new TcpClient();

            try
            {
                TcpClient.Connect(new IPEndPoint(IPAddress.Loopback, ConsolePort));
            }
            catch
            {
                return;
            }

            SendMessage("LocalAdminPlus Companion connected!");

            var streamReader = new StreamReader(TcpClient.GetStream());
            Task.Run(async () =>
            {
                while (true)
                {
                    if (!streamReader.EndOfStream)
                    {
                        ServerConsole.EnterCommand(streamReader.ReadLine());
                    }

                    await Task.Delay(10);
                }
            });
        }

        public void SendMessage(string message, ConsoleColor? color = ConsoleColor.White)
        {
            var data = Encoding.ASCII.GetBytes((color == null ? " " : ((byte)color.Value).ToString("X")) + message + "\n");
            var stream = TcpClient.GetStream();
            stream.Write(data, 0, data.Length);
        }

        public void Disable()
        {
            TcpClient.Dispose();
            Harmony.UnpatchAll();
        }
    }
}
