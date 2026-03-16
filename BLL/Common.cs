using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using External;
using Com.Alipay;

namespace BLL
{
    

    public class Common
    {
        public string CreateOrderNo()
        {
            string strRtn = string.Empty;
            strRtn = CommonTool.Common.CreateOrderNo("fys");
            return strRtn;
        }

        public string UpLoadImage()
        {
            var file = HttpContext.Current.Request.Files[0];
            string strFileName = "/UpLoadFile/" + Guid.NewGuid().ToString() + ".jpg";
            string path = HttpContext.Current.Server.MapPath(strFileName);
            file.SaveAs(path);

            return strFileName;
        }

        /// <summary>
        /// mimiui界面呈现数据格式
        /// </summary>
        /// <param name="strSqlSource"></param>
        /// <param name="strSort"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetMiniUIData(string strSqlSource, string strSort, int index, int size)
        {
            string strRtn = @"{""total"": {0}, ""data"": {1}}";
            string strTotal = "";
            string strData = "";

            //获取数据
            string strSql = @" with 
                               a as
                                    ({0}),
                               b as
                               (select *, 
                                     ROW_NUMBER() over(order by {1}) as xh
                               from a )
                               select * from b where b.xh > {2} and b.xh <= {3}";

            strSql = string.Format(strSql, strSqlSource, strSort, index * size, (index + 1) * size);


            DataTable dt = DBHelper.SqlHelper.GetDataTable(string.Format(strSql));
            strData = CommonTool.JsonHelper.DataTableToJSON(dt);

            //获取总行数
            strSql = " with a as ({0}) select COUNT(*) from a";
            strSql = string.Format(strSql, strSqlSource);
            strTotal = DBHelper.SqlHelper.GetDataItemString(strSql);

            strRtn = strRtn.Replace("{0}", strTotal);
            strRtn = strRtn.Replace("{1}", strData);

            return strRtn;
        }

        public DataTable GetFengYeData(string strSqlSource, string strSort, int index, int size)
        {
            //获取数据
            string strSql = @" with 
                               a as
                                    ({0}),
                               b as
                               (select *, 
                                     ROW_NUMBER() over(order by {1}) as xh
                               from a )
                               select * from b where b.xh > {2} and b.xh <= {3}";

            strSql = string.Format(strSql, strSqlSource, strSort, index * size, (index + 1) * size);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(string.Format(strSql));
            return dt;
        }

        public string GetMiniUIData2(string totalRows, string data)
        {
            string strRtn = "{total: {0}, data: {1}}";
            string strTotal = totalRows;
            string strData = data;

            strRtn = strRtn.Replace("{0}", strTotal);
            strRtn = strRtn.Replace("{1}", strData);

            return strRtn;
        }

        public string ZhifubaoPay(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            //sParaTemp.Add("partner", Config.Partner);
            //sParaTemp.Add("seller_id", Config.Seller_id);
            //sParaTemp.Add("charset", Config.Input_charset.ToLower());
            //sParaTemp.Add("method", "alipay.trade.wap.pay");
            //sParaTemp.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            foreach (string key in dicParm.Keys)
            {
                if (sParaTemp.Keys.Contains(key))
                {
                    sParaTemp[key] = dicParm[key];
                }
                else
                {
                    sParaTemp.Add(key, dicParm[key]);
                }
            }

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");

            strRtn = sHtmlText;

            return strRtn;
        }

        #region 微信相关

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetOpenID()
        {
            string strRtn = string.Empty;
            if (HttpContext.Current.Session["openid"] != null)
            {
                strRtn = HttpContext.Current.Session["openid"].ToString();
            }
            return strRtn;
        }

        #endregion 

        #region 用户级、系统级对象出来

        public void SetSessionObj(SessionObj sessionObj)
        {
            HttpContext.Current.Session["sessionObj"] = sessionObj;
        }

        public SessionObj GetSessionObj()
        {
            SessionObj s = new SessionObj();
            if (HttpContext.Current.Session["sessionObj"] != null)
            {
                s = (SessionObj)HttpContext.Current.Session["sessionObj"];
            }
            return s;
        }

        #endregion 

        #region 系统表操作(sys_xxx)

        //保存
        private string SaveSysDicData(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id user_id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[sys_dic]";
            model.Type = DBHelper.ExecuteType.Insert;

            foreach (string key in dicParm.Keys)
            {
                if (key == "id")
                {
                    id = dicParm[key];
                    continue;
                }

                //if (key == "cur_coin" || key == "coin" || key == "score_get"
                //    || key == "zj_no" || key == "zj_money" || key == "tag")
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

        public bool SaveSysDicData(string strKey, string strVal, string strType)
        {
            bool bRtn = false;

            if (DBHelper.SqlHelper.Exist("dbo.sys_dic", "[key]", strKey))
            {
                bRtn = false;
                return bRtn;
            }

            Dictionary<string, string> dicParm = new Dictionary<string, string>();
            dicParm.Add("[key]", strKey);
            dicParm.Add("value", strVal);
            dicParm.Add("[type]", strType);
            string strid = SaveSysDicData(dicParm);
            if (!string.IsNullOrEmpty(strid))
            {
                bRtn = true;
            }

            return bRtn;
        }

        public bool SaveSysDicData(string strKey, string strVal, string strType, string comment)
        {
            bool bRtn = false;

            if (DBHelper.SqlHelper.Exist("dbo.sys_dic", "[key]", strKey))
            {
                bRtn = false;
                return bRtn;
            }

            Dictionary<string, string> dicParm = new Dictionary<string, string>();
            dicParm.Add("[key]", strKey);
            dicParm.Add("value", strVal);
            dicParm.Add("[type]", strType);
            dicParm.Add("comment", comment);
            string strid = SaveSysDicData(dicParm);
            if (!string.IsNullOrEmpty(strid))
            {
                bRtn = true;
            }

            return bRtn;
        }

        //更新
        public bool UpdateSysDic(string strKey, string strVal)
        {
            bool bRtn = false;

            string strSql = "update dbo.sys_dic set value = '{1}' where [key] = '{0}'";
            strSql = string.Format(strSql, strKey, strVal);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        public bool UpdateSysDic(string id, string strKey, string strVal, string strType, string comment)
        {
            bool bRtn = false;

            string strSql = "update dbo.sys_dic set [key] = '{1}', value = '{2}', [type] = '{3}' , comment = '{4}' where id = '{0}'";
            strSql = string.Format(strSql, id, strKey, strVal, strType, comment);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        //获取一个
        public string GetSysDicByKey(string strKey)
        {
            string strRtn = string.Empty;

            string strSql = "select value from  dbo.sys_dic where [key] = '{0}'";
            strSql = string.Format(strSql, strKey);
            strRtn = DBHelper.SqlHelper.GetDataItemString(strSql);

            return strRtn;
        }

        public DataTable GetSysDicByID(string id)
        {
            DataTable dt = new DataTable();

            string strSql = "select id, [key], value, [type], comment  from  dbo.sys_dic where id = '{0}'";
            strSql = string.Format(strSql, id);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        //获取全部
        public DataTable GetSysDic()
        {
            DataTable dt = new DataTable();

            string strSql = "select id, [key], value, [type], comment from  dbo.sys_dic order by [key] desc";
            dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        //删除
        public bool DeleteSysDic(string strKey)
        {
            bool bRtn = false;

            string strSql = "delete from dbo.sys_dic where [key] = '{0}'";
            strSql = string.Format(strSql, strKey);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        public bool Exist(string strKey)
        {
            return DBHelper.SqlHelper.Exist("dbo.sys_dic", "[key]", strKey);
        }

        #endregion

        #region 下拉框配置操作(dropdown_config)

        //保存下拉框配置
        public bool SaveDropdownConfig(string key, string value, string category, string parentKey, string parentCategory, int sortOrder, int status, string comment)
        {
            bool bRtn = false;

            string id = Guid.NewGuid().ToString();
            string strSql = @"insert into dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
                             values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7}, '{8}', GETDATE(), GETDATE())";
            strSql = string.Format(strSql, id, key, value, category, parentKey, parentCategory, sortOrder, status, comment);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        //更新下拉框配置
        public bool UpdateDropdownConfig(string id, string key, string value, string category, string parentKey, string parentCategory, int sortOrder, int status, string comment)
        {
            bool bRtn = false;

            string strSql = @"update dbo.dropdown_config 
                             set [key] = '{1}', value = '{2}', category = '{3}', parent_key = '{4}', parent_category = '{5}', sort_order = {6}, status = {7}, comment = '{8}', update_time = GETDATE()
                             where id = '{0}'";
            strSql = string.Format(strSql, id, key, value, category, parentKey, parentCategory, sortOrder, status, comment);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        //删除下拉框配置
        public bool DeleteDropdownConfig(string id)
        {
            bool bRtn = false;

            string strSql = "delete from dbo.dropdown_config where id = '{0}'";
            strSql = string.Format(strSql, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            if (tag > 0)
            {
                bRtn = true;
            }
            return bRtn;
        }

        //根据ID获取下拉框配置
        public DataTable GetDropdownConfigById(string id)
        {
            DataTable dt = new DataTable();

            string strSql = "select id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time from dbo.dropdown_config where id = '{0}'";
            strSql = string.Format(strSql, id);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        //根据分类获取下拉框配置
        public DataTable GetDropdownConfigByCategory(string category)
        {
            DataTable dt = new DataTable();

            string strSql = "select [key] as [key], value as value from dbo.dropdown_config where category = '{0}' and status = 1 order by sort_order asc";
            strSql = string.Format(strSql, category);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        //获取所有下拉框配置
        public DataTable GetAllDropdownConfig()
        {
            DataTable dt = new DataTable();

            string strSql = "select id, [key], value, category, sort_order, status, comment, create_time, update_time from dbo.dropdown_config order by category, sort_order asc";
            dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        //获取级联下拉框配置
        public DataTable GetCascadingDropdownConfig(string category, string parentKey, string parentCategory)
        {
            DataTable dt = new DataTable();

            string strSql = "select [key] as [key], value as value from dbo.dropdown_config where category = '{0}' and parent_key = '{1}' and parent_category = '{2}' and status = 1 order by sort_order asc";
            strSql = string.Format(strSql, category, parentKey, parentCategory);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);

            return dt;
        }

        #endregion
    }

    #region 用户级、 系统级对象定义

    /// <summary>
    /// 服务器端用户级对象（该对象的使用规则为：统一赋值，随时取用）
    /// </summary>
    public class SessionObj
    {
        //登录用户
        public string user_id;
        public string user_name;
        public string user_tel;
        public string user_photo;

        //其他信息
        public string other;


    }

    /// <summary>
    /// 服务器端系统级对象（该对象的使用规则为：统一赋值，随时取用）
    /// </summary>
    public class GlobalObj
    {

    }

    #endregion 

    #region 业务对象

    public enum OrderState
    {
        待付款 = 1,
        已取消,
        付款成功,
        付款失败,
        待发货,
        配送中,
        已完成,
        售后,
        售后完成
    }

    public enum RechargeType
    {
        实充 = 1,
        充值奖励,
        注册奖励,
        推荐奖励,
        好友消费奖励,
        累计消费奖励
    }

    #endregion 


    #region 静态变量

    public static class StaticVar 
    {
        static StaticVar()
        {
            
        }
        
    }

    #endregion 

}
