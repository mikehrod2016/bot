﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Common {
  public interface IClient {
    void Run(IProcessor processor);
    void Send(Sendable sendable);
  }
}
