﻿using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class UnMuteBan : TargetedSendable, ISendableVisitable {
    public UnMuteBan(string nick) {
      this.Nick = nick;
    }

    public void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public string GetString() {
      return "Unbanned " + Nick;
    }

    public string GetStringJson() {
      var obj = new UnMuteBanSender(Nick);
      return "UNBAN " + JsonConvert.SerializeObject(obj);
    }
  }
}