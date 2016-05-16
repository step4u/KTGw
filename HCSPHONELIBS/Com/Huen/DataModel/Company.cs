using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;

using Com.Huen.Sql;
using Com.Huen.Libs;

namespace Com.Huen.DataModel
{
    public class Company
    {
        public string memuid { get; set; }
        public string comname { get; set; }
        public string comnum { get; set; }
        public string comhg { get; set; }
        public string comtel { get; set; }
    }

    public class Companies
    {
        private ObservableCollection<Company> _companylist;

        public ObservableCollection<Company> GetList
        {
            get { return _companylist; }
        }

        public Companies()
        {
            StringBuilder __sb = new StringBuilder();
            __sb.Append("select * from VW_DRUGMEMBER;");

            DataTable dt;

            using (MSDBHelper db = new MSDBHelper(__sb.ToString(), util.strDBConn))
            {
                dt = db.GetDataTable();
            }

            _companylist = new ObservableCollection<Company>(
                    (from __row in dt.AsEnumerable()
                     select new Company()
                     {
                         memuid = __row[0].ToString()
                         ,
                         comname = __row[1].ToString()
                         ,
                         comnum = __row[2].ToString()
                         ,
                         comhg = __row[3].ToString()
                         ,
                         comtel = __row[4].ToString()
                     }).ToList<Company>()
                );
        }
    }
}
