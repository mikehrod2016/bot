﻿//http://www.reddit.com/r/InternetIsBeautiful/comments/2zwvpm/%EF%BD%95%EF%BD%8E%EF%BD%89%EF%BD%83%EF%BD%8F%EF%BD%84%EF%BD%85_%EF%BD%94%EF%BD%8F%EF%BD%8F%EF%BD%8C%EF%BD%93/
//todo Destiny Dharma needs another command where you can temp make a phrase a ban phrase for like 30 minutes NoTears Just for all these OuO faggots NoTears

using Dbot.Client;
using Dbot.Utility;

namespace Dbot.Main {
  class Dbot {

    static void Main() {
      var client = new WebSocketListenerClient(PrivateConstants.BotWebsocketAuth);
      new PrimaryLogic(client).Run();
    }
  }
}