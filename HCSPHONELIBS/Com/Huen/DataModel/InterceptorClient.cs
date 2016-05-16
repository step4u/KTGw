using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using Com.Huen.Libs;


namespace Com.Huen.DataModel
{
    public class InterceptorClient
    {
        private IPEndPoint _clientipep = null;
        private DateTime _clientregdate;
        //private InterceptorStatus _clientstatus = InterceptorStatus.None;
        private string _reqtelnum = string.Empty;

        public IPEndPoint ClientIPEP
        {
            get { return _clientipep; }
            set { _clientipep = value; }
        }

        public DateTime ClientRegdate
        {
            get { return _clientregdate; }
            set { _clientregdate = value; }
        }

        //public RTPInterceptorStatus ClientStatus
        //{
        //    get { return _clientstatus; }
        //    set { _clientstatus = value; }
        //}

        public string ReqtelNum
        {
            get { return _reqtelnum; }
            set { _reqtelnum = value; }
        }
    }
}
