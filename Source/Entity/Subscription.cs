using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableauDistTool.Util;

namespace TableauDistTool
{
    public class Subscription
    {
        public int Subscription_id { get; set; }

        public string DomainUserName { get; set; }

        public string ReportPath { get; set; }

        public string ReportArgs { get; set; }

        public string Email
        {
            get
            {
                return StringHelper.GetEmailByDomainUserName(DomainUserName);
            }
        }
    }
}
