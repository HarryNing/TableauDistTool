using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableauDistTool.Util
{
    public static class WebHelper
    {
        public static string BuildReportUrl(Subscription sub)
        {
            return AppConfig.TableauServer + "/" + sub.ReportPath + sub.ReportArgs;
        }

        public static string BuildUrl(bool needTicket, string ticket, Subscription sub)
        {
            if (needTicket)
            {
                return string.Format("{0}/{1}/{2}.{3}{4}", AppConfig.TableauServerTrusted, ticket, sub.ReportPath, AppConfig.Format, sub.ReportArgs);
            }
            else
            { return string.Format("{0}/{1}.{2}", AppConfig.TableauServer, sub.ReportPath, AppConfig.Format); }
        }

    }
}
