﻿using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class UnMuteBan : ISent, ISendableVisitable {
    public UnMuteBan(string beneficiary) {
      this.Beneficiary = beneficiary;
    }

    public string Beneficiary { get; set; }

    private string _nick;
    public string Nick {
      get { return _nick; }
      set { _nick = value.ToLower(); }
    }


    public void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string ToString() {
      return "Unbanned " + Beneficiary;
    }
  }
}