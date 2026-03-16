using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

using System.IO;

namespace WebAppFrame.Controllers
{
    public class HomeController : Controller
    {
        BLL.BLL bll = new BLL.BLL();

        public string GetServDiffSec()
        {
            string i = "0";
            string strSql = "select DATEDIFF(SECOND, GETDATE(), CONVERT(char(10), dateadd(dd,1,getdate()), 120) )";
            i = DBHelper.SqlHelper.GetDataItemString(strSql);
            return i;
        }
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error", "Home");
            }

            string key_pwdid = CommonTool.Common.GetAppSetting("pwdid");
            if (id != key_pwdid)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewData["id_key"] = id;

            string tieManageWebSite = CommonTool.Common.GetAppSetting("tieManageSysServer");
            ViewData["tieManageSysServer"] = tieManageWebSite;

            return View();
        }

        public ActionResult UserNew_Hour(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error", "Home");
            }

            string key_pwdid = CommonTool.Common.GetAppSetting("pwdid");
            if (id != key_pwdid)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewData["id_key"] = id;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            ViewData["dtm"] = dtm;

            string dtm_compare = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

           
            BLL.BLL bll = new BLL.BLL();

            DataTable dt = bll.Sum_NewUser_Hour(Convert.ToDateTime(dtm));
            DataTable dt_com = bll.Sum_NewUser_Hour(Convert.ToDateTime(dtm_compare));
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            string data_com = CommonTool.JsonHelper.DataTableToJSON(dt_com);
            ViewData["data"] = data;
            ViewData["data_com"] = data_com;


            int cur_year = DateTime.Now.Year;
            int cur_month = DateTime.Now.Month;
            int cur_day = DateTime.Now.Day;
            int cur_hour = DateTime.Now.Hour;

            string cur_start = new DateTime(cur_year, cur_month, cur_day, cur_hour, 0, 0).ToString();
            int cur_hour_count = bll.Summary_NewUser_Count(cur_start, DateTime.Now.ToString());
            int today_count = bll.Summary_NewUser_Count(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString());

            ViewData["cur_hour_count"] = cur_hour_count;
            ViewData["today_count"] = today_count;

            return View();
        }


        public ActionResult ScanOnlineUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error", "Home");
            }

            string key_pwdid = CommonTool.Common.GetAppSetting("pwdid");
            if (id != key_pwdid)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewData["id_key"] = id;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            //获取当前在线用户
            string url = servUrl_AppData + "/App/GetCnfigUids_Cur";
            string uids = CommonTool.Common.GetHtmlFromUrl(url);

            BLL.BLL bll = new BLL.BLL();
            DataTable dt = bll.GetInnerOnlineUser();

            ViewBag.dt = dt;
 
            return View();
        }

        public ActionResult test()
        {
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
                            where  ab.create_time > dateadd(day, -1, getdate())
                            order by check_time desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            ViewBag.dt = dt;

            return View();
        }
    
        public ActionResult Server_Time(string id)
        {
            return View();
        }
        #region 概览
        public ActionResult Check_UserInfo()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public ActionResult Check_Survey(string id)
        {
            return View();
        }
        public ActionResult Score_Tx(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public ActionResult Gift_TouSu(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
          
            return View();
        }
        public ActionResult List_Reward(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            ViewData["dtm"] = dtm;

            return View();
        }
        public ActionResult Chat_User(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public ActionResult Check_User_FeedBack(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public ActionResult Check_User_Complain(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public ActionResult Check_User_Album(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

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
                            where  ab.create_time > dateadd(day, -1, getdate()) and {0}
                            order by create_time desc";
            string strWhere = "1=1";
            if (!string.IsNullOrEmpty(id))
            {
                strWhere = string.Format("check_state={0}", id);
            }
            strSql = string.Format(strSql, strWhere);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            ViewBag.dt = dt;

            return View();
        }
        public ActionResult Kill_UnDo(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public ActionResult Seek_People(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public ActionResult User_Chart_AB(string dtm, string uid_from, string uid_to)
        {
            DateTime dtm_Data = Convert.ToDateTime(dtm);
            string start = dtm_Data.ToString("yyyy-MM-dd") + " 00:00:00";
            string end = dtm_Data.ToString("yyyy-MM-dd") + " 23:59:59";
            string data = GetChartA_B(start, end, uid_from, uid_to);
            ViewData["data"] = data;

            ViewData["to"] = uid_to;
            ViewData["from"] = uid_from;


            //获取头像
            string strSql = string.Format("select photo, nick from dbo.[user] where uid = '{0}'", uid_from);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string photo_from = dt.Rows[0]["photo"].ToString();
            string nick_from = dt.Rows[0]["nick"].ToString();

            strSql = string.Format("select photo, nick from dbo.[user] where uid = '{0}'", uid_to);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string photo_to = dt.Rows[0]["photo"].ToString();
            string nick_to = dt.Rows[0]["nick"].ToString();

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["photo_from"] = photo_from;
            ViewData["photo_to"] = photo_to;
            ViewData["nick_from"] = nick_from;
            ViewData["nick_to"] = nick_to;

            return View();
        }
        #endregion
        public string GetChartA_B(string start, string end, string from, string to)
        {
            string table_name = bll.GetTableName_msg(end);

            string strSql = @"select *, RIGHT(CONVERT(char(20), dtm, 120), 10) as sj from {4}
                                    where dtm >= '{0}' 
                                        and dtm <= '{1}'
                                        and (([uid_from] = '{2}' and [uid_to] = '{3}') or ([uid_from] = '{3}' and [uid_to] = '{2}'))
                                        order by dtm asc ";
            strSql = string.Format(strSql, start, end, from, to, table_name);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return data;

        }

        public string GetTalkMsg(string id, string dtm)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }
            if (string.IsNullOrEmpty(dtm))
            {
                dtm = "";
            }
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_Msg");
            string url = servUrl_AppData + "/Home/GetTalkMsg/" + id + "?dtm=" + dtm;
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }
        #region 对外服务部分

        public string OutService_GetChatScore(string user_name, string dtm)
        {
            string strRtn = "0";

            BLL.BLL bll = new BLL.BLL();
            Dictionary<string, int> dic = bll.Get_InnerScore(dtm, user_name);
            int total = 0;
            foreach (string key in dic.Keys)
            {
                total += dic[key];
            }

            strRtn = total.ToString();

            return strRtn;
        }
        public string OutService_GetChatScoreDtl(string user_name, string dtm)
        {
            string strRtn = "0";

            BLL.BLL bll = new BLL.BLL();
            Dictionary<string, int> dic = bll.Get_InnerScore(dtm, user_name);
            strRtn = CommonTool.JsonHelper.ObjectToJSON(dic);

            return strRtn;
        }
        //上线---打招呼
        public string SetOnline_SayHelloMsg()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ReloadCnfig_SayHelloMsg";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }
        //上线---vue常量
        public string SetOnline()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ReloadConstData";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }
        //上线---金币
        public string SetOnline_Coin()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ReloadCnfigCoin";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }
        //上线---会员
        public string SetOnline_Member()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ReloadCnfigMem";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }
        //上线---首充
        public string SetOnline_Mem_First()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ReloadCnfigMem_First";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }
        //上线---节日活动
        public string SetOnline_Mem_Fest()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ReloadCnfigMem_Fest";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }

        public string SetOnline_Uids()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ReloadCnfigUids";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }

        public string Uids_Add(string id)
        {
            //检查uid的合法性（是否为uid, 是否为内部用户）

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/AddCnfigUid/?uid=" + id;
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }

        public string Remove_Uid(string id)
        {
            //检查uid的合法性（是否为uid, 是否为内部用户）

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/RemoveCnfigUid/?uid=" + id;
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }

        public string Clear_Uids()
        {
            //检查uid的合法性（是否为uid, 是否为内部用户）

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/ClearCnfigUid";
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }

        public string Remove_ReceMatch(string id)
        {
            //检查uid的合法性（是否为uid, 是否为内部用户）

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/App/Remove_ReceMath/?uid=" + id;
            string rst = CommonTool.Common.GetHtmlFromUrl(url);
            return rst;
        }
        
        #endregion 

        public string TestMsg()
        {
            string rst = External.SMS.SendMsg("13545121451", "验证码:[999000],数据接口异常，需要紧急处理", CommonTool.Common.GetAppSetting("appName"));
            return rst;
        }

        #region 对外服务部分 cmd-msg 给消息处理服务调用

        //每分钟调用一次
        public string OutService_Minute()
        {
            string strRtn = "";

            //检测数据接口服务是否正常
            bll.Check_AppData_I();


            //每分钟搬运一次金币积分到缓存表中
            strRtn += bll.CopyCoinScore_ToCache();


            //每天凌晨5点钟，生成邀请收益数据
            DateTime dtm_now = DateTime.Now;
            int hour = dtm_now.Hour;
            int minute = dtm_now.Minute;
            if (hour == 5 && minute == 1)
            {
                strRtn += "\r\n";
                strRtn += Create_InviteScore(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            }

            CommonTool.WriteLog.Write(strRtn);
            return strRtn;
        }

        public string Create_InviteScore(string dtm)
        {
            string strRtn = "";
            DateTime date = DateTime.Now;

            if (!string.IsNullOrEmpty(dtm))
            {
                date = Convert.ToDateTime(dtm);
            }

            int count = 0;

            List<string> listAgent = bll.Get_Invite_Agent(date);
            foreach (string uid in listAgent)
            {
                count += bll.Create_InviteFriend_Score(uid, date);
            }

            strRtn = string.Format("生成邀请收益，计算【{0}】个代理，共计【{1}】条邀请积分",listAgent.Count, count);
            return strRtn;
        }

        #endregion 

        //对外服务，测试,通过管理员后台拿到正式环境下的语音数据
        public string Get_Msg_Voice(string dtm, string uid, string size, string time)
        {
            string strRtn = "";

            //获取消息显示控制
            BLL.Common com = new BLL.Common();
            string admin_voice_data = com.GetSysDicByKey("admin_voice_data");
            if (admin_voice_data != "1")
            {
                return strRtn;
            }

            if (string.IsNullOrEmpty(size))
            {
                size = "20";
            }

            if (string.IsNullOrEmpty(dtm))
            {
                dtm = DateTime.Now.ToString("yyyy-MM-dd");
            }
            string tb_name = bll.GetTableName_msg(dtm);


            string strWhere = "";
            if (!string.IsNullOrEmpty(uid))
            {
                strWhere += string.Format(" and msg.uid_from = '{0}'", uid);
            }
            if (!string.IsNullOrEmpty(time))
            {
                strWhere += string.Format(" and msg.dtm < '{0}'", time);
            }


            string strSql = @"select top {1} msg.id,
       CONVERT(char(20), msg.dtm, 120) as dtm,
       msg.uid_from, msg.uid_to, msg.state, msg.type, msg.txt,
       u1.photo, u1.nick, u1.sex, u1.coin_total, u1.coin, u1.score_total, u1.score,
       u2.photo as photo2, u2.nick as nick2, u2.sex as sex2, u2.coin_total as coin_total2, u2.coin as coin2, u2.score_total as score_total2, u2.score as score2
from {0} msg
left join dbo.[user] u1 on (msg.uid_from = u1.uid)
left join dbo.[user] u2 on (msg.uid_to = u2.uid)
where (type = 'voice' or type = 'voice_res') {2}
order by dtm desc";
            strSql = string.Format(strSql, tb_name, size, strWhere);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);

            return strRtn;
        }
    }
}
