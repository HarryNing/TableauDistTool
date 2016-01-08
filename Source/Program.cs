using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using TableauDistTool.Util;
using System.Threading.Tasks;

namespace TableauDistTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                return;
            }

            string schedule = args[1];

            using (SubscriptionsDA da = new SubscriptionsDA())
            {
                List<Subscription> subList = da.GetSubscriptionsBySchedule(schedule);
                var groupedList = subList.GroupBy(x => x.DomainUserName);
                foreach (var userSubs in groupedList)
                {
                    ProcessSubscription(userSubs);
                }
            }
        }

        private static void ProcessSubscription(IGrouping<string, Subscription> userSubs)
        {
            string cookie = string.Empty;
            foreach (var sub in userSubs)
            {
                string fileName = GenerateReport(sub, sub == userSubs.First(), ref cookie);
                if (fileName != string.Empty)
                {
                    Task.Factory.StartNew(() => { SendEmail(sub, fileName); });
                }
            }
        }

        private static void SendEmail(Subscription sub, string fileName)
        {
            try
            {
                string reportName = StringHelper.GetReportName(sub.ReportPath);
                string email = StringHelper.GetEmailByDomainUserName(sub.DomainUserName);
                string subject = string.Format("Tableau report {0} was executed at {1}", reportName, StringHelper.GetDateString());
                EmailHelper.SendMail(email, subject, WebHelper.BuildReportUrl(sub), fileName);
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex.ToString());
            }
        }

        private static string GenerateReport(Subscription sub, bool isFirst, ref string cookie)
        {
            string url = string.Empty;
            if (isFirst)
            {
                string errMsg = string.Empty;
                string ticket = GetTableauTicket(AppConfig.TableauServerTrusted, sub.DomainUserName, ref errMsg);
                if (ticket != "-1")
                {
                    url = WebHelper.BuildUrl(true, ticket, sub);
                }
                else
                {
                    LogHelper.Write(string.Format("{0} doesn't have access to Tableau Server", sub.DomainUserName));
                    return string.Empty;
                }
            }
            else
            {
                url = WebHelper.BuildUrl(false, null, sub);
            }

            string fileName = PathHelper.ReportFilePath(sub);
            DownloadFile(url, fileName, isFirst, ref cookie);
            return fileName;
        }

        
        static string GetTableauTicket(string tabserver, string tabuser, ref string errMsg)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            // the client_ip parameter isn't necessary to send in the POST unless you have
            // wgserver.extended_trusted_ip_checking enabled (it's disabled by default)
            string postData = "username=" + tabuser + "&client_ip=" + "10.128.44.81";
            byte[] data = enc.GetBytes(postData);

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(tabserver);

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = data.Length;

                // Write the request
                Stream outStream = req.GetRequestStream();
                outStream.Write(data, 0, data.Length);
                outStream.Close();

                // Do the request to get the response
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader inStream = new StreamReader(res.GetResponseStream(), enc);
                string resString = inStream.ReadToEnd();
                inStream.Close();

                return resString;
            }
            // if anything bad happens, copy the error string out and return a "-1" to indicate failure
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                return "-1";
            }
        }

        static void DownloadFile(string url, string fileName, bool isFirst, ref string cookie)
        {
            if (isFirst)
            {
                var tReq = BuildWebRequest(url);
                var tRes = (HttpWebResponse)tReq.GetResponse();
                if (tRes.StatusCode == HttpStatusCode.Found)
                {
                    url = tRes.Headers["Location"];
                    cookie = tRes.Headers["Set-Cookie"];
                }
            }
            var req = BuildWebRequest(url);
            req.Headers["Cookie"] = cookie;
            var res = (HttpWebResponse)req.GetResponse();
            Stream stream = res.GetResponseStream();
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                byte[] buff = new byte[512];
                int c = 0;
                while ((c = stream.Read(buff, 0, 512)) > 0)
                {
                    fs.Write(buff, 0, c);
                }
            }
            stream.Close();
        }

        static HttpWebRequest BuildWebRequest(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            req.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.134 Safari/537.36";
            req.KeepAlive = true;
            req.AllowAutoRedirect = false;
            req.Credentials = CredentialCache.DefaultCredentials;
            return req;
        }
    }
}
