﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {

  //todo this replicates functionality from Utility
  internal static class Tools {

    public static void Log(string text, ConsoleColor color = ConsoleColor.White) {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write($"{Process.GetCurrentProcess().Threads.Count} ");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write(DateTime.UtcNow.ToString("T"));
      Console.ForegroundColor = color;
      Console.WriteLine(" {0}", text);
      Console.ResetColor();
    }

    public static string PrettyDeltaTime(TimeSpan span, string rough = "") {
      int day = Convert.ToInt32(span.ToString("%d"));
      int hour = Convert.ToInt32(span.ToString("%h"));
      int minute = Convert.ToInt32(span.ToString("%m"));

      if (span.CompareTo(TimeSpan.Zero) == -1) {
        Log($"Time to sync the clock?{span}", ConsoleColor.Red);
        return "a few seconds";
      }

      if (day > 1) {
        if (hour == 0) return $"{day} days";
        return $"{day} days {hour}h";
      }

      if (day == 1) {
        if (hour == 0) return "1 day";
        return $"1 day {hour}h";
      }

      if (hour == 0) return $"{rough}{minute}m";
      if (minute == 0) return $"{rough}{hour}h";

      return $"{rough}{hour}h {minute}m";
    }
  }
}
