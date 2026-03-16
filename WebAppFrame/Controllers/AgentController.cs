using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using System.IO;
using HtmlAgilityPack;
using BLL;

namespace WebAppFrame.Controllers
{
    public class AgentController : Controller
    {
        // GET: /Agent/
        BLL.BLL bll = new BLL.BLL();
        #region 用户代理
        public ActionResult Ag_Sum_Day(string id)
        {
            ViewData["id"] = id;
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            DataTable dt = bll.InviteFrined_SumScore(id);
            int fried_count = bll.InviteFriend_Sum_Num(id);
            dt.Rows[0]["fried_count"] = fried_count;

            string sumData = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["sumData"] = sumData;

           

            return View();
        }
        public string GetProfitData(string fz_tag, string pageIndex, string pageSize, string start, string end)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere_dtm = "";
            if (string.IsNullOrEmpty(start))
            {
                strWhere_dtm += string.Format(" and dtm>='{0} 00:00:00'", start);
            }
            if (string.IsNullOrEmpty(end))
            {
                strWhere_dtm += string.Format(" and dtm<='{0} 23:59:59'", end);
            }

            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm, 
                                   uid_from,
                                   sum(amount) as score_invite,
                                   sum(case when type_dtl = '邀请收益(充值)' then amount else 0 end) as score_pay,
                                   sum(case when type_dtl = '邀请收益(礼物)' then amount else 0 end) as score_gift,
                                   sum(case when type_dtl = '邀请收益(私信)' then amount else 0 end) as score_msg,
                                   u.uid, u.photo, u.nick, u.sex, u.score_total, u.score, u.coin, u.coin_total
                            from dbo.score s
                            left join dbo.[user] u on (s.uid_from = u.uid)
                            where uid_from='{0}' {1}
                            and (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)') 
                            group by CONVERT(char(10), dtm, 120), uid_from,u.uid, u.photo, u.nick, u.sex, u.score_total, u.score, u.coin, u.coin_total";

            strSql = string.Format(strSql, fz_tag, strWhere_dtm);

            BLL.Common comm = new BLL.Common();
            string strSort = "dtm desc";
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        //邀请明细-----start
        public ActionResult Ag_Invite(string id)
        {
            ViewData["id"] = id;

            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(1).ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetCheckWxData(string id, string pageIndex, string pageSize, string start, string end, string wx_check, string key, string wxnick, string fz_tag, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(wx_check))
            {
                where += string.Format(" and (wx_check='{0}') ", wx_check);
            }

            if (!string.IsNullOrEmpty(key))
            {
                where = string.Format(" (uid='{0}' or tel='{0}' or wx='{0}' or nick='{0}') ", key.Trim());
            }


            if (!string.IsNullOrEmpty(wxnick))
            {
                where = string.Format("u.uid in (select uid from dbo.user_attach where nickname like '%{0}%') ", wxnick);
            }

            if (!string.IsNullOrEmpty(fz_tag))
            {
                where += string.Format(" and (fz_tag={0}) ", fz_tag);
            }

            string strSql = @"select
                                           u.uid,
                                           u.photo,
                                           u.nick,
                                           u.sex,
                                           u.out_id,
                                           u.out_type,
                                           u.ip,
                                           u.tel,
                                           CONVERT(char(20), u.least_time, 120) as least_time,
                                           u.coin_total,u.coin,u.score_total,u.score,u.member,CONVERT(char(20), u.vip_endtime, 120) as vip_endtime,
                                           u.wx,
                                           u.wx_check,
                                           isnull(fz_tag, -1) as fz_tag,
                                           CONVERT(char(20), u.create_time, 120) as first_time
                                    from dbo.[user] u 
                                    where u.create_time >= '{0}' and u.create_time <= '{1}' and {2}";

            strSql = string.Format(strSql, start, end, where);

            string strSort = "first_time desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        //邀请明细-----end

        //支付明细-----start
        public ActionResult Ag_Pay(string id)
        {
            ViewData["id"] = id;

            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetOrderData(string id, string pageIndex, string pageSize, string start, string end, string order_state, string order_type, string comment2, string fz_tag)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            if (string.IsNullOrEmpty(start))
            {
                start = "2019-05-01";
            }
            if (string.IsNullOrEmpty(end))
            {
                end = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(order_state))
            {
                where += string.Format(" and (order_state='{0}') ", order_state);
            }
            if (!string.IsNullOrEmpty(order_type))
            {
                where += string.Format(" and (order_type='{0}') ", order_type);
            }
            if (!string.IsNullOrEmpty(id))
            {
                where += string.Format(" and (o.uid='{0}') ", id);
            }
            if (!string.IsNullOrEmpty(comment2))
            {
                where += " and (comment2 is not null and comment2 <> '') ";
            }
            if (!string.IsNullOrEmpty(fz_tag))
            {
                where += string.Format(" and (u.fz_tag={0}) ", fz_tag);

            }

            string strSql = @"select
                                           o.uid,
                                           o.order_no,
                                           o.order_name,
                                           
                                           o.pay_mny,
                                           CONVERT(char(20), o.dtm_pay, 120) as dtm_pay,
                                           o.order_type,
                                           o.order_state,
                                           o.order_mny,
                                           CONVERT(char(20), o.dtm_create, 120) as dtm_create,
                                           o.order_from,
                                           o.out_id,
                                           isnull(o.comment2, '') as comment2,o.comment,
                                           u.photo,
                                           u.nick,
                                           isnull(u.fz_tag, -1) as fz_tag,
                                           u.sex,
                                           u.loc_province,
                                           u.loc_city,
                                           CONVERT(char(20), u.create_time, 120) as first_time,
                                           CONVERT(char(20), o.create_time, 120) as create_time,
                                           (select ISNULL(sum(ISNULL(pay_mny,0)), 0) from dbo.sys_order where uid=o.uid and order_state='付款成功') as t_mny,
                                           (select ISNULL(sum(ISNULL(pay_mny,0)), 0) from dbo.sys_order where uid=o.uid and order_state='付款成功' and create_time <= o.create_time) as t_mny_now
                                    from dbo.sys_order o
                                    left join dbo.[user] u on (o.uid = u.uid)
                                    where o.create_time >= '{0}' and o.create_time <= '{1}' and {2}";

            strSql = string.Format(strSql, start, end, where);

            string strSort = "create_time desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        //支付明细-----end

        //积分明细-----start
        public ActionResult Ag_Score(string id)
        {
            ViewData["id"] = id;

            string dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetScoreList(string id, string pageIndex, string pageSize, string key, string type_dtl, string dtm)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string bll = string.IsNullOrEmpty(Request.QueryString["bll"]) ? "" : Request.QueryString["bll"];

            string strWhere = "1 = 1 ";

            if (!string.IsNullOrEmpty(key))
            {
                strWhere += string.Format(" and (s.uid_from = '{0}') ", key);
            }
            if (!string.IsNullOrEmpty(type_dtl))
            {
                strWhere += string.Format(" and (s.type_dtl = '{0}') ", type_dtl);
            }
            if (!string.IsNullOrEmpty(id))
            {
                strWhere += string.Format(" and (s.uid_from = '{0}') ", id);
            }
            if (!string.IsNullOrEmpty(bll))
            {
                strWhere += string.Format(" and (s.type = '{0}') ", bll);
            }
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere += string.Format(" and (s.dtm >= '{0} 00:00:00' and s.dtm <= '{0} 23:59:59') ", dtm);
            }

            string strSql = @"select   s.id, 
                                       CONVERT(char(20), s.dtm, 120) as dtm,
                                       s.uid_from,
                                       s.uid_to,
                                       s.amount,
                                       s.type,
                                       s.type_dtl,
                                       s.comment,
                                       s.rel_id, 
       
                                       u.photo,
                                       u.nick,
                                       u.sex,
                                       
                                       u2.photo as photo2,
                                       u2.nick as nick2,
                                       u2.sex as sex2
                                from dbo.score  s
                                left join dbo.[user] u on (s.uid_from = u.uid)
                                left join dbo.[user] u2 on (s.uid_to = u2.uid)
                                where {0} ";

            strSql = string.Format(strSql, strWhere);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        //积分明细-----end
        #endregion 

        #region 管理员代理
        public ActionResult Admin()
        {
            return View();
        }
        public ActionResult Admin_Sum_All()
        {
            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetAdminSumAllData(string pageIndex, string pageSize, string dtm, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strSql = @"select a.*,
                                   b.*
                            from 
                            (select 
                                   ag.fz_tag, 
                                   u.uid, u.photo, u.nick, u.sex, u.score_total, u.score, u.coin, u.coin_total,u.remark,
                                   '{0}' as dtm,
                                   sum(case when ag.create_time >= '{0} 00:00:00' and ag.create_time <= '{0} 23:59:59' then 1 else 0 end) c_dtm,
                                   COUNT(ag.uid) as c1,
                                   sum(case when ag.reg_complete=1 then 1 else 0 end) c2
       
                            from dbo.[user] ag
                            left join dbo.[user] u on (ag.fz_tag = u.uid)
                            where ag.fz_tag <> '0'
                            group by ag.fz_tag, u.uid, u.photo, u.nick, u.sex, u.score_total, u.score, u.coin, u.coin_total,u.remark
                            ) 
                            as a left join 
                            (select 
                                                               uid_from,
                                                               sum(case when s.dtm >= '{0} 00:00:00' and s.dtm <= '{0} 23:59:59' then amount else 0 end) as score_dtm,
                                   
                                                               sum(case when type_dtl = '邀请收益(充值)' then amount else 0 end) as score_pay,
                                                               sum(case when type_dtl = '邀请收益(礼物)' then amount else 0 end) as score_gift,
                                                               sum(case when type_dtl = '邀请收益(私信)' then amount else 0 end) as score_msg,
                                                               sum(amount) as score_invite
                                                        from dbo.score s
                         
                                                        where 
                                                         (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)') 
                                                        group by uid_from)
                            as b on (a.fz_tag = b.uid_from)";

            strSql = string.Format(strSql, dtm);

            BLL.Common comm = new BLL.Common();
            string strSort = "c_dtm desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        public ActionResult Admin_Sum()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];
            string fz_tag = Request.QueryString["fz_tag"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString("yyyy-MM-dd") : end;

            ViewData["start"] = start;
            ViewData["end"] = end;
            CommonTool.WriteLog.Write("start"+ start+"end"+ end+"fz_tag"+ fz_tag);
            DataTable dt = bll.GetInviteSumData(start, end, fz_tag);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            ViewData["data"] = data;
            return View();
        }
        public ActionResult Admin_Sum_Day()
        {
            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetAgentStatisticsData(string pageIndex, string pageSize, string dtm, string fz_tag)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere = "";
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere = string.Format(" (s.dtm >= '{0} 00:00:00' and s.dtm <= '{0} 23:59:59')", dtm);
            }
            if (!string.IsNullOrEmpty(fz_tag))
            {
                strWhere = string.Format(" (s.uid_from = '{0}')", fz_tag);
            }

            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm, 
                                   uid_from,
                                   sum(amount) as score_invite,
                                   sum(case when type_dtl = '邀请收益(充值)' then amount else 0 end) as score_pay,
                                   sum(case when type_dtl = '邀请收益(礼物)' then amount else 0 end) as score_gift,
                                   sum(case when type_dtl = '邀请收益(私信)' then amount else 0 end) as score_msg,
                                   u.uid, u.photo, u.nick, u.sex, u.score_total, u.score, u.coin, u.coin_total,u.remark
                            from dbo.score s
                            left join dbo.[user] u on (s.uid_from = u.uid)
                            where {0}
                            and (type_dtl = '邀请收益(充值)' or type_dtl = '邀请收益(礼物)' or type_dtl = '邀请收益(私信)') 
                            group by CONVERT(char(10), dtm, 120), uid_from,u.uid, u.photo, u.nick, u.sex, u.score_total, u.score, u.coin, u.coin_total,u.remark";

            strSql = string.Format(strSql, strWhere);

            BLL.Common comm = new BLL.Common();
            string strSort = "dtm desc,score_invite desc";
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        //邀请明细
        public ActionResult Admin_Invite()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(1).ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        //支付明细
        public ActionResult Admin_Pay()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        //积分明细
        public ActionResult Admin_Score()
        {
            string dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        #endregion 
    }
}
