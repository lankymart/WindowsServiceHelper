using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceProcess.Helpers.ViewModels
{
    internal enum ServiceState
    {
        Stopping,
        Stopped,
        Starting,
        Started,
        Pausing,
        Paused
    }
}
