﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.Processor;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbot.UnitTest {
  [TestClass]
  public class WebSocketIntegrationTest {
    [TestMethod]
    public async Task SendMessageTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.TestAccountWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(2000);
      client.Send(Make.Message("test" + Tools.RandomString(3)));
    }

    [TestMethod]
    public async Task SendMuteTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(5000);
      client.Send(Make.Mute("dharmatest", TimeSpan.FromMinutes(3), "test reason"));
    }
  }
}