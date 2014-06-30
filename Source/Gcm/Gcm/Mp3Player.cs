using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Threading;

using System.Web;
using System.Net;
using System.IO;
using Utilities;
using System.Text;
using System.Runtime.InteropServices;

namespace Launcher
{
    public class Mp3Player
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public bool Playing { get; private set; }
        private object lockObject;
        public Mp3Player()
        {
            lockObject = new object();
        }
        public void Play(string file)
        {
            lock (lockObject)
            {
                Playing = true;
                mciSendString("open \"" + file + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
                mciSendString("play MediaFile", null, 0, IntPtr.Zero);
            }
        }

        public void PlayNew(string file)
        {
            lock (lockObject)
            {
                if (Playing)
                    StopPlaying();

                Playing = true;
                mciSendString("open \"" + file + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
                mciSendString("play MediaFile", null, 0, IntPtr.Zero);

            }
        }

        public void Stop()
        {
            lock (lockObject)
            {
                if (Playing)
                    StopPlaying();
            }
        }

        void StopPlaying()
        {
            mciSendString("close MediaFile", null, 0, IntPtr.Zero);
            Playing = false;
        }
    }
}
