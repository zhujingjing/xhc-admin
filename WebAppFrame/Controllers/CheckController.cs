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
    public class CheckController : Controller
    {
        BLL.BLL bll = new BLL.BLL();

        //
        // GET: /Check/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Check_Sum()
        {
            

            return View();
        }

        public ActionResult Online_Service()
        {
            return View();
        }
        public ActionResult Service_Sum()
        {
            return View();
        }

        public ActionResult Check_Msg()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = string.Empty;
            dtm = DateTime.Now.ToString("yyyy-MM-dd");

            ViewData["dtm"] = dtm;
        

            return View();
        }

        public string GetCheckMsgData(string pageIndex, string pageSize, string dtm, string where, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            if (string.IsNullOrEmpty(dtm))
            {
                dtm = DateTime.Now.ToString("yyyy-MM-dd");
            }
            string strWhere = "";
            if (!string.IsNullOrEmpty(where))
            {
                strWhere = where;
                if (where == "0" || where == "1" || where == "2" || where == "6")
                {
                    strWhere = string.Format("u2.create_time >= dateadd(day, -{1}, '{0}')", dtm, where);
                }
                strWhere = "where " + strWhere;
            }
                

            string table_name = bll.GetTableName_msgsum(dtm);
            
            //用户发给客服的消息
            string strSql = @"select msg_sum.*,
       u2.photo, u2.nick, u2.sex,u2.create_time,u2.coin_total, u2.score_total,u2.coin, u2.score, u2.member,u2.coin_info,u2.remark,u2.chat_mny_score, u2.chat_mny_score2,
       o.*,
       tx.tx_mny, s.score_today, c.coin_today
from 
(
	select uid_from as uid, 
		   sum(msg_count) as msg_c,
		   count(uid_to) as u_c      
	from {0} msg 
	group by uid_from
) msg_sum
left join dbo.[user] u2 on (msg_sum.uid = u2.uid)	
left join 
(
	select o.uid as o_uid,
		   sum(pay_mny) as mny,
		   count(order_no) as pay_count,
		   sum(case when order_type = '购买金币' then pay_mny else 0 end) as mny_coin,
		   sum(case when order_type = '购买会员' then pay_mny else 0 end) as mny_vip,

           sum(case when o.dtm_create >= '{1}' then pay_mny else 0 end) as mny_0,
		   sum(case when o.dtm_create >= '{1}' then 1 else 0 end) as pay_count_0,
		   sum(case when order_type = '购买金币' and o.dtm_create >=  '{1}' then pay_mny else 0 end) as mny_coin_0,
		   sum(case when order_type = '购买会员' and o.dtm_create >=  '{1}' then pay_mny else 0 end) as mny_vip_0,
	       
		   sum(case when o.dtm_create >= dateadd(day, -7, '{1}') then pay_mny else 0 end) as mny_7,
		   sum(case when o.dtm_create >= dateadd(day, -7, '{1}') then 1 else 0 end) as pay_count_7,
		   sum(case when order_type = '购买金币' and o.dtm_create >= dateadd(day, -7, '{1}') then pay_mny else 0 end) as mny_coin_7,
		   sum(case when order_type = '购买会员' and o.dtm_create >= dateadd(day, -7, '{1}') then pay_mny else 0 end) as mny_vip_7,
	       
		   sum(case when o.dtm_create >= dateadd(day, -30, '{1}') then pay_mny else 0 end) as mny_30,
		   sum(case when o.dtm_create >= dateadd(day, -30, '{1}') then 1 else 0 end) as pay_count_30,
		   sum(case when order_type = '购买金币' and o.dtm_create >= dateadd(day, -30, '{1}') then pay_mny else 0 end) as mny_coin_30,
		   sum(case when order_type = '购买会员' and o.dtm_create >= dateadd(day, -30, '{1}') then pay_mny else 0 end) as mny_vip_30,
	       
		   sum(case when o.dtm_create >= dateadd(day, -90, '{1}') then pay_mny else 0 end) as mny_90,
		   sum(case when o.dtm_create >= dateadd(day, -90, '{1}') then 1 else 0 end) as pay_count_90,
		   sum(case when order_type = '购买金币' and o.dtm_create >= dateadd(day, -90, '{1}') then pay_mny else 0 end) as mny_coin_90,
		   sum(case when order_type = '购买会员' and o.dtm_create >= dateadd(day, -90, '{1}') then pay_mny else 0 end) as mny_vip_90,
	       
		   sum(case when o.dtm_create >= dateadd(day, -365, '{1}') then pay_mny else 0 end) as mny_365,
		   sum(case when o.dtm_create >= dateadd(day, -365, '{1}') then 1 else 0 end) as pay_count_365,
		   sum(case when order_type = '购买金币' and o.dtm_create >= dateadd(day, -365, '{1}') then pay_mny else 0 end) as mny_coin_365,
		   sum(case when order_type = '购买会员' and o.dtm_create >= dateadd(day, -365, '{1}') then pay_mny else 0 end) as mny_vip_365
	       
	from dbo.sys_order o
	where o.order_state = '付款成功' and (o.dtm_create <= '{1} 23:59:59')
	group by o.uid
) o on (msg_sum.uid = o.o_uid)
left join 
(
	select uid as tx_uid,
		   sum(tx_mny) as tx_mny
	from dbo.score_tx
	group by uid
) tx on (msg_sum.uid = tx.tx_uid)
left join 
(
	select  uid_from as score_uid,
			sum(amount) as score_today
	from dbo.score 
	where [type] = '增加' and  (dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59' )
	group by uid_from 
) s on (msg_sum.uid = s.score_uid)
left join 
(
	select  uid_from as coin_uid,
			sum(amount) as coin_today
	from dbo.coin 
	where [type] = '减少' and  (dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59' )
	group by uid_from 
) c on (msg_sum.uid = c.coin_uid)
{2}";
            strSql = string.Format(strSql, table_name, dtm, strWhere);

            string strSort = "msg_c desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetChartA_B_Info(string dtm, string from, string to, string sex_to)
        {
            string strRtn = "";

            string strSql = @"select 'coin' as t, type_dtl, uid_from, uid_to, sum(amount) as c 
from dbo.coin 
where (dtm > '{0} 00:00:00' and dtm < '{0} 23:59:59' ) and ((uid_from = '{1}' and uid_to = '{2}')  or (uid_from = '{2}' and uid_to = '{1}') )
group by uid_from, uid_to, type_dtl
union all
select 'score' as t, type_dtl, uid_from, uid_to, sum(amount) as c 
from dbo.score 
where (dtm > '{0} 00:00:00' and dtm < '{0} 23:59:59' ) and ((uid_from = '{2}' and uid_to = '{1}')  or (uid_from = '{1}' and uid_to = '{2}') )
group by uid_from, uid_to, type_dtl";
            strSql = string.Format(strSql, dtm, from, to);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string scoreCoin = CommonTool.JsonHelper.DataTableToJSON(dt);
            string friendTime = bll.Get_Friend_Time(from, to);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("scoreCoin", scoreCoin);
            dic.Add("friendTime", friendTime);


            string sum_info = "";
            if (sex_to == "男生")
            {
                sum_info = GetUserInfoSumCoin(dtm, to);
            }
            else
            {
                sum_info = GetUserInfoSumScore(dtm, to);
            }
            dic.Add("sum_info", sum_info);

            strRtn = CommonTool.JsonHelper.ObjectToJSON(dic);

            return strRtn;
        }

        public string GetUserInfoSumScore(string dtm, string uid)
        {
            string strRtn = "";
            DataTable dt_score = bll.Get_UserScore_Sum(uid, dtm);
            strRtn = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt_score);
            return strRtn;
        }
        public string GetUserInfoSumCoin(string dtm, string uid)
        {
            string strRtn = "";
            DataTable dt_charge = bll.Get_UserChargeMny_Sum(uid, dtm);
            DataTable dt_coin = bll.Get_UserCoin_Sum(uid, dtm);

            string mny = dt_charge.Rows[0]["mny"].ToString();
            string mny_day = dt_charge.Rows[0]["mny_day"].ToString();

            string c_add = dt_coin.Rows[0]["c_add"].ToString();
            string c_sub = dt_coin.Rows[0]["c_sub"].ToString();

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("mny", mny);
            dic.Add("mny_day", mny_day);
            dic.Add("c_add", c_add);
            dic.Add("c_sub", c_sub);

            strRtn = CommonTool.JsonHelper.ObjectToJSON(dic);

            return strRtn;
        }
        

        public ActionResult Service_Chat()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string start = string.Empty;
            string end = string.Empty;

            start = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            end = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";

            ViewData["start"] = start;
            ViewData["end"] = end;

            return View();
        }
        public ActionResult Service_UserChat()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string start = string.Empty;
            string end = string.Empty;

            start = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            end = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";

            ViewData["start"] = start;
            ViewData["end"] = end;
            return View();
        }
        //概况
        public ActionResult Check_Survey(string id)
        {
            //信息审核
            string strSql = "select COUNT(*) from dbo.user_update where check_state = '待审核' and dtm_apply >= dateadd(day, -3, getdate()) ";
            string checkInfo = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["checkInfo"] = checkInfo;

            //提现审核
            strSql = "select COUNT(*) from dbo.score_tx where state = '待审核' and apply_date >= dateadd(day, -3, getdate())  ";
            string checkTx = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["checkTx"] = checkTx;

            //寻人区
            strSql = "select COUNT(*) from dbo.seek_people where create_time > DATEADD(day, -1, getdate()) and  comment is null ";
            string checkSeekPeople = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["checkSeekPeople"] = checkSeekPeople;

            //礼物投诉
            strSql = "select COUNT(*) from dbo.tousu_gift where state = '待审核' and dtm >= dateadd(day, -3, getdate()) ";
            string checkGifts = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["checkGifts"] = checkGifts;

            //用户反馈
            strSql = "select COUNT(*) from dbo.tousu where check_state = '待审核' and create_time >= dateadd(day, -3, getdate()) ";
            string checkFeedBack = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["checkFeedBack"] = checkFeedBack;

            //聊天举报
            strSql = "select COUNT(*) from dbo.complain where (check_state = '待审核' or check_state is null) and dtm >= dateadd(hour, -1, getdate()) ";
            string checkComPlain = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["checkComPlain"] = checkComPlain;

            //相册审核
            strSql = "select COUNT(*) from dbo.album where check_state = 0 and create_time >= dateadd(day, -3, getdate()) ";
            string checkAlbum = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["checkAlbum"] = checkAlbum;

            //CommonTool.WriteLog.Write("信息审核：" + checkInfo+ "提现审核："+ checkTx);
            return View();
        }
        
        public ActionResult Complain(string id)
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddHours(-1).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public string GetComplainData(string id, string pageIndex, string pageSize, string start, string end, string c_type, string A, string B)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            if (string.IsNullOrEmpty(end))
            {
                end = DateTime.Now.ToString();
            }

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(c_type))
            {
                where += string.Format(" and (c.c_type='{0}') ", c_type);
            }
            if (!string.IsNullOrEmpty(A))
            {
                where += string.Format(" and (c.uid_from='{0}') ", A);
            }
            if (!string.IsNullOrEmpty(B))
            {
                where += string.Format(" and (c.uid_to='{0}') ", B);
            }



            string strSql = @"select  u1.photo as from_photo, 
		                                    u1.nick as from_nick, 
		                                    u1.sex as from_sex, 
		                                    DATEDIFF(DAY, u1.create_time, GETDATE()) as from_diff,
		
		                                    u2.photo as to_photo, 
		                                    u2.nick as to_nick, 
		                                    u2.sex as to_sex, 
		                                    DATEDIFF(DAY, u2.create_time, GETDATE()) as to_diff,
		
		                                    CONVERT(char(20), c.dtm, 120) as dtm,
		                                    c.c_type,
		                                    c.c_type_dtl,
                                            c.uid_from, c.uid_to,
                                            c.check_state,
                                           
                                            convert(char(20), c.dtm_check, 120) as dtm_check,
                                            c.rst_check,
                                            c.id,
                                            DATEDIFF(MINUTE, c.dtm,GETDATE()) as diff_m,
									        DATEDIFF(MINUTE, c.dtm,c.dtm_check) as diff_mc
                                            

                                    from dbo.complain c
                                    left join dbo.[user] u1 on (c.uid_from = u1.uid)
                                    left join dbo.[user] u2 on (c.uid_to = u2.uid)
                                    where c.dtm >= '{0}' and c.dtm <= '{1}' and {2}";

            strSql = string.Format(strSql, start, end, where);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public ActionResult CheckComplain(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        public string Update_Check_Complain(string id, string rst_check, string rst)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update dbo.complain
                              set check_state='{2}',
                               dtm_check=GETDATE(),
                               rst_check='{0}'
                               where id='{1}';";
            strSql = string.Format(strSql, rst_check, id, rst);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "失败";
            }
            return info.ToString();
        }

        #region 寻人区
        public ActionResult Seek_People(string id)
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        //寻人区后台置顶
        public string ReFresh(string id)
        {
            CommonTool.WriteLog.Write("id----" + id);
            string strSql = "update dbo.seek_people set refurbish_time = GETDATE(), refurbish_count+=1 where id = '{0}'";
            strSql = string.Format(strSql, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);

            return tag.ToString();
        }
        public string GetSeekpeopleData(string pageIndex, string pageSize, string start, string end, string c_type, string sort, string key, string publish_uid)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(c_type))
            {
                if (c_type == "未审核")
                {
                    where += "and s.comment is null ";
                }
                else
                {
                    where += string.Format("and s.state='{0}' ", c_type);
                }

            }

            if (!string.IsNullOrEmpty(start))
            {
                where += string.Format("and s.create_time>='{0} 00:00:00' ", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                where += string.Format("and s.create_time<='{0} 23:59:59' ", end);
            }

            if (!string.IsNullOrEmpty(key))
            {
                where += string.Format("and s.title like '%{0}%' ", key);
            }
            if (!string.IsNullOrEmpty(publish_uid))
            {
                where += string.Format("and s.uid='{0}' ", publish_uid);
            }

            string strSql = @"select 
						     s.id,
                             s.uid,
							 u1.photo,
							 u1.nick,
							 u1.sex,
							 s.wx,
							 CONVERT(char(20), s.create_time, 120) as create_time,
							 s.title,
							 s.content,
							 s.state,
                             CONVERT(char(20), s.refurbish_time, 120) as refurbish_time,
                             s.refurbish_count,
                             s.comment,
                             isnull(CONVERT(char(20), s.dtm_check, 120),'2020-03-05 00:00:00')  as dtm_check,
							 isnull(s.wxcheck,'')as wxcheck,
                             isnull(CONVERT(char(20), s.dtm_wxcheck, 120),'2020-03-05 00:00:00')  as dtm_wxcheck
                            from dbo.seek_people s
                            left join dbo.[user] u1 on (s.uid = u1.uid)
                            where {0}";
            strSql = string.Format(strSql, where);
            BLL.Common comm = new BLL.Common();

            string strSort = "comment asc, create_time desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }


            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        public string GetSeekpeopleData_movie()
        {
            string strRtn = string.Empty;
            string strSql = @"select 
						     s.id,
                             s.uid,
							 u1.photo,
							 u1.nick,
							 u1.sex,
							 s.wx,
							 CONVERT(char(20), s.create_time, 120) as create_time,
							 s.title,
							 s.content,
							 s.state,
                             CONVERT(char(20), s.refurbish_time, 120) as refurbish_time,
                             s.refurbish_count,
							 isnull(s.comment,'')as comment,
                             isnull(CONVERT(char(20), s.dtm_check, 120),'2020-03-05 00:00:00')  as dtm_check,
							 isnull(s.wxcheck,'待审核')as wxcheck,
                             isnull(CONVERT(char(20), s.dtm_wxcheck, 120),'2020-03-05 00:00:00')  as dtm_wxcheck
                            from dbo.seek_people s
                            left join dbo.[user] u1 on (s.uid = u1.uid)
							where s.create_time>DATEADD(day,-1,GETDATE()) order by  comment asc, create_time desc";


            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);
            return strRtn;
        }
        public string Check_Seek_people(string id, string state, string operatorName = "")
        {

            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = @"update dbo.seek_people 
                                    set state='{0}',comment='管理员修改',dtm_check=GETDATE()
                                    where id='{1}'";
            strSql = string.Format(strSql, state, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                // 集成任务状态更新
                BLL.Task taskBll = new BLL.Task();
                // 优先使用前端传递的操作者信息
                if (string.IsNullOrEmpty(operatorName))
                {
                    operatorName = Session["sys_user_name"] != null ? Session["sys_user_name"].ToString() : "系统";
                }
                taskBll.UpdateTaskStatusByBusiness("寻人区审核", id, 2, operatorName);
                
                info.State = "1";
                info.Msg = "修改成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "修改失败";
            }
            return info.ToString();
        }

        public string WxCheck_Seek_people(string id, string wxcheck)
        {

            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = @"update dbo.seek_people 
                                    set wxcheck='{0}',dtm_wxcheck=GETDATE()
                                    where id='{1}'";
            strSql = string.Format(strSql, wxcheck, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                info.State = "1";
                info.Msg = "修改成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "修改失败";
            }
            return info.ToString();
        }
        #endregion


        #region 信息审核
        public ActionResult UpData_UserInfo(string id)
        {


            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(0).ToString() : end;

            //start = "2019-05-01";

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public string GetUpDateUserInfoData(string pageIndex, string pageSize, string start, string end, string check_state, string field_name, string uniformity)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(check_state))
            {
                where += string.Format(" and up.check_state='{0}' ", check_state);
            }
            if (!string.IsNullOrEmpty(start))
            {
                where += string.Format(" and up.dtm_apply>='{0} 00:00:00' ", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                where += string.Format(" and up.dtm_apply<='{0} 23:59:59' ", end);
            }
            if (!string.IsNullOrEmpty(field_name))
            {
                where += string.Format(" and up.field_name='{0}' ", field_name);
            }


            BLL.Common com = new BLL.Common();
            string strSql = @"select up.id,
	                               up.uid,
	                               up.field_name,
	                               up.val_old,
	                               up.val_new,
	                               up.check_state,
                                   convert(char(20), up.dtm_apply, 120) as dtm_apply,
                                   convert(char(20), up.dtm_check, 120) as dtm_check,
								   u.photo,
								   u.nick,
								   u.sex,
								   u.create_time
                            from dbo.user_update up
							left join dbo.[user] u on(up.uid=u.uid)
							where {0}";
            strSql = string.Format(strSql, where);
            BLL.Common comm = new BLL.Common();
            string strSort = "dtm_apply desc";
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);
            return strRtn;
        }
        public string Update_UserInfo(string id, string state, string operatorName = "")
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();


            string check_state = "失败";
            if (state == "1")
            {
                check_state = "通过";
            }

            string strSql = @"update dbo.user_update  
                                set check_state = '{1}', dtm_check = getdate()
                                where id = '{0}'";
            strSql = string.Format(strSql, id, check_state);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                // 集成任务状态更新
                BLL.Task taskBll = new BLL.Task();
                // 优先使用前端传递的操作者信息
                if (string.IsNullOrEmpty(operatorName))
                {
                    operatorName = Session["sys_user_name"] != null ? Session["sys_user_name"].ToString() : "系统";
                }
                taskBll.UpdateTaskStatusByBusiness("头像昵称审核", id, 2, operatorName);
                
                info.State = "1";
                info.Msg = "修改成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "修改失败";
            }

            if (state == "1")
            {
                //更新正式表
                strSql = "select uid, field_name, val_new from dbo.user_update  where id = '{0}'";
                strSql = string.Format(strSql, id);
                DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    string uid = dt.Rows[0]["uid"].ToString();
                    string val_new = dt.Rows[0]["val_new"].ToString();
                    string field_name = dt.Rows[0]["field_name"].ToString();
                    strSql = @"update dbo.[user] set {1} = '{2}' where uid = '{0}'";
                    strSql = string.Format(strSql, uid, field_name, val_new);
                    DBHelper.SqlHelper.ExecuteSql(strSql);
                }
            }

            return info.ToString();
        }

        public string DeletePhotos(string ids)
        {
            string strRtn = string.Empty;
            strRtn = ids;
            //返还前打印
            CommonTool.WriteLog.Write(strRtn);

            //删除自定义头像
            int count = bll.DeletePic_DefinePhoto();
            CommonTool.WriteLog.Write("删除头像数量:" + count.ToString());

            return strRtn;
        }

        
        #endregion

        #region 提现

        public ActionResult Score_Tx()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-7).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(0).ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;


            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            BLL.BLL bll = new BLL.BLL();

            double dmoney = bll.Get_PlatAllTiXianMoney("成功");
            double dmoney2 = bll.Get_PlatAllTiXianMoney("待审核");
            double dmoney3 = bll.Get_PlatAllTiXianMoney("失败");
            string strMoney = String.Format("{0:F}", dmoney);
            string strMoney2 = String.Format("{0:F}", dmoney2);
            string strMoney3 = String.Format("{0:F}", dmoney3);
            ViewData["money"] = strMoney;
            ViewData["money2"] = strMoney2;
            ViewData["money3"] = strMoney3;

            return View();
        }
        public string GetScoreTxList(string id, string pageIndex, string pageSize, string key, string check_state, string start, string end)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere = "1 = 1 ";

            if (!string.IsNullOrEmpty(key))
            {
                strWhere += string.Format(" and (a.real_name = '{0}') or (a.wx_account = '{0}') ", key);
            }
            if (!string.IsNullOrEmpty(check_state))
            {
                strWhere += string.Format(" and (a.state = '{0}')", check_state);
            }
            if (!string.IsNullOrEmpty(id))
            {
                strWhere += string.Format(" and (a.uid = '{0}')", id);
            }
            if (!string.IsNullOrEmpty(start))
            {
                strWhere += string.Format(" and (a.apply_date >= '{0} 00:00:00')", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                strWhere += string.Format(" and (a.apply_date <= '{0} 23:59:59')", end);
            }



            string strSql = @"select a.id,    ISNULL(us.uid, '') as uid_inner,
                                   a.apply_no,
                                   a.uid,
                                   a.real_name,a.zhifubao_account, a.wx_account,a.tel,
                                   a.tx_mny, a.pay_mny, a.fee,
                                   a.score_tx, a.score_left,
                                   CONVERT(char(20), a.apply_date, 120) as apply_date,
                                   CONVERT(char(20), a.pay_date, 120) as pay_date,
                                   DATEDIFF(hour, a.apply_date,a.pay_date) as diff_hour,
                                   a.state,
                                   a.brife,
                                   a.create_time,
                                   a.state_wxsend,
                                   u.tel as rz_tel, u.wx as rz_wx, u.photo, u.nick, u.sex, isnull(u.remark , '') as remark
                            from [dbo].[score_tx]  a
                            left join dbo.[user] u on(a.uid = u.uid)
                            left join dbo.user_s us on (a.uid = us.uid)
                            where {0} ";

            strSql = string.Format(strSql, strWhere);

            string strSort = "create_time desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        public ActionResult Score_Tx_Rst(string id, string pay_mny)
        {
            ViewData["id"] = id;
            ViewData["pay_mny"] = pay_mny;
            return View();
        }
        public string UpdateApply_Tx(string id, string state, string reason, string pay_mny, string operatorName = "")
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = "update dbo.score_tx set state = '{1}', brife = '{2}', pay_mny={3}, pay_date=getdate() where id = '{0}'";
            strSql = string.Format(strSql, id, state, reason, pay_mny);

            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                // 集成任务状态更新
                BLL.Task taskBll = new BLL.Task();
                // 优先使用前端传递的操作者信息
                if (string.IsNullOrEmpty(operatorName))
                {
                    operatorName = Session["sys_user_name"] != null ? Session["sys_user_name"].ToString() : "系统";
                }
                
                // 根据提现 id 查询 apply_no
                string getApplyNoSql = string.Format("SELECT apply_no FROM dbo.score_tx WHERE id = '{0}'", id);
                string applyNo = DBHelper.SqlHelper.GetDataItemString(getApplyNoSql);
                
                // 使用 apply_no 作为业务 ID
                taskBll.UpdateTaskStatusByBusiness("提现", applyNo, 2, operatorName);
                
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "失败";
            }

            return info.ToString();
        }
        //发送微信模板消息
        public ActionResult SendWxMsg_Tx(string id)
        {
            string strSql = @"select id, uid, 
                               CONVERT(char(20), pay_date, 120) as dtm, 
                               score_tx, 
                               Convert(decimal(18,2), pay_mny) as mny,
                               score_left 
                        from dbo.score_tx where id = '{0}'";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;
            return View();
        }
        #endregion

        //礼物投诉
        public ActionResult Gif_TouSu(string id)
        {


            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(0).ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public string GetGifTouSuData(string pageIndex, string pageSize, string A, string B, string start, string end, string check_state)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            CommonTool.WriteLog.Write("开始时间：" + start);
            CommonTool.WriteLog.Write("结束时间：" + end);
            CommonTool.WriteLog.Write("状态：" + check_state);
            CommonTool.WriteLog.Write("B：" + B);

            string strwhere = " 1=1 ";
            if (!string.IsNullOrEmpty(A))
            {
                strwhere += string.Format(" and g.uid='{0}'", A);
            }
            if (!string.IsNullOrEmpty(B))
            {
                strwhere += string.Format(" and c.uid_to='{0}'", B);
            }
            if (!string.IsNullOrEmpty(check_state))
            {
                strwhere += string.Format(" and g.state='{0}'", check_state);
            }
            if (!string.IsNullOrEmpty(start))
            {
                strwhere += string.Format(" and g.dtm>='{0} 00:00:00'", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                strwhere += string.Format(" and g.dtm<='{0} 23:59:59'", end);
            }

            BLL.Common com = new BLL.Common();
            string strSql = @"select g.id,
	                                 g.uid,
	                                 g.gift_id,
	                                 convert(char(20), g.dtm, 120) as dtm,
	                                 g.brife,
	                                 g.pics,
	                                 g.state,
									 isnull(g.rst_check,'') as rst_check,
	                                 convert(char(20), g.dtm_check, 120) as dtm_check,

	                                 u1.photo,
	                                 u1.nick,
	                                 u1.sex,

									 c.uid_to,
								     u2.photo as photo2,
								     u2.nick as nick2,
								     u2.sex as sex2,
								     convert(char(20), c.dtm, 120) as gift_time,
								     c.amount,
								     c.comment
                              from [dbo].[tousu_gift] g
							  left join dbo.coin c on(g.gift_id = c.id)
                              left join dbo.[user] u1 on(g.uid=u1.uid)
							  left join dbo.[user] u2 on(c.uid_to = u2.uid)
                              where {0}";
            strSql = string.Format(strSql, strwhere);
            BLL.Common comm = new BLL.Common();
            string strSort = "dtm desc";
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);
            return strRtn;
        }
        public ActionResult Gif_TouSuDtl(string id)
        {
            string data = "";
            string strSql = @"select 
	                                 pics
                              from dbo.tousu_gift
                              where gift_id = '{0}' ";
            strSql = string.Format(strSql, id);

            data = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["data"] = data;

            return View();
        }

        public ActionResult CheckGiftTouSu(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        public string Update_Gift_Check(string id, string rst_check, string rst, string operatorName = "")
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update dbo.tousu_gift
                              set state='{2}',
                               dtm_check=GETDATE(),
                               rst_check='{0}'
                               where id='{1}';";
            strSql = string.Format(strSql, rst_check, id, rst);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                // 集成任务状态更新
                BLL.Task taskBll = new BLL.Task();
                // 优先使用前端传递的操作者信息
                if (string.IsNullOrEmpty(operatorName))
                {
                    operatorName = Session["sys_user_name"] != null ? Session["sys_user_name"].ToString() : "系统";
                }
                taskBll.UpdateTaskStatusByBusiness("礼物投诉", id, 2, operatorName);
                
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "失败";
            }
            return info.ToString();
        }

        //用户反馈
        public ActionResult FeedBack()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(0).ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetTouSuData(string pageIndex, string pageSize, string start, string end, string check_state)
        {
            string strRtn = string.Empty;
            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            CommonTool.WriteLog.Write("开始时间：" + start);
            CommonTool.WriteLog.Write("结束时间：" + end);
            CommonTool.WriteLog.Write("状态：" + check_state);

            string strWhere = " 1=1 ";
            if (!string.IsNullOrEmpty(check_state))
            {
                strWhere += string.Format(" and ts.check_state = '{0}'", check_state);
            }
            if (!string.IsNullOrEmpty(start))
            {
                strWhere += string.Format(" and ts.create_time >= '{0} 00:00:00'", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                strWhere += string.Format(" and ts.create_time <= '{0} 23:59:59'", end);
            }


            BLL.Common com = new BLL.Common();
            string strSql = @"select ts.id,
                                       ts.uid,
	                                   ts.brife,
	                                   ts.check_state,
	                                   convert(char(20), ts.create_time, 120) as create_time,
									   convert(char(20), ts.dtm_check, 120) as dtm_check,
									   ts.rst_check,
	                                   u.photo,
	                                   u.nick,
	                                   u.sex
                                from dbo.tousu ts
                                left join dbo.[user] u on(ts.uid=u.uid) where {0}";
            strSql = string.Format(strSql, strWhere);

            string strSort = "create_time desc";
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);
            return strRtn;
        }

        public ActionResult CheckFeedBack(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        public string Update_Check_TouSu(string id, string rst_check, string rst, string operatorName = "")
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update dbo.tousu
                              set check_state='{2}',
                               dtm_check=GETDATE(),
                               rst_check='{0}'
                               where id='{1}';";
            strSql = string.Format(strSql, rst_check, id, rst);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                // 集成任务状态更新
                BLL.Task taskBll = new BLL.Task();
                // 优先使用前端传递的操作者信息
                if (string.IsNullOrEmpty(operatorName))
                {
                    operatorName = Session["sys_user_name"] != null ? Session["sys_user_name"].ToString() : "系统";
                }
                taskBll.UpdateTaskStatusByBusiness("用户反馈", id, 2, operatorName);
                
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "失败";
            }
            return info.ToString();
        }

        public ActionResult FeedBack_dtl(string id)
        {
            ViewData["id"] = id;

            string strSql = @"select id, uid, brife,pics from [dbo].[tousu] where id='{0}'";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;
            return View();
        }
        public string Get_FeedBack_dtl(string id)
        {
            string strRtn = string.Empty;

            string strSql = @"select id, uid, brife,pics from [dbo].[tousu] where id='{0}'";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            strRtn = data;
            CommonTool.WriteLog.Write(strRtn);
            return strRtn;
        }

        //相册审核
        public ActionResult Album()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public string GetAlbumData(string pageIndex, string pageSize, string start, string end, string c_type, string is_delete, string sort, string publish_uid)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = " 1=1 ";


            if (!string.IsNullOrEmpty(start))
            {
                where += string.Format("and ab.create_time>='{0} 00:00:00' ", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                where += string.Format("and ab.create_time<='{0} 23:59:59' ", end);
            }
            if (!string.IsNullOrEmpty(c_type))
            {
                where += string.Format("and ab.check_state={0}", c_type);
            }
            if (!string.IsNullOrEmpty(is_delete))
            {
                where += string.Format("and ab.is_delete={0}", is_delete);
            }
            if (!string.IsNullOrEmpty(publish_uid))
            {
                where += string.Format("and ab.uid='{0}'", publish_uid);
            }


            string strSql = @"select ab.id, 
	                               ab.uid, u.nick, u.sex, u.photo,
	                               url,
	                               check_state,
	                               sort,
	                               name,
	                               group_name,
	                               CONVERT(char(20), ab.create_time, 120 ) as create_time,
	                               CONVERT(char(20), sort_time, 120 )  as sort_time,
                                   CONVERT(char(20), check_time, 120 )  as check_time,
	                               is_delete
	                         from dbo.album ab
	                         left join dbo.[user] u on (ab.uid = u.uid)
                            where {0}";
            strSql = string.Format(strSql, where);
            BLL.Common comm = new BLL.Common();

            string strSort = "create_time desc";
            if (!string.IsNullOrEmpty(sort))
                strSort = sort;


            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string Check_Album(string id, string state, string operatorName = "")
        {

            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = @"update dbo.album 
                                    set check_state='{0}',check_time=GETDATE()
                                    where id='{1}'";
            strSql = string.Format(strSql, state, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                // 集成任务状态更新
                BLL.Task taskBll = new BLL.Task();
                // 优先使用前端传递的操作者信息
                if (string.IsNullOrEmpty(operatorName))
                {
                    operatorName = Session["sys_user_name"] != null ? Session["sys_user_name"].ToString() : "系统";
                }
                taskBll.UpdateTaskStatusByBusiness("相册审核", id, 2, operatorName);
                
                info.State = "1";
                info.Msg = "修改成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "修改失败";
            }
            return info.ToString();
        }
    }
}
