﻿using System;
using System.Collections.Generic;
using System.Threading;
using Gambot.Core;
using Gambot.Data.InMemory;
using Gambot.IO;
using Gambot.Module.Factoid;
using Gambot.Module.Say;

namespace Gambot.Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger("Gambot");
            logger.Info("Starting the screaming robot.");

            // Temp implementation
            var dataStoreProvider = new InMemoryDataStoreProvider();
            var config = new DataStoreConfig(dataStoreProvider, logger.GetChildLog("DataStoreLogger"));

            var listeners = new List<IListener>
            {
                new FactoidListener(dataStoreProvider),
            };
            var responders = new List<IResponder>
            {
                new SayResponder(),
                new AddFactoidResponder(dataStoreProvider),
                new ForgetFactoidResponder(dataStoreProvider),
                new LiteralFactoidResponder(dataStoreProvider),
                new FactoidResponder(dataStoreProvider),
            };
            var transformers = new List<ITransformer>();

            var messenger = new ConsoleMessenger(logger.GetChildLog("ConsoleMessenger"));

            var processor = new BotProcess(listeners, responders, transformers, messenger, logger.GetChildLog("BotProcess"));

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                logger.Info("Shutting down...");
                if (messenger != null)
                    messenger.Dispose();
                logger.Info("Done.");
                Environment.Exit(0);
            };

            processor.Initialize();

            Thread.Sleep(Timeout.Infinite);
        }
    }

    public class ConsoleLogger : ILogger
    {
        private readonly string _name;

        public ConsoleLogger(string name)
        {
            _name = name;
        }

        public void Debug(string message, params object[] formatArgs)
        {
            WriteLog("DEBUG", message, formatArgs);
        }

        public void Debug(Exception ex, string message, params object[] formatArgs)
        {
            WriteLog("DEBUG", message, formatArgs);
            Console.WriteLine(ex.ToString());
        }

        public void Error(string message, params object[] formatArgs)
        {
            WriteLog("ERROR", message, formatArgs);
        }

        public void Error(Exception ex, string message, params object[] formatArgs)
        {
            WriteLog("ERROR", message, formatArgs);
            Console.WriteLine(ex.ToString());
        }

        public void Fatal(string message, params object[] formatArgs)
        {
            WriteLog("FATAL", message, formatArgs);
        }

        public void Fatal(Exception ex, string message, params object[] formatArgs)
        {
            WriteLog("FATAL", message, formatArgs);
            Console.WriteLine(ex.ToString());
        }

        public void Info(string message, params object[] formatArgs)
        {
            WriteLog("INFO", message, formatArgs);
        }

        public void Info(Exception ex, string message, params object[] formatArgs)
        {
            WriteLog("INFO", message, formatArgs);
            Console.WriteLine(ex.ToString());
        }

        public void Trace(string message, params object[] formatArgs)
        {
            // WriteLog("TRACE", message, formatArgs);
        }

        public void Trace(Exception ex, string message, params object[] formatArgs)
        {
            // WriteLog("TRACE", message, formatArgs);
            Console.WriteLine(ex.ToString());
        }

        public void Warn(string message, params object[] formatArgs)
        {
            WriteLog("WARN ", message, formatArgs);
        }

        public void Warn(Exception ex, string message, params object[] formatArgs)
        {
            WriteLog("WARN ", message, formatArgs);
            Console.WriteLine(ex.ToString());
        }

        private void WriteLog(string level, string message, params object[] formatArgs)
        {
            Console.WriteLine($"{DateTime.Now.ToString("u")} {level} [{_name}] {message}", formatArgs);
        }

        public ILogger GetChildLog(string name)
        {
            return new ConsoleLogger($"{_name}.{name}");
        }
    }
}