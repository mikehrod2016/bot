﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Dbot.Data;
using Dbot.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweetinvi;
using Message = Dbot.CommonModels.Message;

namespace Dbot.Processor {
  public class Commander {
    private readonly string _text;

    private readonly Dictionary<List<string>, Func<string>> _commandDictionary = new Dictionary<List<string>, Func<string>> {
      { new List<string> { "playlist" }, 
        () => "Playlist at last.fm/user/StevenBonnellII" },
      { new List<string> { "rules", "unmoddharma" }, 
        () => "reddit.com/1aufkc" },
      { new List<string> { "refer", "sponsor" }, 
        () => "destiny.gg/amazon ☜(ﾟヮﾟ☜) Amazon referral ☜(⌒▽⌒)☞ 25$ off Sprint network (☞ﾟヮﾟ)☞ destiny.gg/ting\nIn space ༼ ◔◡◔༽ destiny.gg/eve \\(ﾟ◡ﾟ )/ Destiny awaits" },
      { new List<string> { "irc" }, 
        () => "IRC will be implemented Soon™. For now, chat is echoed to Rizon IRC at http://qchat.rizon.net/?channels=#destinyecho . Forwarding of IRC chat to DestinyChat is available." },
      { new List<string> { "bancount" }, 
        () => Datastore.GetStateVariable("bancount") + " souls reaped" },
      { new List<string> { "time" }, 
        () => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Central Standard Time").ToShortTimeString() + " Central Steven Time" },
      { new List<string> { "live" }, 
        () => Tools.LiveStatus() },
      { new List<string> { "blog", "blag" }, 
        () => Tools.FallibleCode(Blog) },
      { new List<string> { "starcraft", "sc" }, 
        () => Tools.FallibleCode(Starcraft) },
      { new List<string> { "song" }, 
        () => Tools.FallibleCode(Song) },
      { new List<string> { "pastsong", "lastsong", "previoussong", "earliersong" }, 
        () => Tools.FallibleCode(EarlierSong) },
      { new List<string> { "twitter", "tweet", "twatter" }, 
        () => Tools.FallibleCode(Twitter) },
      { new List<string> { "youtube", "yt" }, 
        () => Tools.FallibleCode(Youtube) },
    };

    public Commander(Message message) {
      _text = message.Text.Substring(1);
    }

    public Message Run() {
      var command = _commandDictionary.FirstOrDefault(y => y.Key.Any(x => _text.StartsWith(x))).Value;
      return command == null ? null : Make.Message(true, command.Invoke());
    }

    private static string Blog() {
      var rawblog = Tools.DownloadData("http://blog.destiny.gg/feed/").Result;
      using (var reader = XmlReader.Create(new StringReader(rawblog))) {
        reader.ReadToFollowing("item");
        reader.ReadToFollowing("title");
        var title = reader.ReadElementContentAsString();
        reader.ReadToFollowing("link");
        var link = reader.ReadElementContentAsString();
        reader.ReadToFollowing("pubDate");
        var pubdateString = reader.ReadElementContentAsString();
        var pubdate = Convert.ToDateTime(pubdateString).ToUniversalTime();
        return "\"" + title + "\" posted " + Tools.PrettyDeltaTime(DateTime.UtcNow - pubdate) + " ago " + link;
      }
    }

    private static string Starcraft() {
      var json = Tools.DownloadData("http://us.battle.net/api/sc2/profile/310150/1/Destiny/matches");
      dynamic dyn = JsonConvert.DeserializeObject(json.Result);
      var date = (int) dyn.matches[0].date;
      var map = (string) dyn.matches[0].map;
      var delta = Tools.PrettyDeltaTime(Tools.Epoch(DateTime.UtcNow) - TimeSpan.FromSeconds(date), "~");
      var decision = ((string) dyn.matches[0].decision).ToLower();
      var type = ((string) dyn.matches[0].type).ToLower();

      if (decision == "win") {
        decision = "won";
      } else if (decision == "loss") {
        decision = "lost";
      } else {
        decision = "played";
      }
      return "Destiny " + decision + " a " + type + " game on " + map + " " + delta + " ago sc2ranks.com/character/us/310150/Destiny";
    }

    private static string Song() {
      var json = Tools.DownloadData("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=stevenbonnellii&api_key=" + PrivateConstants.LastFMAPIKey + "&format=json");
      var dyn = (JObject) JsonConvert.DeserializeObject(json.Result);
      var artist = dyn.SelectToken("recenttracks.track[0].artist.#text");
      var name = dyn.SelectToken("recenttracks.track[0].name");
      if (dyn.SelectToken("recenttracks.track[0].@attr.nowplaying") != null) {
        return name + " - " + artist + " last.fm/user/stevenbonnellii";
      }
      var epochStringTime = dyn.SelectToken("recenttracks.track[0].date.uts").Value<string>();
      var epochTime = Convert.ToInt32(epochStringTime);
      var delta = Tools.Epoch(DateTime.UtcNow) - TimeSpan.FromSeconds(epochTime);
      var prettyDelta = Tools.PrettyDeltaTime(delta);
      return "No song played/scrobbled. Played " + prettyDelta + " ago: " + name + " - " + artist;
    }

    private static string EarlierSong() {
      var json = Tools.DownloadData("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=stevenbonnellii&api_key=" + PrivateConstants.LastFMAPIKey + "&format=json");
      var dyn = (JObject) JsonConvert.DeserializeObject(json.Result);
      var artist = dyn.SelectToken("recenttracks.track[0].artist.#text");
      var name = dyn.SelectToken("recenttracks.track[0].name");
      var artist2 = dyn.SelectToken("recenttracks.track[1].artist.#text");
      var name2 = dyn.SelectToken("recenttracks.track[1].name");
      var epochStringTime = dyn.SelectToken("recenttracks.track[1].date.uts").Value<string>();
      var epochTime = Convert.ToInt32(epochStringTime);
      var delta = Tools.Epoch(DateTime.UtcNow) - TimeSpan.FromSeconds(epochTime);
      var prettyDelta = Tools.PrettyDeltaTime(delta);
      return name2 + " - " + artist2 + " played before " + name + " - " + artist + " ~" + prettyDelta + " ago";
    }

    private static string Twitter() {
      TwitterCredentials.SetCredentials(PrivateConstants.Twitter_Access_Token, PrivateConstants.Twitter_Access_Token_Secret, PrivateConstants.Twitter_Consumer_Key, PrivateConstants.Twitter_Consumer_Secret);
      var tweet = User.GetUserFromScreenName("steven_bonnell").GetUserTimeline(10).Skip(3).First();
      var delta = Tools.PrettyDeltaTime(DateTime.UtcNow - tweet.CreatedAt);
      return delta + " ago: " + Tools.TweetPrettier(tweet);
    }

    private static string Youtube() {
      var json = Tools.DownloadData("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=1&playlistId=UU554eY5jNUfDq3yDOJYirOQ&key=" + PrivateConstants.Youtube);
      var jObject = (JObject) JsonConvert.DeserializeObject(json.Result);
      var publishedAt = jObject.SelectToken("items[0].snippet.publishedAt").Value<DateTime>();
      var videoId = jObject.SelectToken("items[0].snippet.resourceId.videoId").Value<string>();
      var title = jObject.SelectToken("items[0].snippet.title").Value<string>();
      var delta = Tools.PrettyDeltaTime(DateTime.UtcNow - publishedAt);
      return "\"" + title + "\" posted " + delta + " ago youtu.be/" + videoId;
    }
  }
}
