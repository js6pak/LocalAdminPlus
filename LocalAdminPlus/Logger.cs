using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LocalAdmin.V2
{
    public static class Logger
    {
        private static DateTime Time { get; } = DateTime.Now;

        static Logger()
        {
            Directory.CreateDirectory("logs");
        }

        public static void Log(string? text, Type type = Type.La)
        {
            using var streamWriter = new StreamWriter($"logs/{Time:dd-MM-yyyy_H_mm}-{Enum.GetName(typeof(Type), type)?.ToUpper()}.log", true);
            streamWriter.WriteLine(text);
        }

        public static void Log(object obj, Type type = Type.La)
        {
            Log(obj?.ToString(), type);
        }

        public enum Type
        {
            La,
            Scp
        }
    }
}