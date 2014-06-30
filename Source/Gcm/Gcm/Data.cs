using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GcmShared;

namespace Launcher.GCM
{
    public class GcmLauncherData : GcmDataManager
    {
        public GcmLauncherData(Func<string,string> pathProvider) : base(pathProvider)
        {
        }

        static readonly object writerLock = new object();
        public void SaveVariables()
        {
          lock (writerLock) {
            VariableBin.SaveToFileByPrefix("variables.ini", new[] { "opt_" });

          }
        }
    }
}
