﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Data;

namespace Dbot.ModCommander {
  public class ModCommander {
    public string Message { get; set; }

    public ModCommander(string input) {
      Debug.Assert(input[0] == '!' || input[0] == '<');
      var inputWithoutTriggerChar = input.Substring(1);
      var splitInput = inputWithoutTriggerChar.Split(new[] { ' ' }, 2);
      var commandMatches = Datastore.ModCommands.Where(x => x.Command == splitInput[0]);

      var operationDictionary = new Dictionary<string, Action<ModCommands>> {
        {"message", x =>  this.Message = 1 < splitInput.Count() ? x.Result.Replace("*", splitInput[1]) : x.Result },
        {"set", x => Datastore.UpdateStateValue(x.Command, int.Parse(x.Result))},
        {"db.add", x => Datastore.AddBanWord(x.Result, splitInput[1])},
        {"db.remove", x => Datastore.RemoveBanWord(x.Result, splitInput[1])},
        {"stalk", x => this.Message = Datastore.Stalk(splitInput[1])},
      };

      foreach (var c in commandMatches) {
        if ((c.CommandParameter == null)
          || (splitInput.Count() > 1 && c.CommandParameter == splitInput[1])
          || (splitInput.Count() > 1 && c.CommandParameter == "*")) {
          operationDictionary[c.Operation].Invoke(c);
        }
      }

    }
  }
}