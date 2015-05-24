﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Utility;

namespace Dbot.Processor {
  public static class AntiNuke {
    public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

    public static async void Dissipate(string nukedWord) {
      await Task.Delay(Settings.NukeDuration);
      if (CancellationTokenSource.IsCancellationRequested) {
        CancellationTokenSource = new CancellationTokenSource();
        return;
      }
      TimeSpan duration;
      var success = Nuke.ActiveDuration.TryRemove(nukedWord, out duration);
      Debug.Assert(success);

      Queue<string> victimsQueue;
      Nuke.VictimQueue.TryRemove(nukedWord, out victimsQueue);
      //Debug.Assert(success); // This is sometimes invalid because the bot won't add when there are no victims.

      Tools.Log("NukeDictionary " + Nuke.ActiveDuration.Count + ", NukeVictims " + Nuke.VictimQueue.Count, ConsoleColor.Red);
    }

    public static void Aegis() {
      CancellationTokenSource.Cancel();
      MessageProcessor.Sender.Post(Make.Message("Oh shit, undo! Undo!"));
      foreach (var nukedWord in Nuke.ActiveDuration.Keys) {
        TimeSpan duration;
        var success = Nuke.ActiveDuration.TryRemove(nukedWord, out duration);
        Debug.Assert(success);
      }
      while (Nuke.VictimQueue.Count > 0) {
        Queue<string> victimQueue;
        var success = Nuke.VictimQueue.TryGetValue(Nuke.VictimQueue.First().Key, out victimQueue);
        Debug.Assert(success);
        if (victimQueue.Count == 0) {
          success = Nuke.VictimQueue.TryRemove(Nuke.VictimQueue.First().Key, out victimQueue);
          Debug.Assert(success);
        } else {
          MessageProcessor.Sender.Post(Make.Unban(victimQueue.Dequeue()));
          Thread.Sleep(Settings.NukeLoopWait);
        }
      }
    }
  }
}
