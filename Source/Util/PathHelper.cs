using System;
using System.IO;

namespace TableauDistTool.Util
{
    public static class PathHelper
    {
        public static string ReportFilePath(Subscription sub)
        {
            string userName = StringHelper.GetUserNameByDomainUserName(sub.DomainUserName);
            string reportName = StringHelper.GetReportName(sub.ReportPath);

            string userDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DistEmail", userName);

            if (!Directory.Exists(userDir))
            {
                Directory.CreateDirectory(userDir);
            }

            string fileName = Path.Combine(userDir, string.Format("{0}.{1}", reportName, AppConfig.Format));
            if (!File.Exists(fileName))
            {
                return fileName;
            }
            else
            {
                return Path.Combine(userDir, string.Format("{0}_{1}.{2}", reportName, sub.Subscription_id, AppConfig.Format));
            }
        }
    }
}