﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeCI.TaskDispatcher.NodeSide.EventArgs
{
    public class TaskPushedEventArgs : System.EventArgs
    {
        public string Id { get; set; }
    }
}
