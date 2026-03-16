using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;


namespace BLL
{
    public class User
    {
        #region 平台管理者信息部分

        /// <summary>
        /// 根据用户提供的账号和密码进行登录获取登录人的信息
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public DataTable GetSysUserInfo(string user_name, string password)
        {
            DataTable dt = new DataTable();
            string strSql = string.Empty;
            strSql = @"select [id]
                              ,user_name
                              ,user_pwd
                              ,type
                              ,table_name
                              ,other_id
                              ,comment
                              from dbo.sys_user
                              where[user_name] = '{0}' and [user_pwd] = '{1}'";

            strSql = string.Format(strSql , user_name , password);
            dt = DBHelper.SqlHelper.GetDataTable(strSql);
           
            return dt;
        }

        public Data_Sys_User GetSysUserData(string user_name, string password)
        {
            Data_Sys_User u = new Data_Sys_User();

            if (HttpContext.Current.Session["Data_Sys_User"] != null)
            {
                u = (Data_Sys_User)HttpContext.Current.Session["Data_Sys_User"];
                return u;
            }

            DataTable dt = GetSysUserInfo(user_name, password);
            if (dt != null && dt.Rows.Count > 0)
            {
                u.Login = true;
                u.Id = dt.Rows[0]["id"].ToString();
                u.User_Name = dt.Rows[0]["user_name"].ToString();
                u.User_Pwd = dt.Rows[0]["user_pwd"].ToString();
                u.Type= dt.Rows[0]["type"].ToString();
                u.Comment = dt.Rows[0]["comment"].ToString();

                if (dt.Rows[0]["table_name"] != null)
                {
                    u.Table_Name = dt.Rows[0]["table_name"].ToString();
                }
                if (dt.Rows[0]["other_id"] != null)
                {
                    u.Other_id = dt.Rows[0]["other_id"].ToString();
                }

                HttpContext.Current.Session["Data_Sys_User"] = u;
            }

            return u;
        }


        #region 系统用户表 sys_user
        public string Sys_User_Insert(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[sys_user]";
            model.Type = DBHelper.ExecuteType.Insert;
            foreach (string key in dicParm.Keys)
            {
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

        public bool Sys_User_Update(string id, Dictionary<string, string> dicParm)
        {
            bool bRtn = false;

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[sys_user]";
            model.Type = DBHelper.ExecuteType.Update;
            model.PrimaryKey = "id";
            model.OnlyFlag = id;

            foreach (string key in dicParm.Keys)
            {
                
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }

            int tag = model.Execute();
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        public DataTable Sys_User_List_byId(string id)
        {
            string strSql = @"select user_name,
                               user_pwd,
                               type,
                               ISNULL(comment, '') as name,
                               ISNULL(comment, '') as comment,
                               ISNULL(is_use, '') as is_use
                        from dbo.sys_user  where id = '{0}'";
            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public DataTable Sys_User_List_byCondition(string strWhere)
        {
            string strSql = @"select user_name,
                               user_pwd,
                               type,
                               ISNULL(comment, '') as name,
                               ISNULL(is_use, '') as is_use
                        from dbo.sys_user where {0}";
            strSql = string.Format(strSql, strWhere);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        public int Sys_User_Delete(string strWhere)
        {
            int intRtn = -1;
            string strSql = @"delete from dbo.[sys_user]
                              where {0}";
            strSql = string.Format(strSql, strWhere);
            intRtn = DBHelper.SqlHelper.ExecuteSql(strSql);
            return intRtn;
        }

        #endregion

        /// <summary>
        /// 增加一条登录记录
        /// </summary>
        /// <param name="dicParm"></param>
        /// <returns></returns>
        public string CreateLoginRecord(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id user_id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.[sys_user_record]";
            model.Type = DBHelper.ExecuteType.Insert;

            foreach (string key in dicParm.Keys)
            {
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

    

        #endregion
    }
}
