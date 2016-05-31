using Com.Huen.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using WinSound;

namespace Com.Huen.DataModel
{
    public delegate void EndOfRtpEventHandler(object sender, EventArgs e);

    public class RTPRecInfo : IDisposable
    {
        public RTPRecInfo()
        {

        }

        public event EndOfRtpEventHandler EndOfRtpEvent;

        private Timer endtimer;
        private int endcount = 0;

        public void Add(byte[] buffer)
        {
            if (buffer.Length == 0) this.endcount++;

            RTPPacket rtpPacket = new RTPPacket(buffer);



            // string msg = string.Format("seq:{0}, ext:{1}, peer:{2}, isExtension:{3}, size:{4}, bytesLength:{5}, codec:{6}", obj.seq, obj.extension, obj.peer_number, obj.isExtension, obj.size - 12, obj.voice.Length, obj.codec);
            // util.WriteLogTest3(msg, this.filename);

            //if (obj.size == 0)
            //    endcount++;

            //if (endcount > 1)
            //{
            //    if (endtimer != null)
            //    {
            //        endtimer.Enabled = false;
            //        endtimer = null;
            //    }

            //    System.Threading.Thread.Sleep(3000);

            //    this.MixRtp();

            //    if (EndOfRtpEvent != null)
            //        EndOfRtpEvent(this, new EventArgs());

            //    return;
            //}

            //if (endtimer != null)
            //{
            //    endtimer.Enabled = false;
            //    endtimer.Enabled = true;
            //}
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
