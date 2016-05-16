using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class EventSchedule
    {
        private string _evt_idx = string.Empty;
        private string _com_idx = string.Empty;
        private string _p_idx = string.Empty;
        private string _ch_idx = string.Empty;
        private string _evt_title = string.Empty;
        private string _evt_place = string.Empty;
        private string _evt_memo = string.Empty;
        private DateTime _evt_sdate;
        private DateTime _evt_edate;
        private bool _evt_repeat = false;
        private string _evt_repeatday = string.Empty;
        private DateTime _regdate;

        public string EVT_IDX
        {
            get
            {
                return _evt_idx;
            }
            set
            {
                _evt_idx = value;
            }
        }

        public string COM_IDX
        {
            get
            {
                return _com_idx;
            }
            set
            {
                _com_idx = value;
            }
        }

        public string P_IDX
        {
            get
            {
                return _p_idx;
            }
            set
            {
                _p_idx = value;
            }
        }

        public string CH_IDX
        {
            get
            {
                return _ch_idx;
            }
            set
            {
                _ch_idx = value;
            }
        }

        public string EVT_TITLE
        {
            get
            {
                return _evt_title;
            }
            set
            {
                _evt_title = value;
            }
        }

        public string EVT_PLACE
        {
            get
            {
                return _evt_place;
            }
            set
            {
                _evt_place = value;
            }
        }

        public string EVT_MEMO
        {
            get
            {
                return _evt_memo;
            }
            set
            {
                _evt_memo = value;
            }
        }

        public DateTime EVT_SDATE
        {
            get
            {
                return _evt_sdate;
            }
            set
            {
                _evt_sdate = value;
            }
        }

        public DateTime EVT_EDATE
        {
            get
            {
                return _evt_edate;
            }
            set
            {
                _evt_edate = value;
            }
        }

        public bool EVT_REPEAT
        {
            get
            {
                return _evt_repeat;
            }
            set
            {
                _evt_repeat = value;
            }
        }

        public string EVT_REPEATDAY
        {
            get
            {
                return _evt_repeatday;
            }
            set
            {
                _evt_repeatday = value;
            }
        }

        public DateTime REGDATE
        {
            get
            {
                return _regdate;
            }
            set
            {
                _regdate = value;
            }
        }
    }
}
