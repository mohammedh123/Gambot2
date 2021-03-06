﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gambot.Core;
using Gambot.Data;

namespace Gambot.Module.Factoid
{
    public class LiteralFactoidCommand : ICommand
    {
        private readonly IDataStoreProvider _dataStoreProvider;
        private readonly IDataDumper _dataDumper;

        public LiteralFactoidCommand(IDataStoreProvider dataStoreProvider, IDataDumper dataDumper)
        {
            _dataDumper = dataDumper;
            _dataStoreProvider = dataStoreProvider;
        }

        public async Task<Response> Handle(Message message)
        {
            if (!message.Addressed)
                return null;
            var match = Regex.Match(message.Text, @"^literal (.+)$", RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;
            var trigger = match.Groups[1].Value;

            var dataStore = await _dataStoreProvider.GetDataStore("Factoids");

            var values = await dataStore.GetAll(trigger);

            if (!values.Any())
                return message.Respond($"Sorry, {message.From.Mention}, but I don't know about \"{trigger}.\"");

            if (values.Count() > 10 || values.Sum(x => x.Value.Length) > 500)
            {
                var url = await _dataDumper.Dump("Factoids", trigger);
                return message.Respond($"{trigger}: {url}");
            }

            var result = String.Join(", ", values.Select(x => $"(#{x.Id}) {x.Value}"));

            return message.Respond($"{trigger}: {result}");
        }
    }
}