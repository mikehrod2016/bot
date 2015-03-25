﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {

  public abstract class CommonModels {

  }

  public class User : CommonModels {
    public string Nick { get; set; }
    public bool IsMod { get; set; }
  }

  public class Message : User {
    public string Text { get; set; }
  }
}