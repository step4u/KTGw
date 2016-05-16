using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.DataModel;

namespace Com.Huen.Libs
{
    public class CompareRtpSeq : IComparer<RcvData>
    {
        public int Compare(RcvData x, RcvData y)
        {
            if (x.seqnum > y.seqnum)
                return 1;
            else if (x.seqnum < y.seqnum)
                return -1;
            else
                return 0;
        }
    }

    public class SortRtpSeq : IComparer<Com.Huen.DataModel.ReceivedRtp>
    {
        public int Compare(Com.Huen.DataModel.ReceivedRtp x, Com.Huen.DataModel.ReceivedRtp y)
        {
            if (x.seq > y.seq)
                return 1;
            else if (x.seq < y.seq)
                return -1;
            else
                return 0;
        }
    }
}
