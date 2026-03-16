using System;
using System.Collections.Generic;
using System.Data;

namespace BLL
{
    public class UserRecord
    {
        /// <summary>
        /// 增加一条用户记录
        /// </summary>
        /// <param name="dicParm"></param>
        /// <returns></returns>
        public string CreateUserRecord(Dictionary<string, string> dicParm)
        {
            string strRtn = string.Empty;

            //随机生成的一个新的id
            string id = Guid.NewGuid().ToString();

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.user_record";
            model.Type = DBHelper.ExecuteType.Insert;

            foreach (string key in dicParm.Keys)
            {
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dicParm[key]));
            }

            model.ListFieldItem.Add(new DBHelper.FieldItem("id", id));
            model.ListFieldItem.Add(new DBHelper.FieldItem("create_time", DateTime.Now.ToString()));
            int tag = model.Execute();
            if (tag > 0)
            {
                strRtn = id;
            }

            return strRtn;
        }

        /// <summary>
        /// 获取用户记录列表（带分页和日期范围）
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public DataTable GetUserRecords(string uid, string startDate, string endDate, string source, int pageIndex, int pageSize)
        {
            string where = "1=1";
            if (!string.IsNullOrEmpty(uid))
            {
                where += string.Format(" and uid = '{0}'", uid);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                where += string.Format(" and record_date >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                where += string.Format(" and record_date <= '{0}'", endDate);
            }
            if (!string.IsNullOrEmpty(source))
            {
                where += string.Format(" and source = '{0}'", source);
            }

            int offset = Math.Max(0, (pageIndex - 1) * pageSize);
            string strSql = @"select id, uid, convert(char(10), record_date, 120) as record_date, summary, details, evaluation, processing_result, violation, remark, 
                                   convert(char(20), create_time, 120) as create_time, 
                                   convert(char(20), update_time, 120) as update_time, 
                                   operator_name, source
                            from dbo.user_record 
                            where {0} 
                            order by record_date desc, create_time desc
                            offset {1} rows fetch next {2} rows only";

            strSql = string.Format(strSql, where, offset, pageSize);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        /// <summary>
        /// 获取用户记录总数
        /// </summary>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <returns></returns>
        public int GetUserRecordsCount(string uid, string startDate, string endDate, string source)
        {
            string where = "1=1";
            if (!string.IsNullOrEmpty(uid))
            {
                where += string.Format(" and uid = '{0}'", uid);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                where += string.Format(" and record_date >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                where += string.Format(" and record_date <= '{0}'", endDate);
            }
            if (!string.IsNullOrEmpty(source))
            {
                where += string.Format(" and source = '{0}'", source);
            }

            string strSql = string.Format("select count(*) from dbo.user_record where {0}", where);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            int count = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            return count;
        }

        /// <summary>
        /// 获取用户记录详情
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public DataTable GetUserRecordById(string id)
        {
            string strSql = @"select id, uid, convert(char(10), record_date, 120) as record_date, summary, details, evaluation, processing_result, violation, remark, 
                                   convert(char(20), create_time, 120) as create_time, 
                                   convert(char(20), update_time, 120) as update_time, 
                                   operator_name, source
                            from dbo.user_record 
                            where id = '{0}'";

            strSql = string.Format(strSql, id);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        /// <summary>
        /// 更新用户记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="dicParm">参数</param>
        /// <returns></returns>
        public bool UpdateUserRecord(string id, Dictionary<string, string> dicParm)
        {
            bool bRtn = false;

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.user_record";
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
        /// 删除用户记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns></returns>
        public bool DeleteUserRecord(string id)
        {
            string strSql = "delete from dbo.user_record where id = '{0}'";
            strSql = string.Format(strSql, id);
            int tag = DBHelper.SqlHelper.ExecuteSql(strSql);
            return tag > 0;
        }

        /// <summary>
        /// 获取所有用户记录列表（带分页和日期范围）
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public DataTable GetUserRecordsAll(string uid, string startDate, string endDate, int pageIndex, int pageSize)
        {
            string where = "1=1";
            if (!string.IsNullOrEmpty(uid))
            {
                where += string.Format(" and ur.uid = '{0}'", uid);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                where += string.Format(" and ur.record_date >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                where += string.Format(" and ur.record_date <= '{0}'", endDate);
            }

            int offset = Math.Max(0, (pageIndex - 1) * pageSize);
            string strSql = @"select ur.id, ur.uid, u.photo, u.nick, u.sex, u.remark, 
                                   convert(char(10), ur.record_date, 120) as record_date, 
                                   ur.summary, ur.details, ur.evaluation, ur.processing_result, ur.violation, ur.remark as record_remark, 
                                   ur.operator_name, ur.source,
                                   convert(char(20), ur.create_time, 120) as create_time, 
                                   convert(char(20), ur.update_time, 120) as update_time
                            from dbo.user_record ur
                            left join dbo.[user] u on ur.uid = u.uid
                            where {0} 
                            order by ur.record_date desc, ur.create_time desc
                            offset {1} rows fetch next {2} rows only";

            strSql = string.Format(strSql, where, offset, pageSize);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt;
        }

        /// <summary>
        /// 获取所有用户记录总数
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="startDate">起始日期</param>
        /// <param name="endDate">截止日期</param>
        /// <returns></returns>
        public int GetUserRecordsAllCount(string uid, string startDate, string endDate)
        {
            string where = "1=1";
            if (!string.IsNullOrEmpty(uid))
            {
                where += string.Format(" and ur.uid = '{0}'", uid);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                where += string.Format(" and ur.record_date >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                where += string.Format(" and ur.record_date <= '{0}'", endDate);
            }

            string strSql = string.Format("select count(*) from dbo.user_record ur left join dbo.[user] u on ur.uid = u.uid where {0}", where);
            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            int count = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                count = Convert.ToInt32(dt.Rows[0][0]);
            }
            return count;
        }
    }
}