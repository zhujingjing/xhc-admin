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
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            ContentType = "application/json; charset=utf-8";
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            response.ContentType = ContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                response.Write(CommonTool.JsonHelper.DataTableToJSON((DataTable)Data));
            }
        }
    }

    public class Admin1Controller : Controller
    {
        BLL.BLL bll = new BLL.BLL();
          
        #region 框架部分
        public ActionResult Frame_Main(string id)
        {
            BLL.Data_Sys_User u = new BLL.Data_Sys_User();
            if (Session["Data_Sys_User"] != null)
            {
                u = (BLL.Data_Sys_User)Session["Data_Sys_User"];
            }
            else
            {
                string CtrName = "Admin1";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("admin1", "Admin1");
                dic.Add("seller", "Seller");
                dic.Add("shop", "Shop");
                if (dic.ContainsKey(id))
                {
                    CtrName = dic[id];
                }

                return RedirectToAction("Login", CtrName);
            }

            ViewData["sys_type"] = id;
            return View();
        }
        public ActionResult Frame_Top()
        {

            string sys_user_type = string.Empty;
            string sys_user_id = string.Empty;
            string sys_user_name = string.Empty;
            string sys_user_comment = string.Empty;

            BLL.Data_Sys_User u = new BLL.Data_Sys_User();
            if (Session["Data_Sys_User"] != null)
            {
                u = (BLL.Data_Sys_User)Session["Data_Sys_User"];

                sys_user_type = u.Type;
                sys_user_id = u.Id;
                sys_user_name = u.User_Name;
                sys_user_comment = u.Comment;
            }

            ViewData["sys_user_type"] = sys_user_type;
            ViewData["sys_user_id"] = sys_user_id;
            ViewData["sys_user_name"] = sys_user_name;
            ViewData["sys_user_comment"] = sys_user_comment;

            return View();
        }
        public ActionResult Frame_Left(string id)
        {
            string sys_user_type = string.Empty;
            string sys_user_id = string.Empty;
            string sys_user_name = string.Empty;
            string sys_user_comment = string.Empty; 

            BLL.Data_Sys_User u = new BLL.Data_Sys_User();
            if (Session["Data_Sys_User"] != null)
            {
                u = (BLL.Data_Sys_User)Session["Data_Sys_User"];

                sys_user_type = u.Type;
                sys_user_id = u.Id;
                sys_user_name = u.User_Name;
                sys_user_comment = u.Comment;
            }

            ViewData["sys_user_type"] = sys_user_type;
            ViewData["sys_user_id"] = sys_user_id;
            ViewData["sys_user_name"] = sys_user_name;
            ViewData["sys_user_comment"] = sys_user_comment;

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string id)
        {
            if (!string.IsNullOrEmpty(id) && id == "exit")
            {
                Session.Clear();
            }
            return View();
        }

        public string SysLogin(string user_name, string pwd)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            BLL.User user = new BLL.User();
            BLL.Data_Sys_User u = user.GetSysUserData(user_name, pwd);

            if (u.Login)
            {
                info.State = "1";
                info.Msg = u.Type;

                //增加一条登录记录
                string username = u.User_Name;
                string type = u.Type;
                string userid = u.Id;
                string ip = CommonTool.HttpWeb.GetUserIP();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("user_name", username);
                dic.Add("user_id", userid);
                dic.Add("type", type);
                dic.Add("login_time", DateTime.Now.ToString());
                dic.Add("ip", ip);

                user.CreateLoginRecord(dic);

            }
            else
            {
                info.State = "0";
                info.Msg = "账号不存在或者密码错误";
            }
            return info.ToString();
        }

        #region 系统参数相关（sys_dic表）
        public ActionResult SysSet_Var()
        {
            return View();
        }

        public ActionResult SysSet_VarAdd(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }

            BLL.Common com = new BLL.Common();
            DataTable dt = com.GetSysDicByID(id);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;
            ViewData["id"] = id;
            return View();
        }

        public string GetSysSetParm(string pageIndex, string pageSize, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strSql = @"select id, [key], value, [type], comment from  dbo.sys_dic";
            //strSql = string.Format(strSql, where);
            BLL.Common comm = new BLL.Common();

            string strSort = "[key] desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);
            return strRtn;
        }

        public string SaveSysDicInfo(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            string id = dicParm.ContainsKey("id") ? dicParm["id"] : "";
            string key = dicParm.ContainsKey("key") ? dicParm["key"] : "";
            string value = dicParm.ContainsKey("value") ? dicParm["value"] : "";
            string type = dicParm.ContainsKey("type") ? dicParm["type"] : "";
            string comment = dicParm.ContainsKey("comment") ? dicParm["comment"] : "";

            BLL.Common com = new BLL.Common();
            bool tag = false;
            if (string.IsNullOrEmpty(id))
            {
                tag = com.SaveSysDicData(key, value, type, comment);
            }
            else
            {
                tag = com.UpdateSysDic(id, key, value, type, comment);
            }
            if (tag == true)
            {
                info.State = "1";
                info.Msg = "成功";

                //远程调用--刷新数据接口中数据
                string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");

                string url = servUrl_AppData + "/App/ReloadSysDic";
                string strResponse = CommonTool.Common.GetHtmlFromUrl(url);
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }

            return info.ToString();
        }

        public string SaveSysUserInfo(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            string id = dicParm.ContainsKey("id") ? dicParm["id"] : "";

            BLL.User u = new BLL.User();
            bool tag = false;
            if (string.IsNullOrEmpty(id))
            {
                //添加数据
                dicParm.Remove("id");
                string strid = u.Sys_User_Insert(dicParm);
                if (!string.IsNullOrEmpty(strid))
                {
                    tag = true;
                }
            }
            else
            {
                //修改数据
                tag = u.Sys_User_Update(id, dicParm);
            }

            if (tag)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }

            return info.ToString();
        }

        public string Get_Sys_DB(string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.Common com = new BLL.Common();
            string strSql = @"SELECT
                            TableName = obj.name,
                            TotalRows = prt.rows,
                            SpaceUsed = SUM(alloc.used_pages)*8
                            FROM sys.objects obj
                            JOIN sys.indexes idx on obj.object_id = idx.object_id
                            JOIN sys.partitions prt on obj.object_id = prt.object_id
                            JOIN sys.allocation_units alloc on alloc.container_id = prt.partition_id
                            WHERE
                            obj.type = 'U' AND idx.index_id IN (0, 1)
                            GROUP BY obj.name, prt.rows
                            ORDER BY TotalRows DESC";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            string rows = dt.Rows.Count.ToString();
            CommonTool.WriteLog.Write(rows);
            strRtn = com.GetMiniUIData2(rows, data);

            return strRtn;
        }

        #endregion 

        #endregion

        #region 渠道二维码

        public ActionResult Sys_Create_FzQrCode()
        {
            return View();
        }

        public string DoCreate_QrCode(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (string.IsNullOrEmpty(id))
            {
                info.State = "0";
                info.Msg = "参数为空";
                return info.ToString();
            }

            id = id.Trim();

            if (!CommonTool.Common.IsNumber(id))
            {
                info.State = "0";
                info.Msg = "二维码参数错误";
                return info.ToString();
            }

            int qrcode_parm = Convert.ToInt32(id);

            string file_path = CommonTool.WXOperate.GetQrcode(qrcode_parm);

            info.State = "1";
            info.Msg = file_path;

            return info.ToString();

        }

        #endregion

   

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

        public ActionResult Album_dtl(string url)
        {
            ViewData["url"] = url;
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
            if(!string.IsNullOrEmpty(sort))
            strSort = sort;


            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string Check_Album(string id, string state)
        {

            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = @"update dbo.album 
                                    set check_state='{0}',check_time=GETDATE()
                                    where id='{1}'";
            strSql = string.Format(strSql, state, id);
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

        public ActionResult Chat_Mny()
        {
            return View();
        }

        public ActionResult ChatMny_Sum()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetChatMynSumData(string pageIndex, string pageSize, string dtm, string uid)
        {
            //开始位置打印
            // CommonTool.WriteLog.Write("pageIndex--->" + pageIndex + ";pageSize--->" + pageSize + ";dtm--->" + dtm + ";uid--->" + uid);

            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

         
            string strWhere = "(1=1)";
            string strWhere_sub = "(1=1)";
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere += string.Format(" and (sc.dtm >= '{0} 00:00:00' and sc.dtm <= '{0} 23:59:59') ", dtm);
                strWhere_sub += string.Format(" and (dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59') ", dtm);
            }
            if (!string.IsNullOrEmpty(uid))
            {
                strWhere = string.Format(" sc.uid_from = '{0}'", uid);
                strWhere_sub = string.Format(" uid_from = '{0}'", uid);
            }

            string strSql = @"select CONVERT(char(10), sc.dtm, 120) as dtm,
                                   sc.uid_from,
                                   u.photo,u.nick,u.sex,u.chat_mny_score,
                                   ISNULL(u.remark,'') as remark,
                                   CONVERT(char(20), u.create_time,120) as create_time,
                                   sum(sc.amount) as score, 
                                   COUNT(*) as chat_count,
                                   CONVERT(char(20), MAX(sc.dtm), 120) as dtm_max,
                                   ISNULL(score_ex.score_total, 0) as score_total,
                                   ISNULL(score_ex.score_hello, 0) as score_hello,
                                   ISNULL(score_ex.score_anser, 0) as score_anser,
                                   ISNULL(score_ex.score_online, 0) as score_online,
                                   ISNULL(score_ex.score_login, 0) as score_login,
                                   ISNULL(score_ex.score_gift, 0) as score_gift
                            from dbo.score_msg sc 
                            left join dbo.[user] u on (sc.uid_from = u.uid)
                            left join (
										select uid_from , CONVERT(char(10), dtm, 120) as dtm_sub, sum(amount) as score_total,
                                               max(case when type_dtl = '任务奖励-打招呼' then amount else 0 end) as score_hello,
                                               max(case when type_dtl = '任务奖励-回复' then amount else 0 end) as score_anser,
                                               max(case when type_dtl = '任务奖励-在线' then amount else 0 end) as score_online,
                                               max(case when type_dtl = '任务奖励-登录' then amount else 0 end) as score_login,
                                               max(case when type_dtl = '任务奖励-礼物' then amount else 0 end) as score_gift
										from dbo.score  
										where {1}
										and (type_dtl in ('任务奖励-登录','任务奖励-打招呼','任务奖励-回复','任务奖励-分享','任务奖励-在线','任务奖励-礼物'))
										group by uid_from,CONVERT(char(10), dtm, 120)
										) 
										as score_ex on (sc.uid_from = score_ex.uid_from and score_ex.dtm_sub = CONVERT(char(10), sc.dtm, 120))
                            where {0}
                            group by CONVERT(char(10), dtm, 120),
                                   sc.uid_from ,u.photo,u.nick,u.sex,u.chat_mny_score, u.remark,u.create_time,
                                   score_ex.score_total, score_ex.score_hello , score_ex.score_anser,score_ex.score_online,score_ex.score_login,score_ex.score_gift ";
                                    
            strSql = string.Format(strSql, strWhere, strWhere_sub);
            string strSort = "score desc";
            if (!string.IsNullOrEmpty(uid))
            {
                strSort = "dtm desc";
            }

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            //返还前打印
            //CommonTool.WriteLog.Write(strRtn);
            return strRtn;
        }
        

        public ActionResult ChatMny_Dtl(string uid, string dtm)
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;


            ViewData["dtm"] = dtm;

            ViewData["uid"] = uid;
            return View();
        }
        public string GetChatMynSumDtl(string from_uid, string to_uid, string sort, string user_filter, string evaluate_filter, string pageIndex, string pageSize, string dtm)
        {

            //开始位置打印
            //CommonTool.WriteLog.Write("from_uid--->" + from_uid + ";to_uid--->" + to_uid + ";sort--->" + sort + ";user_filter--->" + user_filter + ";evaluate_filter--->" + evaluate_filter + ";pageIndex--->" + pageIndex + ";pageSize--->" + pageSize + ";dtm--->" + dtm);

            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strSql = bll.GetSQL_ChatScoreGrid(from_uid, to_uid, dtm, user_filter, evaluate_filter);

            string strSort = "dtm_start desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }


            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            //返还前打印
            //CommonTool.WriteLog.Write(strRtn);
            return strRtn;
        }

        public ActionResult ChatMny_Sum_Line(string dtm)
        {
            DateTime dtm_base = DateTime.Now;
            if (!string.IsNullOrEmpty(dtm))
            {
                dtm_base = Convert.ToDateTime(dtm);
            }

            DataTable dt = bll.GetChatScoreSum_Hour(dtm_base);
            DataTable dt_yest = bll.GetChatScoreSum_Hour(dtm_base.AddDays(-1));
            DataTable dt_pre2 = bll.GetChatScoreSum_Hour(dtm_base.AddDays(-2));

            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            string data_yest = CommonTool.JsonHelper.DataTableToJSON(dt_yest);
            string data_pre2 = CommonTool.JsonHelper.DataTableToJSON(dt_pre2);

            ViewData["dtm"] = dtm_base;
            ViewData["data"] = data;
            ViewData["data_yest"] = data_yest;
            ViewData["data_pre2"] = data_pre2;

            ViewData["dtm_1"] = dtm_base.AddDays(-1).ToString("yyyy-MM-dd");
            ViewData["dtm_2"] = dtm_base.AddDays(-2).ToString("yyyy-MM-dd");

            return View();
        }

        public ActionResult ChatMny_Sum_Pie(string dtm, object sum_people, object sum_score)
        {
            ViewData["dtm"] = dtm;
            ViewData["sum_people"] = sum_people;
            ViewData["sum_score"] = sum_score;
            CommonTool.WriteLog.Write("dtm--->" + dtm + ";sum_people--->" + sum_people + ";sum_score--->" + sum_score);

            return View();
        }

        public ActionResult ChatMny_Sum_Line30()
        {
            DataTable dt = bll.GetChatScoreSum_Day(30);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            ViewData["data"] = data;

            dt = bll.GetChatCoinSum_Day(30);
            data = CommonTool.JsonHelper.DataTableToJSON(dt);
            ViewData["data_coin"] = data;

            return View();
        }

        public ActionResult Chat_Mny_Abnormal()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public string GetChatMnyAbnormalData(string pageIndex, string pageSize, string dtm)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);


            BLL.Common com = new BLL.Common();
            string strSql = @"select score.*, ISNULL(coin.coin,0) as coin, score.score - coin.coin as diff
                            from 
                            (
                            select uid_from, uid_to, sum(amount) as score 
                            from dbo.score_msg 
                            where dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59'
                            group by uid_from, uid_to
                            ) as score
                            left join 
                            (
                            select uid_from, uid_to, sum(amount) as coin
                            from dbo.coin_msg 
                            where dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59'
                            group by uid_from, uid_to
                            ) as coin on (score.uid_from = coin.uid_to and score.uid_to = coin.uid_from)
                            where score.score > coin.coin";
            strSql = string.Format(strSql, dtm);

            string strSort = "diff desc";
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public ActionResult Chat_Mny_AbnormalDtl(string uid_from, string uid_to, string dtm)
        {

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["uid_from"] = uid_from;
            ViewData["uid_to"] = uid_to;
            ViewData["dtm"] = dtm;

            return View();
        }

        public string GetAbnormalDtlData(string pageIndex, string pageSize,string uid_from, string uid_to, string dtm)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.Common com = new BLL.Common();
            string strSql = @"select dtm, uid_from, uid_to, amount, '收入' as msg_type,
                               u.photo, u.sex,u.nick
                                from dbo.score_msg m
                                left join dbo.[user] u on(m.uid_from = u.uid)
                                where uid_from='{0}' and uid_to = '{1}' and (dtm >= '{2} 00:00:00' and dtm <= '{2} 23:59:59')

                                union all 

                                select dtm, uid_from, uid_to, amount, '支出' as msg_type,
                                       u.photo, u.sex,u.nick
                                from dbo.coin_msg m
                                left join dbo.[user] u on(m.uid_from = u.uid)
                                where uid_from='{1}' and uid_to = '{0}' and (dtm >= '{2} 00:00:00' and dtm <= '{2} 23:59:59')";
            strSql = string.Format(strSql, uid_from, uid_to, dtm);
            string strSort = "dtm asc";
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public ActionResult ChatMny_Analyse()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public string GetChatScoreAnylysData(string pageIndex, string pageSize, string days, string sort)
        {
            string strRtn = string.Empty;

            if (string.IsNullOrEmpty(days))
            {
                days = "7";
            }

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            DateTime dtm_now = DateTime.Now;

            int day = int.Parse(days);
            string where_day = dtm_now.AddDays(-day).ToString("yyyy-MM-dd");

            string day_0 = dtm_now.ToString("yyyy-MM-dd");
            string day_1 = dtm_now.AddDays(-1).ToString("yyyy-MM-dd");
            string day_2 = dtm_now.AddDays(-2).ToString("yyyy-MM-dd");
            string day_3 = dtm_now.AddDays(-3).ToString("yyyy-MM-dd");
            string day_4 = dtm_now.AddDays(-4).ToString("yyyy-MM-dd");
            string day_5 = dtm_now.AddDays(-5).ToString("yyyy-MM-dd");
            string day_6 = dtm_now.AddDays(-6).ToString("yyyy-MM-dd");
            string day_7 = dtm_now.AddDays(-7).ToString("yyyy-MM-dd");


            string strSql = @"select u.uid,
                   u.photo, u.nick, u.sex, u.remark,
                   CONVERT(char(20), u.create_time, 120) as create_time, 
                   CONVERT(char(20), u.least_time, 120) as least_time, 
                   sum(1) as chat_count,
                   sum(case when dtm >= '{1} 00:00:00' and dtm <= '{1} 23:59:59' then 1 else 0 end) as chat_count0,
                   sum(case when dtm >= '{2} 00:00:00' and dtm <= '{2} 23:59:59' then 1 else 0 end) as chat_count1,
                   sum(case when dtm >= '{3} 00:00:00' and dtm <= '{3} 23:59:59' then 1 else 0 end) as chat_count2,
                   sum(case when dtm >= '{4} 00:00:00' and dtm <= '{4} 23:59:59' then 1 else 0 end) as chat_count3,
                   sum(case when dtm >= '{5} 00:00:00' and dtm <= '{5} 23:59:59' then 1 else 0 end) as chat_count4,
                   sum(case when dtm >= '{6} 00:00:00' and dtm <= '{6} 23:59:59' then 1 else 0 end) as chat_count5,
                   sum(case when dtm >= '{7} 00:00:00' and dtm <= '{7} 23:59:59' then 1 else 0 end) as chat_count6,
                   sum(case when dtm >= '{8} 00:00:00' and dtm <= '{8} 23:59:59' then 1 else 0 end) as chat_count7
            from dbo.[user] u
            left join dbo.score_chat sc on (u.uid = sc.uid_from)
            where u.least_time >  '{0} 00:00:00' and u.sex='女生'  
                  and sc.dtm >  '{8} 00:00:00' 
            group by u.uid,u.photo, u.nick, u.sex, u.remark,
           CONVERT(char(20), u.create_time, 120),
           CONVERT(char(20), u.least_time, 120)";
            strSql = string.Format(strSql, where_day, day_0, day_1, day_2, day_3, day_4, day_5, day_6, day_7);
            BLL.Common comm = new BLL.Common();

            string strSort = "chat_count desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public ActionResult OrderList(string id)
        {
            //CommonTool.WriteLog.Write(id);
            

            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            
            if (id == "渠道商")
            {
                start = string.IsNullOrEmpty(start) ? DateTime.Now.AddMonths(-1).ToString() : start;
            }
            else
            {
                start = string.IsNullOrEmpty(start) ? DateTime.Now.AddHours(-24).ToString() : start;
            }

            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public ActionResult CheckWxList(string id)
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(1).ToString() : end;

            start = "2019-05-01";

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public ActionResult Teacher_Student(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public ActionResult Master_Score(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            ViewData["dtm"] = dtm;
            return View();
        }
        public ActionResult Student_List(string id)
        {
            string uid = string.Empty;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["uid"] = id;
            return View();
        }
        public ActionResult Chat_User(string id)
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(1).ToString() : end;

            start = "2019-05-01";

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

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
        public ActionResult Kill_UnDo()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public ActionResult Warn_User()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }
        public ActionResult Wx_Notice_Record()
        {

            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00" : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59" : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }

        public ActionResult SysSet_Config()
        {
            return View();
        }

        public ActionResult SysSet_VueConst()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string url = servUrl_AppData + "/App/GetConstData_Cur";
            string constData = CommonTool.Common.GetHtmlFromUrl(url);
            ViewData["data"] = constData;
            return View();
        }

        public ActionResult SysSet_VueConst_Json()
        {
            return View();
        }

        public ActionResult SysSet_Hello()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string url = servUrl_AppData + "/App/GetCnfigSayHello_Cur";
            string constData = CommonTool.Common.GetHtmlFromUrl(url);
            ViewData["data"] = constData;
            return View();
        }
        public ActionResult Administration_GIF()
        {
            return View();
        }
        public ActionResult SysSet_Coin()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string url = servUrl_AppData + "/App/GetCnfigCoin_Cur";
            string constData = CommonTool.Common.GetHtmlFromUrl(url);
            ViewData["data"] = constData;
            return View();
        }
        public ActionResult SysSet_Member()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string url = servUrl_AppData + "/App/GetCnfigMem_Cur";
            string constData = CommonTool.Common.GetHtmlFromUrl(url);
            ViewData["data"] = constData;
            return View();
        }
        //购买首充配置
        public ActionResult SysSet_FirstCharge()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string url = servUrl_AppData + "/App/GetCnfigMemFirst_Cur";
            string constData = CommonTool.Common.GetHtmlFromUrl(url);
            ViewData["data"] = constData;
            return View();
        }
        //节日活动配置
        public ActionResult SysSet_Festival()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string url = servUrl_AppData + "/App/GetCnfigMemFest_Cur";
            string constData = CommonTool.Common.GetHtmlFromUrl(url);
            ViewData["data"] = constData;
            return View();
        }
        public ActionResult SysSet_MatchUids()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string url = servUrl_AppData + "/App/GetCnfigUids";
            string uids = CommonTool.Common.GetHtmlFromUrl(url);

            ViewData["uids"] = uids;

            return View();
        }
        public string GetMatchUidsData(string id, string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);


            DataTable dt = bll.GetInnerOnlineUser();

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData2(rows.ToString(), data);

            return strRtn;
        }
        public ActionResult SysSet_JsonCode()
        {
            return View();
        }
        public ActionResult Sys_DB()
        {
            return View();
        }
        public ActionResult SysUserList()
        {
            return View();
        }

        #region 客服工作台
        public ActionResult CustomerService_Workbench()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }

        public string GetCustomerServiceTasks()
        {
            string strRtn = string.Empty;

            // 这里将集成各种任务数据
            // 暂时返回空数据结构
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("task_type", typeof(string));
            dt.Columns.Add("task_name", typeof(string));
            dt.Columns.Add("priority", typeof(string));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("create_time", typeof(string));
            dt.Columns.Add("description", typeof(string));

            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData2("0", data);

            return strRtn;
        }

        public string UpdateTaskStatus(string id, string status)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            // 这里将实现任务状态更新逻辑
            // 暂时返回成功
            info.State = "1";
            info.Msg = "成功";

            return info.ToString();
        }

        public ActionResult CustomerService_TaskDetail(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        #endregion

        #region 下拉框配置管理
        public ActionResult DropdownConfig()
        {
            return View();
        }

        public ActionResult DropdownConfigAdd(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }

            BLL.Common com = new BLL.Common();
            DataTable dt = com.GetDropdownConfigById(id);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;
            ViewData["id"] = id;
            return View();
        }

        public string GetDropdownConfigData(string pageIndex, string pageSize, string category)
        {
            string strRtn = string.Empty;
            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(category))
            {
                where += string.Format(" and category = '{0}'", category);
            }

            string strSql = @"select id, [key] as [key], value, category, parent_key, parent_category, sort_order, status, comment, 
                             CONVERT(varchar(20), create_time, 120) as create_time, 
                             CONVERT(varchar(20), update_time, 120) as update_time 
                             from dbo.dropdown_config 
                             where " + where;

            BLL.Common comm = new BLL.Common();
            string strSort = "category, sort_order asc";
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);
            return strRtn;
        }

        public string SaveDropdownConfig(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            string id = dicParm.ContainsKey("id") ? dicParm["id"] : "";
            string key = dicParm.ContainsKey("key") ? dicParm["key"] : "";
            string value = dicParm.ContainsKey("value") ? dicParm["value"] : "";
            string category = dicParm.ContainsKey("category") ? dicParm["category"] : "";
            string parentKey = dicParm.ContainsKey("parent_key") ? dicParm["parent_key"] : "";
            string parentCategory = dicParm.ContainsKey("parent_category") ? dicParm["parent_category"] : "";
            int sortOrder = dicParm.ContainsKey("sort_order") ? Convert.ToInt32(dicParm["sort_order"]) : 0;
            int status = dicParm.ContainsKey("status") ? Convert.ToInt32(dicParm["status"]) : 1;
            string comment = dicParm.ContainsKey("comment") ? dicParm["comment"] : "";

            BLL.Common com = new BLL.Common();
            bool tag = false;
            if (string.IsNullOrEmpty(id))
            {
                tag = com.SaveDropdownConfig(key, value, category, parentKey, parentCategory, sortOrder, status, comment);
            }
            else
            {
                tag = com.UpdateDropdownConfig(id, key, value, category, parentKey, parentCategory, sortOrder, status, comment);
            }

            if (tag == true)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }
            return info.ToString();
        }

        public string DeleteDropdownConfig(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            BLL.Common com = new BLL.Common();
            bool tag = true;
            string[] ids = id.Split(',');
            foreach (string item in ids)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (!com.DeleteDropdownConfig(item))
                    {
                        tag = false;
                        break;
                    }
                }
            }
            if (tag == true)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }
            return info.ToString();
        }

        public string GetDropdownOptions(string category)
        {
            BLL.Common com = new BLL.Common();
            DataTable dt = com.GetDropdownConfigByCategory(category);
            return CommonTool.JsonHelper.DataTableToJSON(dt);
        }

        public string GetCascadingDropdownOptions(string category, string parentKey, string parentCategory)
        {
            BLL.Common com = new BLL.Common();
            DataTable dt = com.GetCascadingDropdownConfig(category, parentKey, parentCategory);
            return CommonTool.JsonHelper.DataTableToJSON(dt);
        }

        [HttpPost]
        public string GetBatchDropdownOptions(string categories)
        {
            BLL.Common com = new BLL.Common();
            Dictionary<string, DataTable> result = new Dictionary<string, DataTable>();
            
            string[] categoryArray = categories.Split(',');
            foreach (string category in categoryArray)
            {
                if (!string.IsNullOrEmpty(category))
                {
                    DataTable dt = com.GetDropdownConfigByCategory(category);
                    result[category] = dt;
                }
            }
            
            // 将Dictionary转换为JSON
            StringBuilder json = new StringBuilder();
            json.Append("{");
            bool first = true;
            
            foreach (string category in result.Keys)
            {
                if (!first)
                {
                    json.Append(",");
                }
                first = false;
                
                json.AppendFormat("\"{0}\":{1}", category, CommonTool.JsonHelper.DataTableToJSON(result[category]));
            }
            
            json.Append("}");
            return json.ToString();
        }
        #endregion


        public string GetSysUserData(string pageIndex, string pageSize, string user_type)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere = "";
            if (string.IsNullOrEmpty(user_type))
            {
                return strRtn;
            }

            
            if (user_type == "普通用户")
            {
                strWhere = " where [type] = '渠道商' or [type] = '客服'";
            }
            else
            {
                strWhere = string.Format(" where [type] = '{0}'", user_type);
            }
            if (user_type == "平台管理员")
            {
                strWhere = "";
            }
            


            BLL.Common com = new BLL.Common();
            string strSql = @"select id,
                               user_name,
                               user_pwd,
                               type,
                               ISNULL(comment, '') as name,
                               ISNULL(comment, '') as comment,
                               ISNULL(is_use, '') as is_use
                        from dbo.sys_user {0}";
            strSql = string.Format(strSql, strWhere);

            string strSort = "name desc";
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        public ActionResult SysUser_Add(string id, string user_type)
        {
            string data = "";
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }
            else
            {
                BLL.User user = new BLL.User();
                DataTable dt = user.Sys_User_List_byId(id);
                data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            }
            ViewData["user_type"] = user_type;
            ViewData["data"] = data;
            ViewData["id"] = id;
            return View();
        }
        public ActionResult Login_Record()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];
            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;
            return View();
        }
        public string Get_Login_info(string pageIndex, string pageSize, string start, string end)
        {
            //CommonTool.WriteLog.Write("start:"+ start +"，end:"+end);
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.Common com = new BLL.Common();
            string strSql = @"select r.user_name,
                               r.type,
                               CONVERT(char(20), r.login_time, 120) as login_time,
                               r.ip,
                               u.comment as name
                        from dbo.sys_user_record r
                        left join dbo.sys_user u on (r.user_name = u.user_name)
                        where r.login_time >= '{0} 00:00:00' and r.login_time <= '{1} 23:59:59'";
            strSql = string.Format(strSql, start, end);

            string strSort = "login_time desc";
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        public ActionResult Find_Account()
        {
            return View();
        }
        public ActionResult Ex_Coin_Total(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public string GetCoinTotalData(string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.Common com = new BLL.Common();
            string strSql = @"select c.uid_from as uid,
              u.photo, u.nick, u.sex,
              sum(case when type='增加' then amount else 0 end) as c_add,
              sum(case when type='减少' then amount else 0 end) as c_sub,
              sum(case when type='增加' then amount else 0 end) - sum(case when type='减少' then amount else 0 end) as c_diff
                from dbo.coin c
                left join dbo.[user] u on (c.uid_from = u.uid)
                group by c.uid_from,u.photo, u.nick, u.sex";


            string strSort = "c_diff asc";
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            //CommonTool.WriteLog.Write(strSql);
            return strRtn;
        }
        public ActionResult Ex_Score_AddFriend(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public ActionResult FengHao_Rst(string id, string uid)
        {
            ViewData["id"] = id;
            ViewData["uid"] = uid;

            return View();
        }
        public ActionResult Warn_Rst(string id, string uid)
        {

            ViewData["id"] = id;
            ViewData["uid"] = uid;

            return View();
        }
        public ActionResult UpDate_Sex(string id)
        {
            ViewData["uid"] = id;

            return View();
        }
        public string Modify_Sex(string uid, string sex)
        {
            string strRtn = string.Empty;
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update [dbo].[user] set sex='{0}'where uid='{1}';";
            strSql = string.Format(strSql, sex, uid);
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
            strRtn = info.ToString();
            return strRtn;
        }
        public ActionResult Bind_Tel(string id)
        {
            ViewData["uid"] = id;

            return View();
        }
        public string Bind_tel_Number(string uid, string tel)
        {
            string strRtn = string.Empty;
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update [dbo].[user] set tel='{0}'where uid='{1}';";
            strSql = string.Format(strSql, tel, uid);
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
            strRtn = info.ToString();
            return strRtn;
        }

        public string Alter_Risk(string uid, string risk_tip)
        {
            string strRtn = string.Empty;
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update [dbo].[user] set risk_tip='{0}' where uid='{1}';";
            strSql = string.Format(strSql, risk_tip, uid);
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
            strRtn = info.ToString();
            return strRtn;
        }
        
        public string Alter_Remark(string uid, string remark)
        {
            string strRtn = string.Empty;
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update [dbo].[user] set remark='{0}' where uid='{1}';";
            strSql = string.Format(strSql, remark, uid);
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
            strRtn = info.ToString();
            return strRtn;
        }
        public ActionResult Action_coin(string id)
        {
            ViewData["uid"] = id;

            return View();
        }
        public string Action_Coin_Data(string uid, string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            string amount = dicParm.ContainsKey("amount") ? dicParm["amount"] : "";
            string type = dicParm.ContainsKey("type") ? dicParm["type"] : "";
            string type_dtl = dicParm.ContainsKey("type_dtl") ? dicParm["type_dtl"] : "";
            string state = dicParm.ContainsKey("state") ? dicParm["state"] : "";
            string dtm = dicParm.ContainsKey("dtm") ? dicParm["dtm"] : "";
            string comment = dicParm.ContainsKey("comment") ? dicParm["comment"] : "";
            if (string.IsNullOrEmpty(dtm))
            {
                dtm = DateTime.Now.ToString();
            }

            string strSql = @"insert into dbo.coin(uid_from,amount,type,type_dtl,dtm,state,comment) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
            strSql = string.Format(strSql, uid, amount, type, type_dtl, dtm, state,comment);
            if (DBHelper.SqlHelper.ExecuteSql(strSql) > 0)
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
        public ActionResult Action_score(string id, string comment, string type)
        {
            ViewData["uid"] = id;
            ViewData["comment"] = string.IsNullOrEmpty(comment) ? "" : comment;
            ViewData["type"] = string.IsNullOrEmpty(type) ? "" : type;

            return View();
        }
        public ActionResult Remark(string id)
        {
            ViewData["uid"] = id;

            string strSql = "select isnull(remark,'') as remark from dbo.[user] where uid = '{0}'";
            strSql = string.Format(strSql, id);
            string r = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["remark"] = r;

            return View();
        }

        public ActionResult Risk_Tip(string id)
        {
            ViewData["uid"] = id;

            return View();
        }
        public ActionResult Cancel(string id)
        {
            ViewData["uid"] = id;

            BLL.Common com = new Common();
            string zhuxiao_pwd = com.GetSysDicByKey("zhuxiao_pwd");
            ViewData["zhuxiao_pwd"] = zhuxiao_pwd;

            return View();
        }
        public string Do_Cancel(string id)
        {
            string uid = id;

            string strRtn = string.Empty;
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"select ISNULL(out_id, '') from dbo.[user]  where uid =  '{0}'";
            strSql = string.Format(strSql,uid);
            string out_id = DBHelper.SqlHelper.GetDataItemString(strSql);
            if (string.IsNullOrEmpty(out_id))
            {
                info.State = "0";
                info.Msg = "失败-无用户out_id，无需注销";
                strRtn = info.ToString();
                return strRtn;
            }

            string day = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "").Substring(4,4);
            strSql = "update dbo.[user]  set out_id ='{1}'  where uid =  '{0}'";
            strSql = string.Format(strSql, uid, out_id+day);

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
            strRtn = info.ToString();
            return strRtn;
        }

        public string Action_Score_Data(string uid, string send)
        {

            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            string amount = dicParm.ContainsKey("amount") ? dicParm["amount"] : "";
            string type = dicParm.ContainsKey("type") ? dicParm["type"] : "";
            string type_dtl = dicParm.ContainsKey("type_dtl") ? dicParm["type_dtl"] : "";
            string state = dicParm.ContainsKey("state") ? dicParm["state"] : "";
            string dtm = dicParm.ContainsKey("dtm") ? dicParm["dtm"] : "";
            string comment = dicParm.ContainsKey("comment") ? dicParm["comment"] : "";
            if (string.IsNullOrEmpty(dtm))
            {
                dtm = DateTime.Now.ToString();
            }

            string strSql = @"insert into dbo.score(uid_from,amount,type,type_dtl,dtm,state,comment) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
            strSql = string.Format(strSql, uid, amount, type, type_dtl, dtm, state, comment);
            if (DBHelper.SqlHelper.ExecuteSql(strSql) > 0)
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
        public string Check_TouSu(string id, string state)
        {
            string strRtn = string.Empty;
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update [dbo].[tousu] set check_state='{0}'where id='{1}';";
            strSql = string.Format(strSql, state, id);
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
            strRtn = info.ToString();
            return strRtn;
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
        public ActionResult Cancel_Check(string id)
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
        public string GetCancelData(string pageIndex, string pageSize, string start, string end, string check_state, string key)
        {
            string strRtn = string.Empty;

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(key))
            {
                where += string.Format(" and (can.wx='{0}' or can.tel='{0}') ", key);
            }
            if (!string.IsNullOrEmpty(check_state))
            {
                where += string.Format(" and (state='{0}') ", check_state);
            }
            if (!string.IsNullOrEmpty(start))
            {
                where += string.Format(" and (can.create_time>='{0} 00:00:00') ", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                where += string.Format(" and (can.create_time<='{0} 23:59:59') ", end);
            }

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);
            BLL.Common com = new BLL.Common();
            string strSql = @"select can.id,
	                               can.uid,
	                               can.wx,
	                               can.tel,
	                               can.reason,
	                               CONVERT(char(20), can.create_time, 120) as create_time,
	                               can.state,
                                   CONVERT(char(20), can.dtm_check, 120) as dtm_check,
                                   can.rst_check,
	                               u.photo,
	                               u.nick,
	                               u.sex
                            from dbo.cancel can
                            left join dbo.[user] u on(can.uid=u.uid) where {0}";
            strSql = string.Format(strSql, where);
            BLL.Common comm = new BLL.Common();
            string strSort = "create_time desc";
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);
            return strRtn;
        }
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
        public ActionResult Check_Survey(string id)
        {
            return View();
        }
        public ActionResult CheckGiftTouSu(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        public string Update_Gift_Check(string id, string rst_check, string rst)
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
        public ActionResult CheckCancel(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        public string Update_Cancel_Check(string id, string rst_check, string rst)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = @"update dbo.cancel
                              set state='{2}',
                               dtm_check=GETDATE(),
                               rst_check='{0}'
                               where id='{1}';";
            strSql = string.Format(strSql, rst_check, id, rst);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                info.State = "1";
                info.Msg = "成功";
                if (rst == "成功")
                {
                    strSql = @"select uid from dbo.cancel where id='{0}'";
                    strSql = string.Format(strSql, id);
                    string uid = DBHelper.SqlHelper.GetDataItemString(strSql);
                    strSql = @"update dbo.[user] set is_cancel = 1 where uid = '{0}'";
                    strSql = string.Format(strSql, uid);
                    DBHelper.SqlHelper.ExecuteSql(strSql);
                }
            }
            else
            {
                info.State = "0";
                info.Msg = "失败";
            }
            return info.ToString();
        }
        public ActionResult CheckFeedBack(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        public ActionResult CheckComplain(string id)
        {
            ViewData["id"] = id;
            return View();
        }
        public string Update_Check_TouSu(string id, string rst_check, string rst)
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

        public string Update_Check_Complain2(string id, string rst_check, string rst)
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
        public string Get_Update_UserInfo()
        {
            string strRtn = string.Empty;
            string strSql = @"select top 10 up.id,
	                               up.uid,
	                               up.field_name,
	                               up.val_old,
	                               up.val_new,
	                               up.check_state,
                                   convert(char(20), up.dtm_apply, 120) as dtm_apply,
                                   convert(char(20), up.dtm_check, 120) as dtm_check,
								   u.photo,
								   u.nick,
								   u.sex
                             from dbo.user_update up
							 left join dbo.[user] u on(up.uid=u.uid)
							 order by dtm_apply desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);

            return strRtn;
        }

        
        public string GetFeedBackData()
        {
            string strRtn = string.Empty;
            string strSql = @"select top 10 ts.id,
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
                                left join dbo.[user] u on(ts.uid=u.uid)
								order by ts.create_time desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);

            return strRtn;
        }
        public string GetComplainData2()
        {
            string strRtn = string.Empty;
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

									  where c.dtm >= DATEAdd(HH,-2,GETDATE()) and c.dtm <= GETDATE()
									  order by c.dtm desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);

            return strRtn;
        }
        public string GetListReward(string dtm)
        {

            string strRtn = string.Empty;

            DataTable dt = bll.GetScore_Reword(Convert.ToDateTime(dtm));

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData2(rows.ToString(), data);

            return strRtn;
        }
        public string GetScoreTxData()
        {
            string strRtn = string.Empty;
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
                                   u.tel as rz_tel, u.wx as rz_wx, u.photo, u.nick, u.sex, isnull(u.remark , '') as remark
                            from [dbo].[score_tx]  a
                            left join dbo.[user] u on(a.uid = u.uid)
                            left join dbo.user_s us on (a.uid = us.uid)
							where a.apply_date >= DATEAdd(DD,-3,GETDATE()) and a.apply_date <= GETDATE()
							order by a.apply_date desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);

            return strRtn;
        }
        public string GetGiftTouSuData()
        {
            string strRtn = string.Empty;
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
							  where g.dtm >= DATEAdd(DD,-3,GETDATE()) and g.dtm <= GETDATE()
							  order by g.dtm desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);

            return strRtn;
        }
        public string Update_UserInfo(string id, string state)
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

        #region 用户记录
        public ActionResult User_Record_Add(string uid, string id)
        {
            ViewData["uid"] = uid;
            ViewData["id"] = id;
            return View();
        }

        public ActionResult User_Record_All()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            // 设置默认时间：30天前和当前时间
            DateTime today = DateTime.Now;
            DateTime thirtyDaysAgo = today.AddDays(-3);
            ViewData["startDate"] = thirtyDaysAgo.ToString("yyyy-MM-dd");
            ViewData["endDate"] = today.ToString("yyyy-MM-dd");

            return View();
        }

        public ActionResult GetUserRecords(string pageIndex, string pageSize, string uid, string startDate, string endDate, string source)
        {
            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.UserRecord userRecord = new BLL.UserRecord();
            DataTable dt = userRecord.GetUserRecords(uid, startDate, endDate, source, index, size);
            int total = userRecord.GetUserRecordsCount(uid, startDate, endDate, source);

            BLL.Common comm = new BLL.Common();
            string result = comm.GetMiniUIData2(total.ToString(), CommonTool.JsonHelper.DataTableToJSON(dt));
            
            // 设置响应编码为UTF-8
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            return Content(result, "application/json; charset=utf-8");
        }

        public ActionResult AddUserRecord(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            try
            {
                Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);

                // 日志记录
                CommonTool.WriteLog.Write("AddUserRecord 参数: " + send);

                // 获取当前登录的系统用户信息
                BLL.Data_Sys_User sysUser = new BLL.Data_Sys_User();
                if (Session["Data_Sys_User"] != null)
                {
                    sysUser = (BLL.Data_Sys_User)Session["Data_Sys_User"];
                }

                // 检查必要参数
                if (!dicParm.ContainsKey("uid"))
                {
                    info.State = "0";
                    info.Msg = "参数错误：用户ID不能为空";
                    return Content(info.ToString(), "application/json; charset=utf-8");
                }

                if (!dicParm.ContainsKey("record_date"))
                {
                    info.State = "0";
                    info.Msg = "参数错误：记录日期不能为空";
                    return Content(info.ToString(), "application/json; charset=utf-8");
                }

                if (!dicParm.ContainsKey("summary"))
                {
                    info.State = "0";
                    info.Msg = "参数错误：简述不能为空";
                    return Content(info.ToString(), "application/json; charset=utf-8");
                }

                dicParm["operator_name"] = sysUser.User_Name;

                // 日志记录
                CommonTool.WriteLog.Write("AddUserRecord 处理后参数: uid=" + dicParm["uid"] + ", record_date=" + dicParm["record_date"] + ", summary=" + dicParm["summary"]);

                BLL.UserRecord userRecord = new BLL.UserRecord();
                string id = userRecord.CreateUserRecord(dicParm);
                if (!string.IsNullOrEmpty(id))
                {
                    info.State = "1";
                    info.Msg = "成功";
                }
                else
                {
                    info.State = "0";
                    info.Msg = "数据库操作失败";
                }
            }
            catch (Exception ex)
            {
                info.State = "0";
                info.Msg = "保存失败：" + ex.Message;
                CommonTool.WriteLog.Write("AddUserRecord 异常: " + ex.Message + "\n" + ex.StackTrace);
            }
            return Content(info.ToString(), "application/json; charset=utf-8");
        }

        public ActionResult UpdateUserRecord(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            string id = dicParm.ContainsKey("id") ? dicParm["id"] : "";

            if (string.IsNullOrEmpty(id))
            {
                info.State = "0";
                info.Msg = "参数错误：记录ID不能为空";
                return Content(info.ToString(), "application/json; charset=utf-8");
            }

            dicParm.Remove("id");

            BLL.UserRecord userRecord = new BLL.UserRecord();
            bool tag = userRecord.UpdateUserRecord(id, dicParm);
            if (tag)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }
            return Content(info.ToString(), "application/json; charset=utf-8");
        }

        public ActionResult DeleteUserRecord(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (string.IsNullOrEmpty(id))
            {
                info.State = "0";
                info.Msg = "参数错误：记录ID不能为空";
                return Content(info.ToString(), "application/json; charset=utf-8");
            }

            BLL.UserRecord userRecord = new BLL.UserRecord();
            bool tag = userRecord.DeleteUserRecord(id);
            if (tag)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }
            return Content(info.ToString(), "application/json; charset=utf-8");
        }

        public ActionResult GetUserRecordById(string id)
        {
            string strRtn = string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                return Content(strRtn, "application/json; charset=utf-8");
            }

            BLL.UserRecord userRecord = new BLL.UserRecord();
            DataTable dt = userRecord.GetUserRecordById(id);
            strRtn = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            return Content(strRtn, "application/json; charset=utf-8");
        }

        public ActionResult GetUserRecordsAll(string pageIndex, string pageSize, string uid, string startDate, string endDate)
        {
            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.UserRecord userRecord = new BLL.UserRecord();
            DataTable dt = userRecord.GetUserRecordsAll(uid, startDate, endDate, index, size);
            int total = userRecord.GetUserRecordsAllCount(uid, startDate, endDate);

            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            BLL.Common comm = new BLL.Common();
            string result = comm.GetMiniUIData2(total.ToString(), data);

            // 设置响应编码为UTF-8
            Response.ContentType = "application/json; charset=utf-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            return Content(result, "application/json; charset=utf-8");
        }
        #endregion

        #region 陪玩
        //页面
        public ActionResult Play_List(string id)
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

        public ActionResult Play_Order(string id)
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
        //获取数据
        public string GetPlay_ListData(string pageIndex, string pageSize, string start, string end, string c_type, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strSql = @"select id,

                                   uid,
	                               photo,
	                               nick,
	                               sex,
	                               age_range,
								   wx,
								   tel,
	                               loc_province,
	                               loc_city,
	                               loc_area,
								   ISNULL(exper, '') as exper,
								   ISNULL(sign_self, '') as sign_self,
								   ISNULL(play_level, '') as play_level,
	                               ISNULL(vol, '') as vol,
								   ISNULL(pics, '') as pics,
								   ISNULL(video, '') as video,
								   ISNULL(tags, '') as tags,
								   ISNULL(service_type, '') as service_type,
								   ISNULL(mny_config, '') as mny_config,
								   ISNULL(state_check, '待审核') as state_check,
								   CONVERT(char(20), dtm_check, 120) as dtm_check,
								   ISNULL(state_lnk, '') as state_lnk,
								   CONVERT(char(20), dtm_lnk, 120) as dtm_lnk,
								   CONVERT(char(20), create_time, 120) as create_time
                            from[dbo].[user_play]";
            
            BLL.Common comm = new BLL.Common();

            string strSort = "create_time desc";

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        public string GetPlay_OrderData(string pageIndex, string pageSize, string start, string end, string c_type, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strSql = @"select o.id,
	                           o.uid_player,
	                           o.uid_to,
	                           ISNULL(o.wx, '') as wx,
	                           o.play_type,
	                           o.play_time_ch,
	                           ISNULL(o.play_time, '1') as play_time,
	                           CONVERT(char(20), o.dtm_start, 120) as dtm_start,
	                           CONVERT(char(20), o.dtm_end, 120) as dtm_end,
	                           o.order_mny,
	                           CONVERT(char(20), o.dtm_order, 120) as dtm_order,
	                           o.pay_mny,
	                           CONVERT(char(20), o.dtm_pay, 120) as dtm_pay,
	                           ISNULL(o.order_state, '待服务') as order_state,
	                           CONVERT(char(20), o.create_time, 120) as create_time,

	                           u.photo,
	                           u.nick,
	                           u.sex,

	                           u2.photo as photo2,
	                           u2.nick as nick2,
	                           u2.sex as sex2

                        from dbo.[order_play]  o 
                        left join dbo.[user] u on (o.uid_to =u.uid)
                        left join dbo.[user_play] u2 on (o.uid_player =u2.uid)";

            BLL.Common comm = new BLL.Common();

            string strSort = "create_time desc";

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        //审核是否显示
        public string Check_Play_List(string id, string state)
        {

            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = @"update dbo.user_play 
                                    set state_check='{0}',dtm_check=GETDATE()
                                    where id='{1}'";
            strSql = string.Format(strSql, state, id);
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

        //查看用户上传图片
        public ActionResult LookPics(string id)
        {
            string data = "";
            string strSql = @"select 
	                                 pics
                              from   dbo.user_play
                              where  id = '{0}' ";
            strSql = string.Format(strSql, id);

            data = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["data"] = data;

            return View();
        }

        public ActionResult LookComment(string id)
        {
            string data = "";
            string strSql = @"select 
	                                 com_dtl
                              from   dbo.play_comment
                              where  play_order_id = '{0}' ";
            strSql = string.Format(strSql, id);

            data = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["data"] = data;

            return View();
        }
        //价格表配置
        public ActionResult Mny_Config(string id)
        {
            string data = "";
            string strSql = @"select ISNULL(mny_config, '') as mny_config
                                from[dbo].[user_play]
                                where id='{0}'";
            strSql = string.Format(strSql, id);

            data = DBHelper.SqlHelper.GetDataItemString(strSql);
            ViewData["id"] = id;
            ViewData["data"] = data;

            return View();
        }
        public ActionResult LookCommentAll(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            ViewData["uid"] = id;
            return View();
        }
        public string GetCommentData(string id,string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);
            string strSql = @"select u.photo,
                                       u.nick as nick1,
	                                   u2.nick as nick2,
                                       c.com_dtl,
                                       c.com_rst,
                                       c.create_time,
	                                   o.play_type,
	                                   o.play_time_ch
                                from   dbo.[play_comment] c
                                left join dbo.[order_play] o on (c.play_order_id=o.id)
                                left join dbo.[user] u on (o.uid_to =u.uid)
                                left join dbo.[user_play] u2 on (o.uid_player =u2.uid)
                                where o.uid_player = '{0}'";
            strSql = string.Format(strSql, id);
            BLL.Common comm = new BLL.Common();
            string strSort = "create_time desc";

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string Update_User_Play(string play_id, string data)
        {

            CommonTool.WriteLog.Write(data);
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(data);

            BLL.BLL bll = new BLL.BLL();
            bool b = bll.Play_Update(play_id, dicParm);

            if (b == true)
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
        #endregion

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
            CommonTool.WriteLog.Write("id----"+id);
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
                if (c_type== "未审核")
                {
                    where += "and s.comment is null ";
                }else
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
        public string Check_Seek_people(string id, string state)
        {

            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = @"update dbo.seek_people 
                                    set state='{0}',comment='管理员修改',dtm_check=GETDATE()
                                    where id='{1}'";
            strSql = string.Format(strSql, state, id);
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


        public string GetGiftOne(string id)
        {
            string data = "";

            string strSql = @"select      c.id,
                                          CONVERT(char(20), c.dtm, 120) as dtm,
                                          c.comment as gift_name,
                                          c.amount,
                                          u.photo,
                                          u.nick,
                                          u.sex
                            from dbo.coin c
                            left join dbo.[user] u on (c.uid_to = u.uid)
                            where c.id = '{0}' ";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);

            return data;
        }
        public string GetGiftOneData(string id)
        {
            string data = "";
            string strSql = @"select id,
	                                 uid,
	                                 gift_id,
                                     convert(char(20), dtm, 120) as dtm,
	                                 brife,
	                                 pics,
									 state,
									 convert(char(20), dtm_check, 120) as dtm_check,
									 rst_check
                              from dbo.tousu_gift
                              where gift_id = '{0}' ";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);

            return data;
        }
        public ActionResult Big_Photo(string pic)
        {
            ViewData["pic"] = pic;
            return View();
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="A">发起者</param>
        /// <param name="B">被投诉者</param>
        /// <returns></returns>
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

        public string FengHao(string uid, string hour, string reason)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (string.IsNullOrEmpty(uid))
            {
                info.State = "0";
                info.Msg = "参数错误";
                return info.ToString();
            }

            if (string.IsNullOrEmpty(hour))
            {
                hour = "3";
            }
            if (string.IsNullOrEmpty(reason))
            {
                reason = "你的聊天内容违规";
            }

            BLL.BLL bll = new BLL.BLL();

            if (bll.Kill_User(uid, hour, reason))
            {
                info.State = "1";
                info.Msg = "封杀成功";
                //刷新数据接口
                string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
                string url = servUrl_AppData + "/App/Reload_KillUid";
                string tag2 = CommonTool.Common.GetHtmlFromUrl(url);
            }
            else
            {
                info.State = "0";
                info.Msg = "封杀失败";
            }

            return info.ToString();
        }
        //警告
        public string Warn(string uid, string sec, string reason)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (string.IsNullOrEmpty(uid))
            {
                info.State = "0";
                info.Msg = "参数错误";
                return info.ToString();
            }

            if (string.IsNullOrEmpty(sec))
            {
                sec = "15";
            }
            if (string.IsNullOrEmpty(reason))
            {
                reason = "你的聊天内容违规,遭到多次投诉举报";
            }

            BLL.BLL bll = new BLL.BLL();

            if (bll.Warn_User(uid, sec, reason))
            {
                info.State = "1";
                info.Msg = "警告成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "失败";
            }

            return info.ToString();
        }
        public string GetExScoreAddFriendData(string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.Common com = new BLL.Common();
            string strSql = @"
select s.uid_from, 
              u1.photo as photo_from,
              u1.sex as sex_from,
              u1.nick as nick_from,
              ISNULL(us.name, '') as name,
              
              s.uid_to, 
              u2.photo as photo_to,
              u2.sex as sex_to,
              u2.nick as nick_to,
              CONVERT(char(10), MAX(s.dtm), 120)  as d1, CONVERT(char(10), MIN(s.dtm), 120) as d2, COUNT(*) as c 
from dbo.score s
left join dbo.[user] u1 on (s.uid_from = u1.uid)
left join dbo.[user] u2 on (s.uid_to = u2.uid)
left join dbo.user_s us on (s.uid_from = us.uid)
where s.type_dtl = '同意添加好友'
group by s.uid_from , s.uid_to,
              u1.photo ,
              u1.sex ,
              u1.nick ,
              ISNULL(us.name, ''),
              
              u2.photo ,
              u2.sex ,
              u2.nick 
having COUNT(*) > 1 ";

            string strSort = "c desc";
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            //CommonTool.WriteLog.Write(strSql);
            return strRtn;
        }
        public string FindAccount(string new_uid, string new_openid, string old_uid, string old_openid)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = @"insert into dbo.user_remove (uid_new, out_id_new, uid_old, out_id_old)
values('{0}', '{1}', '{2}', '{3}')
update dbo.[user] set out_id = '{1}' where uid = '{2}';
update dbo.[user] set out_id = '' where uid = '{0}';";
            strSql = string.Format(strSql, new_uid, new_openid, old_uid, old_openid);
            if (DBHelper.SqlHelper.ExecuteSql(strSql) > 0)
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

        public string DeletePhotos(string ids)
        {
            string strRtn = string.Empty;
            strRtn = ids;
            //返还前打印
            CommonTool.WriteLog.Write(strRtn);

            //进行删除操作


            return strRtn;
        }

        #region 聊天消息
        public ActionResult Msg_Sum_Hour()
        {
            string dtm = Request.QueryString["dtm"];
            string dtm_compare = Request.QueryString["dtm_compare"];

            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            dtm_compare = string.IsNullOrEmpty(dtm_compare) ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") : dtm_compare;

            DataTable dt = bll.Msg_Sum_Hour(Convert.ToDateTime(dtm));
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            DataTable dt_com = bll.Msg_Sum_Hour(Convert.ToDateTime(dtm_compare));
            string data_com = CommonTool.JsonHelper.DataTableToJSON(dt_com);

            ViewData["data"] = data;
            ViewData["dtm"] = dtm;

            ViewData["data_com"] = data_com;
            ViewData["dtm_compare"] = dtm_compare;

            return View();
        }

        public ActionResult Msg_Sum_Min()
        {
            string dtm = Request.QueryString["dtm"];

            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;

            DataTable dt = bll.Msg_Sum_Mi(Convert.ToDateTime(dtm));
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            ViewData["data"] = data;
            ViewData["dtm"] = dtm;

            return View();
        }

        public ActionResult Msg_Dtl()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            ViewData["dtm"] = dtm;
            return View();
        }


        public ActionResult Msg_SumInner()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            return View();
        }

        public ActionResult Msg_SumInner_Black()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            return View();
        }

        #endregion 

        #region 统计

        public ActionResult Sum_NewUser_Hour()
        {
            string dtm = Request.QueryString["dtm"];
            string dtm_compare = Request.QueryString["dtm_compare"];

            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            dtm_compare = string.IsNullOrEmpty(dtm_compare) ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") : dtm_compare;

            DataTable dt = bll.Sum_NewUser_Hour(Convert.ToDateTime(dtm));
            DataTable dt_com = bll.Sum_NewUser_Hour(Convert.ToDateTime(dtm_compare));
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            string data_com = CommonTool.JsonHelper.DataTableToJSON(dt_com);

            ViewData["data"] = data;
            ViewData["data_com"] = data_com;

            ViewData["dtm"] = dtm;
            ViewData["dtm_compare"] = dtm_compare;


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

        public ActionResult Sum_NewUser_Day()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + " 00:00:00" : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59" : end;

            CommonTool.WriteLog.Write(start+ end);

            ViewData["start"] = start;
            ViewData["end"] = end;

            DataTable dt = bll.Sum_NewUser_Day(Convert.ToDateTime(start), Convert.ToDateTime(end));
            DataTable dt_kill = bll.Sum_KillUser_Day(Convert.ToDateTime(start), Convert.ToDateTime(end));

            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            string data_kill = CommonTool.JsonHelper.DataTableToJSON(dt_kill);

            ViewData["data"] = data;
            ViewData["data_kill"] = data_kill;

            return View();
        }

        public ActionResult Sum_Inner_Gift()
        {

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            return View();
        }

        public string GetGiftSum_Inner(string pageIndex, string pageSize, string dtm, string name)
        {

            //CommonTool.WriteLog.Write("dtm:"+dtm+  "; uids:"+uids);
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.Common com = new Common();
            string fest_name = com.GetSysDicByKey("fest_name");

            string strSql = @"select u.uid,
                                       u.photo,
                                       u.nick,
                                       u.sex,

                                       sum(case when s.type_dtl = '同意添加好友' then 1 else 0 end ) as count_好友,
                                       sum(case when s.type_dtl = '领取徒弟收益' then amount else 0 end ) as count_收徒,

                                       sum(case when s.comment = '棒棒糖' or s.comment = '{2}' then 1 else 0 end ) as count_棒棒糖,
                                       sum(case when s.comment = '鲜花' then 1 else 0 end ) as count_鲜花,
                                       sum(case when s.comment = '黄瓜' then 1 else 0 end ) as count_黄瓜,
                                       sum(case when s.comment = '啪啪啪' then 1 else 0 end ) as count_啪啪啪,
                                       sum(case when s.comment = '幸运星' then 1 else 0 end ) as count_幸运星,

                                       sum(case when s.comment = '巧克力' then 1 else 0 end ) as count_巧克力,
                                       sum(case when s.comment = '气球' then 1 else 0 end ) as count_气球,
                                       sum(case when s.comment = '口红' then 1 else 0 end ) as count_口红,
       
                                       sum(case when s.comment = '香水' then 1 else 0 end ) as count_香水,
                                       sum(case when s.comment = '包包' then 1 else 0 end ) as count_包包,
                                       sum(case when s.comment = '钻戒' then 1 else 0 end ) as count_钻戒,
                                       sum(case when s.comment = '水晶鞋' then 1 else 0 end ) as count_水晶鞋,
                                       sum(case when s.comment = '皇冠' then 1 else 0 end ) as count_皇冠,
                                       sum(case when s.comment = '法拉利' then 1 else 0 end ) as count_法拉利,
                                       sum(case when s.comment = '飞机' then 1 else 0 end ) as count_飞机,
                                       sum(case when s.comment = '火箭' then 1 else 0 end ) as count_火箭
                                from dbo.[user] u 
                                left join dbo.score s on (u.uid = s.uid_from and type='增加' and (type_dtl='收到礼物' or type_dtl='同意添加好友' or type_dtl='领取徒弟收益') and (s.dtm >= '{0} 00:00:00' and s.dtm <= '{0} 23:59:59') and s.uid_to not in ('{3}') )
                                where u.uid in ({1})
                                group by u.uid,
                                       u.photo,
                                       u.nick,
                                       u.sex ";

            string uids = bll.GetInnerManUIds(name);
            string uids_not = bll.GetInnerBoyUids();
            strSql = string.Format(strSql, dtm, uids, fest_name, uids_not.Replace(",", "','"));

            string strSort = "count_棒棒糖 desc";
            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }


        public string GetChartScore(string dtm, string user_name)
        {
            Dictionary<string, int> dic = bll.Get_InnerScore(dtm, user_name);
            string data = CommonTool.JsonHelper.ObjectToJSON(dic);
            return data;
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
        public ActionResult Score_Tx_Rst(string id, string pay_mny)
        {
            ViewData["id"] = id;
            ViewData["pay_mny"] = pay_mny;
            return View();
        }
        #endregion

        #region 全貌

        public ActionResult User_Complain(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;

            return View();
        }

        public ActionResult User_GiftTouSu(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;

            return View();
        }
        public ActionResult Warn_Record(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;

            return View();
        }

        public ActionResult UserAll(string id, string user_type)
        {
            ViewData["id"] = id;
            ViewData["user_type"] = user_type;
            return View();
        }

        public ActionResult UserAllExternal(string uid, string user_type)
        {
            ViewData["id"] = uid;
            ViewData["user_type"] = user_type ?? "普通用户";
            
            // 查询用户昵称和备注
            string strSql = "select nick, isnull(remark,'') as remark from dbo.[user] where uid = '{0}'";
            strSql = string.Format(strSql, uid);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            
            if (dt.Rows.Count > 0)
            {
                ViewData["nick"] = dt.Rows[0]["nick"].ToString();
                ViewData["remark"] = dt.Rows[0]["remark"].ToString();
            }
            else
            {
                ViewData["nick"] = "";
                ViewData["remark"] = "";
            }
            
            return View("UserAllExternal");
        }

        public ActionResult Sum_Day()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            return View();
        }

        public string GetSumDayData(string pageIndex, string pageSize, string start, string end)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            if (string.IsNullOrEmpty(start))
            {
                start = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(end))
            {
                end = DateTime.Now.ToString("yyyy-MM-dd");
            }

            string strSql = @"with a as 
                                (
                                 select CONVERT(char(10),u.create_time, 120) as dtm,
                                        COUNT(1) as new_count,
                                        sum(case when fz_tag = '0' then 1 else 0 end) as new_count_self
      
                                 from dbo.[user] u
                                 where create_time >= '{0} 00:00:00' and create_time <= '{1} 23:59:59'
                                 group by CONVERT(char(10),u.create_time, 120)
                                ),
                                b as 
                                (
                                select CONVERT(char(10),u.dtm, 120) as dtm,
                                       COUNT(1) as act_count
      
                                 from dbo.sum_user u
                                 where dtm >= '{0} 00:00:00' and dtm <= '{1} 23:59:59'
                                  group by CONVERT(char(10),u.dtm, 120)
                                ),
                                c as 
                                (
                                select  CONVERT(char(10),o.create_time, 120) as dtm,
                                        COUNT(distinct o.uid) as men_count,
                                        COUNT(*) as order_count,
                                        sum(pay_mny) as mny,
                                        sum(case when o.order_type='购买金币' then o.pay_mny else 0 end) as mny_coin,
                                        sum(case when o.order_type='购买会员' then o.pay_mny else 0 end) as mny_mem,
                                        COUNT(distinct case when CONVERT(char(10),o.create_time, 120) = CONVERT(char(10),u.create_time, 120) then o.uid else '' end)-1 as newmen_count,
                                        COUNT(distinct case when CONVERT(char(10),o.create_time, 120) <> CONVERT(char(10),u.create_time, 120) then o.uid else '' end)-1 as oldmen_count,
                                        sum(case when CONVERT(char(10),o.create_time, 120) = CONVERT(char(10),u.create_time, 120) then pay_mny else 0 end) as new_mny,
                                        sum(case when CONVERT(char(10),o.create_time, 120) <> CONVERT(char(10),u.create_time, 120) then pay_mny else 0 end) as old_mny
                                from dbo.sys_order o 
                                left join dbo.[user] u on (o.uid = u.uid)
                                where o.create_time >= '{0} 00:00:00' and o.create_time <= '{1} 23:59:59' and order_state = '付款成功' and o.comment2=''
                                group by CONVERT(char(10),o.create_time, 120)
                                )

                                select a.dtm, a.new_count, a.new_count_self,
                                       b.act_count,
                                       c.men_count,c.order_count,
                                       c.mny, c.mny_coin,c.mny_mem,
                                       c.newmen_count,
                                       case when c.new_mny =0 then  c.oldmen_count + 1 else c.oldmen_count end as  oldmen_count,
                                       c.new_mny,
                                       c.old_mny,
                                       c.newmen_count * 1.0 / c.men_count * 100 as r1,
                                       (case when c.new_mny =0 then  c.oldmen_count + 1 else c.oldmen_count end) * 1.0 / c.men_count * 100 as r2
                                from a 
                                left join b on (a.dtm = b.dtm)
                                left join c on (a.dtm = c.dtm)
                                 order by a.dtm desc";

            strSql = string.Format(strSql, start, end);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            BLL.Common comm = new BLL.Common();

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
          
            strRtn = comm.GetMiniUIData2(rows.ToString(), data);

           

            return strRtn;
        }


        public ActionResult User_Sum(string id)
        {
            ViewData["id"] = id;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            BLL.BLL bll = new BLL.BLL();

            double tx_total = bll.GetUserTotalTiXian(id);
            double tx_ing = bll.GetUser_TiXianing(id);
            double tx_can = 0.00;

            int score_total = bll.GetUserScore_Total(id);
            int score_canuse = bll.GetUserScore_Left(id);
            int score_used = bll.GetUserScore_Used(id);

            int coin_total = bll.GetUserCoin_Total(id);
            int coin_canuse = bll.GetUserCoin_Left(id);
            int coin_used = bll.GetUserCoin_Used(id);

            int p_score = bll.GetUserP_Score(id);

            tx_can = score_canuse * 1.00 / 1000;

            // 计算注册天数
            string strSql = "select create_time from dbo.[user] where uid = '{0}'";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            int register_days = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime create_time = Convert.ToDateTime(dt.Rows[0]["create_time"]);
                register_days = (DateTime.Now - create_time).Days + 1; // +1 包含注册当天
            }

            ViewData["tx_total"] = String.Format("{0:F}", tx_total);
            ViewData["tx_ing"] = String.Format("{0:F}", tx_ing);
            ViewData["tx_can"] = String.Format("{0:F}", tx_can);

            ViewData["score_total"] = score_total;
            ViewData["score_canuse"] = score_canuse;
            ViewData["score_used"] = score_used;

            ViewData["coin_total"] = coin_total;
            ViewData["coin_canuse"] = coin_canuse;
            ViewData["coin_used"] = coin_used;

            ViewData["p_score"] = p_score;
            ViewData["register_days"] = register_days;

            return View();
        }
        public ActionResult User_Coin(string id)
        {
            ViewData["id"] = id;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            int coin_total = bll.GetUserCoin_Total(id);
            int coin_canuse = bll.GetUserCoin_Left(id);
            int coin_used = bll.GetUserCoin_Used(id);

            ViewData["coin_total"] = coin_total;
            ViewData["coin_canuse"] = coin_canuse;
            ViewData["coin_used"] = coin_used;

            System.Data.DataTable coinSourceStats = bll.GetCoinSourceStats(id);
            ViewData["coin_source_stats"] = coinSourceStats;

            System.Data.DataTable coinConsumeStats = bll.GetCoinConsumeStats(id);
            ViewData["coin_consume_stats"] = coinConsumeStats;

            return View();
        }
        public ActionResult User_Score(string id)
        {
            ViewData["id"] = id;

            int score_total = bll.GetUserScore_Total(id);
            int score_canuse = bll.GetUserScore_Left(id);
            int score_used = bll.GetUserScore_Used(id);

            ViewData["score_total"] = score_total;
            ViewData["score_canuse"] = score_canuse;
            ViewData["score_used"] = score_used;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            //统计数据
            DataTable dt = bll.GetUserScore_Sum(id);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;

            return View();
        }
        public ActionResult User_Score_Pie(string id, string dtm)
        {
            ViewData["id"] = id;
     
            //统计数据
            DataTable dt = bll.GetUserScore_Sum(id);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;

            //统计最近一次数据
            string data_least = "";
            string strSql = @"select top 1 CONVERT(char(20),apply_date, 120) as apply_date, score_left 
                              from dbo.score_tx 
                              where uid = '{0}' and state <> '待审核' 
                              order by apply_date desc";
            strSql = string.Format(strSql, id);
            string dtm_start = "";
            string score_left = "0";
            dt = DBHelper.SqlHelper.GetDataTable(strSql);
            if (dt.Rows.Count > 0)
            {
                dtm_start = dt.Rows[0]["apply_date"].ToString();
                score_left = dt.Rows[0]["score_left"].ToString();
            }
            if (!string.IsNullOrEmpty(dtm))
            {
                dtm_start = dtm;
            }
            if (!string.IsNullOrEmpty(dtm_start))
            {
                dt = bll.GetUserScore_Sum_Dtm(id, dtm_start);
                data_least = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            }
            ViewData["data_least"] = data_least;

            ViewData["dtm_start"] = dtm_start;
            ViewData["score_left"] = score_left;

            return View();
        }

        public ActionResult User_ChatScore(string id)
        {
            ViewData["id"] = id;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public ActionResult User_Score_Trend(string id)
        {
            ViewData["id"] = id;

            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-15).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            DataTable dt = bll.GetSum_UserScore(Convert.ToDateTime(start), Convert.ToDateTime(end), id);


            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            ViewData["data"] = data;

            return View();
        }
        public ActionResult User_Chart(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = string.Empty;
            dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;
            ViewData["uid"] = id;
            return View();
        }

        public ActionResult User_Record(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            Common com = new Common();
            string xhc_admin_uid = com.GetSysDicByKey("xhc_admin_uid");

            string dtm = string.Empty;
            dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;
            ViewData["uid"] = id;
            ViewData["xhc_admin_uid"] = xhc_admin_uid;
            return View();
        }

        public ActionResult User_Chart_AB(string dtm, string uid_from, string uid_to)
        {
            DateTime dtm_Data = Convert.ToDateTime(dtm);
            string start = dtm_Data.ToString("yyyy-MM-dd") + " 00:00:00";
            string end = dtm_Data.ToString("yyyy-MM-dd") + " 23:59:59";
            string data = GetChartA_B(start, end, uid_from, uid_to);
            ViewData["data"] = data;

            ViewData["dtm"] = dtm;
            ViewData["dtm_pre"] = dtm_Data.AddDays(-1).ToString("yyyy-MM-dd");

            ViewData["to"] = uid_to;
            ViewData["from"] = uid_from;

            ViewData["start"] = start;
            ViewData["end"] = end;


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

            //获取消息显示控制
            BLL.Common com = new Common();
            string msg_dtl = com.GetSysDicByKey("msg_dtl");
            ViewData["msg_dtl"] = msg_dtl;

            return View();
        }

        public ActionResult User_Msg(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;
            string dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;
            return View();
        }
        public ActionResult User_Order(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string ignore = Request.QueryString["ignore"];
            ignore = string.IsNullOrEmpty(ignore) ? "1" : ignore;

            ViewData["ignore"] = ignore;
            ViewData["id"] = id;
            return View();
        }

        public ActionResult User_Base(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;

            string strSql = @"select * from dbo.[user] where uid = '{0}' ";

            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;

            string data2 = GetWxInfoData(id);
            ViewData["data2"] = data2;

            return View();
        }

        public ActionResult User_Kill(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;

            return View();
        }
        public ActionResult TouSu_Record(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;

            return View();
        }
        public ActionResult Gift_TouSu_Record(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;

            return View();
        }

        public ActionResult User_Login(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;
            string dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;
            return View();
        }

        public ActionResult User_Match(string id)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["id"] = id;
            string dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;
            return View();
        }

        public ActionResult User_Live(string id, string dtm_start, string dtm_end)
        {
            ViewData["id"] = id;

            if (string.IsNullOrEmpty(dtm_start))
            {
                //默认开始时间为用户注册时间
                //dtm_start = DBHelper.SqlHelper.GetDataItemString(string.Format("select CONVERT(char(10), create_time, 120) from dbo.[user] where uid = '{0}'", id));

                //2024.4.3 调整为最近3个月
                dtm_start = DBHelper.SqlHelper.GetDataItemString(string.Format("select CONVERT(char(10), create_time, 120) from dbo.[user] where uid = '{0}'", id));
                if (Convert.ToDateTime(dtm_start) < DateTime.Now.AddDays(-90))
                {
                    dtm_start = DateTime.Now.AddDays(-90).ToString();
                }

            }
            if (string.IsNullOrEmpty(dtm_end))
            {
                dtm_end = DateTime.Now.ToString("yyyy-MM-dd");
            }

            DataTable dt = bll.Sum_UserDtl(id, Convert.ToDateTime(dtm_start), Convert.ToDateTime(dtm_end));
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            ViewData["data"] = data;

            ViewData["dtm_start"] = dtm_start;
            ViewData["dtm_end"] = dtm_end;



            return View();
        }

        public ActionResult User_ExportData_UserChat(string id, string dtm_start, string dtm_end)
        {
            ViewData["id"] = id;

            if (string.IsNullOrEmpty(dtm_start))
            {
                dtm_start = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(dtm_end))
            {
                dtm_end = DateTime.Now.ToString("yyyy-MM-dd");
            }

            ViewData["dtm_start"] = dtm_start;
            ViewData["dtm_end"] = dtm_end;

            return View();
        }

        public string GetUserChatExportData(string pageIndex, string pageSize, string uid, string dtm_start, string dtm_end)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            // 先从 sum_user 表中查询用户有数据的天数
            string sumUserSql = @"select CONVERT(char(10), dtm, 120) as dtm 
                                from dbo.sum_user 
                                where uid = '{0}' and dtm >= '{1} 00:00:00' and dtm <= '{2} 23:59:59' 
                                group by CONVERT(char(10), dtm, 120)";
            sumUserSql = string.Format(sumUserSql, uid, dtm_start, dtm_end);
            DataTable dtSumUser = DBHelper.SqlHelper.GetDataTable(sumUserSql);

            // 构建所有日期的表名和SQL语句
            StringBuilder sqlBuilder = new StringBuilder();
            for (int i = 0; i < dtSumUser.Rows.Count; i++)
            {
                string dateStr = dtSumUser.Rows[i]["dtm"].ToString();
                DateTime currentDate = Convert.ToDateTime(dateStr);
                string tableName = bll.GetTableName_msg(currentDate);
                
                if (sqlBuilder.Length > 0)
                {
                    sqlBuilder.Append(" UNION ALL ");
                }
                
                sqlBuilder.AppendFormat(@"select 
                                            msg.dtm,
                                            msg.uid_from,
                                            msg.uid_to,
                                            u.nick as name_to,
                                            msg.state,
                                            msg.type,
                                            msg.txt
                                        from {0} msg
                                        left join dbo.[user] u on (msg.uid_to = u.uid)
                                        where (msg.uid_from = '{1}' or msg.uid_to = '{1}')
                                        and msg.dtm >= '{2} 00:00:00' 
                                        and msg.dtm <= '{3} 23:59:59'", 
                                        tableName, uid, dateStr, dateStr);
            }

            string strSql = sqlBuilder.ToString();
            if (string.IsNullOrEmpty(strSql))
            {
                // 如果没有数据，返回空结果
                return "{\"total\": 0, \"data\": []}";
            }

            BLL.Common comm = new BLL.Common();
            string strSort = "dtm desc";
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public ActionResult ExportUserChatData(string uid, string dtm_start, string dtm_end)
        {
            // 先从 sum_user 表中查询用户有数据的天数
            string sumUserSql = @"select CONVERT(char(10), dtm, 120) as dtm 
                                from dbo.sum_user 
                                where uid = '{0}' and dtm >= '{1} 00:00:00' and dtm <= '{2} 23:59:59' 
                                group by CONVERT(char(10), dtm, 120)";
            sumUserSql = string.Format(sumUserSql, uid, dtm_start, dtm_end);
            DataTable dtSumUser = DBHelper.SqlHelper.GetDataTable(sumUserSql);

            // 构建所有日期的表名和SQL语句
            StringBuilder sqlBuilder = new StringBuilder();
            for (int i = 0; i < dtSumUser.Rows.Count; i++)
            {
                string dateStr = dtSumUser.Rows[i]["dtm"].ToString();
                DateTime currentDate = Convert.ToDateTime(dateStr);
                string tableName = bll.GetTableName_msg(currentDate);
                
                if (sqlBuilder.Length > 0)
                {
                    sqlBuilder.Append(" UNION ALL ");
                }
                
                sqlBuilder.AppendFormat(@"select 
                                            msg.dtm as 日期,
                                            msg.uid_from as 发送用户ID,
                                            msg.uid_to as 接收用户ID,
                                            u.nick as 接收用户昵称,
                                            msg.state as 状态,
                                            msg.type as 类型,
                                            msg.txt as 消息内容
                                        from {0} msg
                                        left join dbo.[user] u on (msg.uid_to = u.uid)
                                        where (msg.uid_from = '{1}' or msg.uid_to = '{1}')
                                        and msg.dtm >= '{2} 00:00:00' 
                                        and msg.dtm <= '{3} 23:59:59'", 
                                        tableName, uid, dateStr, dateStr);
            }

            string strSql = sqlBuilder.ToString();
            if (string.IsNullOrEmpty(strSql))
            {
                // 如果没有数据，返回空结果
                return Content("没有找到聊天数据");
            }

            strSql += " order by 日期 desc";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            // 导出为CSV
            string fileName = string.Format("聊天数据_{0}_{1}.csv", uid, DateTime.Now.ToString("yyyyMMddHHmmss"));
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "UTF-8";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/csv";

            System.IO.StringWriter sw = new System.IO.StringWriter();
            
            // 写入表头
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write("\"" + dt.Columns[i].ColumnName + "\"");
                if (i < dt.Columns.Count - 1)
                    sw.Write(",");
            }
            sw.WriteLine();
            
            // 写入数据行
            foreach (System.Data.DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string value = row[i].ToString();
                    // 处理包含逗号、引号或换行符的值
                    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                    {
                        value = value.Replace("\"", "\"\"");
                        sw.Write("\"" + value + "\"");
                    }
                    else
                    {
                        sw.Write(value);
                    }
                    if (i < dt.Columns.Count - 1)
                        sw.Write(",");
                }
                sw.WriteLine();
            }

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return null;
        }

        public string GetCoinList(string id, string pageIndex, string pageSize, string key, string type_dtl, string dtm)
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
                                       
                                from dbo.coin  s
                                left join dbo.[user] u on (s.uid_from = u.uid)
                                left join dbo.[user] u2 on (s.uid_to = u2.uid)
                                where {0}";

            strSql = string.Format(strSql, strWhere);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        // 金币交易统计功能
        public ActionResult GetCoinTransaction(string userA, string userB)
        {
            // 从A到B的金币消耗
            string sqlAtoB = string.Format(@"
                SELECT 
                    'A->B' as direction,
                    type_dtl as transaction_type,
                    SUM(amount) as amount
                FROM dbo.coin
                WHERE uid_from = '{0}' AND uid_to = '{1}' AND type = '减少'
                GROUP BY type_dtl
                ORDER BY amount DESC
            ", userA, userB);
            
            // 从B到A的金币消耗
            string sqlBtoA = string.Format(@"
                SELECT 
                    'B->A' as direction,
                    type_dtl as transaction_type,
                    SUM(amount) as amount
                FROM dbo.coin
                WHERE uid_from = '{1}' AND uid_to = '{0}' AND type = '减少'
                GROUP BY type_dtl
                ORDER BY amount DESC
            ", userA, userB);
            
            // 执行查询并返回结果
            var resultAtoB = DBHelper.SqlHelper.GetDataTable(sqlAtoB);
            var resultBtoA = DBHelper.SqlHelper.GetDataTable(sqlBtoA);
            
            // 使用CommonTool.JsonHelper处理数据
            var result = new {
                AtoB = CommonTool.JsonHelper.DataTableToJSON(resultAtoB),
                BtoA = CommonTool.JsonHelper.DataTableToJSON(resultBtoA)
            };
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // 积分交易统计功能
        public ActionResult GetScoreTransaction(string userA, string userB)
        {
            // 从A到B的积分活动
            string sqlAtoB = string.Format(@"
                SELECT 
                    'A->B' as direction,
                    type_dtl as transaction_type,
                    SUM(amount) as amount
                FROM dbo.score
                WHERE uid_from = '{0}' AND uid_to = '{1}' AND type = '增加'
                GROUP BY type_dtl
                ORDER BY amount DESC
            ", userA, userB);
            
            // 从B到A的积分活动
            string sqlBtoA = string.Format(@"
                SELECT 
                    'B->A' as direction,
                    type_dtl as transaction_type,
                    SUM(amount) as amount
                FROM dbo.score
                WHERE uid_from = '{1}' AND uid_to = '{0}' AND type = '增加'
                GROUP BY type_dtl
                ORDER BY amount DESC
            ", userA, userB);
            
            // 执行查询并返回结果
            var resultAtoB = DBHelper.SqlHelper.GetDataTable(sqlAtoB);
            var resultBtoA = DBHelper.SqlHelper.GetDataTable(sqlBtoA);
            
            // 使用CommonTool.JsonHelper处理数据
            var result = new {
                AtoB = CommonTool.JsonHelper.DataTableToJSON(resultAtoB),
                BtoA = CommonTool.JsonHelper.DataTableToJSON(resultBtoA)
            };
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // 按日期统计交易情况
        public ActionResult GetTransactionByDate(string userA, string userB)
        {
            string sql = string.Format(@"
                WITH all_transactions AS (
                    SELECT 
                        CONVERT(char(10), dtm, 120) as date,
                        '金币' as transaction_type,
                        CASE 
                            WHEN uid_from = '{0}' AND uid_to = '{1}' AND type = '减少' THEN amount
                            WHEN uid_from = '{1}' AND uid_to = '{0}' AND type = '减少' THEN amount
                            ELSE 0 
                        END as coin_amount,
                        0 as score_amount
                    FROM dbo.coin
                    WHERE ((uid_from = '{0}' AND uid_to = '{1}') OR (uid_from = '{1}' AND uid_to = '{0}')) AND type = '减少'
                    
                    UNION ALL
                    
                    SELECT 
                        CONVERT(char(10), dtm, 120) as date,
                        '积分' as transaction_type,
                        0 as coin_amount,
                        CASE 
                            WHEN uid_from = '{0}' AND uid_to = '{1}' AND type = '增加' THEN amount
                            WHEN uid_from = '{1}' AND uid_to = '{0}' AND type = '增加' THEN amount
                            ELSE 0 
                        END as score_amount
                    FROM dbo.score
                    WHERE ((uid_from = '{0}' AND uid_to = '{1}') OR (uid_from = '{1}' AND uid_to = '{0}')) AND type = '增加'
                )
                SELECT 
                    date,
                    SUM(coin_amount) as 金币消耗,
                    SUM(score_amount) as 积分产生
                FROM all_transactions
                GROUP BY date
                ORDER BY date DESC
            ", userA, userB);
            
            var result = DBHelper.SqlHelper.GetDataTable(sql);
            
            // 使用CommonTool.JsonHelper处理数据
            string jsonResult = CommonTool.JsonHelper.DataTableToJSON(result);
            return Content(jsonResult, "application/json");
        }

        // 交易情况详情页面
        public ActionResult TransactionDetails(string userA, string userB)
        {
            ViewData["userA"] = userA;
            ViewData["userB"] = userB;
            return View();
        }

        public string GetScoreList(string id, string pageIndex, string pageSize, string key, string type_dtl, string dtm, string fz_tag)
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
                if (type_dtl == "agent")
                {
                    strWhere += " and (s.type_dtl = '收到礼物' or s.type_dtl = '消息收入') ";
                }
                else
                {
                    strWhere += string.Format(" and (s.type_dtl = '{0}') ", type_dtl);
                }
                
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
            if (!string.IsNullOrEmpty(fz_tag))
            {
                if (fz_tag == "agent")
                {
                    strWhere += " and (u.fz_tag <> '0') ";
                }
                else
                {
                    strWhere += string.Format(" and (u.fz_tag = '{0}') ", fz_tag);
                }
                
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
                                       u.fz_tag,
                                       
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

        public string GetChartDtl(string id, string pageIndex, string pageSize, string dtm, string sort, string sex_from, string data_from)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_msg(dtm);
            

// 本段 SQL 用于查询指定用户（id）在某一天（dtm）与其所有聊天对象的互动明细。
// 整体思路：
// 1) 先按“我发出去的消息”分组（msg_from），拿到每个聊天对象的首末条消息时间、消息条数。
// 2) 再按“对方发过来的消息”分组（msg_to），拿到同样的统计，用于后续对比。
// 3) 用 left join 把两套数据拼在一起，这样就能知道“我发了多少、对方回了多少”。
// 4) 继续 left join 用户表（u2）拿到聊天对象的头像、昵称、积分、金币等基本信息。
// 5) 取第一条消息的内容（msg2）作为展示样本。
// 6) 再分别汇总当天我给对方消耗的硬币（coin_to.c）和对方得到的积分（score_to.s），用于展示“我花了多少钱、对方赚了多少分”。
// 7) 最后返回的字段里：
//    tiaoshu              —— 我发给该对象的消息条数
//    tiaoshu_receive      —— 该对象回我的消息条数（可能为 0）
//    diff_first           —— 双方首条消息的时间差（秒），可用于判断谁先开口
//    photo2/nick2/sex2... —— 聊天对象的资料
//    txt/type/state       —— 我发的第一条消息内容及其状态
//    c                    —— 当天我给该对象消耗的硬币总额
//    s                    —— 当天该对象因我获得的积分总额
string strSql = @"
SELECT  msg_from.*,
        ISNULL(msg_to.tiaoshu, 0) AS tiaoshu_receive,
        DATEDIFF(SECOND, msg_from.time_start, msg_to.time_start) AS diff_first,
        u2.photo AS photo2,
        u2.nick AS nick2,
        u2.sex AS sex2,
        u2.create_time,
        u2.coin_total,
        u2.score_total,
        u2.coin,
        u2.score,
        u2.member,
        u2.coin_info,
        u2.remark,
        u2.chat_mny_score,
        u2.chat_mny_score2,
        msg2.txt,
        msg2.type,
        msg2.state,
        ISNULL(coin_from.c, 0) + ISNULL(coin_to.c2, 0) AS c,
        ISNULL(score_from.s, 0) + ISNULL(score_to.s2, 0) AS s
FROM   (SELECT uid_from,
               uid_to,
               MIN(dtm) AS time_start2,
               CONVERT(CHAR(20), MIN(dtm), 120) AS time_start,
               CONVERT(CHAR(20), MAX(dtm), 120) AS time_end,
               DATEDIFF(SECOND, MIN(dtm), MAX(dtm)) AS sec_diff,
               COUNT(*) AS tiaoshu
        FROM   {0}
        WHERE  uid_from = '{2}'
        GROUP  BY uid_from, uid_to) msg_from
LEFT JOIN
       (SELECT uid_from,
               uid_to,
               MIN(dtm) AS time_start2,
               CONVERT(CHAR(20), MIN(dtm), 120) AS time_start,
               CONVERT(CHAR(20), MAX(dtm), 120) AS time_end,
               DATEDIFF(SECOND, MIN(dtm), MAX(dtm)) AS sec_diff,
               COUNT(*) AS tiaoshu
        FROM   {0}
        WHERE  uid_to = '{2}'
        GROUP  BY uid_from, uid_to) msg_to
ON     (msg_from.uid_to = msg_to.uid_from)
LEFT JOIN dbo.[user] u2
ON     (msg_from.uid_to = u2.uid)
LEFT JOIN {0} msg2
ON     (msg_from.uid_from = msg2.uid_from
        AND msg_from.time_start2 = msg2.dtm)
LEFT JOIN
       (SELECT uid_to AS coin_uid,
               SUM(amount) AS c
        FROM   dbo.coin
        WHERE  [type] = '减少'
          AND  uid_from = '{2}'
          AND  (dtm >= '{1} 00:00:00'
                AND dtm <= '{1} 23:59:59')
        GROUP  BY uid_to) coin_from
ON     (msg_from.uid_to = coin_from.coin_uid)
LEFT JOIN
       (SELECT uid_from AS coin_uid2,
               SUM(amount) AS c2
        FROM   dbo.coin
        WHERE  [type] = '减少'
          AND  uid_to = '{2}'
          AND  (dtm >= '{1} 00:00:00'
                AND dtm <= '{1} 23:59:59')
        GROUP  BY uid_from) coin_to
ON     (msg_from.uid_to = coin_to.coin_uid2)
LEFT JOIN
       (SELECT uid_to AS score_uid,
               SUM(amount) AS s
        FROM   dbo.score
        WHERE  [type] = '增加'
          AND  uid_from = '{2}'
          AND  (dtm >= '{1} 00:00:00'
                AND dtm <= '{1} 23:59:59')
        GROUP  BY uid_to) score_from
ON     (msg_from.uid_to = score_from.score_uid)
LEFT JOIN
       (SELECT uid_from AS score_uid2,
               SUM(amount) AS s2
        FROM   dbo.score
        WHERE  [type] = '增加'
          AND  uid_to = '{2}'
          AND  (dtm >= '{1} 00:00:00'
                AND dtm <= '{1} 23:59:59')
        GROUP  BY uid_from) score_to
ON     (msg_from.uid_to = score_to.score_uid2)";
strSql = string.Format(strSql, table_name, dtm, id);

            //更换数据源---别人发过来的消息（用户没有回复）
            if (data_from == "msg_to_only")
            {
                strSql = Get_ChartDtl_SQL_To(id,dtm);
            }
            

            string strSort = "time_start2 asc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        private string Get_ChartDtl_SQL_To(string uid, string dtm)
        {
            string table_name = bll.GetTableName_msg(dtm);
            string strSql = @"select  '{2}' as uid_from,  msg_to.uid_from as uid_to, 0 as tiaoshu, msg_to.time_start,msg_to.time_end, msg_to.sec_diff, ISNULL(msg_to.tiaoshu, 0) as tiaoshu_receive,0 as diff_first,
                                       u2.photo as photo2, u2.nick as nick2, u2.sex as sex2, u2.create_time,u2.coin_total, u2.score_total,u2.coin, u2.score, u2.member,u2.coin_info,u2.remark
                                       ,msg2.txt, msg2.type,msg2.state , 0 as c, 0as s
                                from (select  uid_from, uid_to, 
                                       
                                       MIN(dtm) as time_start2,
                                       CONVERT(char(20), MIN(dtm), 120)  as time_start, 
                                       CONVERT(char(20), MAX(dtm), 120)  as time_end, 
                                       DATEDIFF(SECOND,MIN(dtm),MAX(dtm)) as sec_diff, 
                                      
                                       COUNT(*) as tiaoshu 
                                from {0}
                                where uid_to = '{2}' 
                                group by uid_from, uid_to
                                ) msg_to
                                left join 
                                (
                                select  uid_from, uid_to, 
                                       
                                                                       MIN(dtm) as time_start2,
                                                                       CONVERT(char(20), MIN(dtm), 120)  as time_start, 
                                                                       CONVERT(char(20), MAX(dtm), 120)  as time_end, 
                                                                       DATEDIFF(SECOND,MIN(dtm),MAX(dtm)) as sec_diff, 
                                      
                                                                       COUNT(*) as tiaoshu 
                                from {0}
                                where uid_from = '{2}'
                                group by uid_from, uid_to
                                ) msg_from on (msg_to.uid_from = msg_from.uid_to)
                                left join dbo.[user] u2 on (msg_to.uid_from = u2.uid)
                                left join {0} msg2 on(msg_to.uid_from = msg2.uid_from and msg_to.time_start2 = msg2.dtm) 
                                where msg_from.uid_from is null";
                                
            strSql = string.Format(strSql, table_name, dtm, uid);

            return strSql;
        }

        public string GetChart_Plat_Dtl(string id, string pageIndex, string pageSize, string start, string days, string sort)
        {
            string strRtn = string.Empty;

            int index = 1;
            int size = 200;
            if (!string.IsNullOrEmpty(pageIndex))
            {
                index = Convert.ToInt32(pageIndex);
            }
            if (!string.IsNullOrEmpty(pageSize))
            {
                size = Convert.ToInt32(pageSize);
            }

            Common com = new Common();
            string xhc_admin_uid = com.GetSysDicByKey("xhc_admin_uid");

            int iDays = 7;
            if (!string.IsNullOrEmpty(days))
            {
                iDays = Convert.ToInt32(days);
            }
            
            DateTime dtm_start = DateTime.Now;
            DateTime dtm = dtm_start;
            string table_name = bll.GetTableName_msg(dtm);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < iDays; i++)
            {
                if (i > 0)
                {
                    sb.Append(" union all ");
                }

                dtm = dtm_start.AddDays(-i);
                table_name = bll.GetTableName_msg(dtm);

                string strSql = @"select '{3}' as dtm, count(*) as c, 
       isnull(sum(case when uid_from = '{1}' and uid_to = '{2}' then 1 else 0 end),0) as tiaoshu,
	   isnull(sum(case when uid_from = '{2}' and uid_to = '{1}' then 1 else 0 end),0) as tiaoshu_receive 
from {0} where  [type] <> 'sys_probe' and ((uid_from = '{1}' and uid_to = '{2}') or (uid_from = '{2}' and uid_to = '{1}'))
";
                strSql = string.Format(strSql, table_name, xhc_admin_uid, id, dtm.ToString("yyyy-MM-dd"));
                sb.Append(strSql);
            }

            string strSort = "dtm desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(sb.ToString(), strSort, index, size);

            return strRtn;
        }

        public string GetRecord(string id, string pageIndex, string pageSize, string start, string end, string sort)
        {
            string strRtn = string.Empty;

            int index = 1;
            int size = 200;
            if (!string.IsNullOrEmpty(pageIndex))
            {
                index = Convert.ToInt32(pageIndex);
            }
            if (!string.IsNullOrEmpty(pageSize))
            {
                size = Convert.ToInt32(pageSize);
            }

            string strSql = @"";
            strSql = string.Format(strSql, id);    
                

            string strSort = "dtm desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        

        public string GetChartDtl_OnlineService(string service_uid, string pageIndex, string pageSize, string start, string end, string sort, string type)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_msg(end);
            string strWhere_Dtm = string.Format(" (dtm >= '{0}' and dtm <= '{1}') ", start, end);

            string strWhere_type = "";
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "check")
                {
                    strWhere_type = string.Format("and type like '{0}%'", type);
                }
                else
                {
                    strWhere_type = string.Format("and type='{0}'", type);
                }
            }


            //用户发给客服的消息
            string msg_from = @"select  uid_from, uid_to, 
                                       
                                       MIN(dtm) as time_start2,
                                       CONVERT(char(20), MIN(dtm), 120)  as time_start, 
                                       CONVERT(char(20), MAX(dtm), 120)  as time_end, 
                                       DATEDIFF(SECOND,MIN(dtm),MAX(dtm)) as sec_diff, 
                                      
                                       COUNT(*) as tiaoshu 
                                from {0}
                                where uid_to='{1}' and {2} {3}
                                group by uid_from, uid_to";
            msg_from = string.Format(msg_from, table_name, service_uid, strWhere_Dtm, strWhere_type);

            //客服发给用户的消息
            string msg_onlineService = @"select  uid_from, uid_to,
                                                 MIN(dtm) as time_start2,
                                                 CONVERT(char(20), MIN(dtm), 120)  as time_start, 
                                                 CONVERT(char(20), MAX(dtm), 120)  as time_end, 
                                                 DATEDIFF(SECOND,MIN(dtm),MAX(dtm)) as sec_diff, 
                                      
                                                 COUNT(*) as tiaoshu 
                                from {0}
                                where uid_from='{1}' and {2} 
                                group by uid_from, uid_to";
            msg_onlineService = string.Format(msg_onlineService, table_name, service_uid, strWhere_Dtm);

            string strSql = @"select   msg_from.*, 
                                       ISNULL(msg_to.tiaoshu, 0) as tiaoshu_receive,
                                       DATEDIFF(SECOND,msg_from.time_start,msg_to.time_start) as diff_first,

                                       u2.photo as photo2, u2.nick as nick2, u2.sex as sex2,u2.create_time,u2.coin_total, u2.score_total, u2.member,u2.coin_info
                                       ,msg2.txt, msg2.type,msg2.state 
                                from ({1}) msg_from
                                left join ({2}) msg_to on (msg_from.uid_from = msg_to.uid_to)
                                left join dbo.[user] u2 on (msg_from.uid_from = u2.uid)
                                left join {0} msg2 on(msg_from.uid_from = msg2.uid_from and msg_from.time_start2 = msg2.dtm) ";

            strSql = string.Format(strSql, table_name, msg_from, msg_onlineService);

            string strSort = "time_start2 asc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

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

        public string GetMsgOff_AB(string dtm, string uid_self, string uid_to, string preOrAfter)
        {
            string data = string.Empty;

            if (string.IsNullOrEmpty(dtm))
            {
                dtm = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (!string.IsNullOrEmpty(preOrAfter))
            {
                int days = Convert.ToInt32(preOrAfter);
                dtm = Convert.ToDateTime(dtm).AddDays(days).ToString("yyyy-MM-dd HH:mm:ss");
            }

            string dtm_end = DateTime.Now.ToString();
            string dtm_com = dtm_end;

            DataTable dt = bll.GetDataTable_Msg(dtm, uid_self, uid_to, ref dtm_end);
            data = CommonTool.JsonHelper.DataTableToJSON(dt);

            if (dtm_end != dtm_com)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("dtm", dtm_end);
                data = CommonTool.JsonHelper.ObjectToJSON(dic);
            }

            return data;
        }

        public string GetUserMsgDtl(string id, string pageIndex, string pageSize, string dtm)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_msg(dtm);

            if (string.IsNullOrEmpty(dtm))
            {
                dtm = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }

            string strWhere = " (1=1) ";

            if (!string.IsNullOrEmpty(id))
            {
                strWhere += string.Format(" and (uid_from = '{0}')", id);
            }
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere += string.Format(" and (dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59')", dtm);
            }


            string strSql = @"select msg.id, 
                                       CONVERT(char(20), dtm, 120) as dtm,
                                       txt,
                                       type,
                                       state,
                                       uid_to,
                                       u.photo,
                                       u.nick,
                                       u.sex
                                from {1} msg 
                                left join dbo.[user] u on (msg.uid_to = u.uid)
                                where {0}";

            strSql = string.Format(strSql, strWhere, table_name);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetUserLoginDtl(string id, string pageIndex, string pageSize, string dtm)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_login(dtm);

            string strWhere = " (1=1) ";

            if (!string.IsNullOrEmpty(id))
            {
                strWhere += string.Format(" and (uid = '{0}')", id);
            }
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere += string.Format(" and (dtm >= '{0} 00:00:00' and dtm <= '{0} 23:59:59')", dtm);
            }


            string strSql = @"select CONVERT(char(20), dtm, 120) as dtm,
                                       sex,
                                       isnull(loc_province, '') as loc_province,
                                       isnull(loc_city, '') as loc_city,
                                       isnull(loc_area, '') as loc_area,
                                       inst_tag 
                                from {1}
                                where {0}";

            strSql = string.Format(strSql, strWhere, table_name);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetUserMatchDtl(string id, string pageIndex, string pageSize, string dtm)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name_match = bll.GetTableName_match(dtm);
            string table_name_msg = bll.GetTableName_msg(dtm);

            string strWhere = " (1=1) ";

            if (!string.IsNullOrEmpty(id))
            {
                strWhere += string.Format(" and (m.uid = '{0}')", id);
            }
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere += string.Format(" and (m.dtm >= '{0} 00:00:00' and m.dtm <= '{0} 23:59:59')", dtm);
            }


            string strSql = @"with a as 
(
   select dtm, uid_from, uid_to, state
   from {2}
   where uid_from = '{0}' or uid_to = '{0}'
)

select CONVERT(char(20), m.dtm, 120) as dtm,
       m.to_uid,
       u.photo,u.nick, u.sex,
       0 as total_coin,--dbo.get_user_totalcoin(m.to_uid) 0 as total_coin,
       0 as total_score,--dbo.get_user_totalscore(m.to_uid) as total_score,
       --isnull((select name from dbo.user_s where uid = m.to_uid), '') as inner_name,
       --发送情况
       isnull((select COUNT(*) from a where a.uid_from = '{0}' and a.uid_to = m.to_uid),0) as send_count,
       isnull((select sum(case when state = 'success' then 1 else 0 end)  from a where a.uid_from = '{0}' and a.uid_to = m.to_uid ),0) as send_count_suc,
       isnull((select sum(case when state = 'fail' then 1 else 0 end)  from a where a.uid_from = '{0}' and a.uid_to = m.to_uid ),0) as send_count_fail,
       
       --接收情况
       isnull((select COUNT(*) from a where a.uid_to = '{0}' and a.uid_from = m.to_uid),0) as rece_count,
       isnull((select sum(case when state = 'success' then 1 else 0 end)  from a where a.uid_to = '{0}' and a.uid_from = m.to_uid ),0) as rece_count_suc,
       isnull((select sum(case when state = 'fail' then 1 else 0 end)  from a where a.uid_to = '{0}' and a.uid_from = m.to_uid ),0) as rece_count_fail
from {1} m
left join dbo.[user] u on (m.to_uid = u.uid)
where {3} order by dtm desc";

            strSql = string.Format(strSql, id, table_name_match, table_name_msg, strWhere);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData2(rows.ToString(), data);

            return strRtn;
        }


        public string GetWxInfoData(string uid)
        {
            string strRtn = string.Empty;
            string strSql = @"select subscribe,
                                     nickname,
                                     sex,
                                     [language],
                                     city,
                                     province,
                                     country,
                                     headimgurl,
                                     subscribe_time,
                                     CONVERT(char(20), least_update_time, 120) as least_update_time,
                                     CONVERT(char(20), unsub_time, 120) as unsub_time
                            from dbo.user_attach 
                            where uid = '{0}'";
            strSql = string.Format(strSql, uid);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);

            return strRtn;
        }

        #endregion

        #region 金币积分

        public ActionResult Coin()
        {
            string dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public ActionResult Score()
        {
            string dtm = DateTime.Now.ToString("yyyy-MM-dd");
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public ActionResult Score_Reword()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            return View();
        }

        public string GetScoreReword(string dtm, string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            DataTable dt = bll.GetScore_Reword(Convert.ToDateTime(dtm));

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData2(rows.ToString(), data);

            return strRtn;
        }
        public string GetScoreReword_m(string dtm, string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            DataTable dt = bll.GetScore_Reword(Convert.ToDateTime(dtm));

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);



            return data;
        }
        public string Create_Rank_Reword(string dtm, string uid, string r)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            //参数检查
            if (!CommonTool.Common.IsDateTime(dtm) || !CommonTool.Common.IsNumber(r))
            {
                info.State = "-1";
                info.Msg = "参数错误";
                return info.ToString();
            }

            bool tag = bll.Save_ScoreReword(Convert.ToDateTime(dtm), uid, Convert.ToInt32(r));
            if (tag)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }

            return info.ToString();
        }

        #endregion 

        #region 统计分析


        public ActionResult Ansy_ScoreChat()
        {
            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public ActionResult Ansy_LeastOnline()
        {
            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public ActionResult Ansy_OverAll()
        {
            return View();
        }

        public ActionResult Ansy_CoinScore()
        {
            return View();
        }

        public ActionResult Ansy_CoinScore_Daily()
        {
            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            return View();
        }

        public ActionResult Ansy_CoinScore_30Trend()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.AddDays(0).ToString("yyyy-MM-dd") : end; 

            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm,
                               sum(case when type = '增加' then amount else 0 end) as coin_add,  
                               sum(case when type = '增加' and type_dtl = '购买' then amount else 0 end) as coin_add_buy,
                               sum(case when type = '增加' and type_dtl = '任务奖励-会员' then amount else 0 end) as coin_add_vip,
                               sum(case when type = '增加' and type_dtl = '签到' then amount else 0 end) as coin_add_sign,
                               sum(case when type = '增加' and (type_dtl = 'newuser_1' or type_dtl = 'newuser_2' or type_dtl = '任务奖励-打招呼' or type_dtl = '任务奖励-回复' or type_dtl = '任务奖励-在线' or type_dtl = '任务奖励-登录') then amount else 0 end) as coin_add_task,
       
       
                               sum(case when type = '减少' then amount else 0 end) as coin_sub,
                               sum(case when type = '减少' and type_dtl = '发送礼物' then amount else 0 end) as coin_sub_gift,
                               sum(case when type = '减少' and type_dtl = '消息支出' then amount else 0 end) as coin_sub_msg,
                               sum(case when type = '减少' and type_dtl = '添加好友' then amount else 0 end) as coin_sub_friend
       
                        from dbo.coin 
                        where dtm >= '{0}' and dtm <= '{1}'
                        group by CONVERT(char(10), dtm, 120)
                        order by dtm asc";
            strSql = string.Format(strSql, start, end);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            ViewData["coin_sum"] = data;

            strSql = @"select CONVERT(char(10), dtm, 120) as dtm,
                           sum(case when type = '增加' then amount else 0 end) as score_add,  
                           sum(case when type = '增加' and type_dtl = '收到礼物' then amount else 0 end) as score_add_gift,
                           sum(case when type = '增加' and type_dtl = '消息收入' then amount else 0 end) as score_add_msg,
                           sum(case when type = '增加' and type_dtl = '同意添加好友' then amount else 0 end) as score_add_friend,
                           sum(case when type = '增加' and (type_dtl = '任务奖励-打招呼' or type_dtl = '任务奖励-回复' or type_dtl = '任务奖励-在线' or type_dtl = '任务奖励-登录') then amount else 0 end) as score_add_task,
       
       
                           sum(case when type = '减少' then amount else 0 end) as score_sub,
                           sum(case when type = '减少' and type_dtl = '提现' then amount else 0 end) as score_sub_tx,
                           sum(case when type = '减少' and type_dtl = '兑换金币' then amount else 0 end) as score_sub_coin
       
                    from dbo.score 
                    where dtm >= '{0}' and dtm <= '{1}'
                    group by CONVERT(char(10), dtm, 120)
                    order by dtm asc";
            strSql = string.Format(strSql, start, end);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);
            data = CommonTool.JsonHelper.DataTableToJSON(dt);

            ViewData["score_sum"] = data;

            return View();
        }
        public string GetAnsy_CoinData(string pageIndex, string pageSize, string dtm)
        {

            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere = "(1=1)";
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere += string.Format(" and (dtm >= '{0} 00:00:00.000' and dtm < '{0} 23:59:59.999') ", dtm);
            }

            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm, type, type_dtl,
                                   COUNT(*) as c1,
                                   sum(amount) as c2
                            from dbo.coin
                            where {0}
                            group by CONVERT(char(10), dtm, 120),type, type_dtl";

            strSql = string.Format(strSql, strWhere);
            string strSort = "type desc, c2 desc";

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            //返还前打印
            //CommonTool.WriteLog.Write(strRtn);
            return strRtn;
        }
        public string GetAnsy_ScoreData(string pageIndex, string pageSize, string dtm)
        {

            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);


            string strWhere = "(1=1)";
            if (!string.IsNullOrEmpty(dtm))
            {
                strWhere += string.Format(" and (dtm >= '{0} 00:00:00.000' and dtm < '{0} 23:59:59.999') ", dtm);
            }

            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm, type, type_dtl,
                                   COUNT(*) as c1,
                                   sum(amount) as c2
                            from dbo.score
                            where {0}
                            group by CONVERT(char(10), dtm, 120),type, type_dtl";

            strSql = string.Format(strSql, strWhere);
            string strSort = "type desc, c2 desc";

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            //返还前打印
            //CommonTool.WriteLog.Write(strRtn);
            return strRtn;
        }

        public ActionResult Ansy_VipAll()
        {
            return View();
        }

        public ActionResult Ansy_Vip()
        {
            //标准结构
            string strSql = @"select COUNT(*) as c_total,
       --上个月
       sum(case when dtm_pay > DATEADD(month, DATEDIFF(month, 0, GETDATE())-1, 0) and dtm_pay < DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0) then 1 else 0 end) as c_month_pre,
       --本月
       sum(case when dtm_pay > DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0) then 1 else 0 end) as c_month_cur,
       --昨日
       sum(case when dtm_pay > CAST(DATEADD(day, -1, GETDATE()) AS DATE) and dtm_pay < CAST(GETDATE() AS DATE) then 1 else 0 end) as c_yest,
       --今日
       sum(case when dtm_pay > CAST(GETDATE() AS DATE) then 1 else 0 end) as c_today,
       --当前会员
       sum(case when DATEADD(day, vip_time, dtm_pay) > GETDATE()  then 1 else 0 end) as c_canuse
from dbo.sys_order 
where order_state = '付款成功' and order_type='购买会员' ";
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;

            //
            string strSql_2 = @"select CONVERT(char(10), dtm_pay, 120) as dtm,
       COUNT(*) as c
from dbo.sys_order 
where order_state = '付款成功' and order_type='购买会员' and dtm_pay > CAST(DATEADD(DAY, -30, GETDATE()) as date)
group by CONVERT(char(10), dtm_pay, 120)
order by dtm asc";
            dt = DBHelper.SqlHelper.GetDataTable(strSql_2);
            string data_line = CommonTool.JsonHelper.DataTableToJSON(dt);
            ViewData["data_line"] = data_line;


            string strSql_3 = @"select CONVERT(char(7), dtm_pay, 120) as dtm,
       COUNT(*) as c
from dbo.sys_order 
where order_state = '付款成功' and order_type='购买会员' and dtm_pay > '2023-01-01'
group by CONVERT(char(7), dtm_pay, 120)
order by dtm asc
 
select DATEADD(MONTH, -12, GETDATE())";
            dt = DBHelper.SqlHelper.GetDataTable(strSql_3);
            string data_lineMonth = CommonTool.JsonHelper.DataTableToJSON(dt);
            ViewData["data_lineMonth"] = data_lineMonth;

            return View();
        }

        public ActionResult Ansy_Vip_RenewalCycle()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd") : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString("yyyy-MM-dd") : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public string GetVIP_RenewalCyclerData(string pageIndex, string pageSize, string start, string end, string uid, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = "";
            if (!string.IsNullOrEmpty(start))
            {
                where += string.Format(" and o.create_time > '{0} 00:00:00' ", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                where += string.Format(" and o.create_time < '{0} 23:59:59' ", end);
            }
            if (!string.IsNullOrEmpty(uid))
            {
                where += string.Format(" and o.uid = '{0}' ", uid);
            }

            string strSql = @"select u.uid, u.photo, u.nick, u.sex, 
       u.coin_total, u.coin, u.score_total, u.score,
       CONVERT(char(20), u.create_time,120) as create_time,
       CONVERT(char(20), u.least_time,120) as least_time,
       
       order_no, order_name, dtm_pay, pay_mny,
       isnull((select top 1 DATEADD(DAY, vip_time, dtm_pay) from dbo.sys_order where uid = o.uid and dtm_pay < o.dtm_pay and order_state = '付款成功' and order_type='购买会员' order by dtm_pay desc), '') as pre_end_time,
       isnull((select top 1 order_name from dbo.sys_order where uid = o.uid and dtm_pay < o.dtm_pay and order_state = '付款成功' and order_type='购买会员' order by dtm_pay desc), '') as pre_order_name
from dbo.sys_order o
left join dbo.[user] u on (o.uid = u.uid)
where o.order_state = '付款成功' and o.order_type='购买会员' {0}";

            strSql = string.Format(strSql, where);

            string strSort = "dtm_pay desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }
        //全部会员分析
        public ActionResult Ansy_Vip_All()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString("yyyy-MM-dd") : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        //全部会员分析数据
        public string GetVipAllData(string pageIndex, string pageSize, string dtmReg_start, string dtmReg_end, string dtmOnline_start, string dtmOnline_end, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strSql = @"select aa.*, 
       DATEADD(DAY, aa.vip_time, aa.dtm_pay) as vip_endtime
from (
select u.uid, u.photo, u.sex,  
       u.coin_total, u.coin, u.score_total, u.score,
       u.create_time, u.least_time,
       isnull((select MAX(dtm_pay) from dbo.sys_order where uid = u.uid and order_state = '付款成功' and order_type='购买会员'), '') as dtm_pay,
       isnull((select top 1 vip_time from dbo.sys_order where uid = u.uid and order_state = '付款成功' and order_type='购买会员'), 0) as vip_time
from dbo.[user] u
where u.uid in (select uid from dbo.sys_order where order_state = '付款成功' and order_type='购买会员')
      and u.least_time > '2024-10-01' --and u.create_time > '2024-10-01' 
) aa";

            strSql = string.Format(strSql);

            string strSort = "dtm_pay desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        //最近七天登录情况
        public ActionResult Ansy_Vip_Seven()
        {
            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public string GetLoginSevenData(string pageIndex, string pageSize, string dtm, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            DateTime dtm_now = DateTime.Now;
            string s1 = dtm_now.AddDays(-1).ToString("yyyy-MM-dd");
            string s2 = dtm_now.AddDays(-2).ToString("yyyy-MM-dd");
            string s3 = dtm_now.AddDays(-3).ToString("yyyy-MM-dd");
            string s4 = dtm_now.AddDays(-4).ToString("yyyy-MM-dd");
            string s5 = dtm_now.AddDays(-5).ToString("yyyy-MM-dd");
            string s6 = dtm_now.AddDays(-6).ToString("yyyy-MM-dd");
            string s7 = dtm_now.AddDays(-7).ToString("yyyy-MM-dd");

            string strSql = @"select u.uid, u.photo, u.sex,  
       u.coin_total, u.coin, u.score_total, u.score,
       u.create_time, u.least_time,
       u.member, u.vip_endtime,
       ISNULL(s1.c_login, 0) as login_1,
       ISNULL(s2.c_login, 0) as login_2,
       ISNULL(s3.c_login, 0) as login_3,
       ISNULL(s4.c_login, 0) as login_4,
       ISNULL(s5.c_login, 0) as login_5,
       ISNULL(s6.c_login, 0) as login_6,
       ISNULL(s7.c_login, 0) as login_7
       
from dbo.[user] u
left join (select * from dbo.sum_user where dtm = '{0} 00:00:00') s1 on (u.uid = s1.uid)
left join (select * from dbo.sum_user where dtm = '{1} 00:00:00') s2 on (u.uid = s2.uid)
left join (select * from dbo.sum_user where dtm = '{2} 00:00:00') s3 on (u.uid = s3.uid)
left join (select * from dbo.sum_user where dtm = '{3} 00:00:00') s4 on (u.uid = s4.uid)
left join (select * from dbo.sum_user where dtm = '{4} 00:00:00') s5 on (u.uid = s5.uid)
left join (select * from dbo.sum_user where dtm = '{5} 00:00:00') s6 on (u.uid = s6.uid)
left join (select * from dbo.sum_user where dtm = '{6} 00:00:00') s7 on (u.uid = s7.uid)
where u.member <> '' ";

            strSql = string.Format(strSql, s1,s2,s3,s4,s5,s6,s7);

            string strSort = "least_time desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

       

        public string GetLeastOnlineData(string uid, string pageIndex, string pageSize, string dtm)
        {

            //开始位置打印
            //CommonTool.WriteLog.Write("uid--->" + uid + ";pageIndex--->" + pageIndex + ";pageSize--->" + pageSize + ";dtm--->" + dtm);

            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

           
            strRtn = "";

            //返还前打印
            //CommonTool.WriteLog.Write(strRtn);
            return strRtn;
        }

        public ActionResult Ansy_Score()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString("yyyy-MM-dd") : end;

            DataTable dt = bll.GetSumScore_Day(start, end);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            ViewData["data"] = data;
            ViewData["start"] = start;
            ViewData["end"] = end;

            return View();
        }

        public ActionResult Ansy_DaySum()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString("yyyy-MM-dd") : end;

            DataTable dt = bll.GetSumData_Day(Convert.ToDateTime(start), Convert.ToDateTime(end), "");
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            ViewData["data"] = data;
            ViewData["start"] = start;
            ViewData["end"] = end;

            return View();
        }

        public ActionResult Ansy_NewUser()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00" : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }
        public string GeAnsy_NewUserData(string pageIndex, string pageSize, string start, string end, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string tbl_name_msg = bll.GetTableName_msg(start);

            string strSql = @"select
                                       u.uid,
                                       u.photo,
                                       u.nick,
                                       u.sex,
                                       u.out_id,

                                       CONVERT(char(20), u.create_time, 120) as first_time,
       
                                       (select COUNT(*) from dbo.login_on where uid = u.uid) as login_count,
                                       (select COUNT(*) from dbo.match where uid = u.uid) as match_count,
                                       (select COUNT(*) from {2} where uid_from = u.uid) as msg_count
       
                                from dbo.[user] u 
                                where u.create_time >= '{0}' and u.create_time <= '{1}'";

            strSql = string.Format(strSql, start, end, tbl_name_msg);

            if (Convert.ToDateTime(start).Date < DateTime.Now.Date)
            {
                strSql = @"select
                                   u.uid,
                                   u.photo,
                                   u.nick,
                                   u.sex,
                                   u.out_id,

                                   CONVERT(char(20), u.create_time, 120) as first_time,
       
                                   su.c_login as login_count,
                                   su.c_match as match_count,
                                   su.c_msg_suc as msg_count
       
                            from dbo.[user] u 
                            left join dbo.[sum_user] su on (u.uid = su.uid and su.dtm = '{2}')
                            where u.create_time >= '{0}' and u.create_time <= '{1}' ";
                strSql = string.Format(strSql, start, end, Convert.ToDateTime(start).ToString("yyyy-MM-dd"));
            }

            string strSort = "first_time desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }


        public ActionResult Match()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddMinutes(-15).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }

        public string GetMatchData(string id, string pageIndex, string pageSize, string start, string end)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string tbl_name = "dbo.match";
            DateTime dtm_start = Convert.ToDateTime(start);
            if (dtm_start < Convert.ToDateTime(DateTime.Now.ToString("yyy-MM-dd")))
            {
                string out_db = CommonTool.Common.GetAppSetting("out_db");
                string prex = "dbo.match";
                string subfix = bll.GetTableNameSuffix(start);

                tbl_name = string.Format("{0}.{1}_{2}", out_db, prex, subfix);
            }

            string tbl_name_msg = bll.GetTableName_msg(dtm_start);

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(id))
            {
                where += string.Format(" and (m.uid='{0}') ", id);
            }

            string strSql = @"select   CONVERT(char(19), m.dtm, 120) as dtm,
                                       m.uid, 
                                       u1.photo as photo_from,
                                       u1.nick as  nick_from,
                                       u1.sex as sex_from,
                                       CONVERT(char(19), u1.create_time , 120) as first_time_from,
                                       dbo.get_user_totalcoin(m.uid) as totalcoin_from,
                                       dbo.get_user_totalscore(m.uid) as totalscore_from,

                                       m.to_uid,
                                       u2.photo as photo_to,
                                       u2.nick as  nick_to,
                                       u2.sex as sex_to,
                                       CONVERT(char(19), u2.create_time , 120) as first_time_to,
                                       dbo.get_user_totalcoin(m.to_uid) as totalcoin_to,
                                       dbo.get_user_totalscore(m.to_uid) as totalscore_to,
                                    
                                       (select COUNT(*) from {4} where state='success' and  ((uid_from = m.uid and uid_to = m.to_uid) ))  as msg_count_suc,
                                       (select COUNT(*) from {4} where state='fail' and  ((uid_from = m.uid and uid_to = m.to_uid) )) as msg_count_fail,
                                       (select COUNT(*) from {4} where state='success' and  (uid_from = m.uid) )  as msg_count_suc_today,
                                       (select COUNT(*) from {4} where state='fail' and (uid_from = m.uid )) as msg_count_fail_today
                                from {3} m 
                                left join dbo.[user] u1 on(m.uid = u1.uid)
                                left join dbo.[user] u2 on(m.to_uid = u2.uid)
                                where dtm >= '{0}' and dtm <= '{1}' and {2} ";


            strSql = string.Format(strSql, start, end, where, tbl_name, tbl_name_msg);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }


        public ActionResult SysSet_ReceMatch()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString() : dtm;
            ViewData["dtm"] = dtm;

            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            return View();
        }
        public string GetReceMatchData(string id, string pageIndex, string pageSize, string dtm, string name)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(dtm))
            {
                //当前在线数据
                dt = bll.Get_ReceMathData();
            }
            else
            {
                //历史数据
                dt = bll.Get_ReceMathData(dtm, name);
            }

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData2(rows.ToString(), data);

            return strRtn;
        }

        public ActionResult ReceMatch_Dtl(string uid, string dtm_start, string dtm_end)
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            ViewData["uid"] = uid;
            ViewData["dtm_start"] = dtm_start;
            ViewData["dtm_end"] = dtm_end;

            return View();
        }
        public string GetUserReceMatchDtl(string uid, string pageIndex, string pageSize, string dtm_start, string dtm_end)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name_match = bll.GetTableName_match(dtm_start);
            string table_name_msg = bll.GetTableName_msg(dtm_start);

            string strWhere = " (1=1) ";

            if (!string.IsNullOrEmpty(uid))
            {
                strWhere += string.Format(" and (m.to_uid = '{0}')", uid);
            }
            if (!string.IsNullOrEmpty(dtm_start))
            {
                strWhere += string.Format(" and (m.dtm >= '{0}' and m.dtm <= '{1}')", dtm_start, dtm_end);
            }


            string strSql = @"with a as 
(
   select dtm, uid_from, uid_to, state
   from {2}
   where uid_from = '{0}' or uid_to = '{0}'
),
b as
(
select CONVERT(char(20), max(m.dtm), 120) as dtm,
       m.uid,
       u.photo,u.nick, u.sex,
       dbo.get_user_totalcoin(m.uid) as total_coin,
       dbo.get_user_totalscore(m.uid) as total_score,
       DATEDIFF(MINUTE, u.create_time, max(m.dtm)) as reg_time,
       COUNT(dtm) as match_count,
       
       --发送情况
       isnull((select COUNT(*) from a where a.uid_from = '{0}' and a.uid_to = m.uid),0) as send_count,
       isnull((select sum(case when state = 'success' then 1 else 0 end)  from a where a.uid_from = '{0}' and a.uid_to = m.uid ),0) as send_count_suc,
       isnull((select sum(case when state = 'fail' then 1 else 0 end)  from a where a.uid_from = '{0}' and a.uid_to = m.uid ),0) as send_count_fail,
       
       --接收情况
       isnull((select COUNT(*) from a where a.uid_to = '{0}' and a.uid_from = m.uid),0) as rece_count,
       isnull((select sum(case when state = 'success' then 1 else 0 end)  from a where a.uid_to = '{0}' and a.uid_from = m.uid ),0) as rece_count_suc,
       isnull((select sum(case when state = 'fail' then 1 else 0 end)  from a where a.uid_to = '{0}' and a.uid_from = m.uid ),0) as rece_count_fail
from {1} m 
left join dbo.[user] u on (m.uid = u.uid)
where {3} 
group by m.uid,u.photo,u.nick, u.sex,u.create_time
)
select ROW_NUMBER() over(order by dtm desc) as xh, * 
from b 
order by dtm desc";

            strSql = string.Format(strSql, uid, table_name_match, table_name_msg, strWhere);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);

            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData2(rows.ToString(), data);

            return strRtn;
        }

        #endregion

        #region 签约用户管理



        public ActionResult S_User()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            BLL.Common com = new Common();
            string fn_user_s = com.GetSysDicByKey("fn_user_s");
            ViewData["fn_user_s"] = fn_user_s;

            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            return View();
        }


        public ActionResult Sum_UserScore_line()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];
            string name = Request.QueryString["name"];

            CommonTool.WriteLog.Write(start + "," + end + "," + name);

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddDays(-30).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;
            ViewData["name"] = name;
            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            string where = "1=1";
            if (!string.IsNullOrEmpty(name))
            {
                where = string.Format("( name = '{0}' )", name);
            }

            string strSql = @"select CONVERT(char(10), dtm, 120) as dtm,
                               sum(total) as total,
       
                               sum(s_msg) as s_msg,
                               sum(s_blackq) as s_blackq,
                               sum(s_gift) as s_gift,
       
                               sum(c_msg_suc) as c_msg_suc,
                               sum(c_blackq) as c_blackq,
                               sum(c_blackq_men) as c_blackq_men,
                               case when sum(total) > 0 then  CONVERT(DECIMAL(13,2), sum(s_gift) * 1.0 / sum(total) * 100)  else 0 end as rate
                                    from dbo.sum_score 
                                    where dtm >= '{0}' and dtm <= '{1}'  and {2}
                                    group by dtm 
                                    order by dtm asc ";

            strSql = string.Format(strSql, start, end, where);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            ViewData["data"] = data;

            strSql = @"select sum(total) as total, sum(s_gift) as gift,
                                    case when sum(total) > 0 then  CONVERT(DECIMAL(13,2), sum(s_gift) * 1.0 / sum(total) * 100)  else 0 end as rate
                                    from dbo.sum_score 
                                    where dtm >= '{0}' and dtm <= '{1}' and {2}";

            strSql = string.Format(strSql, start, end, where);

            dt = DBHelper.SqlHelper.GetDataTable(strSql);
            data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data2"] = data;

            return View();
        }

        public ActionResult S_User_Add(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "";
                ViewData["data"] = "{}";
                ViewData["id"] = "";
                return View();
            }

            DataTable dt = bll.UserS_List_byId(id);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;
            ViewData["id"] = id;
            return View();
        }

        public string GetSUserData(string pageIndex, string pageSize, string name, string is_use, string is_use_outnet)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(name))
            {
                where += string.Format(" and (s.name='{0}') ", name);
            }
            if (!string.IsNullOrEmpty(is_use))
            {
                where += string.Format(" and (s.is_use='{0}') ", is_use);
            }
            if (!string.IsNullOrEmpty(is_use_outnet))
            {
                where += string.Format(" and (s.is_use_outnet='{0}') ", is_use_outnet);
            }

            string strSql = @"select s.id,
                                           s.uid,
                                           s.name,
                                           s.is_use,
                                           s.is_use_outnet,
                                           s.score_lv,
                                           
                                           u.photo,
                                           u.nick,
                                           u.sex,
                                           dbo.get_user_totalscore(s.uid) as totalscore,s.create_time
                                    from dbo.user_s s
                                    left join dbo.[user] u on (s.uid = u.uid)
                                    where {0}";

            strSql = string.Format(strSql, where);

            string strSort = "create_time asc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string SaveSUser(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            string id = dicParm["id"];
            bool tag = false;
            if (string.IsNullOrEmpty(id))
            {
                dicParm.Remove("id");
                id = bll.UserS_Insert(dicParm);
                tag = true;
            }
            else
            {
                tag = bll.UserS_Update(id, dicParm);
            }


            if (tag)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "数据库操作失败";
            }

            return info.ToString();
        }

        public ActionResult Sum_UserScore()
        {
            string dtm = Request.QueryString["dtm"];
            dtm = string.IsNullOrEmpty(dtm) ? DateTime.Now.ToString("yyyy-MM-dd") : dtm;
            ViewData["dtm"] = dtm;

            //昨天
            string dtm_y = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            ViewData["dtm_y"] = dtm_y;

            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            int month = DateTime.Now.Month;
            ViewData["month"] = month;

            BLL.Common com = new Common();
            string fn_user_s = com.GetSysDicByKey("fn_user_s");
            ViewData["fn_user_s"] = fn_user_s;

            return View();
        }

        public string CreatePScore(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (string.IsNullOrEmpty(id))
            {
                info.State = "0";
                info.Msg = "参数错误";
                return info.ToString();
            }

            string dtm = id;

            BLL.BLL bll = new BLL.BLL();
            int count = bll.Sum_UserS(Convert.ToDateTime(dtm));
            if (count > 0)
            {
                info.State = "1";
                info.Msg = "success";
            }
            else
            {
                info.State = "0";
                info.Msg = "fail";
            }

            return info.ToString();
        }

        public string GetPScoreData(string pageIndex, string pageSize, string year, string month, string user_name, string dtm)
        {

            //CommonTool.WriteLog.Write("dtm:" + dtm + "; tie_man:" + tie_man);
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere = "1=1";
            if (!string.IsNullOrEmpty(user_name))
            {
                strWhere += string.Format(" and name = '{0}' ", user_name);
            }


            if (string.IsNullOrEmpty(year))
            {
                year = DateTime.Now.Year.ToString();
            }
            string start = year.ToString() + "-" + month + "-01";
            string end = Convert.ToDateTime(start).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            if (!string.IsNullOrEmpty(dtm))
            {
                start = dtm + " 00:00:00";
                end = dtm + " 23:59:59";
            }

            BLL.BLL bll = new BLL.BLL();
            string strGroup = bll.GetGroupData(start, end, SummaryType.day);

            string strSql = @"with 
                                a as ({3})
                                ,
                                b as (
                                select id, CONVERT(char(10), dtm, 120) as dtm_,
                                       name,
                                       score_lv,
                                       
                                       c_msg,
                                       c_msg_suc,
                                       c_blackq,
                                       c_blackq_men,
                                       total,
                                       s_msg,
                                       s_blackq,
                                       s_gift,
                                       s_msg_r,
                                       s_blackq_r,
                                       s_gift_r,
                                       s_other,
                                       s_other_note
                                from dbo.sum_score where {0} and dtm >= '{1}' and dtm <= '{2}'
                                )
                                select a.dtm,
                                       b.*
                                from a 
                                left join b on (a.dtm = b.dtm_)
                                order by a.dtm asc, total desc";
            strSql = string.Format(strSql, strWhere, start, end, strGroup);

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            BLL.Common comm = new BLL.Common();
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            string rows = dt.Rows.Count.ToString();

            strRtn = comm.GetMiniUIData2(rows, data);

            return strRtn;
        }


        public ActionResult PScore_Dtl(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }

            BLL.BLL bll = new BLL.BLL();
            DataTable dt = bll.SumScore_List_byId(id);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;
            ViewData["id"] = id;
            return View();
        }
        public ActionResult PScore_Update(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }

            BLL.BLL bll = new BLL.BLL();
            DataTable dt = bll.SumScore_List_byId(id);
            string data = CommonTool.JsonHelper.DataTableToJSONOneRaw(dt);
            ViewData["data"] = data;
            ViewData["id"] = id;
            return View();
        }

        public string UpdatePscore(string id, string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            //检查参数
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(send))
            {
                info.State = "0";
                info.Msg = "参数错误";
                return info.ToString();
            }

            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            BLL.BLL bll = new BLL.BLL();
            bool btag = bll.SumScore_Update(id, dicParm);
            if (btag)
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


        public ActionResult S_User_Account()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            return View();
        }

        public string GetSUserAccountData(string pageIndex, string pageSize, string name)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(name))
            {
                where += string.Format(" and (s.name='{0}') ", name);
            }


            string strSql = @"select s.id,
                                           s.uid,
                                           s.name,
                                           s.is_use,
                                           s.is_use_outnet,
                                           s.score_lv,
                                           
                                           u.photo,
                                           u.nick,
                                           u.sex,
                                           dbo.get_user_totalscore(s.uid) as totalscore,
                                           
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=1 then su.c_msg_suc else 0 end) as msg1,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=2 then su.c_msg_suc else 0 end) as msg2,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=3 then su.c_msg_suc else 0 end) as msg3,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=4 then su.c_msg_suc else 0 end) as msg4,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=5 then su.c_msg_suc else 0 end) as msg5,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=6 then su.c_msg_suc else 0 end) as msg6,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=7 then su.c_msg_suc else 0 end) as msg7,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=8 then su.c_msg_suc else 0 end) as msg8,
                                           SUM(case when DateDiff(dd, su.dtm, getdate())=9 then su.c_msg_suc else 0 end) as msg9
                                    from dbo.user_s s
                                    left join dbo.[user] u on (s.uid = u.uid)
                                    left join dbo.sum_user su on (s.uid = su.uid and su.dtm > DATEADD(day, -10, getdate()))
                                    where s.is_use = 1 and {0}
                                    group by s.id,
                                           s.uid,
                                           s.name,
                                           s.is_use,
                                           s.is_use_outnet,
                                           s.score_lv,
                                           
                                           u.photo,
                                           u.nick,
                                           u.sex";

            strSql = string.Format(strSql, where);

            string strSort = "msg1 desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }


        public ActionResult S_User_Boy()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;
            return View();
        }

        public string GetSUserBoyData(string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strSql = @"select u.uid,
                                           
                                           u.photo,
                                           u.nick,
                                           u.sex,
                                           dbo.get_user_totalcoin(u.uid) as totalcoin,
                                           dbo.get_user_leftcoin(u.uid) as leftcoin
                                           
                                    from dbo.[user] u 
                                    where u.uid in ('{0}')";
            string uids = CommonTool.Common.GetAppSetting("i_boy");
            strSql = string.Format(strSql, uids.Replace(",", "','"));

            string strSort = "leftcoin desc, totalcoin asc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public ActionResult Inner_Gift_Score()
        {
            string start = Request.QueryString["start"];
            string end = Request.QueryString["end"];

            start = string.IsNullOrEmpty(start) ? DateTime.Now.AddHours(-2).ToString() : start;
            end = string.IsNullOrEmpty(end) ? DateTime.Now.ToString() : end;

            ViewData["start"] = start;
            ViewData["end"] = end;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;


            List<string> listMan = bll.GetInnerMan();
            ViewBag.listMan = listMan;

            return View();
        }

        public string GetGiftSum_Inner_2(string pageIndex, string pageSize, string dtm_start, string dtm_end, string name)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            BLL.Common com = new Common();
            string fest_name = com.GetSysDicByKey("fest_name");

            string strSql = @"select u.uid,
                                       u.photo,
                                       u.nick,
                                       u.sex,
       
                                       sum(case when s.type_dtl = '同意添加好友' then 1 else 0 end ) as count_好友,
                                       sum(case when s.type_dtl = '领取徒弟收益' then amount else 0 end ) as count_收徒,

                                       sum(case when s.comment = '棒棒糖' or s.comment = '{3}' then 1 else 0 end ) as count_棒棒糖,
                                       sum(case when s.comment = '鲜花' then 1 else 0 end ) as count_鲜花,
                                       sum(case when s.comment = '黄瓜' then 1 else 0 end ) as count_黄瓜,
                                       sum(case when s.comment = '啪啪啪' then 1 else 0 end ) as count_啪啪啪,
                                       sum(case when s.comment = '幸运星' then 1 else 0 end ) as count_幸运星,

                                       sum(case when s.comment = '巧克力' then 1 else 0 end ) as count_巧克力,
                                       sum(case when s.comment = '气球' then 1 else 0 end ) as count_气球,
                                       sum(case when s.comment = '口红' then 1 else 0 end ) as count_口红,
       
                                       sum(case when s.comment = '香水' then 1 else 0 end ) as count_香水,
                                       sum(case when s.comment = '包包' then 1 else 0 end ) as count_包包,
                                       sum(case when s.comment = '钻戒' then 1 else 0 end ) as count_钻戒,
                                       sum(case when s.comment = '水晶鞋' then 1 else 0 end ) as count_水晶鞋,
                                       sum(case when s.comment = '皇冠' then 1 else 0 end ) as count_皇冠,
                                       sum(case when s.comment = '法拉利' then 1 else 0 end ) as count_法拉利,
                                       sum(case when s.comment = '飞机' then 1 else 0 end ) as count_飞机,
                                       sum(case when s.comment = '火箭' then 1 else 0 end ) as count_火箭
                                from dbo.[user] u 
                                left join dbo.score s on (u.uid = s.uid_from and type='增加' and (type_dtl='收到礼物' or type_dtl='同意添加好友' or type_dtl='领取徒弟收益') and (s.dtm >= '{0}' and s.dtm <= '{1}') and s.uid_to not in ('{4}') )
                                where u.uid in ({2})
                                group by u.uid,
                                       u.photo,
                                       u.nick,
                                       u.sex ";

            string uids = bll.GetInnerManUIds(name);
            string uids_not = bll.GetInnerBoyUids();
            strSql = string.Format(strSql, dtm_start, dtm_end, uids, fest_name, uids_not.Replace(",", "','"));

            string strSort = "count_棒棒糖 desc";
            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }


        public string GetChartScore2(string dtm_start, string dtm_end, string user_name)
        {
            int score = bll.Calculate_Score_Gift_Dtm(Convert.ToDateTime(dtm_start), Convert.ToDateTime(dtm_end), user_name);
            return score.ToString();
        }

        #endregion 

        #region 业务处理

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
                if (fz_tag == "agent")
                {
                    where += string.Format(" and (u.fz_tag <> '0') ");
                }
                else
                {
                    where += string.Format(" and (u.fz_tag='{0}') ", fz_tag);
                }
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
                                           isnull(u.remark, '') as remark,
                                           isnull(u.fz_tag, '0') as fz_tag,
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

                if (key == "风险提示")
                {
                    where = " (risk_tip<>'') ";
                }
            }


            if (!string.IsNullOrEmpty(wxnick))
            {
                where = string.Format("u.uid in (select uid from dbo.user_attach where nickname like '%{0}%') ", wxnick);
            }

            if (!string.IsNullOrEmpty(fz_tag))
            {
                if (fz_tag == "agent")
                {
                    where += " and (fz_tag<>'0') ";
                }
                else
                {
                    where += string.Format(" and (fz_tag='{0}') ", fz_tag);
                }
                
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
                                           isnull(risk_tip, '') as risk_tip,
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

        public string GetChatUserData(string id, string pageIndex, string pageSize, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string dtm_0 = DateTime.Now.ToString("yyyy-MM-dd");
            string dtm_1 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string dtm_2 = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            string strSql = @"select
                                           u.uid,
                                           u.photo,
                                           u.nick,
                                           u.sex,

                                           score_total as s_total,
                                           score as s_tx,
                                           dbo.get_user_score(u.uid, '{0}') as s_jt,
                                           dbo.get_user_score(u.uid, '{1}') as s_1,
                                           dbo.get_user_score(u.uid, '{2}') as s_2,

                                           u.remark,
                                           CONVERT(char(10), u.create_time, 120) as first_time
                                    from dbo.[user] u 
                                    where remark!=''";

            strSql = string.Format(strSql, dtm_0, dtm_1, dtm_2);

            string strSort = "first_time desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetTeacherStudentData(string id, string pageIndex, string pageSize, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);


            string strSql = @"select um.uid, 
                                   um.photo,
                                   um.nick,
                                   um.sex,
                                   um.remark,
                                   COUNT(ua.uid) as c 
                            from dbo.[user] um
                            left join dbo.[user] ua on (um.tel = ua.pre_tel)
                            where um.tel in (select pre_tel from dbo.[user] where pre_tel <> '')
                            group by um.uid, 
                                   um.photo,
                                   um.nick,
                                   um.sex,
                                   um.remark";

            BLL.Common comm = new BLL.Common();
            string strSort = "c desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetMasterScoreData(string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);


            string strSql = @"select u1.uid as uid_1,
                                       u1.photo as photo_1, 
                                       u1.nick as nick_1, 
                                       ISNULL(u1.remark, '') as remark_1, 
                                       u1.sex as sex_1,
                                       dbo.get_user_score2(u1.uid, CONVERT(char(10), GETDATE(), 120)) as s_today,
                                       dbo.get_user_score2(u1.uid, CONVERT(char(10), DATEADD(day, -1, getdate()), 120)) as s_yest,
                                       u2.uid as uid_2,
                                       u2.photo as photo_2, 
                                       u2.nick as nick_2, 
                                       ISNULL(u2.remark, '') as remark_2, 
                                       u2.sex as sex_2,
                                       0 as s_reward1,
                                       ISNULL(s.amount, 0) as s_reward2
                                from dbo.[user] u1
                                left join dbo.[user] u2 on (u1.pre_tel = u2.tel)
                                left join dbo.score s on (u2.uid = s.uid_from and u1.uid = s.uid_to and s.type_dtl='领取徒弟收益' and s.comment = CONVERT(char(10), DATEADD(day, -1, getdate()), 120) )
                                where u1.pre_tel <> ''";

            BLL.Common comm = new BLL.Common();
            string strSort = "s_yest desc";

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetStudentData(string id, string pageIndex, string pageSize, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string dtm_0 = DateTime.Now.ToString("yyyy-MM-dd");
            string dtm_1 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string dtm_2 = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            string strSql = @"select
                                           u.uid,
                                           u.photo,
                                           u.nick,
                                           u.sex,

                                           dbo.get_user_totalscore(u.uid) as s_total,
                                           dbo.get_user_leftscore(u.uid) as s_tx,
                                           dbo.get_user_score(u.uid, '{0}') as s_jt,
                                           dbo.get_user_score(u.uid, '{1}') as s_1,
                                           dbo.get_user_score(u.uid, '{2}') as s_2,

                                           u.remark,
                                           CONVERT(char(10), u.create_time, 120) as first_time
                                    from dbo.[user] u 
                                    where u.pre_tel = (select tel from dbo.[user] where uid = '{3}')";
            strSql = string.Format(strSql, dtm_0, dtm_1, dtm_2, id);

            string strSort = "first_time desc";
            if (!string.IsNullOrEmpty(sort))
            {
                strSort = sort;
            }
            BLL.Common comm = new BLL.Common();
            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }


        public string GetChatUserData_m(string id, string pageIndex, string pageSize, string sort)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string dtm_0 = DateTime.Now.ToString("yyyy-MM-dd");
            string dtm_1 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string dtm_2 = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            string strSql = @"select
                                           u.uid,
                                           u.photo,
                                           u.nick,
                                           u.sex,

                                           dbo.get_user_totalscore(u.uid) as s_total,
                                           dbo.get_user_leftscore(u.uid) as s_tx,
                                           dbo.get_user_score(u.uid, '{0}') as s_jt,
                                           dbo.get_user_score(u.uid, '{1}') as s_1,
                                           dbo.get_user_score(u.uid, '{2}') as s_2,

                                           u.remark,
                                           CONVERT(char(10), u.create_time, 120) as first_time
                                    from dbo.[user] u 
                                    where remark!='' 
                                    order by s_jt desc";

            strSql = string.Format(strSql, dtm_0, dtm_1, dtm_2);


            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);

            int rows = dt.Rows.Count;
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);


            return data;
        }

        public string GetMsgSum_Inner(string pageIndex, string pageSize, string dtm, string name)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_msg(dtm);

            string strSql = @" select CONVERT(char(13), dtm, 120) as dtm, 
                                       COUNT(*) as c1,
                                       SUM(case when state = 'success' then 1 else 0 end) as c2
                                from {0}
                                where uid_from in ({1})
                                group by CONVERT(char(13), dtm, 120)";

            string uids = bll.GetInnerManUIds(name);

            strSql = string.Format(strSql, table_name, uids);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetMsgSum_Inner2(string pageIndex, string pageSize, string dtm, string name)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_msg(dtm);

            string strSql = @" select CONVERT(char(13), msg.dtm, 120) as dtm, 
                                       u.name,
                                       SUM(case when state = 'success' then 1 else 0 end) as c2
                                from {0} msg
                                left join dbo.user_s u on (msg.uid_from = u.uid)
                                where uid_from in ({1})
                                group by CONVERT(char(13), msg.dtm, 120), u.name";

            string uids = bll.GetInnerManUIds(name);

            strSql = string.Format(strSql, table_name, uids);

            string strSort = "dtm desc, c2 desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetMsgSummaryData(string name, string dtm)
        {
            string data = string.Empty;
            DataTable dt = bll.Sum_InnerMsg(Convert.ToDateTime(dtm), name);
            data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return data;
        }

        public string GetMsgSummaryData2(string name, string dtm)
        {
            string data = string.Empty;
            DataTable dt = bll.Sum_InnerMsg_Min(Convert.ToDateTime(dtm), name, 8, 23);
            data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return data;
        }

        public string GetMsgSum_InnerBlack(string id, string pageIndex, string pageSize, string dtm, string name)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_msg(dtm);

            string strSql = @"select   uid_from, 
                                       u1.photo as photo_from, 
                                       u1.nick as nick_from,
                                       uid_to, dbo.is_date_vip(uid_to, '{2}') as isdayvip,
                                       u2.photo as photo_to, 
                                       u2.nick as nick_to,
                                       COUNT(*) as c1
                                from {0} msg
                                left join dbo.[user] u1 on (msg.uid_from = u1.uid)
                                left join dbo.[user] u2 on (msg.uid_to = u2.uid)
                                where uid_from in ({1})
                                and state = 'success'
                                group by uid_from, uid_to,
                                u1.photo,u1.nick, u2.photo,u2.nick
                                having COUNT(*) > 5";



            string uids = bll.GetInnerManUIds(name);

            strSql = string.Format(strSql, table_name, uids, dtm);

            string strSort = "uid_from, uid_to, c1 desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetMsgDtlData(string id, string pageIndex, string pageSize, string dtm)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string table_name = bll.GetTableName_msg(dtm);

            string strSql = @"select msg.id, 
                                       CONVERT(char(20), dtm, 120) as dtm,
                                       txt,
                                       type,
                                       state,
                                       uid_from as uid,
                                       u.photo,
                                       u.nick,
                                       u.sex
                                from {0} msg 
                                left join dbo.[user] u on (msg.uid_from = u.uid) ";

            strSql = string.Format(strSql, table_name);

            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }


        public string CheckWx(string uid, string wx_check)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = "update dbo.[user] set wx_check='{1}' where uid='{0}' ";
            strSql = string.Format(strSql, uid, wx_check);
            if (DBHelper.SqlHelper.ExecuteSql(strSql) > 0)
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="c_type"></param>
        /// <param name="A">发其投诉者</param>
        /// <param name="B">被投诉者</param>
        /// <returns></returns>
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


        public string GetKillUserList(string id, string pageIndex, string pageSize, string key, string check_state)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere = "1 = 1 ";

            if (!string.IsNullOrEmpty(key))
            {
                strWhere += string.Format(" and (nick like '%{0}%') ", key);
            }
            if (!string.IsNullOrEmpty(check_state))
            {
                strWhere += string.Format(" and (state = '{0}') ", check_state);

            }
            if (!string.IsNullOrEmpty(id))
            {
                strWhere += string.Format(" and (k.uid = '{0}') ", id);
            }

            string strSql = @"select k.id, k.uid,
									
								    CONVERT(char(20), k.dtm, 120) as dtm,
                                    CONVERT(char(20), DATEADD(hour, k.kill_hour, k.dtm), 120) as dtm_line,
								    k.kill_hour,
								    k.state,
								    k.comment,
								    k.reason, k.kill_type,
								    
									au.photo,
									au.nick,
									au.sex,
									
									k.create_time
                                    from dbo.kill_user k 
                                    left join dbo.[user] au on (k.uid = au.uid)
                                    where {0} 
                             ";

            strSql = string.Format(strSql, strWhere);

            string strSort = "create_time desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetKillUserList_movie()
        {
            string strRtn = string.Empty;
            string strSql = @"select k.id, k.uid,
									
								    CONVERT(char(20), k.dtm, 120) as dtm,
                                    CONVERT(char(20), DATEADD(hour, k.kill_hour, k.dtm), 120) as dtm_line,
								    k.kill_hour,
								    k.state,
								    isnull(k.comment,'') as comment,
								    k.reason,
								    
									au.photo,
									au.nick,
									au.sex,
									
									k.create_time
                                    from dbo.kill_user k 
                                    left join dbo.[user] au on (k.uid = au.uid)
                              where k.dtm>DATEADD(day,-1,GETDATE()) order by create_time desc";


            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            strRtn = CommonTool.JsonHelper.DataTableToJSON(dt);
            return strRtn;
        }
        public string GetWarnUserList(string id, string pageIndex, string pageSize)
        {
            string strRtn = string.Empty;

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);

            string strWhere = " 1=1 ";


            if (!string.IsNullOrEmpty(id))
            {
                strWhere += string.Format(" and (w.uid = '{0}') ", id);
            }


            string strSql = @"select w.id, w.uid,
									
								   
                                    CONVERT(char(20), w.dtm, 120) as dtm,
								    w.warn_sec,
								    w.state,
								    w.comment,
								    w.reason,
									w.warn_type,
                                    w.scan_count,
								    
									au.photo,
									au.nick,
									au.sex
									
                                    from dbo.warn_user w 
                                    left join dbo.[user] au on (w.uid = au.uid)
                                    where {0} ";

            strSql = string.Format(strSql, strWhere);
            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string GetWxNoticeRecord(string pageIndex, string pageSize, string start, string end, string type, string key)
        {


            string strRtn = string.Empty;
            string where = " 1=1 ";
            if (!string.IsNullOrEmpty(type))
            {
                where += string.Format(" and (w.msg_type='{0}') ", type);
            }
            if (!string.IsNullOrEmpty(start))
            {
                where += string.Format(" and (w.dtm>='{0}') ", start);
            }
            if (!string.IsNullOrEmpty(end))
            {
                where += string.Format(" and (w.dtm<='{0}') ", end);
            }
            if (!string.IsNullOrEmpty(key))
            {
                where += string.Format(" and (w.uid='{0}') ", key);
            }

            int index = Convert.ToInt32(pageIndex);
            int size = Convert.ToInt32(pageSize);



            string strSql = @"select w.id,
                                   CONVERT(char(20), w.dtm, 120) as dtm,
                                   w.uid,
                                   w.openid,
                                   w.msg_type,
                                   w.msg_type_ch,
                                   w.msg_txt,
                                   w.msg_rst,
                                   w.msg_rst_ch,
                                   w.create_time,
                                   isnull( w.send_opr, '1') as send_opr,
                                   isnull( w.send_desc, '正常') as send_desc,
								   

	                               au.photo,
                                   au.nick,
                                   au.sex 
                            from [dbo].[wx_msg] w
                            left join dbo.[user] au on (w.uid = au.uid) where {0}";

            strSql = string.Format(strSql, where);
            string strSort = "dtm desc";

            BLL.Common comm = new BLL.Common();

            strRtn = comm.GetMiniUIData(strSql, strSort, index, size);

            return strRtn;
        }

        public string UnKillUser(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            if (string.IsNullOrEmpty(id))
            {
                info.State = "0";
                info.Msg = "参数错误";
                return info.ToString();
            }

            //解封
            string strSql = "update dbo.kill_user set state = '已解封', comment = '后台管理人员解封'  where id = '{0}'";
            strSql = string.Format(strSql, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                info.State = "1";
                info.Msg = "解封成功";
                //刷新数据接口
                string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
                string url = servUrl_AppData + "/App/Reload_KillUid";
                string tag2 = CommonTool.Common.GetHtmlFromUrl(url);
            }
            else
            {
                info.State = "0";
                info.Msg = "解封失败";
            }


            return info.ToString();
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
        public string UpdateApply_Tx(string id, string state, string reason, string pay_mny)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();

            string strSql = "update dbo.score_tx set state = '{1}', brife = '{2}', pay_mny={3}, pay_date=getdate() where id = '{0}'";
            strSql = string.Format(strSql, id, state, reason, pay_mny);

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
        #endregion

        #region 审核概览
        public string PlatSummary_UserInfo()
        {
            string a = "";
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = "select COUNT(*) from dbo.user_update where check_state = '待审核' and dtm_apply >= dateadd(day, -3, getdate()) ";
            a = DBHelper.SqlHelper.GetDataItemString(strSql);

            info.State = "1";
            info.Msg = a;

            return info.ToString();
        }

        public string PlatSummary_ScoreTx()
        {
            string a = "";
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = "select COUNT(*) from dbo.score_tx where state = '待审核' and apply_date >= dateadd(day, -3, getdate())  ";
            a = DBHelper.SqlHelper.GetDataItemString(strSql);

            info.State = "1";
            info.Msg = a;

            return info.ToString();
        }
        public string PlatSummary_Seek_people()
        {
            string a = "";
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = "select COUNT(*) from dbo.seek_people where create_time > DATEADD(day, -1, getdate()) and  comment is null ";
            a = DBHelper.SqlHelper.GetDataItemString(strSql);

            info.State = "1";
            info.Msg = a;

            return info.ToString();
        }

        public string PlatSummary_GiftTs()
        {
            string a = "";
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = "select COUNT(*) from dbo.tousu_gift where state = '待审核' and dtm >= dateadd(day, -3, getdate()) ";
            a = DBHelper.SqlHelper.GetDataItemString(strSql);

            info.State = "1";
            info.Msg = a;

            return info.ToString();
        }

        public string PlatSummary_FeedBack()
        {
            string a = "";
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = "select COUNT(*) from dbo.tousu where check_state = '待审核' and create_time >= dateadd(day, -3, getdate()) ";
            a = DBHelper.SqlHelper.GetDataItemString(strSql);

            info.State = "1";
            info.Msg = a;

            return info.ToString();
        }

        public string PlatSummary_ALbum()
        {
            string a = "";
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = "select COUNT(*) from dbo.album where check_state = 0 and create_time >= dateadd(day, -3, getdate()) ";
            a = DBHelper.SqlHelper.GetDataItemString(strSql);

            info.State = "1";
            info.Msg = a;

            return info.ToString();
        }

        public string PlatSummary_ComPlain()
        {
            string a = "";
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            string strSql = "select COUNT(*) from dbo.complain where (check_state = '待审核' or check_state is null) and dtm >= dateadd(hour, -1, getdate()) ";
            a = DBHelper.SqlHelper.GetDataItemString(strSql);

            info.State = "1";
            info.Msg = a;

            return info.ToString();
        }
        #endregion

        #region 发送微信模板消息
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
        public ActionResult SendWxMsg_Sbjl(string uid, string s, string dtm)
        {
            ViewData["uid"] = uid;
            ViewData["s"] = s;
            ViewData["dtm"] = dtm;
            return View();
        }
        public ActionResult SendWxMsg_lwts(string uid, string amount, string dtm)
        {
            ViewData["uid"] = uid;
            ViewData["amount"] = amount;
            ViewData["dtm"] = dtm;
            return View();
        }

        public ActionResult SendWxMsg_stjf(string uid, string s, string dtm)
        {
            ViewData["uid"] = uid;
            ViewData["s"] = s;
            ViewData["dtm"] = dtm;
            return View();
        }

        public string DoSendWxMsg(string id, string uid, string tag, string strData)
        {
            string rtn = "";

            Dictionary<string, string> dicData = CommonTool.JsonHelper.GetParms2(strData);

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            string url = servUrl_AppData + "/Common/WxSendMsg/?uid=" + uid + "&tag=" + tag + "&data=" + CommonTool.JsonHelper.ObjectToJSON(dicData);
            rtn = CommonTool.Common.GetHtmlFromUrl(url);

            //此为提现情况
            if (tag == "3")
            {
                //修改状态
                string strSql = "";
                if (rtn == "ok")
                {
                    strSql = @"update [dbo].[score_tx] 
                                    set state_wxsend='成功'
                                    where id='{0}'";
                }
                else
                {
                    strSql = @"update [dbo].[score_tx] 
                                    set state_wxsend='失败'
                                    where id='{0}'";
                }
                strSql = string.Format(strSql, id);
                DBHelper.SqlHelper.ExecuteSql(strSql);
            }


            return rtn;
        }

        public string TT(string id)
        {
            string rtn = "";
            return rtn;
        }

        #endregion 

        #region 聊天审核
        public ActionResult Chat_Check_List()
        {
            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            // 设置默认时间：前3天和当前时间
            DateTime today = DateTime.Now;
            DateTime threeDaysAgo = today.AddDays(-3);
            ViewData["startDate"] = threeDaysAgo.ToString("yyyy-MM-dd HH:mm:ss");
            ViewData["endDate"] = today.ToString("yyyy-MM-dd HH:mm:ss");

            return View();
        }

        public ActionResult GetChatCheckList(string pageIndex, string pageSize, string uidFrom, string uidTo, string startDate, string endDate, string source)
        {
            int index = 1;
            int size = 50;
            if (!string.IsNullOrEmpty(pageIndex))
            {
                index = Convert.ToInt32(pageIndex);
            }
            if (!string.IsNullOrEmpty(pageSize))
            {
                size = Convert.ToInt32(pageSize);
            }

            BLL.ChatCheck chatCheck = new BLL.ChatCheck();
            string data = chatCheck.GetChatCheckList(uidFrom, uidTo, startDate, endDate, source, index, size);

            return Content(data);
        }

        public ActionResult Chat_Check_Add(string id, string uidFrom, string uidTo, string dtm, string coinUsed, string scoreEarned, string sendCount, string receiveCount, string chatDepth)
        {
            ViewData["id"] = id;
            ViewData["uidFrom"] = uidFrom;
            ViewData["uidTo"] = uidTo;
            ViewData["dtm"] = dtm;
            ViewData["coinUsed"] = coinUsed;
            ViewData["scoreEarned"] = scoreEarned;
            ViewData["sendCount"] = sendCount;
            ViewData["receiveCount"] = receiveCount;
            ViewData["chatDepth"] = chatDepth;

            string servUrl_AppData = CommonTool.Common.GetAppSetting("servUrl_AppData");
            ViewData["servUrl_AppData"] = servUrl_AppData;

            return View();
        }

        public ActionResult AddChatCheck(string send)
        {
            string strRtn = "{\"state\":0,\"msg\":\"保存失败\"}";

            try
            {
                if (string.IsNullOrEmpty(send))
                {
                    strRtn = "{\"state\":0,\"msg\":\"参数为空\"}";
                    return Content(strRtn);
                }

                Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);

                BLL.ChatCheck chatCheck = new BLL.ChatCheck();
                string id = chatCheck.CreateChatCheck(dicParm);
                if (!string.IsNullOrEmpty(id))
                {
                    strRtn = "{\"state\":1,\"msg\":\"保存成功\",\"id\":\"" + id + "\"}";
                }
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("AddChatCheck 异常: " + ex.Message + "\n" + ex.StackTrace);
                strRtn = "{\"state\":0,\"msg\":\"系统异常\"}";
            }

            return Content(strRtn);
        }

        public ActionResult UpdateChatCheck(string send)
        {
            string strRtn = "{\"state\":0,\"msg\":\"保存失败\"}";

            try
            {
                if (string.IsNullOrEmpty(send))
                {
                    strRtn = "{\"state\":0,\"msg\":\"参数为空\"}";
                    return Content(strRtn);
                }

                Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);

                string id = dicParm["id"];
                dicParm.Remove("id");

                BLL.ChatCheck chatCheck = new BLL.ChatCheck();
                bool tag = chatCheck.UpdateChatCheck(id, dicParm);
                if (tag)
                {
                    strRtn = "{\"state\":1,\"msg\":\"保存成功\"}";
                }
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("UpdateChatCheck 异常: " + ex.Message + "\n" + ex.StackTrace);
                strRtn = "{\"state\":0,\"msg\":\"系统异常\"}";
            }

            return Content(strRtn);
        }

        public ActionResult GetChatCheckById(string id)
        {
            BLL.ChatCheck chatCheck = new BLL.ChatCheck();
            DataTable dt = chatCheck.GetChatCheckById(id);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return Content(data);
        }

        public ActionResult DeleteChatCheck(string id)
        {
            string strRtn = "{\"state\":0,\"msg\":\"删除失败\"}";

            try
            {
                BLL.ChatCheck chatCheck = new BLL.ChatCheck();
                bool tag = chatCheck.DeleteChatCheck(id);
                if (tag)
                {
                    strRtn = "{\"state\":1,\"msg\":\"删除成功\"}";
                }
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("DeleteChatCheck 异常: " + ex.Message + "\n" + ex.StackTrace);
                strRtn = "{\"state\":0,\"msg\":\"系统异常\"}";
            }

            return Content(strRtn);
        }

        public ActionResult CheckExistingChatCheck(string uidFrom, string uidTo, string dtm)
        {
            string strRtn = "{\"id\":\"\"}";

            try
            {
                BLL.ChatCheck chatCheck = new BLL.ChatCheck();
                string id = chatCheck.CheckExistingChatCheck(uidFrom, uidTo, dtm);
                strRtn = "{\"id\":\"" + id + "\"}";
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("CheckExistingChatCheck 异常: " + ex.Message + "\n" + ex.StackTrace);
                strRtn = "{\"id\":\"\"}";
            }

            return Content(strRtn);
        }

        #endregion
    }
}


