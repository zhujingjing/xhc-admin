using System;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class ChatCheck
    {
        /// <summary>
        /// 创建聊天审核记录
        /// </summary>
        /// <param name="dicParm"></param>
        /// <returns></returns>
        public string CreateChatCheck(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.chat_check";
            model.Type = DBHelper.ExecuteType.Insert;

            foreach (string key in dicParm.Keys)
            {
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }
            model.ListFieldItem.Add(new DBHelper.FieldItem("id", id));
            model.ListFieldItem.Add(new DBHelper.FieldItem("create_time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            model.ListFieldItem.Add(new DBHelper.FieldItem("update_time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            int tag = model.Execute();
            if (tag > 0)
            {
                strRtn = id;
            }

            return strRtn;
        }

        /// <summary>
        /// 获取聊天审核记录列表（带分页和条件查询）
        /// </summary>
        /// <param name="uidFrom">发送方用户ID</param>
        /// <param name="uidTo">接收方用户ID</param>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="source">来源</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public string GetChatCheckList(string uidFrom, string uidTo, string startDate, string endDate, string source, int pageIndex, int pageSize)
        {
            string where = "1=1";
            if (!string.IsNullOrEmpty(uidFrom))
            {
                where += string.Format(" and uid_from = '{0}'", uidFrom);
            }
            if (!string.IsNullOrEmpty(uidTo))
            {
                where += string.Format(" and uid_to = '{0}'", uidTo);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                where += string.Format(" and cc.create_time >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                where += string.Format(" and cc.create_time <= '{0}'", endDate);
            }
            if (!string.IsNullOrEmpty(source))
            {
                where += string.Format(" and source = '{0}'", source);
            }

            string strSql = @"select cc.id, 
                                   convert(char(20), cc.dtm_chat, 120) as dtm_chat, 
                                   cc.uid_from, 
                                   cc.uid_to, 
                                   cc.coin_used, 
                                   cc.score_earned, 
                                   cc.send_count, 
                                   cc.receive_count, 
                                   cc.chat_depth, 
                                   cc.evaluation, 
                                   cc.brief, 
                                   cc.details, 
                                   cc.source, 
                                   convert(char(20), cc.create_time, 120) as create_time, 
                                   convert(char(20), cc.update_time, 120) as update_time, 
                                   cc.operator_name,
                                   u1.nick as from_nick,
                                   u1.photo as from_photo,
                                   u1.sex as from_sex,
                                   u1.remark as from_remark,
                                   u2.nick as to_nick,
                                   u2.photo as to_photo,
                                   u2.sex as to_sex,
                                   u2.remark as to_remark
                            from dbo.chat_check cc
                            left join dbo.[user] u1 on cc.uid_from = u1.uid
                            left join dbo.[user] u2 on cc.uid_to = u2.uid
                            where {0}";

            strSql = string.Format(strSql, where);

            Common comm = new Common();
            string strRtn = comm.GetMiniUIData(strSql, "create_time desc", pageIndex, pageSize);

            return strRtn;
        }

        /// <summary>
        /// 根据ID获取聊天审核记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public DataTable GetChatCheckById(string id)
        {
            string strSql = @"select id, 
                                   convert(char(20), dtm_chat, 120) as dtm_chat, 
                                   uid_from, 
                                   uid_to, 
                                   coin_used, 
                                   score_earned, 
                                   send_count, 
                                   receive_count, 
                                   chat_depth, 
                                   evaluation, 
                                   brief, 
                                   details, 
                                   source, 
                                   convert(char(20), create_time, 120) as create_time, 
                                   convert(char(20), update_time, 120) as update_time, 
                                   operator_name
                            from dbo.chat_check 
                            where id = '{0}'";

            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        /// <summary>
        /// 更新聊天审核记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="dicParm">参数</param>
        /// <returns></returns>
        public bool UpdateChatCheck(string id, Dictionary<string, string> dicParm)
        {
            bool bRtn = false;

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.chat_check";
            model.Type = DBHelper.ExecuteType.Update;
            model.PrimaryKey = "id";
            model.OnlyFlag = id;

            foreach (string key in dicParm.Keys)
            {
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }

            model.ListFieldItem.Add(new DBHelper.FieldItem("update_time", DateTime.Now.ToString()));
            int tag = model.Execute();
            if (tag > 0)
            {
                bRtn = true;
            }

            return bRtn;
        }

        /// <summary>
        /// 删除聊天审核记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public bool DeleteChatCheck(string id)
        {
            string strSql = "delete from dbo.chat_check where id = '{0}'";
            strSql = string.Format(strSql, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            return tag > 0;
        }

        /// <summary>
        /// 检查是否已存在当天的聊天评价记录
        /// </summary>
        /// <param name="uidFrom">发送方用户ID</param>
        /// <param name="uidTo">接收方用户ID</param>
        /// <param name="dtm">聊天时间</param>
        /// <returns>记录ID，如果不存在返回空字符串</returns>
        public string CheckExistingChatCheck(string uidFrom, string uidTo, string dtm)
        {
            string strSql = "select id from dbo.chat_check where uid_from = '{0}' and uid_to = '{1}' and convert(char(10), dtm_chat, 120) = '{2}'";
            strSql = string.Format(strSql, uidFrom, uidTo, dtm);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["id"].ToString();
            }
            return string.Empty;
        }
    }
}