﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.SampleClient {
  public class SampleClient : IClient {
    private IProcessor _processor;

    public async void Run(IProcessor processor) {
      _processor = processor;
      foreach (var message in _messageList) {
        processor.ProcessMessage(message);
        await Task.Run(() => Task.Delay(0));
      }
    }

    public void Forward(Message message) {
      _processor.ProcessMessage(message);
    }

    public void Send(Sendable input) {
      if (input is Message) {
        Tools.Log("MSG " + ((Message) input).Text, ConsoleColor.Red);
        //_websocket.Send("MSG " + jsonMsg);
      } else if (input is Mute) {
        Tools.Log("Muted " + ((Mute) input).Nick);
      } else if (input is Ban) {
        if (((Ban) input).Duration.Minutes < 0) {
          Tools.Log("Unbanned " + input.Nick);
        } else {
          Tools.Log("Banned " + input.Nick);
        }
      }
    }

    private readonly List<Message> _messageList = new List<Message> {
      Make.Message("a", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("b", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("c", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("d", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("e", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("f", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("g", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("h", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("i", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("j", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("k", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("l", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("m", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("n", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("o", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("p", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("q", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("r", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("s", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("t", "abcdefghijklmnopqrstuvwxyz"),
      Make.Message("u", "abcdefghijklmnopqrstuvwxyz"),
    };
  }
}