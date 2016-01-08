using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TableauDistTool
{
    public class SubscriptionsDA : IDisposable
    {
        SqlConnection _Connection;

        public SubscriptionsDA()
        {
            _Connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["TableauDistConnectionString"].ConnectionString);
            _Connection.Open();
        }

        public void Dispose()
        {
            _Connection.Dispose();
        }

        public List<Subscription> GetSubscriptionsBySchedule(string schedule)
        {
            string sql = @" SELECT s.[Subscription_id]
                                  ,s.[DomainUserName]
	                              ,s.[ReportPath]
	                              ,s.[ReportArgs] 
                            FROM [dbo].[Subscriptions] s WITH(NOLOCK) 
                            INNER JOIN Schedules sd WITH(NOLOCK) ON sd.Schedule_id=s.Schedule_id
                            WHERE sd.ScheduleName=@ScheduleName AND IsValid=1";
            SqlCommand cmd = new SqlCommand(sql, _Connection);
            cmd.Parameters.Add(new SqlParameter("@ScheduleName", schedule));
            return ParseSubscription(cmd.ExecuteReader());
        }

        private List<Subscription> ParseSubscription(SqlDataReader sqlDataReader)
        {
            List<Subscription> subList = new List<Subscription>();
            int iSubscription_id = sqlDataReader.GetOrdinal("Subscription_id");
            int iDomainUserName = sqlDataReader.GetOrdinal("DomainUserName");
            int iReportPath = sqlDataReader.GetOrdinal("ReportPath");
            int iReportArgs = sqlDataReader.GetOrdinal("ReportArgs");
            while (sqlDataReader.Read())
            {
                Subscription sub = new Subscription()
                {
                    Subscription_id = int.Parse(sqlDataReader[iSubscription_id].ToString()),
                    DomainUserName = sqlDataReader[iDomainUserName].ToString(),
                    ReportPath = sqlDataReader[iReportPath].ToString(),
                    ReportArgs = sqlDataReader[iReportArgs].ToString()
                };
                subList.Add(sub);
            }
            sqlDataReader.Close();
            return subList;
        }

        public void InsertSubscriptionLog(List<Subscription> sentList)
        {
            string insertSql = BuildInsertSql(sentList);
            SqlCommand cmd = new SqlCommand(insertSql, _Connection);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }

        private string BuildInsertSql(List<Subscription> sentList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO [SubscriptionsLog]([Subscription_id],[Email],[ReportFullPath])");
            foreach (var sent in sentList)
            {
                if (sent == sentList.Last())
                {
                    sb.AppendLine(string.Format("SELECT '{0}', '{1}', '{2}'", sent.Subscription_id, sent.Email, sent.ReportPath));
                }
                else
                {
                    sb.AppendLine(string.Format("SELECT '{0}', '{1}', '{2}' UNION ALL", sent.Subscription_id, sent.Email, sent.ReportPath));
                }
            }
            return sb.ToString();
        }
    }
}
