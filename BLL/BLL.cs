using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace BLL
{
    public class BLL
    {
        #region 聊天消息统计

        public DataTable Msg_Sum_Hour(DateTime dtm)
        {
            string table_name = GetTableName_msg(dtm);

            string start = dtm.ToString("yyyy-MM-dd") + " 00:00:00";
            string end = dtm.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00";
            string sql_group_hour = GetGroupData(start, end, SummaryType.hour);

            string sql_main = @"select CONVERT(char(13), dtm, 120) as dtm, 
                                               COUNT(*) as c1 
                                        from {0}
                                        group by  CONVERT(char(13), dtm, 120)";
            sql_main = string.Format(sql_main, table_name);

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as 数量
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group_hour, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        public DataTable Msg_Sum_Mi(DateTime dtm)
        {
            string table_name = GetTableName_msg(dtm);

            string start = dtm.ToString("yyyy-MM-dd") + " 00:00:00";
            string end = dtm.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00";
            string sql_group = GetGroupData(start, end, SummaryType.minite);

            string sql_main = @"select CONVERT(char(16), dtm, 120) as dtm, 
                                               COUNT(*) as c1 
                                        from {0}
                                        group by  CONVERT(char(16), dtm, 120)";
            sql_main = string.Format(sql_main, table_name);

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as 数量
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }

        public string GetTableName_msg(DateTime dtm)
        {
            string out_db = CommonTool.Common.GetAppSetting("out_db");
            string prex = "dbo.msg";
            string subfix = GetTableNameSuffix(dtm);

            string table_name = string.Format("{0}.{1}_{2}", out_db, prex, subfix);
            return table_name;
        }

        public string GetTableName_msg(string dtm)
        {
            string out_db = CommonTool.Common.GetAppSetting("out_db");
            string prex = "dbo.msg";
            string subfix = GetTableNameSuffix(dtm);

            string table_name = string.Format("{0}.{1}_{2}", out_db, prex, subfix);
            return table_name;
        }

        public string GetTableName_msgsum(DateTime dtm)
        {
            string out_db = CommonTool.Common.GetAppSetting("out_db");
            string prex = "dbo.msgsum";
            string subfix = GetTableNameSuffix(dtm);

            string table_name = string.Format("{0}.{1}_{2}", out_db, prex, subfix);
            return table_name;
        }

        public string GetTableName_msgsum(string dtm)
        {
            string out_db = CommonTool.Common.GetAppSetting("out_db");
            string prex = "dbo.msgsum";
            string subfix = GetTableNameSuffix(dtm);

            string table_name = string.Format("{0}.{1}_{2}", out_db, prex, subfix);
            return table_name;
        }
        

        public string GetTableName_login(string dtm)
        {
            string out_db = CommonTool.Common.GetAppSetting("out_db");
            string prex = "dbo.login_on";
            string subfix = GetTableNameSuffix(dtm);

            string table_name = string.Format("{0}.{1}_{2}", out_db, prex, subfix);

            if(Convert.ToDateTime(dtm).Date == DateTime.Now.Date)
            {
                table_name = prex;
            }

            return table_name;
        }
        public string GetTableName_match(string dtm)
        {
            string out_db = CommonTool.Common.GetAppSetting("out_db");
            string prex = "dbo.match";
            string subfix = GetTableNameSuffix(dtm);

            string table_name = string.Format("{0}.{1}_{2}", out_db, prex, subfix);

            if (Convert.ToDateTime(dtm).Date == DateTime.Now.Date)
            {
                table_name = prex;
            }

            return table_name;
        }


        public string GetTableNameSuffix(DateTime dtm)
        {
            return dtm.ToString("yyyy_MM_dd");
        }
        public string GetTableNameSuffix(string dtm)
        {
            DateTime dtm_ = Convert.ToDateTime(dtm);
            return dtm_.ToString("yyyy_MM_dd");
        }

        public string GetGroupData(string start, string end, SummaryType summaryType)
        {
            string strRtn = string.Empty;

            StringBuilder sb = new StringBuilder();
            DateTime dtmStart = Convert.ToDateTime(start);
            DateTime dtmEnd = Convert.ToDateTime(end);
            for (int i = 0; dtmStart <= dtmEnd; i++)
            {
                if (i > 0)
                {
                    sb.Append(" union all ");
                }

                if (summaryType == SummaryType.day)
                {
                    sb.AppendFormat("select '{0}' as dtm ", dtmStart.ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 10));
                    dtmStart = dtmStart.AddDays(1);
                }
                else if (summaryType == SummaryType.hour)
                {
                    sb.AppendFormat("select '{0}' as dtm ", dtmStart.ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 13));
                    dtmStart = dtmStart.AddHours(1);
                }
                else
                {
                    sb.AppendFormat("select '{0}' as dtm ", dtmStart.ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 16));
                    dtmStart = dtmStart.AddMinutes(1);
                }
            }

            strRtn = sb.ToString();
            return strRtn;
        }
        public string GetGroupData(DateTime dtmStart, DateTime dtmEnd, SummaryType summaryType)
        {
            string strRtn = string.Empty;

            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; dtmStart <= dtmEnd; i++)
            {
                if (i > 0)
                {
                    sb.Append(" union all ");
                }

                if (summaryType == SummaryType.day)
                {
                    sb.AppendFormat("select '{0}' as dtm ", dtmStart.ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 10));
                    dtmStart = dtmStart.AddDays(1);
                }
                else if (summaryType == SummaryType.hour)
                {
                    sb.AppendFormat("select '{0}' as dtm ", dtmStart.ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 13));
                    dtmStart = dtmStart.AddHours(1);
                }
                else
                {
                    sb.AppendFormat("select '{0}' as dtm ", dtmStart.ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 16));
                    dtmStart = dtmStart.AddMinutes(1);
                }
            }

            strRtn = sb.ToString();
            return strRtn;
        }
        
        #endregion 

        #region 历史消息

        public DataTable GetDataTable_Msg(string dtm, string uidA, string uidB, ref string dtm_end)
        {
            DataTable dt = GetDataTable_Msg2Dtl(dtm, uidA, uidB);
            //命中，则直接返回
            if (dt.Rows.Count > 0)
            {
                return dt;
            }

            DateTime dtmA = DBHelper.SqlHelper.GetDataItemDateTime(string.Format("select create_time from dbo.[user] where uid = '{0}'", uidA));
            DateTime dtmB = DBHelper.SqlHelper.GetDataItemDateTime(string.Format("select create_time from dbo.[user] where uid = '{0}'", uidB));
            string target_uid = uidA;
            if (dtmB > dtmA)
            {
                target_uid = uidB;
            }
            //循环查找，直到找完全部数据
            string strSql = @"select  CONVERT(char(19), dtm, 120) as dtm
                                from dbo.sum_user 
                                where uid = '{0}' and dtm < '{1}'
                                order by dtm desc";
            strSql = string.Format(strSql, target_uid, Convert.ToDateTime(dtm).ToString("yyyy-MM-dd"));
            DataTable dt_sum = DBHelper.SqlHelper.GetDataTable(strSql);
            for (int i = 0; i < dt_sum.Rows.Count; i++)
            {
                dtm = dt_sum.Rows[i]["dtm"].ToString();
                dt = GetDataTable_Msg2Dtl(dtm, uidA, uidB);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }

                if (i == 7)
                {
                    dtm_end = dtm;
                    break;
                }
            }

            return dt;
        }

        public DataTable GetDataTable_Msg2Dtl(string dtm, string uidA, string uidB)
        {
            string table_name = GetTableName_msg(dtm);
            string strSql = @"select CONVERT(char(19), dtm, 120) as dtm,
                                           uid_from,
                                           uid_to,
                                           state,
                                           type,
                                           txt 
                            from {0}
                            where ((uid_from = '{1}' and uid_to = '{2}') 
                                   or (uid_from = '{2}' and uid_to = '{1}'))
                            order by dtm asc ";
            strSql = string.Format(strSql, table_name, uidA, uidB);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        #endregion 

        #region 统计

        public DataTable Sum_NewUser_Hour(DateTime dtm)
        {
            string start = dtm.ToString("yyyy-MM-dd") + " 00:00:00";
            string end = dtm.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00";
            string sql_group_hour = GetGroupData(start, end, SummaryType.hour);

            string sql_main = @"select CONVERT(char(13), create_time, 120) as dtm, 
                                               COUNT(*) as c1 
                                        from dbo.[user]
                                        where out_type = 1 and create_time >= '{0}' and create_time <= '{1}'
                                        group by  CONVERT(char(13), create_time, 120)";
            sql_main = string.Format(sql_main, start, end);

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as 数量
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group_hour, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }

        public DataTable Sum_NewUser_Day(DateTime start, DateTime end)
        {
            
            string sql_group = GetGroupData(start, end, SummaryType.day);

            string sql_main = @"select CONVERT(char(10), create_time, 120) as dtm, 
                                               COUNT(*) as c1  ,
                                               sum(case when fz_tag = '0' then 1 else 0 end) as c2
                                        from dbo.[user]
                                        where out_type = 1 and create_time >= '{0}' and create_time <= '{1}' 
                                        group by  CONVERT(char(10), create_time, 120)";
            sql_main = string.Format(sql_main, start.ToString("yyyy-MM-dd") + " 00:00:00", end.ToString("yyyy-MM-dd") + " 23:59:59");

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as 数量,b.c2 as 数量2
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }


        public DataTable Sum_KillUser_Day(DateTime start, DateTime end)
        {
            string sql_group = GetGroupData(start, end, SummaryType.day);

            string sql_main = @"select CONVERT(char(10), create_time, 120) as dtm, 
                                               COUNT(*) as c1 
                                        from dbo.kill_user
                                        where create_time >= '{0}' and create_time <= '{1}'
                                        group by  CONVERT(char(10), create_time, 120)";
            sql_main = string.Format(sql_main, start.ToString("yyyy-MM-dd") + " 00:00:00", end.ToString("yyyy-MM-dd") + " 23:59:59");

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as 数量
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }

        public int Summary_NewUser_Count(string start, string end)
        {
            string strSql = @"select COUNT(*)
                                        from dbo.[user]
                                        where out_type = 1 and create_time >= '{0}' and create_time <= '{1}'";
            strSql = string.Format(strSql, start, end);
            int rtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);
            return rtn;
        }

        public DataTable Sum_InnerMsg(DateTime dtm, string name)
        {
            string start = dtm.ToString("yyyy-MM-dd") + " 00:00:00";
            string end = dtm.ToString("yyyy-MM-dd") + " 23:59:59";
            string sql_group_hour = GetGroupData(start, end, SummaryType.hour);

            string sql_main = @"select CONVERT(char(13), dtm, 120) as dtm, 
                                       COUNT(*) as c1,
                                       SUM(case when state = 'success' then 1 else 0 end) as c2
                                from {0}
                                where uid_from in ({1})
                                group by CONVERT(char(13), dtm, 120)";
            string table_name = GetTableName_msg(dtm);
            string uids = GetInnerManUIds(name);
            sql_main = string.Format(sql_main, table_name, uids);

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as c1, b.c2 as c2
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group_hour, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }

        public DataTable Sum_InnerMsg_Min(DateTime dtm, string name, int start_hour, int end_hour)
        {
            string start = dtm.ToString("yyyy-MM-dd") + " " + start_hour + ":00:00";
            string end = dtm.ToString("yyyy-MM-dd") + " " + end_hour + ":59:59";
            string sql_group_hour = GetGroupData(start, end, SummaryType.minite);

            string sql_main = @"select CONVERT(char(16), dtm, 120) as dtm, 
                                       COUNT(*) as c1,
                                       SUM(case when state = 'success' then 1 else 0 end) as c2
                                from {0}
                                where uid_from in ({1})
                                group by CONVERT(char(16), dtm, 120)";
            string table_name = GetTableName_msg(dtm);
            string uids = GetInnerManUIds(name);
            sql_main = string.Format(sql_main, table_name, uids);

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as c1, b.c2 as c2
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group_hour, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }

        public DataTable Sum_UserDtl(string uid, DateTime start, DateTime end)
        {

            string sql_group = GetGroupData(start, end, SummaryType.day);

            string sql_main = @"select CONVERT(char(10), dtm, 120) as dtm,
                                       c_login,
                                       c_match,
                                       c_msg_suc
                                from dbo.sum_user 
                                where uid = '{0}' and dtm >= '{1}' and dtm <= '{2}'";
                               
            sql_main = string.Format(sql_main, uid, start.ToString("yyyy-MM-dd") + " 00:00:00", end.ToString("yyyy-MM-dd") + " 23:59:59");

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.*
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }
 
        #endregion 

        #region 好友 

        public string Get_Friend_Time(string from, string to)
        {
            string strRtn = "";

            string strSql = @"select CONVERT(char(19), dtm, 120) as dtm from dbo.friend where (uid_from = '{0}' and uid_to = '{1}') or (uid_from = '{1}' and uid_to = '{0}')";
            strSql = string.Format(strSql, from, to);
            strRtn = DBHelper.SqlHelper.GetDataItemString(strSql);

            return strRtn;
        }

        #endregion 

        #region 金币、积分;充值、提现
        //提现， 积分兑换
        public double Get_PlatAllTiXianMoney(string tag)
        {
            double d = 0.00;

            string strSql = "select SUM(tx_mny) from dbo.score_tx where state = '{0}' ";
            strSql = string.Format(strSql, tag);
            d = DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return d;
        }

        public double GetUserTotalTiXian(string uid)
        {
            double dRtn = 0.00;

            string strSql = @"select ISNULL(SUM(tx_mny), 0) from dbo.score_tx where uid = '{0}' ";
            strSql = string.Format(strSql, uid);
            dRtn = DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return dRtn;
        }

        public double GetUser_TiXianing(string uid)
        {
            double dRtn = 0.00;

            string strSql = @"select ISNULL(SUM(tx_mny), 0) from dbo.score_tx where state = '待审核' and uid = '{0}' ";
            strSql = string.Format(strSql, uid);
            dRtn = DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return dRtn;
        }

        public int GetUserCoin_Left(string uid)
        {
            int iRtn = 0;

            string strSql = "select dbo.get_user_leftcoin('{0}')";
            strSql = string.Format(strSql, uid);
            iRtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return iRtn;

        }
        public int GetUserCoin_Total(string uid)
        {
            int iRtn = 0;

            string strSql = "select dbo.get_user_totalcoin('{0}')";
            strSql = string.Format(strSql, uid);
            iRtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return iRtn;
        }
        public int GetUserCoin_Used(string uid)
        {
            int iRtn = 0;

            string strSql = "select dbo.get_user_usedcoin('{0}')";
            strSql = string.Format(strSql, uid);
            iRtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return iRtn;
        }

        public System.Data.DataTable GetCoinSourceStats(string uid)
        {
            string strSql = @"
                SELECT TOP 3
                    type_dtl as source_type,
                    SUM(amount) as total_amount,
                    COUNT(*) as count
                FROM dbo.coin
                WHERE uid_from = '{0}' AND type = '增加'
                GROUP BY type_dtl
                ORDER BY total_amount DESC
            ";
            strSql = string.Format(strSql, uid);
            System.Data.DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public System.Data.DataTable GetCoinConsumeStats(string uid)
        {
            string strSql = @"
                SELECT TOP 3
                    type_dtl as consume_type,
                    SUM(amount) as total_amount,
                    COUNT(*) as count
                FROM dbo.coin
                WHERE uid_from = '{0}' AND type = '减少'
                GROUP BY type_dtl
                ORDER BY total_amount DESC
            ";
            strSql = string.Format(strSql, uid);
            System.Data.DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public int GetUserScore_Left(string uid)
        {
            int iRtn = 0;

            string strSql = "select dbo.get_user_leftscore('{0}')";
            strSql = string.Format(strSql, uid);
            iRtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return iRtn;
        }
        public int GetUserScore_Total(string uid)
        {
            int iRtn = 0;

            string strSql = "select dbo.get_user_totalscore('{0}')";
            strSql = string.Format(strSql, uid);
            iRtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return iRtn;
        }
        public int GetUserScore_Used(string uid)
        {
            int iRtn = 0;

            string strSql = "select dbo.get_user_usedscore('{0}')";
            strSql = string.Format(strSql, uid);
            iRtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return iRtn;
        }

        public DataTable GetUserScore_Sum(string uid)
        {
            string strSql = @" select isnull(sum(case when type = '增加' then amount else 0 end), 0) as s_total,
                               isnull(sum(case when type_dtl = '收到礼物' or type_dtl = '同意添加好友' then amount end), 0) as s_gift,
                               isnull(sum(case when type_dtl = '消息收入' then amount end), 0) as s_msg,
                               isnull(sum(case when type_dtl = '上榜奖励'  then amount end), 0) as s_award,
                               isnull(sum(case when type_dtl = '平台操作-增加'  then amount end), 0) as s_plat,
                               isnull(sum(case when type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)'  then amount end), 0) as s_invite,
                               isnull(sum(case when type_dtl = '任务奖励-打招呼' or type_dtl = '任务奖励-回复' or type_dtl = '任务奖励-在线' or type_dtl = '任务奖励-登录'  or type_dtl = '任务奖励-礼物' or type_dtl = '任务奖励' then amount end), 0) as s_task,
                               isnull(sum(case when type_dtl = '任务奖励-聊天' or type_dtl = '聊天额外奖励'  then amount end), 0) as s_task_old
                        from dbo.score 
                        where uid_from = '{0}'";
            strSql = string.Format(strSql, uid);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }
        public DataTable GetUserScore_Sum_Dtm(string uid, string start)
        {
            string strSql = @" select isnull(sum(case when type = '增加' then amount else 0 end), 0) as s_total,
                               isnull(sum(case when type_dtl = '收到礼物' or type_dtl = '同意添加好友' then amount end), 0) as s_gift,
                               isnull(sum(case when type_dtl = '消息收入' then amount end), 0) as s_msg,
                               isnull(sum(case when type_dtl = '上榜奖励'  then amount end), 0) as s_award,
                               isnull(sum(case when type_dtl = '平台操作-增加'  then amount end), 0) as s_plat,
                               isnull(sum(case when type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)'  then amount end), 0) as s_invite,
                               isnull(sum(case when type_dtl = '任务奖励-打招呼' or type_dtl = '任务奖励-回复' or type_dtl = '任务奖励-在线' or type_dtl = '任务奖励-登录'  or type_dtl = '任务奖励-礼物' or type_dtl = '任务奖励' then amount end), 0) as s_task,
                               isnull(sum(case when type_dtl = '任务奖励-聊天' or type_dtl = '聊天额外奖励'  then amount end), 0) as s_task_old
                        from dbo.score 
                        where uid_from = '{0}' and dtm >= '{1}'";

            strSql = string.Format(strSql, uid, start);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        //计算用户礼物得分
        public int GetUserP_Score(string uid)
        {
            int iRtn = 0;

            iRtn = Calculate_Score_Gift(uid);

            return iRtn;
        }

        public DataTable GetScore_Reword(DateTime dtm)
        {
            string strSql = @"select top 10 c.uid_from as uid, 
                                       u.photo,
                                       u.nick,
                                       u.sex,
                                       
                                       us.name,
       
                                       SUM(c.amount) as score_target,
                                       dbo.get_user_totalscore(c.uid_from) as score_total,
                                       ROW_NUMBER() OVER(ORDER BY SUM(c.amount) DESC) AS xh,
                                       0 as r_1,
                                       ISNULL(t.amount, 0) as r_2
                                from dbo.score c 
                                left join dbo.[user] u on (c.uid_from = u.uid)
                                left join dbo.user_s us on (c.uid_from = us.uid)
                                left join (select uid_from as uid, amount from dbo.score where type_dtl = '上榜奖励' and dtm = '{0}') t on (c.uid_from = t.uid) 
                                where (c.type='增加' and state = '成功' and (type_dtl = '收到礼物'  or type_dtl = '同意添加好友'  or type_dtl = '上榜奖励' or (type_dtl = '任务奖励-聊天' or type_dtl = '任务奖励-登录' or type_dtl = '任务奖励-礼物') )  and c.type_dtl != '上榜奖励')
                                and (dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59')
                                group by c.uid_from,
		                                   u.photo,
		                                   u.nick,
		                                   u.sex,us.name,t.amount
                                order by score_target desc";

            strSql = string.Format(strSql, dtm.ToString("yyyy-MM-dd"));
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }
       
        public bool Save_ScoreReword(DateTime dtm, string uid, int score_r)
        {
            bool bRtn = false;

            //保存检查(检查日期，只生成近3天的数据；检查是否已经生成)
            DateTime dtm_now = DateTime.Now;
            TimeSpan ts = dtm_now - dtm;
            int diff_day = ts.Days;
            if (diff_day < 1 || diff_day > 3)
            {
                bRtn = false;
                return bRtn;
            }

            string dtm_str = dtm.ToString("yyyy-MM-dd");

            string strSql = "select 1 from dbo.score where uid_from = '{0}' and dtm = '{1}' and type='增加' and type_dtl = '上榜奖励'";
            strSql = string.Format(strSql, uid, dtm_str);
            string tag = DBHelper.SqlHelper.GetDataItemString(strSql);
            if(tag == "1")
            {
                bRtn = false;
                return bRtn;
            }

            strSql = @"insert into dbo.score(dtm, uid_from, amount, type, type_dtl, comment, state)
                                      values('{0}', '{1}', {2}, '增加', '上榜奖励','', '成功' )";
            strSql = string.Format(strSql, dtm_str, uid, score_r);
            int i = DBHelper.SqlHelper.ExecuteSql(strSql);
            if(i > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        //统计用户-金币情况
        public DataTable Get_UserCoin_Sum(string uid, string dtm_YYYYMMDD)
        {
            string strSql = @"select isnull(sum(case when [type] = '增加' then amount else 0 end), 0) as c_add,
       isnull(sum(case when [type] = '减少' then amount else 0 end), 0) as c_sub
from dbo.coin where uid_from = '{0}' and dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59'";
            strSql = string.Format(strSql, uid, dtm_YYYYMMDD);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }
        //统计用户-充值情况
        public DataTable Get_UserChargeMny_Sum(string uid, string dtm_YYYYMMDD)
        {
            string strSql = @"select isnull(sum(case when create_time >= '{1} 00:00:00' and create_time <= '{1} 23:59:59' then pay_mny else 0 end), 0) as mny_day,
       isnull(sum(pay_mny), 0) as mny
from dbo.sys_order where uid = '{0}' and order_state = '付款成功'";
            strSql = string.Format(strSql, uid, dtm_YYYYMMDD);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public DataTable Get_UserScore_Sum(string uid, string dtm_YYYYMMDD)
        {
            string strSql = @"select isnull(sum(case when [type] = '增加' and (dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59') then amount else 0 end), 0) as s_add,
       isnull(sum(case when [type] = '减少' and type_dtl = '提现' then amount else 0 end), 0) as s_sub
from dbo.score where uid_from = '{0}'";
            strSql = string.Format(strSql, uid, dtm_YYYYMMDD);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        #endregion 

        #region 统计分析

        public DataTable GetSumScore_Day(string start, string end)
        {
            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm,
                                       sum(case when type='增加' then amount else 0 end) as score_total,
                                       sum(case when type='增加' and (type_dtl='收到礼物' or type_dtl='同意添加好友') then amount else 0 end) as score_coin,
                                       sum(case when type='增加' and (type_dtl='任务奖励-登录'  or type_dtl='任务奖励-在线' or type_dtl='任务奖励-聊天' or type_dtl='聊天额外奖励'  or type_dtl='任务奖励-礼物') then amount else 0 end) as score_task,
                                       sum(case when type='增加' and (type_dtl='上榜奖励'  or type_dtl='平台操作-增加' ) then amount else 0 end) as score_plat
                                from dbo.score
                                where dtm >= '{0} 00:00:00' and dtm <= '{1} 23:59:59'
                                group by CONVERT(char(10), dtm, 120)
                                order by dtm asc";
            strSql = string.Format(strSql, start, end);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;

        }

        public DataTable GetSumData_Day(DateTime start, DateTime end, string uid)
        {
            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(uid)) 
            {
                where = string.Format(" uid='{0}' ", uid);
            }

            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm, 
                                       COUNT(uid) as c_user,
                                       SUM(c_login) as c_login,
                                       SUM(c_match) as c_match,
                                       SUM(c_match_) as c_match_,
                                       SUM(c_msg_suc) as c_msg_suc,
                                       SUM(c_msg_suc_) as c_msg_suc_,
                                       SUM(c_msg_fail) as c_msg_fail,
                                       SUM(c_msg_fail_) as c_msg_fail_
                                from dbo.sum_user 
                                where {2} and dtm >= '{0}' and dtm <= '{1}'
                                group by dtm 
                                order by dtm asc";
            strSql = string.Format(strSql, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"), where);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }

        public DataTable GetSum_UserScore(DateTime start, DateTime end, string uid)
        {

            string sql_group = GetGroupData(start, end, SummaryType.day);

            string sql_main = @"select CONVERT(char(10), dtm, 120) as dtm,
                                sum(amount) as c1,
                                sum(case when type_dtl = '收到礼物' or type_dtl = '同意添加好友' or type_dtl = '消息收入' or  type_dtl = '语音收入' then amount else 0 end) as c2,
                                sum(case when type_dtl = '任务奖励' or type_dtl = '任务奖励-打招呼' or type_dtl = '任务奖励-登录' or  type_dtl = '任务奖励-回复' or type_dtl = '任务奖励-礼物'  or type_dtl = '任务奖励-聊天'  or type_dtl = '任务奖励-在线' then amount else 0 end) as c3
                                from dbo.score 
                                where uid_from = '{0}'
                                and [type] = '增加'
                                and dtm >= '{1}' and dtm <= '{2}'
                                group by CONVERT(char(10), dtm, 120)";
            sql_main = string.Format(sql_main, uid, start.ToString("yyyy-MM-dd") + " 00:00:00", end.ToString("yyyy-MM-dd") + " 23:59:59");

            string strSql = @"with 
                                a as ({0}),
                                b as ({1})
                                select a.dtm,
                                       b.c1 as 数量, 
                                       isnull(b.c2,0) as score_gift, 
                                       isnull(b.c3,0) as score_task, 
                                       b.c1 - isnull(b.c2,0) - isnull(b.c3,0) as score_other
                                from a 
                                left join b on (a.dtm = b.dtm)
                                order by a.dtm asc";
            strSql = string.Format(strSql, sql_group, sql_main);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;

        }

        #endregion 


        public bool Kill_User(string uid, string hour, string reason)
        {
            bool bRtn = false;

            if (string.IsNullOrEmpty(hour)) { hour = "0"; }

            //检查是否已经封杀了，10分钟内不重复封杀同一个人
            string strSql = "select 1 from dbo.kill_user where uid = '{0}' and dtm > DATEADD(Mi, -10, getdate())";
            strSql = string.Format(strSql, uid);
            string str_tag = DBHelper.SqlHelper.GetDataItemString(strSql);
            if (!string.IsNullOrEmpty(str_tag))
            {
                bRtn = true;
                return bRtn;
            }

            strSql = @"insert into dbo.kill_user (uid, kill_hour, reason, state, kill_type, comment)
                              values ('{0}', {1}, '{2}', '封杀', '小火柴后台手动封杀', '')";
            strSql = string.Format(strSql, uid, hour, reason);

            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }


        public bool Warn_User(string uid, string sec, string reason)
        {
            bool bRtn = false;

            if (string.IsNullOrEmpty(sec)) { sec = "0"; }

            //检查是否已经警告过了，3分钟内不重复警告同一个人
            string strSql = "select 1 from dbo.warn_user where uid = '{0}' and dtm > DATEADD(Mi, -3, getdate())";
            strSql = string.Format(strSql, uid);
            string str_tag = DBHelper.SqlHelper.GetDataItemString(strSql);
            if (!string.IsNullOrEmpty(str_tag))
            {
                bRtn = true;
                return bRtn;
            }

            strSql = @"insert into dbo.warn_user (uid, warn_sec, reason, state, warn_type, comment)
                              values ('{0}', {1}, '{2}', '警告', '小火柴后台手动警告', '')";
            strSql = string.Format(strSql, uid, sec, reason);

            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        //计算礼物得分（礼物得分规则，参考礼物得分规则表【绩效表格】）
        public int Calculate_Score_Gift(string uid)
        {
            int irtn = 0;

            Common com = new Common();
            string fest_name = com.GetSysDicByKey("fest_name");

            string strSql = @"select sum(amount) as c
from dbo.score 
where uid_from = '{0}' and (type_dtl = '收到礼物' or type_dtl = '同意添加好友' or type_dtl = '消息收入')";
           
            strSql = string.Format(strSql, uid);
            double score = DBHelper.SqlHelper.GetDataItemDouble(strSql);
            irtn = (int)(score / 1000);

            return irtn;
        }


        #region inner统计（得分、聊天）

        public Dictionary<string, double> GetGiftScoreDic()
        {
            Dictionary<string, double> dicScore = new Dictionary<string, double>();
            dicScore.Add("收徒", 0.001);
            dicScore.Add("好友", 0.5);
            dicScore.Add("棒棒糖", 0.2); dicScore.Add("鲜花", 0.5); dicScore.Add("黄瓜", 1); dicScore.Add("啪啪啪", 2);
            dicScore.Add("幸运星", 3); dicScore.Add("巧克力", 5); dicScore.Add("气球", 6); dicScore.Add("口红", 8);
            dicScore.Add("香水", 10); dicScore.Add("包包", 20); dicScore.Add("钻戒", 30); dicScore.Add("水晶鞋", 50);
            dicScore.Add("皇冠", 60); dicScore.Add("法拉利", 80); dicScore.Add("飞机", 100); dicScore.Add("火箭", 180);
            return dicScore;
        }

        public Dictionary<string, int> Get_InnerScore(string dtm, string user_name)
        {
            Dictionary<string, int> dicRtn = new Dictionary<string, int>();

            DateTime dtm_ = Convert.ToDateTime(dtm);

            int score_chart = 0;
            int score_gift = 0;
            int score_chartBlackQ = 0;
            int score_other = 0;

            //得分奖励部分（休息日、工作日的非工作时间两部分）
            int score_chart_rest = 0;
            int score_gift_rest = 0;
            int score_chartBlackQ_rest = 0;

            //性能优化处理(根据聊天量，判断是否继续进行计算)
            bool check = Check_Calculate(dtm_, user_name);
            if(!check)
            {
                dicRtn.Add("chart", score_chart);
                dicRtn.Add("gift", score_gift);
                dicRtn.Add("score_chartBlackQ", score_chartBlackQ);
                dicRtn.Add("chart_rest", score_chart_rest);
                dicRtn.Add("gift_rest", score_gift_rest);
                dicRtn.Add("score_chartBlackQ_rest", score_chartBlackQ_rest);

                return dicRtn;
            }

            score_chart = Calculate_Score_Chart(dtm_, user_name);
            score_gift = Calculate_Score_Gift(dtm_, user_name);
            score_chartBlackQ = Calculate_Score_Chart_BlackQ(dtm_, user_name);
            score_other = Calculate_Score_Other(dtm_, user_name);

            //得分奖励部分
            bool is_restDay = Is_RestDay(dtm);
            double rate = GetRestRate();
            if (is_restDay)
            {
                //休息日
                score_chart_rest = (int)(score_chart * (rate - 1));
                score_gift_rest = (int)(score_gift * (rate - 1));
                score_chartBlackQ_rest = (int)(score_chartBlackQ * (rate - 1));
            }
            else
            {
                //工作日的非工作时间两部分
                Common com = new Common();
                string rest_can_use = com.GetSysDicByKey("rest_can_use");

                if (rest_can_use == "1")
                {
                    score_chart_rest = (int)(Calculate_Score_Chart_Rest(dtm_, user_name) * (rate - 1));
                    score_gift_rest = (int)(Calculate_Score_Gift_Rest(dtm_, user_name) * (rate - 1));
                    score_chartBlackQ_rest = (int)(Calculate_Score_Chart_BlackQ_Rest(dtm_, user_name) * (rate - 1));
                }
            }

            dicRtn.Add("chart", score_chart);
            dicRtn.Add("gift", score_gift);
            dicRtn.Add("score_chartBlackQ", score_chartBlackQ);
            dicRtn.Add("score_other", score_other);

            dicRtn.Add("chart_rest", score_chart_rest);
            dicRtn.Add("gift_rest", score_gift_rest);
            dicRtn.Add("score_chartBlackQ_rest", score_chartBlackQ_rest);

            return dicRtn;
        }

        public bool Check_Calculate(DateTime dtm, string user_name)
        {
            bool brtn = false;

            string uids = GetInnerManUIds(user_name);
            string table_name = GetTableName_msg(dtm);

            string strSql = "select COUNT(*) as c1 from {0} where uid_from in ({1})";
            strSql = string.Format(strSql, table_name, uids);

            double d = DBHelper.SqlHelper.GetDataItemDouble(strSql);
            if(d > 0)
            {
                brtn = true;
            }

            return brtn;
        }

        //计算聊天得分（每个小时聊天量>xxx则得1分）
        public int Calculate_Score_Chart(DateTime dtm, string user_name)
        {
            int irtn = 0;

            string uids = GetInnerManUIds(user_name);
            string table_name = GetTableName_msg(dtm);

            string strSql = @" select CONVERT(char(13), dtm, 120) as dtm, 
                                       COUNT(*) as c1,
                                       SUM(case when state = 'success' then 1 else 0 end) as c2
                                from {0}
                                where uid_from in ({1})
                                group by CONVERT(char(13), dtm, 120)";
           
            strSql = string.Format(strSql, table_name, uids);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            int hour = 0;
            int c2 = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    hour = Convert.ToInt32(dt.Rows[i]["dtm"].ToString().Substring(11, 2));
                    c2 = Convert.ToInt32(dt.Rows[i]["c2"].ToString());

                    if (c2 >= 150)
                    {
                        irtn += 1;
                    }
                    if (c2 >= 300)
                    {
                        irtn += 1;
                    }
                    //if (c2 >= 400)
                    //{
                    //    irtn += 1;
                    //}
                }
            }

            return irtn;
        }

        //消除黑圈得分
        public int Calculate_Score_Chart_BlackQ(DateTime dtm, string user_name)
        {
            int irtn = 0;

            string uids = GetInnerManUIds(user_name);
            string table_name = GetTableName_msg(dtm);

            string strSql = @"select uid_from, uid_to, dbo.is_date_vip(uid_to, '{2}') as isdayvip,
                                       COUNT(*) as c1
                                from {0} 
                                where uid_from in ({1})
                                and state = 'success'
                                group by uid_from, uid_to,dbo.is_date_vip(uid_to, '{2}')
                                having COUNT(*) > 5
                                order by uid_from, uid_to, c1 desc ";


            strSql = string.Format(strSql, table_name, uids, dtm.ToString("yyyy-MM-dd"));

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            int count = dt.Rows.Count;

            irtn = count / 10;

            //计算会员部分
            //...
            int count_vip = 0;
            int isdayvip = 0;
            for (int i = 0; i < count; i++)
            {
                isdayvip = Convert.ToInt32(dt.Rows[i]["isdayvip"]);
                if (isdayvip > 0)
                {
                    count_vip++;
                }
            }
            int score_vip = count_vip / 5;
           
            irtn += score_vip;

            return irtn;
        }

        //计算礼物得分（礼物得分规则，参考礼物得分规则表【绩效表格】）
        public int Calculate_Score_Gift(DateTime dtm, string user_name)
        {
            Dictionary<string, double> dicScore = GetGiftScoreDic();

            int irtn = 0;

            string uids = GetInnerManUIds(user_name);
            Common com = new Common();
            string fest_name = com.GetSysDicByKey("fest_name");

            string strSql = @"select 
                                       isnull(sum(case when type_dtl = '同意添加好友' then 1 else 0 end ), 0) as count_好友,
                                       isnull(sum(case when type_dtl = '领取徒弟收益' then amount else 0 end ), 0) as count_收徒,
       
                                       isnull(sum(case when comment = '棒棒糖' or comment = '{2}' then 1 else 0 end ), 0) as count_棒棒糖,
                                       isnull(sum(case when comment = '鲜花' then 1 else 0 end ), 0) as count_鲜花,
                                       isnull(sum(case when comment = '黄瓜' then 1 else 0 end ), 0) as count_黄瓜,
                                       isnull(sum(case when comment = '啪啪啪' then 1 else 0 end ), 0) as count_啪啪啪,
                                       isnull(sum(case when comment = '幸运星' then 1 else 0 end ), 0) as count_幸运星,


                                       isnull(sum(case when comment = '巧克力' then 1 else 0 end ), 0) as count_巧克力,
                                       isnull(sum(case when comment = '气球' then 1 else 0 end ), 0) as count_气球,
                                       isnull(sum(case when comment = '口红' then 1 else 0 end ), 0) as count_口红,
       
                                       isnull(sum(case when comment = '香水' then 1 else 0 end ), 0) as count_香水,
                                       isnull(sum(case when comment = '包包' then 1 else 0 end ), 0) as count_包包,
                                       isnull(sum(case when comment = '钻戒' then 1 else 0 end ), 0) as count_钻戒,
                                       isnull(sum(case when comment = '水晶鞋' then 1 else 0 end ), 0) as count_水晶鞋,
                                       isnull(sum(case when comment = '皇冠' then 1 else 0 end ), 0) as count_皇冠,
                                       isnull(sum(case when comment = '法拉利' then 1 else 0 end ), 0) as count_法拉利,
                                       isnull(sum(case when comment = '飞机' then 1 else 0 end ), 0) as count_飞机,
                                       isnull(sum(case when comment = '火箭' then 1 else 0 end ), 0) as count_火箭
                                from dbo.score 
                                where uid_from in ({1}) and (type='增加' and (type_dtl='收到礼物' or type_dtl='同意添加好友' or type_dtl='领取徒弟收益') and (dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59') and uid_to not in ('{3}')  )
                             ";
            string uids_not = GetInnerBoyUids();
            strSql = string.Format(strSql, dtm.ToString("yyyy-MM-dd"), uids, fest_name, uids_not.Replace(",", "','"));
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (string key in dicScore.Keys)
                {
                    irtn += (int)(Convert.ToInt32(dt.Rows[0]["count_" + key].ToString()) * dicScore[key]);
                }
            }

            return irtn;
        }


        public int Calculate_Score_Other(DateTime dtm, string user_name)
        {
            Dictionary<string, double> dicScore = GetGiftScoreDic();

            int irtn = 0;

            string strWhere = "1=1";
            if(!string.IsNullOrEmpty(user_name))
            {
                strWhere = string.Format("name = '{0}'", user_name);
            }
            

            string strSql = @"select sum(s_other)
                                from dbo.sum_score 
                                where dtm = '{0}' and {1}";
                               
            strSql = string.Format(strSql, dtm.ToString("yyyy-MM-dd"), strWhere);
            irtn = (int)DBHelper.SqlHelper.GetDataItemDouble(strSql);

            return irtn;
        }
        #region 聊天奖励部分

        //计算聊天得分（每个小时聊天量>120则得1分）
        public int Calculate_Score_Chart_Rest(DateTime dtm, string user_name)
        {
            int irtn = 0;

            string uids = GetInnerManUIds(user_name);
            string table_name = GetTableName_msg(dtm);

            string strWhere = GetRestWhere("dtm", dtm);

            string strSql = @" select CONVERT(char(13), dtm, 120) as dtm, 
                                       COUNT(*) as c1,
                                       SUM(case when state = 'success' then 1 else 0 end) as c2
                                from {0}
                                where uid_from in ({1}) and ({2})
                                group by CONVERT(char(13), dtm, 120)";

            strSql = string.Format(strSql, table_name, uids, strWhere);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            int hour = 0;
            int c2 = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    hour = Convert.ToInt32(dt.Rows[i]["dtm"].ToString().Substring(11, 2));
                    c2 = Convert.ToInt32(dt.Rows[i]["c2"].ToString());

                    if (c2 >= 150)
                    {
                        irtn += 1;
                    }
                    if (c2 >= 300)
                    {
                        irtn += 1;
                    }
                    //if (c2 >= 400)
                    //{
                    //    irtn += 1;
                    //}
                }
            }

            return irtn;
        }

        //消除黑圈得分
        public int Calculate_Score_Chart_BlackQ_Rest(DateTime dtm, string user_name)
        {
            int irtn = 0;

            string uids = GetInnerManUIds(user_name);
            string table_name = GetTableName_msg(dtm);

            string strWhere = GetRestWhere("dtm", dtm);

            string strSql = @"select uid_from, uid_to, dbo.is_date_vip(uid_to, '{3}') as isdayvip,
                                       COUNT(*) as c1
                                from {0} 
                                where uid_from in ({1})
                                and state = 'success' and ({2})
                                group by uid_from, uid_to,dbo.is_date_vip(uid_to, '{3}')
                                having COUNT(*) > 5
                                order by uid_from, uid_to, c1 desc ";


            strSql = string.Format(strSql, table_name, uids, strWhere, dtm.ToString("yyyy-MM-dd"));

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            int count = dt.Rows.Count;

            irtn = count / 10;

            //计算会员部分
            //...
            int count_vip = 0;
            int isdayvip = 0;
            for (int i = 0; i < count; i++)
            {
                isdayvip = Convert.ToInt32(dt.Rows[i]["isdayvip"]);
                if (isdayvip > 0)
                {
                    count_vip++;
                }
            }
            int score_vip = count_vip / 5;
           
            irtn += score_vip;

            return irtn;
        }

        //计算礼物得分（礼物得分规则，参考礼物得分规则表【绩效表格】）
        public int Calculate_Score_Gift_Rest(DateTime dtm, string user_name)
        {
            Dictionary<string, double> dicScore = GetGiftScoreDic();

            int irtn = 0;

            string uids = GetInnerManUIds(user_name);
            Common com = new Common();
            string fest_name = com.GetSysDicByKey("fest_name");
            string strWhere = GetRestWhere("dtm", dtm);

            string strSql = @"select 
                                       isnull(sum(case when type_dtl = '同意添加好友' then 1 else 0 end ), 0) as count_好友,
       
                                       isnull(sum(case when comment = '棒棒糖' or comment = '{2}' then 1 else 0 end ), 0) as count_棒棒糖,
                                       isnull(sum(case when comment = '鲜花' then 1 else 0 end ), 0) as count_鲜花,
                                       isnull(sum(case when comment = '黄瓜' then 1 else 0 end ), 0) as count_黄瓜,
                                       isnull(sum(case when comment = '啪啪啪' then 1 else 0 end ), 0) as count_啪啪啪,
                                       isnull(sum(case when comment = '杜蕾斯' then 1 else 0 end ), 0) as count_杜蕾斯,

                                       isnull(sum(case when comment = '水晶拉珠' then 1 else 0 end ), 0) as count_水晶拉珠,
                                       isnull(sum(case when comment = '珍珠T裤' then 1 else 0 end ), 0) as count_珍珠T裤,
                                       isnull(sum(case when comment = '小怪兽' then 1 else 0 end ), 0) as count_小怪兽,
       
                                       isnull(sum(case when comment = '香水' then 1 else 0 end ), 0) as count_香水,
                                       isnull(sum(case when comment = '包包' then 1 else 0 end ), 0) as count_包包,
                                       isnull(sum(case when comment = '钻戒' then 1 else 0 end ), 0) as count_钻戒,
                                       isnull(sum(case when comment = '水晶鞋' then 1 else 0 end ), 0) as count_水晶鞋,
                                       isnull(sum(case when comment = '皇冠' then 1 else 0 end ), 0) as count_皇冠,
                                       isnull(sum(case when comment = '法拉利' then 1 else 0 end ), 0) as count_法拉利,
                                       isnull(sum(case when comment = '飞机' then 1 else 0 end ), 0) as count_飞机,
                                       isnull(sum(case when comment = '火箭' then 1 else 0 end ), 0) as count_火箭
                                from dbo.score
                                where uid_from in ({1}) and (type='增加' and (type_dtl='收到礼物' or type_dtl='同意添加好友') and ({0}) and uid_to not in ('{3}') )
                             ";
            string uids_not = GetInnerBoyUids();
            strSql = string.Format(strSql, strWhere, uids, fest_name, uids_not.Replace(",", "','"));
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (string key in dicScore.Keys)
                {
                    irtn += (int)(Convert.ToInt32(dt.Rows[0]["count_" + key].ToString()) * dicScore[key]);
                }
            }

            return irtn;
        }

        #endregion

        //计算礼物得分（根据时间段计算）
        public int Calculate_Score_Gift_Dtm(DateTime dtm_start, DateTime dtm_end, string user_name)
        {
            Dictionary<string, double> dicScore = GetGiftScoreDic();

            int irtn = 0;

            string uids = GetInnerManUIds(user_name);
            Common com = new Common();
            string fest_name = com.GetSysDicByKey("fest_name");

            string strSql = @"select 
                                       isnull(sum(case when type_dtl = '同意添加好友' then 1 else 0 end ), 0) as count_好友,
                                       isnull(sum(case when type_dtl = '领取徒弟收益' then 1 else 0 end ), 0) as count_收徒,

                                       isnull(sum(case when comment = '棒棒糖' or comment = '{2}' then 1 else 0 end ), 0) as count_棒棒糖,
                                       isnull(sum(case when comment = '鲜花' then 1 else 0 end ), 0) as count_鲜花,
                                       isnull(sum(case when comment = '黄瓜' then 1 else 0 end ), 0) as count_黄瓜,
                                       isnull(sum(case when comment = '啪啪啪' then 1 else 0 end ), 0) as count_啪啪啪,
                                       isnull(sum(case when comment = '幸运星' then 1 else 0 end ), 0) as count_幸运星,

                                       isnull(sum(case when comment = '巧克力' then 1 else 0 end ), 0) as count_巧克力,
                                       isnull(sum(case when comment = '气球' then 1 else 0 end ), 0) as count_气球,
                                       isnull(sum(case when comment = '口红' then 1 else 0 end ), 0) as count_口红,
       
                                       isnull(sum(case when comment = '香水' then 1 else 0 end ), 0) as count_香水,
                                       isnull(sum(case when comment = '包包' then 1 else 0 end ), 0) as count_包包,
                                       isnull(sum(case when comment = '钻戒' then 1 else 0 end ), 0) as count_钻戒,
                                       isnull(sum(case when comment = '水晶鞋' then 1 else 0 end ), 0) as count_水晶鞋,
                                       isnull(sum(case when comment = '皇冠' then 1 else 0 end ), 0) as count_皇冠,
                                       isnull(sum(case when comment = '法拉利' then 1 else 0 end ), 0) as count_法拉利,
                                       isnull(sum(case when comment = '飞机' then 1 else 0 end ), 0) as count_飞机,
                                       isnull(sum(case when comment = '火箭' then 1 else 0 end ), 0) as count_火箭
                                from dbo.score
                                where uid_from in ({1}) and (type='增加' and (type_dtl='收到礼物' or type_dtl='同意添加好友' or type_dtl='领取徒弟收益') and ({0}) and uid_to not in ('{3}') )
                             ";

            string strWhere = "(dtm >'{0}' and dtm <'{1}') ";
            strWhere = string.Format(strWhere, dtm_start.ToString(), dtm_end.ToString());
            string uids_not = GetInnerBoyUids();
            strSql = string.Format(strSql, strWhere, uids, fest_name, uids_not.Replace(",", "','"));
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (string key in dicScore.Keys)
                {
                    irtn += (int)(Convert.ToInt32(dt.Rows[0]["count_" + key].ToString()) * dicScore[key]);
                }
            }

            return irtn;
        }

        //获取全部统计对象
        public List<string> GetInnerMan()
        {
            List<string> list = new List<string>();

            DataTable dt = GetUserSName();
            for (int i = 0; i < dt.Rows.Count; i++ )
            {
                list.Add(dt.Rows[i]["name"].ToString());
            }

            return list;
        }

        public string GetInnerManUIds(string name)
        {
            string strRtn = string.Empty;

            if(string.IsNullOrEmpty(name))
            {
                name = "";
            }
            else
            {
                name = name.Trim();
            }

            if (string.IsNullOrEmpty(name))
            {
                strRtn = "select uid from dbo.user_s where is_use = 1";
            }
            else
            {
                strRtn = "select uid from dbo.user_s where is_use = 1 and name='{0}' ";
                strRtn = string.Format(strRtn, name);
            }

            return strRtn;
        }

        public string GetInnerBoyUids()
        {
            string uids = CommonTool.Common.GetAppSetting("i_boy");
            return uids;
        }

        public bool Is_RestDay(string dtm)
        {
            bool bRtn = false;

            Common com = new Common();
            string restDay = com.GetSysDicByKey("rest_day");
            string[] ary = restDay.Split(new char[]{','});
            List<string> listRest = new List<string>(ary);

            DateTime dtm_ = Convert.ToDateTime(dtm);
            string day = dtm_.Day.ToString();
            if (listRest.Contains(day))
            {
                bRtn = true;
            }

            return bRtn;
        }

        public double GetRestRate()
        {
            double iRtn = 1;

            Common com = new Common();
            string restRate = com.GetSysDicByKey("rest_rate");

            iRtn = Convert.ToDouble(restRate);

            return iRtn;
        }

        //(工作日的非工作部分：9点之前，中午1点到2点，下午7点之后)
        public string GetRestWhere(string field, DateTime dtm)
        {
            string date = dtm.ToString("yyyy-MM-dd");
            string strWhere = "";
            strWhere = "({0}>'{1} 00:00:00' and {0}<'{1} 09:00:00') ";
            strWhere = string.Format(strWhere, field, date);
            return strWhere;
        }

        #endregion 

        #region 内部得分统计

        /// <summary>
        /// 生成某一天全部的数据
        /// </summary>
        /// <param name="dtm"></param>
        /// <returns></returns>
        public int Sum_UserS(DateTime dtm)
        {
            int count = 0;
            int total = 0;

            List<string> listMan = GetInnerMan();
            foreach(string name in listMan)
            {
                total += Create_UserS_One(dtm, name);
            }

            count = listMan.Count;

            return count;
        }

        /// <summary>
        /// 生成某一个人的数据，并保存
        /// </summary>
        /// <param name="dtm"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public int Create_UserS_One(DateTime dtm, string user_name)
        {
            int total = 0;

            Dictionary<string, string> dicData = Sum_UserS_One(dtm, user_name);
            foreach(string key in dicData.Keys)
            {
                if(key == "s_msg" 
                    || key == "s_blackq"
                    || key == "s_gift"
                    || key == "s_msg_r"
                    || key == "s_blackq_r"
                    || key == "s_gift_r" )
                {
                    total += Convert.ToInt32(dicData[key]);
                }
                
            }

            if (dicData.ContainsKey("total"))
            {
                dicData["total"] = total.ToString();
            }
            else
            {
                dicData.Add("total", total.ToString());
            }

            //生成数据到数据库
            string strSql = "select id from dbo.sum_score where dtm = '{0}' and name = '{1}'";
            strSql = string.Format(strSql, dtm.ToString("yyyy-MM-dd"), user_name);
            string key_id = DBHelper.SqlHelper.GetDataItemString(strSql);
            if (string.IsNullOrEmpty(key_id))
            {
                SumScore_Insert(dicData);
            }
            else
            {
                SumScore_Update(key_id, dicData);
            }

            return total;
        }

        public int Update_UserS_One(string id, int s_other, string s_other_note)
        {
            int total = 0;

            return total;
        }

        public Dictionary<string, string> Sum_UserS_One(DateTime dtm, string user_name)
        {
            Dictionary<string, string> dicRtn = new Dictionary<string, string>();

            dicRtn.Add("dtm", dtm.ToString("yyyy-MM-dd"));
            dicRtn.Add("name", user_name);

            string table_name = GetTableName_msg(dtm);
            string uids = GetInnerManUIds(user_name);
            string strSql = "";

            //得分等级
            string score_lv = "1";
            strSql = "select MAX(score_lv) from dbo.user_s where  name = '{0}'";
            strSql = string.Format(strSql, user_name);
            score_lv = DBHelper.SqlHelper.GetDataItemString(strSql);
            dicRtn.Add("score_lv", score_lv);

            //聊天量统计
            string c_msg = "0";
            string c_msg_suc = "0";
            strSql = @" select  
                                       COUNT(*) as c1,
                                       ISNULL(SUM(case when state = 'success' then 1 else 0 end), 0) as c2
                                from {0}
                                where uid_from in ({1})";
            strSql = string.Format(strSql, table_name, uids);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            if(dt != null && dt.Rows.Count > 0)
            {
                c_msg = dt.Rows[0]["c1"].ToString();
                c_msg_suc = dt.Rows[0]["c2"].ToString();
            }
            dicRtn.Add("c_msg", c_msg);
            dicRtn.Add("c_msg_suc", c_msg_suc);

            //消除黑圈统计
            string c_blackq = "0";
            string c_blackq_men = "0";
            strSql = @"with a 
                        as (                             
                        select COUNT(*) as c1,
                               dbo.is_date_vip(uid_to, '{2}') as c2
                        from {0} msg
                        where uid_from in ({1})
                        and state = 'success'
                        group by uid_from, uid_to
                        having COUNT(*) > 5)
                        select COUNT(*) as c1,
                               ISNULL(sum(case when c2 >0 then 1 else 0 end), 0) as c2
                        from a";
            strSql = string.Format(strSql, table_name, uids, dtm.ToString("yyyy-MM-dd"));
            dt = DBHelper.SqlHelper.GetDataTable(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                c_blackq = dt.Rows[0]["c1"].ToString();
                c_blackq_men = dt.Rows[0]["c2"].ToString();
            }
            dicRtn.Add("c_blackq", c_blackq);
            dicRtn.Add("c_blackq_men", c_blackq_men);

            //核心6项得分
            Dictionary<string, int> dic = Get_InnerScore(dtm.ToString("yyyy-MM-dd"), user_name);
            dicRtn.Add("s_msg", dic["chart"].ToString());
            dicRtn.Add("s_blackq", dic["score_chartBlackQ"].ToString());
            dicRtn.Add("s_gift", dic["gift"].ToString());
            dicRtn.Add("s_msg_r", dic["chart_rest"].ToString());
            dicRtn.Add("s_blackq_r", dic["score_chartBlackQ_rest"].ToString());
            dicRtn.Add("s_gift_r", dic["gift_rest"].ToString());

            //其他得分
            //dicRtn.Add("s_other", "0");
            //dicRtn.Add("s_other_note", "");

            //扣分
            //dicRtn.Add("s_sub", "0");
            //dicRtn.Add("s_sub_note", "");

            return dicRtn;
        }


        #endregion 

        #region 内部人员相关

        public DataTable GetInnerOnlineUser()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/GetCnfigUids_Cur";
            string uids = CommonTool.Common.GetHtmlFromUrl(url);

            string tbl_name = GetTableName_msg(DateTime.Now);

            string strSql = @"with a as
                                (
                                  select to_uid as uid, COUNT(*) as c
                                  from dbo.match 
                                  where to_uid in ('{0}') and dtm > dateadd(mi,-{2},getdate())
                                  group by to_uid
                                ),
                                b as 
                                (
                                  select uid_from as uid,
                                         SUM(case when state='success' then 1 else 0 end) as send_suc,
                                         SUM(case when state='fail' then 1 else 0 end) as send_fail
                                  from {1}
                                  where uid_from in ('{0}') and dtm > dateadd(mi,-{2},getdate())
                                  group by uid_from
                                ),
                                b1 as 
                                (
                                  select uid_to as uid,
                                         SUM(case when state='success' then 1 else 0 end) as send_suc_,
                                         SUM(case when state='fail' then 1 else 0 end) as send_fail_
                                  from {1}
                                  where uid_to in ('{0}') and dtm > dateadd(mi,-{2},getdate())
                                  group by uid_to
                                )

                                select
                                           u.uid,
                                           u.photo,
                                           u.nick,
                                           u.sex, us.name,
                                           dbo.get_user_totalscore(u.uid) as total_score,
                                          ISNULL(a.c ,0)  as match_,
                                          ISNULL(b.send_suc ,0) as send_suc,
                                          ISNULL(b.send_fail ,0) as send_fail,
                                          ISNULL(b1.send_suc_ ,0) as send_suc_,
                                          ISNULL(b1.send_fail_ ,0) as send_fail_ 
                                          
                                    from dbo.[user] u 
                                    left join a on(u.uid = a.uid)
                                    left join b on(u.uid = b.uid)
                                    left join b1 on(u.uid =b1.uid)
                                    left join dbo.user_s us on (u.uid = us.uid)
                                    where u.uid in ('{0}') order by name desc";

            strSql = string.Format(strSql, uids.Replace(",", "','"), tbl_name, 10);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        public DataTable GetUserSName()
        {
            string strSql = "select distinct name from dbo.user_s where is_use = 1 order by name desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        #endregion 

        #region 接收新用户匹配

        public DataTable Get_ReceMathData()
        {
            string strSql = @"select rm.name, 
                                       rm.uid, 
                                       CONVERT(char(20), rm.dtm, 120) as dtm_start,
                                       CONVERT(char(20), GETDATE(), 120) as dtm_end,
                                       DATEDIFF(MINUTE, rm.dtm, GETDATE()) as online_mi,
       
                                       u.photo, u.nick, u.sex,
                                       dbo.get_user_totalscore(rm.uid) as total_score,
       
                                       0 as rece_match_count,
                                       0 as rece_match_user,
                                       0 as rece_msg_user,
                                       0 as rece_msg_count,
                                       0 as rece_msg_count_sucess,
                                       0 as rece_msg_count_fail,
                                       0 as send_msg_user,
                                       0 as send_msg_count,
                                       0 as send_msg_count_sucess,
                                       0 as send_msg_count_fail
 
                                from dbo.rece_match rm 
                                left join dbo.[user] u on (rm.uid = u.uid) order by name asc, nick desc, dtm_end desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            //循环每条数据，计算匹配聊天数据
            string uid = "";
            string dtm_start = "";
            string dtm_end = "";
            Dictionary<string, string> dic_item = new Dictionary<string, string>();
            int row_count = dt.Rows.Count;
            for(int i=0; i<row_count; i++)
            {
                uid = dt.Rows[i]["uid"].ToString();
                dtm_start = dt.Rows[i]["dtm_start"].ToString();
                dtm_end = dt.Rows[i]["dtm_end"].ToString();

                dic_item.Clear();
                dic_item = Get_User_ReceMatchData(uid, dtm_start, dtm_end);

                dt.Rows[i]["rece_match_count"] = dic_item["rece_match_count"];
                dt.Rows[i]["rece_match_user"] = dic_item["rece_match_user"];
                dt.Rows[i]["rece_msg_user"] = dic_item["rece_msg_user"];
                dt.Rows[i]["rece_msg_count"] = dic_item["rece_msg_count"];
                dt.Rows[i]["rece_msg_count_sucess"] = dic_item["rece_msg_count_sucess"];
                dt.Rows[i]["rece_msg_count_fail"] = dic_item["rece_msg_count_fail"];
                dt.Rows[i]["send_msg_user"] = dic_item["send_msg_user"];
                dt.Rows[i]["send_msg_count"] = dic_item["send_msg_count"];
                dt.Rows[i]["send_msg_count_sucess"] = dic_item["send_msg_count_sucess"];
                dt.Rows[i]["send_msg_count_fail"] = dic_item["send_msg_count_fail"];
            }

            return dt;
        }

        public DataTable Get_ReceMathData(string dtm, string name)
        {
            string strWhere = "";
            if(!string.IsNullOrEmpty(name))
            {
                strWhere = string.Format("and name = '{0}'", name);
            }

            string strSql = @"with a as --结束时间
                                (
                                select * from dbo.rece_match_r 
                                where dtm >= '{0} 00:00:00' 
                                and dtm <= '{0} 23:59:59'
                                {1}
                                and opt_state = 0),
                                b as --开始时间
                                (
                                select * from dbo.rece_match_r 
                                where dtm >= '{0} 00:00:00' 
                                and dtm <= '{0} 23:59:59'
                                {1}
                                and opt_state = 1)

                                select a.name,
                                       a.uid,
                                       convert(char(20), (select MAX(dtm) from b where a.uid = b.uid and b.dtm < a.dtm ), 120) as dtm_start,
                                       convert(char(20), a.dtm, 120) as dtm_end,
                                       DATEDIFF(MINUTE, (select MAX(dtm) from b where a.uid = b.uid and b.dtm < a.dtm ), a.dtm) as online_mi,
       
                                       u.photo, u.nick, u.sex,
                                       dbo.get_user_totalscore(a.uid) as total_score,
       
                                       0 as rece_match_count,
	                                   0 as rece_match_user,
	                                   0 as rece_msg_user,
	                                   0 as rece_msg_count,
	                                   0 as rece_msg_count_sucess,
	                                   0 as rece_msg_count_fail,
	                                   0 as send_msg_user,
	                                   0 as send_msg_count,
	                                   0 as send_msg_count_sucess,
	                                   0 as send_msg_count_fail
                                from a
                                left join dbo.[user] u on(a.uid = u.uid) order by  name asc, nick desc, dtm_end desc";
            strSql = string.Format(strSql, dtm, strWhere);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            //循环每条数据，计算匹配聊天数据
            string uid = "";
            string dtm_start = "";
            string dtm_end = "";
            Dictionary<string, string> dic_item = new Dictionary<string, string>();
            int row_count = dt.Rows.Count;
            for (int i = 0; i < row_count; i++)
            {
                uid = dt.Rows[i]["uid"].ToString();
                dtm_start = dt.Rows[i]["dtm_start"].ToString();
                dtm_end = dt.Rows[i]["dtm_end"].ToString();

                dic_item.Clear();
                dic_item = Get_User_ReceMatchData(uid, dtm_start, dtm_end);

                dt.Rows[i]["rece_match_count"] = dic_item["rece_match_count"];
                dt.Rows[i]["rece_match_user"] = dic_item["rece_match_user"];
                dt.Rows[i]["rece_msg_user"] = dic_item["rece_msg_user"];
                dt.Rows[i]["rece_msg_count"] = dic_item["rece_msg_count"];
                dt.Rows[i]["rece_msg_count_sucess"] = dic_item["rece_msg_count_sucess"];
                dt.Rows[i]["rece_msg_count_fail"] = dic_item["rece_msg_count_fail"];
                dt.Rows[i]["send_msg_user"] = dic_item["send_msg_user"];
                dt.Rows[i]["send_msg_count"] = dic_item["send_msg_count"];
                dt.Rows[i]["send_msg_count_sucess"] = dic_item["send_msg_count_sucess"];
                dt.Rows[i]["send_msg_count_fail"] = dic_item["send_msg_count_fail"];
            }

            return dt;
        }

        public DataTable Calculate_User_ReceMatchData(string uid, string dtm_start, string dtm_end)
        {
            string table_name_math = "dbo.match";
            string table_name_msg = "";

            string aft_fix = GetTableNameSuffix(dtm_start);

            DateTime dtm_start_ = Convert.ToDateTime(dtm_start);
            if (dtm_start_.Date < DateTime.Now.Date)
            {
                //历史数据
                table_name_math = "xhc_daily.dbo.match_" + aft_fix;
                table_name_msg = "xhc_daily.dbo.msg_" + aft_fix;
            }
            else
            {
                //当天
                table_name_math = "dbo.match";
                table_name_msg = "xhc_daily.dbo.msg_" + GetTableNameSuffix(DateTime.Now);
            }

            #region sql
            string strSql = @"begin 

--接收匹配情况
declare @rece_match_count integer
declare @rece_match_user integer

select @rece_match_count = COUNT(*), @rece_match_user = COUNT(distinct uid)
from {3}
where dtm >= '{1}' and dtm <= '{2}'
and to_uid = '{0}'  


--接收消息情况
declare @rece_msg_user integer
declare @rece_msg_count integer
declare @rece_msg_count_sucess integer
declare @rece_msg_count_fail integer
 
select @rece_msg_user = COUNT(distinct uid_from) ,
       @rece_msg_count = COUNT(*) ,
       @rece_msg_count_sucess = isnull(sum(case when state = 'success' then 1 else 0 end),0) ,
       @rece_msg_count_fail = isnull(sum(case when state = 'fail' then 1 else 0 end),0) 
from {4} msg
where dtm >= '{1}' and dtm <= '{2}'
and uid_to = '{0}'
and uid_from in (select uid
				from {3}
				where dtm >= '{1}' and dtm <= '{2}'
				and to_uid = '{0}' )
				
				
---回复情况
declare @send_msg_user integer
declare @send_msg_count integer
declare @send_msg_count_sucess integer
declare @send_msg_count_fail integer
 
select @send_msg_user = COUNT(distinct uid_to) ,
       @send_msg_count = COUNT(*) ,
       @send_msg_count_sucess = isnull(sum(case when state = 'success' then 1 else 0 end),0),
       @send_msg_count_fail = isnull(sum(case when state = 'fail' then 1 else 0 end),0) 
from {4} msg
where dtm >= '{1}' and dtm <= '{2}'
and uid_from = '{0}'
and uid_to in (select uid
				from {3}
				where dtm >= '{1}' and dtm <= '{2}'
				and to_uid = '{0}' )

select 
       @rece_match_count as rece_match_count,
       @rece_match_user as rece_match_user,
       
       @rece_msg_user as rece_msg_user,
       @rece_msg_count as rece_msg_count,
       @rece_msg_count_sucess as rece_msg_count_sucess,
       @rece_msg_count_fail as rece_msg_count_fail,
       
       @send_msg_user as send_msg_user,
       @send_msg_count as send_msg_count,
       @send_msg_count_sucess as send_msg_count_sucess,
       @send_msg_count_fail as send_msg_count_fail
       
end";
            #endregion 

            strSql = string.Format(strSql, uid, dtm_start, dtm_end, table_name_math, table_name_msg);

            DataTable dt_row = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt_row;
        }

        public Dictionary<string, string> Get_User_ReceMatchData(string uid, string dtm_start, string dtm_end)
        {
            DataTable dt_row = Calculate_User_ReceMatchData(uid, dtm_start, dtm_end);
            string str = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt_row);
            return CommonTool.JsonHelper.GetParms2(str);
        }

        #endregion 


        #region 聊天积分相关

        public DataTable GetChatScoreSum_Hour(DateTime dtm)
        {
            string strDtm = dtm.ToString("yyyy-MM-dd");
            string strSql = @"select CONVERT(char(13), dtm, 120) as dtm,
       COUNT(id) as chat_count,
       sum(amount) as chat_score
from dbo.score_msg 
where dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59'
group by CONVERT(char(13), dtm, 120)
order by dtm asc";
            strSql = string.Format(strSql, strDtm);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }
        public DataTable GetChatScoreSum_Day(int days)
        {
            string start = DateTime.Now.AddDays(-days).ToString("yyyy-MM-dd");
            string end = DateTime.Now.ToString("yyyy-MM-dd");
            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm,
       COUNT(id) as chat_count,
       sum(amount) as chat_score
from dbo.score_msg 
where dtm >= '{0} 00:00:00' and dtm <= '{1} 23:59:59'
group by CONVERT(char(10), dtm, 120)
order by dtm asc";
            strSql = string.Format(strSql, start, end);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public DataTable GetChatCoinSum_Day(int days)
        {
            string start = DateTime.Now.AddDays(-days).ToString("yyyy-MM-dd");
            string end = DateTime.Now.ToString("yyyy-MM-dd");
            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm,
       COUNT(id) as chat_count,
       sum(amount) as chat_coin
from dbo.coin_msg 
where dtm >= '{0} 00:00:00' and dtm <= '{1} 23:59:59'
group by CONVERT(char(10), dtm, 120)
order by dtm asc";
            strSql = string.Format(strSql, start, end);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public string GetSQL_ChatScoreGrid(string from_uid, string to_uid, string dtm, string filter, string eval)
        {
            string sub_1 = @"select uid_from, uid_to, 
                                   COUNT(*) as msg_count,
                                   sum(amount) as msg_socre,
                                   max(dtm) as dtm_end,
                                   min(dtm) as dtm_start
                            from dbo.score_msg 
                            where dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59'
                            group by uid_from, uid_to";
            sub_1 = string.Format(sub_1, dtm);

            string sub_2 = @"select uid_from, uid_to, 
                                   COUNT(*) as msg_count_coin,
                                   sum(amount) as msg_coin,
                                   max(dtm) as dtm_end_coin,
                                   min(dtm) as dtm_start_coin
                            from dbo.coin_msg 
                            where dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59'
                            group by uid_from, uid_to";
            sub_2 = string.Format(sub_2, dtm);

            string msgsum_table = GetTableName_msgsum(dtm);
            string sub_3 = @"select msg1.uid_from, msg1.uid_to, 
								       msg1.from_vip, msg1.to_vip,
									   msg1.msg_count as send_msg_count, 
									   msg1.msg_suc as send_msg_suc, 
									   msg1.msg_fail as send_msg_fail,
									   msg1.dtm_start as send_dtm_start, 
									   msg1.dtm_end as send_dtm_end,
								       
									   ISNULL(msg2.msg_count,0) as rec_msg_count, 
									   ISNULL(msg2.msg_suc,0)  as rec_msg_suc, 
									   ISNULL(msg2.msg_fail,0)  as rec_msg_fail,
									   ISNULL(msg2.dtm_start,0)  as rec_dtm_start, 
									   ISNULL(msg2.dtm_end,0)  as rec_dtm_end
								from {0} msg1
								left join {0} msg2 on (msg1.uid_from = msg2.uid_to and msg1.uid_to = msg2.uid_from)";
            sub_3 = string.Format(sub_3, msgsum_table);

            

            string strSql = @"select CONVERT(char(20), sc.dtm_start,120) as dtm_start,CONVERT(char(20), sc.dtm_end,120) as dtm_end,
                               sc.msg_socre,sc.msg_count,
                               ISNULL(cc.msg_coin,0) as msg_coin,ISNULL(cc.msg_count_coin,0) as msg_count_coin,
                               u.uid, u.photo,u.nick,u.sex,
                               CONVERT(char(20), u.create_time,120) as create_time,
                               u2.uid as from_uid, u2.photo as from_photo,u2.nick as from_nick,u2.sex as from_sex,u2.chat_mny_score,
                               CONVERT(char(20), u2.create_time,120) as from_create_time,
                               
                               msg.from_vip,
                               msg.to_vip,
                               msg.send_msg_count,
                               msg.send_msg_suc,
                               msg.send_msg_fail,
                               CONVERT(char(20), msg.send_dtm_start,120) as send_dtm_start,
                               CONVERT(char(20), msg.send_dtm_end,120) as send_dtm_end,
                               
                               msg.rec_msg_count,
                               msg.rec_msg_suc,
                               msg.rec_msg_fail,
                               CONVERT(char(20), msg.rec_dtm_start,120) as rec_dtm_start,
                               CONVERT(char(20), msg.rec_dtm_end,120) as rec_dtm_end
                               
                        from ({0}) sc
                        left join ({1}) cc on (sc.uid_from = cc.uid_to and sc.uid_to = cc.uid_from)
                        left join ({2}) msg on (sc.uid_from = msg.uid_from and sc.uid_to = msg.uid_to)
                        left join dbo.[user] u on (sc.uid_to = u.uid) 
                        left join dbo.[user] u2 on (sc.uid_from = u2.uid) 
                        where 1=1 {3}";
            string strWhere = "";
            if (!string.IsNullOrEmpty(from_uid))
            {
                strWhere += string.Format(" and sc.uid_from='{0}'", from_uid);
            }
            if (!string.IsNullOrEmpty(to_uid))
            {
                strWhere += string.Format(" and sc.uid_to='{0}'", to_uid);
            }

            strSql = string.Format(strSql,sub_1, sub_2,sub_3, strWhere);

            return strSql;
        }

        public string CopyCoinScore_ToCache()
        {
            string strSql = @"insert into dbo.coin_msg
                              select * from dbo.coin where type_dtl = '消息支出' and dtm > (select max(dtm) from dbo.coin_msg)";
            int count_coin = DBHelper.SqlHelper.ExecuteSql(strSql);

            strSql = @"insert into dbo.score_msg 
                       select * from dbo.score where type_dtl = '消息收入' and dtm > (select max(dtm) from dbo.score_msg)";
            int count_score = DBHelper.SqlHelper.ExecuteSql(strSql);

            string strRtn = string.Format("消息支出【{0}】条（金币），消息收入【{1}】条（积分）", count_coin, count_score);
            return strRtn;
        }

        public bool Check_AppData_I()
        {
            bool bRtn = true;


            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/Home/Index";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            if (rst != "hello...")
            {
                CommonTool.WriteLog.Write("!!!数据接口服务异常...");

                //发送短信
                External.SMS.SendMsg("13545121451", "验证码:[999000],数据接口异常，需要紧急处理", CommonTool.Common.GetAppSetting("appName"));

                bRtn = false;
                return bRtn;
            }

            return bRtn;
        }

        #endregion 

        #region 数据库表操作

        #region 签约用户表 user_s
        public string UserS_Insert(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[user_s]";
            model.Type = DBHelper.ExecuteType.Insert;
            foreach (string key in dicParm.Keys)
            {
                if (key == "is_use"
                    || key == "is_use_outnet"
                    || key == "score_lv")
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key], CommonTool.JsonValueType.Number));
                    continue;
                }
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }
            model.ListFieldItem.Add(new DBHelper.FieldItem("id", id));
            int tag = model.Execute();
            if (tag > 0)
            {
                strRtn = id;
            }

            return strRtn;
        }

        public bool UserS_Update(string id, Dictionary<string, string> dicParm)
        {
            bool bRtn = false;

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[user_s]";
            model.Type = DBHelper.ExecuteType.Update;
            model.PrimaryKey = "id";
            model.OnlyFlag = id;

            foreach (string key in dicParm.Keys)
            {
                if (key == "is_use"
                    || key == "is_use_outnet"
                    || key == "score_lv")
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key], CommonTool.JsonValueType.Number));
                    continue;
                }
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }

            int tag = model.Execute();
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        public DataTable UserS_List_byId(string id)
        {
            string strSql = @"select id,
                                       uid,
                                       name,
                                       is_use,
                                       is_use_outnet,
                                       score_lv
                                from dbo.user_s where id = '{0}'";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public DataTable UserS_List_byCondition(string strWhere)
        {
            string strSql = @"select id,
                                       uid,
                                       name,
                                       is_use,
                                       is_use_outnet,
                                       score_lv
                                from dbo.user_s where {0}";
            strSql = string.Format(strSql, strWhere);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public int UserS_Delete(string strWhere)
        {
            int intRtn = -1;
            string strSql = @"delete from dbo.[user_s]
                              where {0}";
            strSql = string.Format(strSql, strWhere);
            intRtn = DBHelper.SqlHelper.ExecuteSql(strSql);
            return intRtn;
        }

        #endregion

        #region 签约用户绩效统计表 sum_score
        public string SumScore_Insert(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[sum_score]";
            model.Type = DBHelper.ExecuteType.Insert;
            foreach (string key in dicParm.Keys)
            {
                if (key == "s_msg"
                    || key == "s_blackq"
                    || key == "s_gift"
                    || key == "s_msg_r"
                    || key == "s_blackq_r"
                    || key == "s_gift_r"
                    || key == "s_other"
                    || key == "s_sub"

                    || key == "score_lv"
                    || key == "c_msg"
                    || key == "c_msg_suc"
                    || key == "c_blackq"
                    || key == "c_blackq_men"
                    || key == "total")
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key], CommonTool.JsonValueType.Number));
                    continue;
                }
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }
            model.ListFieldItem.Add(new DBHelper.FieldItem("id", id));
            int tag = model.Execute();
            if (tag > 0)
            {
                strRtn = id;
            }

            return strRtn;
        }

        public bool SumScore_Update(string id, Dictionary<string, string> dicParm)
        {
            bool bRtn = false;

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[sum_score]";
            model.Type = DBHelper.ExecuteType.Update;
            model.PrimaryKey = "id";
            model.OnlyFlag = id;

            foreach (string key in dicParm.Keys)
            {
                if (key == "s_msg"
                    || key == "s_blackq"
                    || key == "s_gift"
                    || key == "s_msg_r"
                    || key == "s_blackq_r"
                    || key == "s_gift_r"
                    || key == "s_other"
                    || key == "s_sub"

                    || key == "score_lv"
                    || key == "c_msg"
                    || key == "c_msg_suc"
                    || key == "c_blackq"
                    || key == "c_blackq_men"
                    || key == "total")
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key], CommonTool.JsonValueType.Number));
                    continue;
                }
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }

            int tag = model.Execute();
            if (tag > 0)
            {
                bRtn = true;
                //更新成功之后从新计算总得分
                string strSql = "update dbo.sum_score set total = s_gift + s_blackq + s_msg + s_gift_r + s_blackq_r + s_msg_r + s_other - s_sub where id = '{0}'";
                strSql = string.Format(strSql, id);
                DBHelper.SqlHelper.ExecuteSql(strSql);
            }

            return bRtn;
        }

        public DataTable SumScore_List_byId(string id)
        {
            string strSql = @"select id, CONVERT(char(10), dtm, 120) as dtm,
                                                       name,
                                                       score_lv,
       
                                                       c_blackq,
                                                       c_blackq_men,
                                                       c_msg,
                                                       c_msg_suc,
       
                                                       s_blackq,
                                                       s_blackq_r,
                                                       s_gift,
                                                       s_gift_r,
                                                       s_msg,
                                                       s_msg_r,
       
                                                       s_other,
                                                       s_other_note,
       
                                                       s_sub,
                                                       s_sub_note,
       
                                                       total
                                from dbo.sum_score where id = '{0}'";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public DataTable SumScore_List_byCondition(string strWhere)
        {
            string strSql = @"select *
                                from dbo.sum_score where {0}";
            strSql = string.Format(strSql, strWhere);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public int SumScore_Delete(string strWhere)
        {
            int intRtn = -1;
            string strSql = @"delete from dbo.[sum_score]
                              where {0}";
            strSql = string.Format(strSql, strWhere);
            intRtn = DBHelper.SqlHelper.ExecuteSql(strSql);
            return intRtn;
        }

        #endregion

        #region 陪玩表

        public string Play_Insert(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[user_play]";
            model.Type = DBHelper.ExecuteType.Insert;
            foreach (string key in dicParm.Keys)
            {
                //if (key == "refurbish_count")
                //{
                //    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key], CommonTool.JsonValueType.Number));
                //    continue;
                //}
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }
            model.ListFieldItem.Add(new DBHelper.FieldItem("id", id));
            int tag = model.Execute();
            if (tag > 0)
            {
                strRtn = id;
            }

            return strRtn;
        }

        public bool Play_Update(string id, Dictionary<string, string> dicParm)
        {
            bool bRtn = false;

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[user_play]";
            model.Type = DBHelper.ExecuteType.Update;
            model.PrimaryKey = "id";
            model.OnlyFlag = id;

            foreach (string key in dicParm.Keys)
            {
                //if (key == "refurbish_count")
                //{
                //    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key], CommonTool.JsonValueType.Number));
                //    continue;
                //}
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }

            int tag = model.Execute();
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        public DataTable Play_List_byId(string id)
        {
            DataTable dt = new DataTable();
            string strSql = @"select    
		                                  create_time
                              from dbo.[seek_people]
                              where id = '{0}'";
            strSql = string.Format(strSql, id);

            dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public int Play_Delete(string strWhere)
        {
            int intRtn = -1;
            string strSql = @"delete from dbo.[xxxxx]
                              where {0}";
            strSql = string.Format(strSql, strWhere);
            intRtn = DBHelper.SqlHelper.ExecuteSql(strSql);
            return intRtn;
        }
        #endregion

        #endregion 

        #region 邀请好友，代理系统

        public DataTable InviteFrined_SumScore(string uid)
        {
            string date_1 = DateTime.Now.ToString("yyyy-MM-dd");
            string date_2 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string date_30 = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

            string strSql = @"select ISNULL(sum(case when type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)'  then amount else 0 end), 0) as score_total,
       ISNULL(sum(case when type_dtl='邀请收益(充值)' then amount else 0 end), 0) as score_pay,
       ISNULL(sum(case when type_dtl='邀请收益(礼物)' then amount else 0 end), 0) as score_gift,
       ISNULL(sum(case when type_dtl='邀请收益(私信)' then amount else 0 end), 0) as score_msg,
       
       ISNULL(sum(case when (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)') and (dtm>='{1} 00:00:00' and dtm<='{1} 23:59:59') then amount else 0 end), 0) as score_1,
       ISNULL(sum(case when (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)') and (dtm>='{2} 00:00:00' and dtm<='{2} 23:59:59') then amount else 0 end), 0) as score_2,
       ISNULL(sum(case when (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)') and (dtm>='{3} 00:00:00' and dtm<='{1} 23:59:59') then amount else 0 end), 0) as score_3,
       0 as fried_count, 0 as coin_invite
from dbo.score 
where uid_from = '{0}'
and (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)' )";
            strSql = string.Format(strSql, uid, date_1, date_2, date_30);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        //统计---好友数量
        public int InviteFriend_Sum_Num(string uid)
        {
            string strSql = "select COUNT(*) as c1, ISNULL(sum(case when reg_complete = 1 then 1 else 0 end), 0) as c2 from dbo.[user] where fz_tag = '{0}'";
            strSql = string.Format(strSql, uid);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            int count = Convert.ToInt32(dt.Rows[0]["c1"].ToString());
            return count;
        }

        public List<string> Get_Invite_Agent(DateTime dtm)
        {
            List<string> list = new List<string>();
            string str_dtm = dtm.ToString("yyyy-MM-dd");
            string strSql = "";

            //有充值的代理
            strSql = @"select u.fz_tag, sum(o.pay_mny) as mny
from dbo.sys_order o
left join dbo.[user] u on (o.uid = u.uid)
where o.order_state = '付款成功' 
and (o.dtm_pay >= '{0} 00:00:00' and o.dtm_pay <= '{0} 23:59:59')
and u.fz_tag <> '0'
group by u.fz_tag";
            strSql = string.Format(strSql, str_dtm);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(dt.Rows[i]["fz_tag"].ToString());
            }

            //有积分的代理
            strSql = @"select u.fz_tag,
       sum(amount) as score,
       sum(case when s.type_dtl = '收到礼物' then amount else 0 end) as score_gift,
       sum(case when s.type_dtl = '消息收入' then amount else 0 end) as score_msg
from dbo.score s
left join dbo.[user] u on (s.uid_from = u.uid)
where (s.type_dtl = '收到礼物' or s.type_dtl = '消息收入')
and (s.dtm >= '{0} 00:00:00' and s.dtm <= '{0} 23:59:59')
and u.fz_tag <> '0'
group by u.fz_tag";
            strSql = string.Format(strSql, str_dtm);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string uid;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                uid = dt.Rows[i]["fz_tag"].ToString();
                if(!list.Contains(uid))
                {
                    list.Add(uid);
                }
            }

            return list;
        }

        public int Create_InviteFriend_Score(string uid, DateTime dtm)
        {
            int iRows = 0;

            string strDtm = dtm.ToString("yyyy-MM-dd");

            //计算充值收益
            string strSql = @"select ISNULL(sum(pay_mny),0) as paymny from dbo.sys_order 
where uid in (select uid from dbo.[user] where fz_tag = '{0}') 
and order_state = '付款成功' 
and (dtm_pay >= '{1} 00:00:00' and dtm_pay <= '{1} 23:59:59') ";
            strSql = string.Format(strSql, uid, strDtm);
            double pay_mny = DBHelper.SqlHelper.GetDataItemDouble(strSql);

            //计算积分收益（礼物 + 消息）
            strSql = @"select ISNULL(sum(case when type_dtl='收到礼物' then amount else 0 end),0) as score_gift,
       ISNULL(sum(case when type_dtl='消息收入' then amount else 0 end),0) as score_msg
from dbo.score 
where uid_from in (select uid from dbo.[user] where fz_tag = '{0}') 
and (dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59')
and type = '增加'
and (type_dtl = '收到礼物' or type_dtl = '消息收入')";
            strSql = string.Format(strSql, uid, strDtm);
            DataTable dt_score = DBHelper.SqlHelper.GetDataTable(strSql);
            int score_gift = Convert.ToInt32(dt_score.Rows[0]["score_gift"].ToString());
            int score_msg = Convert.ToInt32(dt_score.Rows[0]["score_msg"].ToString());

            //整理数据保存进数据库
            int inviteScore_mny = (int)(pay_mny * 1000 * Invite_ScoreRate.pay_mny);
            int inviteScore_gift = (int)(score_gift * Invite_ScoreRate.gift_score);
            int inviteScore_msg = (int)(score_msg * Invite_ScoreRate.msg_score);

            string strSql_All = "";
            if (inviteScore_mny > 0)
            {
                strSql = Create_Invite_Sql(uid, strDtm, inviteScore_mny, "邀请收益(充值)", Invite_ScoreRate.pay_mny);
                strSql_All += strSql;
                iRows++;
            }

            if (inviteScore_gift > 0)
            {
                strSql = Create_Invite_Sql(uid, strDtm, inviteScore_gift, "邀请收益(礼物)", Invite_ScoreRate.gift_score);
                strSql_All += strSql;
                iRows++;
            }
            if (inviteScore_msg > 0)
            {
                strSql = Create_Invite_Sql(uid, strDtm, inviteScore_msg, "邀请收益(私信)", Invite_ScoreRate.msg_score);
                strSql_All += strSql;
                iRows++;
            }

            if (!string.IsNullOrEmpty(strSql_All))
            {
                int tag = DBHelper.SqlHelper.ExecuteSql(strSql_All);
            }

            return iRows;
        }

        private string Create_Invite_Sql(string uid, string dtm, int score, string type, double comment)
        {
            //对时间做额外处理,如果生成的是历史数据（不是当日数据的情况）
            string dtm_yyyyMMdd = Convert.ToDateTime(dtm).ToString("yyyy-MM-dd");
            string dtm_today = DateTime.Now.ToString("yyyy-MM-dd");
            string dtm_score = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (dtm_yyyyMMdd != dtm_today)
            {
                dtm_score = dtm_yyyyMMdd + " 23:59:00";
            }

            string strSql = @"
if exists (select 1 from dbo.score where uid_from = '{0}' and type_dtl = '{3}' and dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59')
begin 
   update dbo.score set amount = {2} where uid_from = '{0}' and type_dtl = '{3}' and dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59'
end 
else 
begin
   insert into dbo.score(dtm, uid_from, amount, type, type_dtl, comment, state)
   values ('{5}', '{0}', {2}, '增加', '{3}', '{4}', '成功');
end 
";

            strSql = string.Format(strSql, uid, dtm, score, type, comment, dtm_score);
            return strSql;
        }


        //统计部分（管理后台）
        public DataTable GetInviteSumData(string start, string end, string fz_tag)
        {
            string dt_dtm = GetGroupData(start, end, SummaryType.day);

            string dt_score = @"select  CONVERT(char(10), dtm, 120) as dtm,
                                    sum(amount) as score
                            from dbo.score 
                            where (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)') and (dtm >= '{0} 00:00:00' and dtm <= '{1} 23:59:59')
                            group by CONVERT(char(10), dtm, 120) ";
            dt_score = string.Format(dt_score, start, end);

            string dt_user = @"select CONVERT(char(10), create_time, 120) as dtm,
                                   COUNT(uid) as c1,
                                   sum(case when reg_complete = 1 then 1 else 0 end) as c2
                            from dbo.[user] 
                            where fz_tag <> '0' and (create_time >= '{0} 00:00:00' and create_time <= '{1} 23:59:59')
                            group by CONVERT(char(10), create_time, 120)";
            dt_user = string.Format(dt_user, start, end);

            string strSql = @"select a.dtm,
                                   b.score,
                                   c.c1, c.c2
                            from ({0}) as a
                            left join ({1}) b on (a.dtm = b.dtm)
                            left join ({2}) c on (a.dtm = c.dtm)
                            order by a.dtm asc";
            strSql = string.Format(strSql, dt_dtm, dt_score, dt_user);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }


        #endregion 

        //删除图片
        public int DeletePic_DefinePhoto()
        {
            //删除图片 E:\work\数据接口\WebAppFrame\photo\photo_define
            string base_dic = @"E:\work\数据接口\WebAppFrame\photo\photo_define";
            DirectoryInfo dic = new DirectoryInfo(base_dic);
            FileInfo[] ary_f = dic.GetFiles();
            FileInfo f;
            string file_name;
            bool exists;
            int ii = 0;
            for (int i = 0; i < ary_f.Length; i++)
            {
                f = ary_f[i];
                file_name = f.Name;
                CommonTool.WriteLog.Write(file_name);

                exists = DBHelper.SqlHelper.Exist("dbo.user_update", "val_new", "photo_define/" + file_name);
                if (!exists)
                {
                    ii++;
                    CommonTool.WriteLog.Write(f.FullName + "--->不存在,删除" + ii.ToString());
                    System.IO.File.Delete(f.FullName);
                }
            }
            return ii;
        }

    }

    public static class Invite_ScoreRate
    {
        public static double pay_mny = 0.1;
        public static double gift_score = 0.1;
        public static double msg_score = 0.2;
    }

    public enum SummaryType
    {
        day = 10,
        hour = 13,
        minite = 16
    }

}
