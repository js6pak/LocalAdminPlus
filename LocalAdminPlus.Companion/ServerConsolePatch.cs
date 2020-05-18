using System.Threading;
using Harmony;

namespace LocalAdminPlus.Companion
{
    public class ServerConsolePatch
    {
        // TODO Transpiler
        [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.Prompt))]
        public class PromptPatch
        {
            public static bool Prefix(ServerConsole __instance)
            {
                if (!Companion.Instance.TcpClient.Connected)
                    return true;

                while (!ServerConsole._disposing)
                {
                    if (ServerConsole.PrompterQueue.Count == 0 || !ServerConsole._accepted)
                    {
                        Thread.Sleep(25);
                    }
                    else
                    {
                        string text = ServerConsole.PrompterQueue.Dequeue();
                        if (!__instance._errorSent || !text.Contains("Could not update the session - Server is not verified."))
                        {
                            __instance._errorSent = true;
                            ServerConsole._logId++;
                            Companion.Instance.SendMessage(text);
                        }
                    }
                }
                return false;
            }
        }
    }
}
