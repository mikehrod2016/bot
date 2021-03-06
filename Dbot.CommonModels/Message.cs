using System;
using System.Diagnostics;
using Dbot.CommonModels.Users;

namespace Dbot.CommonModels {
  [DebuggerDisplay("{Ordinal}. {OriginalText}")]
  public abstract class Message : ISendable, ISendableVisitable, IEquatable<Message> {
    protected Message(string senderName, string originalText) {
      Sender = new User(senderName);
      OriginalText = originalText;
    }

    protected Message(IUser user, string originalText) {
      Sender = user;
      OriginalText = originalText;
    }

    public abstract void Accept(IClientVisitor visitor);

    public IUser Sender { get; set; }

    public string SenderName => Sender.Nick;

    public bool IsMod => Sender.IsMod;

    public string OriginalText {
      get { return _originalText; }
      set {
        _originalText = value;
        _sanitizedText = value.ToLower();
      }
    }
    private string _originalText;

    public string SanitizedText {
      get { return _sanitizedText; }
      set { OriginalText = value; }
    }
    private string _sanitizedText;

    public int Ordinal { get; set; }

    public bool Equals(Message that) {
      return
        this.Sender.Equals(that.Sender) &&
        this.SanitizedText == that.SanitizedText &&
        this.IsMod == that.IsMod &&
        this.Ordinal == that.Ordinal;
    }
  }
}