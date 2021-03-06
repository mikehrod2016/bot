﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public class SimpleIrcListenerClient : ConsolePrintClient, IDisposable {
    private readonly string _server;
    private readonly int _port;
    private readonly string _channel;
    private readonly string _nick;
    private readonly string _pass;
    private readonly string _broadcaster;
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private StreamReader _streamReader;
    private StreamWriter _streamWriter;
    private IProcessor _processor;

    public SimpleIrcListenerClient(string server, int port, string channel, string nick, string pass = null) {
      _server = server;
      _port = port;
      _channel = channel;
      _nick = nick;
      _pass = pass;
      _broadcaster = server.Contains("twitch") ? _channel.Substring(1) : "";
    }

    public void Connect() {
      try {
        _tcpClient = new TcpClient(_server, _port);
      } catch {
        Logger.Write("Connection Error");
        throw;
      }

      try {
        _networkStream = _tcpClient.GetStream();
        _streamReader = new StreamReader(_networkStream);
        _streamWriter = new StreamWriter(_networkStream);
        if (_pass != null)
          Send($"PASS {_pass}");
        Send($"NICK {_nick}");
        Send($"USER {_nick} {_nick} {_nick} :{_nick}");
      } catch {
        Logger.Write("Communication error");
        throw;
      }
    }

    public void Send(string data) {
      _streamWriter.WriteLine(data);
      _streamWriter.Flush();
      Logger.Write($"< {data}");
    }

    public void SendMsg(string message) {
      Send($"PRIVMSG {_channel} :{message}");
    }

    public override void Run(IProcessor processor) {
      _processor = processor;
      Connect();
      var nullCount = 0;
      while (true) {
        var data = _streamReader.ReadLine();
        if (data == null) {
          Logger.Write($"> NULL{nullCount}");
          nullCount++;
          if (nullCount > 1000) {
            Logger.Write($">>>CLOSING...<<<");
            _streamReader.Close();
            Thread.Sleep(10000);
            nullCount = 0;
            Logger.Write($">>>...RECONNECTING<<<");
            Connect();
          }
          continue;
        };

        Logger.Write($"> {data}");

        var pingMatch = new Regex(@"^PING (.*)").Match(data);
        if (pingMatch.Success) {
          Send($"PONG {pingMatch.Groups[1].Value}");
        }

        var motdEndMatch = new Regex(@"^\S+ 376 .*").Match(data);
        if (motdEndMatch.Success) {
          Send($"CAP REQ :twitch.tv/tags");
          Send($"JOIN {_channel}");
        }

        var privmsgMatch = new Regex(@"\S+mod=([0|1]);\S+ :(\S+)!\S+ PRIVMSG #\w+ :(.*)").Match(data);
        if (privmsgMatch.Success) {
          var isMod = privmsgMatch.Groups[1].Value == "1";
          var nick = privmsgMatch.Groups[2].Value;
          var msg = privmsgMatch.Groups[3].Value;
          if (nick == _broadcaster) isMod = true;

          var meMatch = new Regex(@"\u0001ACTION (.*)\u0001").Match(msg);
          if (meMatch.Success) {
            msg = $"/me {meMatch.Groups[1]}";
          }

          var user = new User(nick);
          if (isMod) {
            user.Flair.Add("mod");
          }
          processor.Process(new PublicMessage(user, msg));
        }

      }
    }

    public void Dispose() {
      _streamReader?.Close();
      _streamWriter?.Close();
      _networkStream?.Close();
      _tcpClient?.Close();
    }

    public override void Forward(PublicMessage message) {
      _processor.Process(message);
    }
  }
}
