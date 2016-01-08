using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableauDistTool.Util
{
    public static class StringHelper
    {
        public static string GetDateString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToShortTimeString();
        }

        public static string GetUserNameByDomainUserName(string domainUserName)
        {
            return domainUserName.Substring(domainUserName.IndexOf("\\") + 1);
        }

        public static string GetEmailByDomainUserName(string domainUserName)
        {
            string userName = GetUserNameByDomainUserName(domainUserName);
            return string.Format("{0}@ef.com", userName);
        }

        public static string GetReportName(string reportPath)
        {
            return reportPath.Substring(reportPath.LastIndexOf("/") + 1);
        }
    }
}
