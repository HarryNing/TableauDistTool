using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TableauDistTool.Util
{
    public static class AppConfig
    {
        public static string Format = ConfigurationManager.AppSettings["ReportFormat"].ToString();

        public static string TableauServer = ConfigurationManager.AppSettings["TableauServer"].ToString();

        public static string TableauServerTrusted = TableauServer + "/trusted";
    }
}
