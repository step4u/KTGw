using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Com.Huen.Controldata
{
    public class HospitalInfo
    {
        private string _hosp_name;
        private string _hosp_ceo;
        private string _hosp_tel;
        private string _status;

        public string HospName
        {
            get { return _hosp_name; }
            set { _hosp_name = value; }
        }
        public string HospCeo
        {
            get { return _hosp_ceo; }
            set { _hosp_ceo = value; }
        }
        public string HospTel
        {
            get { return _hosp_tel; }
            set { _hosp_tel = value; }
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public HospitalInfo(string hospname, string hospceo, string hsoptel, string status)
        {
            HospName = hospname;
            HospCeo = hospceo;
            HospTel = hsoptel;
            Status = status;
        }
    }

    public class HospitalInfoList : ObservableCollection<HospitalInfo>
    {
        public HospitalInfoList()
        {
            Add(new HospitalInfo("가병원", "가", "07070901600", "0"));
            Add(new HospitalInfo("나병원", "나", "01045455962", "0"));
            Add(new HospitalInfo("다병원", "다", "07077091302", "0"));
            Add(new HospitalInfo("라병원", "라", "07077091303", "3"));
        }
    }
}
