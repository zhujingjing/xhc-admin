using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DBHelper;

namespace BLL
{
    public class Task
    {
        #region 任务分类相关
        /// <summary>
        /// 获取任务分类列表
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public DataTable GetTaskCategoryList(string categoryName)
        {
            string strWhere = "";
            if (!string.IsNullOrEmpty(categoryName))
            {
                strWhere = string.Format(" WHERE CategoryName LIKE '%{0}%'", categoryName);
            }
            
            string strSql = @"SELECT CategoryID
                             , CategoryName
                             , ParentID
                             , Description
                             , CreateTime
                             , UpdateTime 
                             FROM dbo.TaskCategory " + strWhere + @"
                             ORDER BY CreateTime DESC";
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 根据ID获取任务分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetTaskCategoryById(string id)
        {
            string strSql = string.Format(@"SELECT CategoryID
                                          , CategoryName
                                          , ParentID
                                          , Description
                                          , CreateTime
                                          , UpdateTime 
                                          FROM dbo.TaskCategory 
                                          WHERE CategoryID = '{0}'", id);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 保存任务分类
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool SaveTaskCategory(Dictionary<string, string> dic)
        {
            string CategoryID = dic.ContainsKey("CategoryID") ? dic["CategoryID"] : "";

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.TaskCategory";

            if (string.IsNullOrEmpty(CategoryID))
            {
                // 新增
                model.Type = DBHelper.ExecuteType.Insert;
                model.ListFieldItem.Add(new DBHelper.FieldItem("CategoryID", Guid.NewGuid().ToString()));
                model.ListFieldItem.Add(new DBHelper.FieldItem("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            else
            {
                // 修改
                model.Type = DBHelper.ExecuteType.Update;
                model.PrimaryKey = "CategoryID";
                model.OnlyFlag = CategoryID;
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            // 遍历所有参数
            foreach (string key in dic.Keys)
            {
                if (key == "CategoryID")
                {
                    continue; // 主键已单独处理
                }

                // 特殊处理数字类型字段
                if (key == "ParentID" && string.IsNullOrEmpty(dic[key]))
                {
                    // ParentID为空时不添加，保持NULL
                    continue;
                }

                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key]));
            }

            return model.Execute() > 0;
        }

        /// <summary>
        /// 删除任务分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTaskCategory(string id)
        {
            // 先检查是否有任务引用了该分类
            string checkTaskSql = string.Format("SELECT COUNT(*) FROM dbo.Task WHERE CategoryID = '{0}'", id);
            string taskCountStr = DBHelper.SqlHelper.GetDataItemString(checkTaskSql);
            int taskCount = 0;
            if (!string.IsNullOrEmpty(taskCountStr))
            {
                taskCount = Convert.ToInt32(taskCountStr);
            }
            if (taskCount > 0)
            {
                // 有任务引用了该分类，不能删除
                return false;
            }
            
            // 再检查是否有任务模板引用了该分类
            string checkTemplateSql = string.Format("SELECT COUNT(*) FROM dbo.TaskTemplate WHERE CategoryID = '{0}'", id);
            string templateCountStr = DBHelper.SqlHelper.GetDataItemString(checkTemplateSql);
            int templateCount = 0;
            if (!string.IsNullOrEmpty(templateCountStr))
            {
                templateCount = Convert.ToInt32(templateCountStr);
            }
            if (templateCount > 0)
            {
                // 有任务模板引用了该分类，不能删除
                return false;
            }
            
            // 再检查是否有时间段设置引用了该分类
            string checkTimeSql = string.Format("SELECT COUNT(*) FROM dbo.Task_Time WHERE CategoryID = '{0}'", id);
            string timeCountStr = DBHelper.SqlHelper.GetDataItemString(checkTimeSql);
            int timeCount = 0;
            if (!string.IsNullOrEmpty(timeCountStr))
            {
                timeCount = Convert.ToInt32(timeCountStr);
            }
            if (timeCount > 0)
            {
                // 有时间段设置引用了该分类，不能删除
                return false;
            }
            
            // 没有引用，执行删除
            string strSql = string.Format("DELETE FROM dbo.TaskCategory WHERE CategoryID = '{0}'", id);
            return DBHelper.SqlHelper.ExecuteSql(strSql) > 0;
        }
        #endregion

        #region 任务模板相关
        /// <summary>
        /// 获取任务模板列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTaskTemplateList()
        {
            string strSql = @"SELECT t.TemplateID
                             , t.TemplateName
                             , t.CategoryID
                             , c.CategoryName
                             , t.Description
                             , t.Priority
                             , t.StandardScore
                             , t.IsActive
                             , t.AssignedTo
                             , t.DeadlineType
                             , t.DeadlineValue
                             , t.DeadlineUnit
                             , t.ExternalUrl
                             , t.ExternalUrlParams
                             , t.ExternalUrlEnabled
                             , t.CreateTime
                             , t.UpdateTime 
                             FROM dbo.TaskTemplate t 
                             LEFT JOIN dbo.TaskCategory c ON t.CategoryID = c.CategoryID 
                             ORDER BY t.CreateTime DESC";
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 根据ID获取任务模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetTaskTemplateById(string id)
        {
            string strSql = string.Format(@"SELECT TemplateID
                                          , TemplateName
                                          , CategoryID
                                          , Description
                                          , Priority
                                          , StandardScore
                                          , IsActive
                                          , AssignedTo
                                          , DeadlineType
                                          , DeadlineValue
                                          , DeadlineUnit
                                          , ExternalUrl
                                          , ExternalUrlParams
                                          , ExternalUrlEnabled
                                          , CreateTime
                                          , UpdateTime 
                                          FROM dbo.TaskTemplate 
                                          WHERE TemplateID = '{0}'", id);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 保存任务模板
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool SaveTaskTemplate(Dictionary<string, string> dic)
        {
            string TemplateID = dic.ContainsKey("TemplateID") ? dic["TemplateID"] : "";

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.TaskTemplate";

            if (string.IsNullOrEmpty(TemplateID))
            {
                // 新增
                model.Type = DBHelper.ExecuteType.Insert;
                model.ListFieldItem.Add(new DBHelper.FieldItem("TemplateID", Guid.NewGuid().ToString()));
                model.ListFieldItem.Add(new DBHelper.FieldItem("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            else
            {
                // 修改
                model.Type = DBHelper.ExecuteType.Update;
                model.PrimaryKey = "TemplateID";
                model.OnlyFlag = TemplateID;
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            // 遍历所有参数
            foreach (string key in dic.Keys)
            {
                if (key == "TemplateID" || key == "OperatorName")
                {
                    continue; // 主键和不存在的列已单独处理
                }

                // 特殊处理数字类型字段
                if (key == "Priority" || key == "StandardScore" || key == "IsActive" || key == "DeadlineType" || key == "DeadlineUnit" || key == "ExternalUrlEnabled")
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key], CommonTool.JsonValueType.Number));
                }
                else if ((key == "CategoryID" || key == "ExternalUrl" || key == "ExternalUrlParams") && string.IsNullOrEmpty(dic[key]))
                {
                    // 这些字段为空时不添加，保持NULL
                    continue;
                }
                else
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key]));
                }
            }

            return model.Execute() > 0;
        }

        /// <summary>
        /// 删除任务模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTaskTemplate(string id)
        {
            // 先检查是否有任务引用了该模板
            string checkSql = string.Format("SELECT COUNT(*) FROM dbo.Task WHERE TemplateID = '{0}'", id);
            string countStr = DBHelper.SqlHelper.GetDataItemString(checkSql);
            int count = 0;
            if (!string.IsNullOrEmpty(countStr))
            {
                count = Convert.ToInt32(countStr);
            }
            if (count > 0)
            {
                // 有任务引用了该模板，不能删除
                return false;
            }
            
            // 没有任务引用，执行删除
            string strSql = string.Format("DELETE FROM dbo.TaskTemplate WHERE TemplateID = '{0}'", id);
            return DBHelper.SqlHelper.ExecuteSql(strSql) > 0;
        }
        #endregion

        #region 时间段设置相关
        /// <summary>
        /// 获取时间段设置列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public DataTable GetTaskTimeList(string categoryId)
        {
            string strWhere = "";
            if (!string.IsNullOrEmpty(categoryId))
            {
                strWhere = string.Format(" WHERE t.CategoryID = '{0}'", categoryId);
            }
            
            string strSql = @"SELECT t.SettingID
                             , t.CategoryID
                             , c.CategoryName
                             , CONVERT(VARCHAR(8), t.StartTime, 108) AS StartTime
                             , CONVERT(VARCHAR(8), t.EndTime, 108) AS EndTime
                             , t.IsActive
                             , CONVERT(VARCHAR(20), t.CreateTime, 120) AS CreateTime
                             , CONVERT(VARCHAR(20), t.UpdateTime, 120) AS UpdateTime 
                             FROM dbo.Task_Time t 
                             LEFT JOIN dbo.TaskCategory c ON t.CategoryID = c.CategoryID " + strWhere + @"
                             ORDER BY t.CreateTime DESC";
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 根据ID获取时间段设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetTaskTimeById(string id)
        {
            string strSql = string.Format(@"SELECT SettingID
                                          , CategoryID
                                          , CONVERT(VARCHAR(8), StartTime, 108) AS StartTime
                                          , CONVERT(VARCHAR(8), EndTime, 108) AS EndTime
                                          , IsActive
                                          , CONVERT(VARCHAR(20), CreateTime, 120) AS CreateTime
                                          , CONVERT(VARCHAR(20), UpdateTime, 120) AS UpdateTime 
                                          FROM dbo.Task_Time 
                                          WHERE SettingID = '{0}'", id);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 保存时间段设置
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool SaveTaskTime(Dictionary<string, string> dic)
        {
            string SettingID = dic.ContainsKey("SettingID") ? dic["SettingID"] : "";

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.Task_Time";

            if (string.IsNullOrEmpty(SettingID))
            {
                // 新增
                model.Type = DBHelper.ExecuteType.Insert;
                model.ListFieldItem.Add(new DBHelper.FieldItem("SettingID", Guid.NewGuid().ToString()));
                model.ListFieldItem.Add(new DBHelper.FieldItem("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            else
            {
                // 修改
                model.Type = DBHelper.ExecuteType.Update;
                model.PrimaryKey = "SettingID";
                model.OnlyFlag = SettingID;
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            // 遍历所有参数
            foreach (string key in dic.Keys)
            {
                if (key == "SettingID")
                {
                    continue; // 主键和不存在的列已单独处理
                }

                // 特殊处理布尔类型字段
                if (key == "IsActive")
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key], CommonTool.JsonValueType.Number));
                }
                else if (key == "CategoryID" && string.IsNullOrEmpty(dic[key]))
                {
                    // CategoryID为空时不添加，保持NULL
                    continue;
                }
                else
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key]));
                }
            }

            return model.Execute() > 0;
        }

        /// <summary>
        /// 删除时间段设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTaskTime(string id)
        {
            string strSql = string.Format("DELETE FROM dbo.Task_Time WHERE SettingID = '{0}'", id);
            return DBHelper.SqlHelper.ExecuteSql(strSql) > 0;
        }

        /// <summary>
        /// 检查当前时间是否在设置的时间段内
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public bool IsInTimeSlot(string categoryID)
        {
            string strSql = "";

            if (!string.IsNullOrEmpty(categoryID))
            {
                // 先检查特定分类的时间段设置
                strSql = string.Format(@"SELECT TOP 1 * 
                                          FROM dbo.Task_Time 
                                          WHERE (CategoryID = '{0}' OR CategoryID IS NULL) 
                                          AND IsActive = 1 
                                          AND '{1}' BETWEEN StartTime AND EndTime", categoryID, DateTime.Now.TimeOfDay);
            }
            else
            {
                // 检查全局时间段设置
                strSql = string.Format(@"SELECT TOP 1 * 
                                          FROM dbo.Task_Time 
                                          WHERE CategoryID IS NULL 
                                          AND IsActive = 1 
                                          AND '{0}' BETWEEN StartTime AND EndTime", DateTime.Now.TimeOfDay);
            }

            DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
            return dt.Rows.Count > 0;
        }
        #endregion

        #region 任务相关
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="assignedTo"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="categoryId"></param>
        /// <param name="timeFilter"></param>
        /// <returns></returns>
        public DataTable GetTaskList(string assignedTo, string status, string priority, string categoryId, string timeFilter)
        {
            string strWhere = "";

            if (!string.IsNullOrEmpty(assignedTo))
            {
                strWhere += string.Format(" AND t.AssignedTo = '{0}'", assignedTo);
            }

            if (!string.IsNullOrEmpty(status))
            {
                int statusInt = Convert.ToInt32(status);
                strWhere += string.Format(" AND t.Status = {0}", statusInt);
            }

            if (!string.IsNullOrEmpty(priority))
            {
                int priorityInt = Convert.ToInt32(priority);
                strWhere += string.Format(" AND t.Priority = {0}", priorityInt);
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                strWhere += string.Format(" AND t.CategoryID = '{0}'", categoryId);
            }

            if (!string.IsNullOrEmpty(timeFilter))
            {
                strWhere += string.Format(" AND t.Deadline >= '{0}'", timeFilter);
            }

            string strSql = string.Format(@"SELECT t.TaskID
                             , t.TaskName
                             , t.TemplateID
                             , tmpl.TemplateName
                             , t.CategoryID
                             , c.CategoryName
                             , t.Description
                             , t.AssignedTo
                             , t.Priority
                             , t.Status
                             , t.StandardScore
                             , t.ActualScore
                             , t.Result
                             , CONVERT(VARCHAR(20), t.StartTime, 120) AS StartTime
                             , CONVERT(VARCHAR(20), t.EndTime, 120) AS EndTime
                             , CONVERT(VARCHAR(20), t.Deadline, 120) AS Deadline
                             , t.Creator
                             , CONVERT(VARCHAR(20), t.CreateTime, 120) AS CreateTime
                             , CONVERT(VARCHAR(20), t.UpdateTime, 120) AS UpdateTime
                             , t.BusinessType
                             , t.BusinessId
                             , t.FullExternalUrl 
                             FROM dbo.Task t 
                             LEFT JOIN dbo.TaskTemplate tmpl ON t.TemplateID = tmpl.TemplateID 
                             LEFT JOIN dbo.TaskCategory c ON t.CategoryID = c.CategoryID 
                             WHERE 1=1 {0}
                             ORDER BY t.CreateTime DESC", strWhere);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 获取任务列表（兼容旧版）
        /// </summary>
        /// <param name="assignedTo"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public DataTable GetTaskList(string assignedTo, string status, string priority, string categoryId)
        {
            return GetTaskList(assignedTo, status, priority, categoryId, null);
        }

        /// <summary>
        /// 根据ID获取任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetTaskById(string id)
        {
            string strSql = string.Format(@"SELECT t.TaskID
                                          , t.TaskName
                                          , t.TemplateID
                                          , t.CategoryID
                                          , c.CategoryName
                                          , tmpl.TemplateName
                                          , t.Description
                                          , t.AssignedTo
                                          , t.Priority
                                          , t.Status
                                          , t.StandardScore
                                          , t.ActualScore
                                          , CONVERT(VARCHAR(20), t.StartTime, 120) AS StartTime
                                          , CONVERT(VARCHAR(20), t.EndTime, 120) AS EndTime
                                          , CONVERT(VARCHAR(20), t.Deadline, 120) AS Deadline
                                          , t.Creator
                                          , CONVERT(VARCHAR(20), t.CreateTime, 120) AS CreateTime
                                          , CONVERT(VARCHAR(20), t.UpdateTime, 120) AS UpdateTime
                                          , t.Result
                                          , t.Remarks
                                          , t.Images
                                          , t.Parms
                                          , t.BusinessType
                                          , t.BusinessId
                                          , t.FullExternalUrl 
                                          FROM dbo.Task t
                                          LEFT JOIN dbo.TaskCategory c ON t.CategoryID = c.CategoryID
                                          LEFT JOIN dbo.TaskTemplate tmpl ON t.TemplateID = tmpl.TemplateID
                                          WHERE t.TaskID = '{0}'", id);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool SaveTask(Dictionary<string, string> dic)
        {
            string TaskID = dic.ContainsKey("TaskID") ? dic["TaskID"] : "";
            string Operator = dic.ContainsKey("Operator") ? dic["Operator"] : "系统";
            string taskName = dic.ContainsKey("TaskName") ? dic["TaskName"] : "";

            // 1. 保存是否为新任务的状态
            bool isNewTask = string.IsNullOrEmpty(TaskID);
            
            // 2. 判断是否为执行记录修改
            bool isExecutionUpdate = false;
            if (!isNewTask && (dic.ContainsKey("Result") || dic.ContainsKey("Remarks") || dic.ContainsKey("Images")))
            {
                isExecutionUpdate = true;
            }

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.Task";

            if (isNewTask)
            {
                // 新增
                TaskID = Guid.NewGuid().ToString();
                model.Type = DBHelper.ExecuteType.Insert;
                model.ListFieldItem.Add(new DBHelper.FieldItem("TaskID", TaskID));
                model.ListFieldItem.Add(new DBHelper.FieldItem("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            else
            {
                // 修改
                model.Type = DBHelper.ExecuteType.Update;
                model.PrimaryKey = "TaskID";
                model.OnlyFlag = TaskID;
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            // 遍历所有参数
            foreach (string key in dic.Keys)
            {
                if (key == "TaskID" || key == "Operator")
                {
                    continue; // 主键和操作人已单独处理
                }

                // 特殊处理数字类型字段
                if (key == "Priority" || key == "Status" || key == "StandardScore" || key == "ActualScore")
                {
                    if (!string.IsNullOrEmpty(dic[key]))
                    {
                        model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key], CommonTool.JsonValueType.Number));
                    }
                }
                else if ((key == "TemplateID" || key == "CategoryID" || key == "EndTime" || key == "Deadline" || key == "AssignedTo" || key == "FullExternalUrl") && string.IsNullOrEmpty(dic[key]))
                {
                    // 这些字段为空时不添加，保持NULL
                    continue;
                }
                else
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key]));
                }
            }

            bool result = model.Execute() > 0;
            
            // 记录操作日志
            if (result)
            {
                Dictionary<string, string> logDic = new Dictionary<string, string>();
                logDic.Add("TaskID", TaskID);
                logDic.Add("Operator", Operator);
                
                // 3. 根据状态设置操作类型和内容
                if (isNewTask)
                {
                    logDic.Add("OperationType", "创建");
                    logDic.Add("OperationContent", string.IsNullOrEmpty(taskName) ? "创建任务" : "创建任务: " + taskName);
                }
                else if (isExecutionUpdate)
                {
                    logDic.Add("OperationType", "执行记录");
                    logDic.Add("OperationContent", "修改执行记录");
                }
                else
                {
                    logDic.Add("OperationType", "修改");
                    logDic.Add("OperationContent", string.IsNullOrEmpty(taskName) ? "修改任务" : "修改任务: " + taskName);
                }
                
                LogTaskOperation(logDic);
            }

            return result;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTask(string id)
        {
            string strSql = string.Format("DELETE FROM dbo.Task WHERE TaskID = '{0}'", id);
            return DBHelper.SqlHelper.ExecuteSql(strSql) > 0;
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(string id, int status, string operatorName = "系统")
        {
            // 获取任务当前状态
            DataTable dt = GetTaskById(id);
            int oldStatus = 0;
            string taskName = "";
            if (dt.Rows.Count > 0)
            {
                oldStatus = Convert.ToInt32(dt.Rows[0]["Status"]);
                taskName = dt.Rows[0]["TaskName"].ToString();
            }

            string strSql = string.Format("UPDATE dbo.Task SET Status = {0}, UpdateTime = GETDATE()", status);
            if (status == 1 && oldStatus == 0) // 从待处理变为进行中
            {
                strSql += ", StartTime = GETDATE()";
            }
            else if (status == 2) // 已完成
            {
                strSql += ", EndTime = GETDATE()";
            }
            strSql += string.Format(" WHERE TaskID = '{0}'", id);

            bool result = DBHelper.SqlHelper.ExecuteSql(strSql) > 0;
            
            // 记录操作日志
            if (result && oldStatus != status)
            {
                string statusText = GetStatusText(status);
                string oldStatusText = GetStatusText(oldStatus);
                
                Dictionary<string, string> logDic = new Dictionary<string, string>();
                logDic.Add("TaskID", id);
                logDic.Add("Operator", operatorName);
                logDic.Add("OperationType", "状态变更");
                logDic.Add("OperationContent", string.Format("任务状态从 【{0}】 变更为 【{1}】", oldStatusText, statusText));
                if (!string.IsNullOrEmpty(taskName))
                {
                    logDic["OperationContent"] += " (" + taskName + ")";
                }
                
                LogTaskOperation(logDic);
            }

            return result;
        }

        /// <summary>
        /// 获取状态文本
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private string GetStatusText(int status)
        {
            switch (status)
            {
                case 0: return "待处理";
                case 1: return "进行中";
                case 2: return "已完成";
                case 3: return "已审核";
                default: return "未知";
            }
        }

        /// <summary>
        /// 批量更新任务状态
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        public bool BatchUpdateTaskStatus(string[] ids, int status, string operatorName = "系统")
        {
            bool result = true;
            foreach (string id in ids)
            {
                if (!UpdateTaskStatus(id, status, operatorName))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 计算任务得分
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public decimal CalculateTaskScore(string taskId)
        {
            DataTable dt = GetTaskById(taskId);
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            DataRow row = dt.Rows[0];
            decimal standardScore = Convert.ToDecimal(row["StandardScore"]);
            DateTime startTime = Convert.ToDateTime(row["StartTime"]);
            DateTime endTime = row["EndTime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["EndTime"]);
            DateTime deadline = row["Deadline"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["Deadline"]);

            if (endTime == DateTime.MinValue)
            {
                return 0;
            }

            TimeSpan duration = endTime - startTime;
            decimal score = standardScore;

            // 根据完成时间计算得分
            if (deadline != DateTime.MinValue)
            {
                TimeSpan deadlineDuration = deadline - startTime;
                if (duration < deadlineDuration)
                {
                    // 提前完成，加分
                    double earlyRatio = (deadlineDuration.TotalMinutes - duration.TotalMinutes) / deadlineDuration.TotalMinutes;
                    score += standardScore * (decimal)(earlyRatio * 0.2); // 最多加20%
                }
                else if (duration > deadlineDuration)
                {
                    // 超时完成，减分
                    double delayRatio = (duration.TotalMinutes - deadlineDuration.TotalMinutes) / deadlineDuration.TotalMinutes;
                    score -= standardScore * (decimal)(delayRatio * 0.3); // 最多减30%
                    if (score < 0)
                    {
                        score = 0;
                    }
                }
            }

            return Math.Round(score, 2);
        }
        #endregion

        #region 任务审核相关
        /// <summary>
        /// 获取任务审核列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTaskAuditList()
        {
            string strSql = @"SELECT a.AuditID
                             , a.TaskID
                             , t.TaskName
                             , a.Auditor
                             , a.AuditResult
                             , a.AuditOpinion
                             , a.AuditTime 
                             FROM dbo.TaskAudit a 
                             LEFT JOIN dbo.Task t ON a.TaskID = t.TaskID 
                             ORDER BY a.AuditTime DESC";
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 保存任务审核
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool SaveTaskAudit(Dictionary<string, string> dic)
        {
            try
            {
                string taskId = dic.ContainsKey("TaskID") ? dic["TaskID"] : "";
                string auditor = dic.ContainsKey("Auditor") ? dic["Auditor"] : "";
                bool auditResult = dic.ContainsKey("AuditResult") ? Convert.ToBoolean(dic["AuditResult"]) : false;
                string auditOpinion = dic.ContainsKey("AuditOpinion") ? dic["AuditOpinion"] : "";

                // 获取任务信息
                DataTable dt = GetTaskById(taskId);
                string taskName = "";
                if (dt.Rows.Count > 0)
                {
                    taskName = dt.Rows[0]["TaskName"].ToString();
                }

                // 插入审核记录
                DBHelper.DataModal auditModel = new DBHelper.DataModal();
                auditModel.TableName = "dbo.TaskAudit";
                auditModel.Type = DBHelper.ExecuteType.Insert;
                auditModel.ListFieldItem.Add(new DBHelper.FieldItem("AuditID", Guid.NewGuid().ToString()));
                auditModel.ListFieldItem.Add(new DBHelper.FieldItem("TaskID", taskId));
                auditModel.ListFieldItem.Add(new DBHelper.FieldItem("Auditor", auditor));
                auditModel.ListFieldItem.Add(new DBHelper.FieldItem("AuditResult", auditResult ? "1" : "0", CommonTool.JsonValueType.Number));
                auditModel.ListFieldItem.Add(new DBHelper.FieldItem("AuditOpinion", auditOpinion));
                auditModel.ListFieldItem.Add(new DBHelper.FieldItem("AuditTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                auditModel.Execute();

                // 更新任务状态和得分
                int status = auditResult ? 3 : 0; // 审核通过为3，拒绝为0
                decimal score = 0;
                if (auditResult)
                {
                    score = CalculateTaskScore(taskId);
                }

                DBHelper.DataModal taskModel = new DBHelper.DataModal();
                taskModel.TableName = "dbo.Task";
                taskModel.Type = DBHelper.ExecuteType.Update;
                taskModel.PrimaryKey = "TaskID";
                taskModel.OnlyFlag = taskId;
                taskModel.ListFieldItem.Add(new DBHelper.FieldItem("Status", status.ToString(), CommonTool.JsonValueType.Number));
                taskModel.ListFieldItem.Add(new DBHelper.FieldItem("ActualScore", score.ToString(), CommonTool.JsonValueType.Number));
                taskModel.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                taskModel.Execute();

                // 记录操作日志
                Dictionary<string, string> logDic = new Dictionary<string, string>();
                logDic.Add("TaskID", taskId);
                logDic.Add("Operator", auditor);
                logDic.Add("OperationType", "审核");
                logDic.Add("OperationContent", string.Format("任务审核{0}", auditResult ? "通过" : "拒绝"));
                if (!string.IsNullOrEmpty(taskName))
                {
                    logDic["OperationContent"] += " (" + taskName + ")";
                }
                if (!string.IsNullOrEmpty(auditOpinion))
                {
                    logDic["OperationContent"] += "，意见：" + auditOpinion;
                }
                
                LogTaskOperation(logDic);

                return true;
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("SaveTaskAudit error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 批量审核任务
        /// </summary>
        /// <param name="taskIds"></param>
        /// <param name="auditor"></param>
        /// <param name="auditResult"></param>
        /// <param name="auditOpinion"></param>
        /// <returns></returns>
        public bool BatchAuditTasks(string[] taskIds, string auditor, bool auditResult, string auditOpinion)
        {
            bool result = true;
            foreach (string taskId in taskIds)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("TaskID", taskId);
                dic.Add("Auditor", auditor);
                dic.Add("AuditResult", auditResult.ToString());
                dic.Add("AuditOpinion", auditOpinion);
                
                if (!SaveTaskAudit(dic))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据任务ID获取审核信息
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public DataTable GetTaskAudit(string taskId)
        {
            string strSql = string.Format(@"SELECT AuditID
                                          , TaskID
                                          , Auditor
                                          , AuditResult
                                          , AuditOpinion
                                          , AuditTime 
                                          FROM dbo.TaskAudit 
                                          WHERE TaskID = '{0}' 
                                          ORDER BY AuditTime DESC", taskId);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }
        #endregion

        #region 外部URL相关
        /// <summary>
        /// 从任务模板生成外部链接
        /// </summary>
        /// <param name="templateId">任务模板ID</param>
        /// <param name="businessType">业务类型</param>
        /// <param name="businessId">业务ID</param>
        /// <param name="businessParams">业务参数</param>
        /// <returns>生成的完整外部URL</returns>
        public string GenerateExternalUrlFromTemplate(string templateId, string businessType = null, string businessId = null, Dictionary<string, string> businessParams = null)
        {
            try
            {
                // 获取任务模板信息
                DataTable template = GetTaskTemplateById(templateId);
                if (template.Rows.Count == 0)
                {
                    return string.Empty;
                }

                DataRow row = template.Rows[0];
                string externalUrl = row["ExternalUrl"] == DBNull.Value ? string.Empty : row["ExternalUrl"].ToString();
                string externalUrlParams = row["ExternalUrlParams"] == DBNull.Value ? string.Empty : row["ExternalUrlParams"].ToString();
                bool externalUrlEnabled = row["ExternalUrlEnabled"] == DBNull.Value ? false : Convert.ToBoolean(row["ExternalUrlEnabled"]);

                if (!externalUrlEnabled || string.IsNullOrEmpty(externalUrl))
                {
                    return string.Empty;
                }

                // 构建完整的URL
                string fullUrl = externalUrl;

                // 添加参数
                if (!string.IsNullOrEmpty(externalUrlParams))
                {
                    // 替换参数模板中的动态值
                    string paramsString = externalUrlParams;
                    paramsString = paramsString.Replace("{BusinessType}", businessType ?? "");
                    paramsString = paramsString.Replace("{BusinessId}", businessId ?? "");

                    // 替换业务参数
                    if (businessParams != null && businessParams.Count > 0)
                    {
                        foreach (var param in businessParams)
                        {
                            paramsString = paramsString.Replace(string.Format("{{{0}}}", param.Key), param.Value ?? "");
                        }
                    }

                    // 添加参数到URL
                    if (fullUrl.Contains("?"))
                    {
                        fullUrl += "&" + paramsString;
                    }
                    else
                    {
                        fullUrl += "?" + paramsString;
                    }
                }

                return fullUrl;
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("GenerateExternalUrlFromTemplate error: " + ex.Message);
                return string.Empty;
            }
        }
        #endregion

        #region 任务操作日志相关
        /// <summary>
        /// 记录任务操作日志
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool LogTaskOperation(Dictionary<string, string> dic)
        {
            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.TaskOperationLog";
            model.Type = DBHelper.ExecuteType.Insert;
            model.ListFieldItem.Add(new DBHelper.FieldItem("LogID", Guid.NewGuid().ToString()));
            model.ListFieldItem.Add(new DBHelper.FieldItem("OperationTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

            // 遍历所有参数
            foreach (string key in dic.Keys)
            {
                model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key]));
            }

            return model.Execute() > 0;
        }

        /// <summary>
        /// 获取任务操作日志
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public DataTable GetTaskOperationLog(string taskId)
        {
            string strSql = string.Format(@"SELECT LogID
                                          , TaskID
                                          , Operator
                                          , OperationType
                                          , OperationContent
                                          , OperationTime 
                                          FROM dbo.TaskOperationLog 
                                          WHERE TaskID = '{0}' 
                                          ORDER BY OperationTime DESC", taskId);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }
        #endregion

        #region 任务统计相关
        /// <summary>
        /// 获取任务统计数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetTaskStatistics()
        {
            string strSql = @"SELECT UserName
                             , TotalTasks
                             , CompletedTasks
                             , AuditedTasks
                             , TotalScore
                             , AverageScore
                             , CompletionRate 
                             FROM dbo.TaskStatisticsView";
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 获取用户任务统计
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable GetUserTaskStatistics(string userName)
        {
            string strSql = string.Format(@"SELECT UserName
                                          , TotalTasks
                                          , CompletedTasks
                                          , AuditedTasks
                                          , TotalScore
                                          , AverageScore
                                          , CompletionRate 
                                          FROM dbo.TaskStatisticsView 
                                          WHERE UserName = '{0}'", userName);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }
        #endregion

        #region 任务调度相关
        /// <summary>
        /// 获取调度列表
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public DataTable GetTaskTemplateScheduleList(string templateId)
        {
            string strWhere = "";
            if (!string.IsNullOrEmpty(templateId))
            {
                strWhere = string.Format(" WHERE s.TemplateID = '{0}'", templateId);
            }
            
            string strSql = @"SELECT s.ScheduleID
                             , s.TemplateID
                             , t.TemplateName
                             , s.ScheduleType
                             , s.DayOfWeek
                             , s.DayOfMonth
                             , CONVERT(VARCHAR(8), s.ExecuteTime, 108) AS ExecuteTime
                             , s.IsActive
                             , CONVERT(VARCHAR(20), s.CreateTime, 120) AS CreateTime
                             , CONVERT(VARCHAR(20), s.UpdateTime, 120) AS UpdateTime 
                             FROM dbo.TaskTemplateSchedule s 
                             LEFT JOIN dbo.TaskTemplate t ON s.TemplateID = t.TemplateID " + strWhere + @"
                             ORDER BY s.CreateTime DESC";
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 根据ID获取调度
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetTaskTemplateScheduleById(string id)
        {
            string strSql = string.Format(@"SELECT ScheduleID
                                          , TemplateID
                                          , ScheduleType
                                          , DayOfWeek
                                          , DayOfMonth
                                          , CONVERT(VARCHAR(8), ExecuteTime, 108) AS ExecuteTime
                                          , IsActive
                                          , CONVERT(VARCHAR(20), CreateTime, 120) AS CreateTime
                                          , CONVERT(VARCHAR(20), UpdateTime, 120) AS UpdateTime 
                                          FROM dbo.TaskTemplateSchedule 
                                          WHERE ScheduleID = '{0}'", id);
            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 保存调度配置
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool SaveTaskTemplateSchedule(Dictionary<string, string> dic)
        {
            string ScheduleID = dic.ContainsKey("ScheduleID") ? dic["ScheduleID"] : "";

            DBHelper.DataModal model = new DBHelper.DataModal();
            model.TableName = "dbo.TaskTemplateSchedule";

            if (string.IsNullOrEmpty(ScheduleID))
            {
                // 新增
                model.Type = DBHelper.ExecuteType.Insert;
                model.ListFieldItem.Add(new DBHelper.FieldItem("ScheduleID", Guid.NewGuid().ToString()));
                model.ListFieldItem.Add(new DBHelper.FieldItem("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            else
            {
                // 修改
                model.Type = DBHelper.ExecuteType.Update;
                model.PrimaryKey = "ScheduleID";
                model.OnlyFlag = ScheduleID;
                model.ListFieldItem.Add(new DBHelper.FieldItem("UpdateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            // 遍历所有参数
            foreach (string key in dic.Keys)
            {
                if (key == "ScheduleID")
                {
                    continue; // 主键已单独处理
                }

                // 特殊处理数字类型字段
                if (key == "ScheduleType" || key == "DayOfWeek" || key == "DayOfMonth" || key == "IsActive")
                {
                    if (!string.IsNullOrEmpty(dic[key]))
                    {
                        model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key], CommonTool.JsonValueType.Number));
                    }
                    // 对于可选字段，为空时不添加，保持NULL
                }
                else
                {
                    model.ListFieldItem.Add(new DBHelper.FieldItem(key, dic[key]));
                }
            }

            return model.Execute() > 0;
        }

        /// <summary>
        /// 删除调度配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTaskTemplateSchedule(string id)
        {
            string strSql = string.Format("DELETE FROM dbo.TaskTemplateSchedule WHERE ScheduleID = '{0}'", id);
            return DBHelper.SqlHelper.ExecuteSql(strSql) > 0;
        }

        /// <summary>
        /// 更新调度状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public bool UpdateTaskTemplateScheduleStatus(string id, bool isActive)
        {
            string strSql = string.Format("UPDATE dbo.TaskTemplateSchedule SET IsActive = {0}, UpdateTime = GETDATE() WHERE ScheduleID = '{1}'", isActive ? 1 : 0, id);
            return DBHelper.SqlHelper.ExecuteSql(strSql) > 0;
        }
        #endregion

        #region 任务自动生成相关
        /// <summary>
        /// 获取当前需要执行的调度记录
        /// </summary>
        /// <returns></returns>
        public DataTable GetCurrentSchedules()
        {
            DateTime now = DateTime.Now;
            int currentHour = now.Hour;
            int currentMinute = now.Minute;
            int currentDayOfWeek = (int)now.DayOfWeek;
            if (currentDayOfWeek == 0) currentDayOfWeek = 7; // 将周日从0改为7
            int currentDayOfMonth = now.Day;

            string strSql = string.Format(@"SELECT s.ScheduleID
                                         , s.TemplateID
                                         , t.TemplateName
                                         , t.CategoryID
                                         , t.Description
                                         , t.Priority
                                         , t.StandardScore
                                         , t.DeadlineType
                                         , t.DeadlineValue
                                         , t.DeadlineUnit
                                         , t.AssignedTo
                                         , s.ScheduleType
                                         , s.DayOfWeek
                                         , s.DayOfMonth
                                         , s.ExecuteTime
                                         FROM dbo.TaskTemplateSchedule s
                                         INNER JOIN dbo.TaskTemplate t ON s.TemplateID = t.TemplateID
                                         WHERE s.IsActive = 1
                                         AND t.IsActive = 1
                                         AND DATEPART(HOUR, s.ExecuteTime) = {0}
                                         AND DATEPART(MINUTE, s.ExecuteTime) = {1}
                                         AND (
                                             (s.ScheduleType = 1) -- 每天
                                             OR (s.ScheduleType = 2 AND s.DayOfWeek = {2})
                                             OR (s.ScheduleType = 3 AND s.DayOfMonth = {3})
                                         )", currentHour, currentMinute, currentDayOfWeek, currentDayOfMonth);

            return DBHelper.SqlHelper.GetDataTable(strSql);
        }

        /// <summary>
        /// 检查当天是否已生成过任务
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public bool HasGeneratedTaskToday(string templateId)
        {
            DateTime now = DateTime.Now;
            string today = now.ToString("yyyy-MM-dd");
            int currentHour = now.Hour;
            int currentMinute = now.Minute;
            
            // 检查当前小时和分钟是否已生成过任务
            // 这样可以允许同一个模板在不同时间生成任务
            string strSql = string.Format(@"SELECT COUNT(*) 
                                         FROM dbo.Task 
                                         WHERE TemplateID = '{0}'
                                         AND CONVERT(VARCHAR(10), CreateTime, 120) = '{1}'
                                         AND DATEPART(HOUR, CreateTime) = {2}
                                         AND DATEPART(MINUTE, CreateTime) = {3}", 
                                         templateId, today, currentHour, currentMinute);

            string countStr = DBHelper.SqlHelper.GetDataItemString(strSql);
            int count = 0;
            if (!string.IsNullOrEmpty(countStr))
            {
                count = Convert.ToInt32(countStr);
            }
            return count > 0;
        }

        /// <summary>
        /// 计算截止时间
        /// </summary>
        /// <param name="deadlineType"></param>
        /// <param name="deadlineValue"></param>
        /// <param name="deadlineUnit"></param>
        /// <returns></returns>
        public DateTime CalculateDeadline(int deadlineType, string deadlineValue, int deadlineUnit)
        {
            DateTime now = DateTime.Now;

            switch (deadlineType)
            {
                case 0: // 无截止
                    return DateTime.MinValue;
                case 1: // 相对生成时间
                    int value;
                    if (int.TryParse(deadlineValue, out value))
                    {
                        switch (deadlineUnit)
                        {
                            case 1: // 分钟
                                return now.AddMinutes(value);
                            case 2: // 小时
                                return now.AddHours(value);
                            case 3: // 天
                                return now.AddDays(value);
                        }
                    }
                    break;
                case 2: // 固定时间点
                    if (!string.IsNullOrEmpty(deadlineValue))
                    {
                        try
                        {
                            DateTime deadlineTime = DateTime.Parse(deadlineValue);
                            return new DateTime(now.Year, now.Month, now.Day, deadlineTime.Hour, deadlineTime.Minute, 0);
                        }
                        catch { }
                    }
                    break;
                case 3: // 当天结束
                    return new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                case 4: // 次日开始工作
                    DateTime tomorrow = now.AddDays(1);
                    return new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 9, 0, 0); // 假设9点开始工作
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// 生成任务
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="taskName"></param>
        /// <param name="categoryId"></param>
        /// <param name="description"></param>
        /// <param name="assignedTo"></param>
        /// <param name="priority"></param>
        /// <param name="standardScore"></param>
        /// <param name="deadline"></param>
        /// <param name="businessType"></param>
        /// <param name="businessId"></param>
        /// <param name="businessParams"></param>
        /// <returns></returns>
        public bool GenerateTask(string templateId, string taskName, string categoryId, string description, string assignedTo, int priority, decimal standardScore, DateTime deadline, string businessType = null, string businessId = null, Dictionary<string, string> businessParams = null)
        {
            Dictionary<string, string> taskData = new Dictionary<string, string>();
            taskData.Add("TemplateID", templateId);
            taskData.Add("TaskName", taskName);
            taskData.Add("CategoryID", categoryId);
            taskData.Add("Description", description);
            taskData.Add("AssignedTo", assignedTo);
            taskData.Add("Priority", priority.ToString());
            taskData.Add("StandardScore", standardScore.ToString());
            taskData.Add("Status", "0"); // 待处理
            taskData.Add("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            taskData.Add("Creator", "系统");
            taskData.Add("Operator", "系统");

            if (deadline != DateTime.MinValue)
            {
                taskData.Add("Deadline", deadline.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            // 添加业务类型和业务ID
            if (!string.IsNullOrEmpty(businessType))
            {
                taskData.Add("BusinessType", businessType);
            }

            if (!string.IsNullOrEmpty(businessId))
            {
                taskData.Add("BusinessId", businessId);
            }

            // 生成外部链接
            string fullExternalUrl = GenerateExternalUrlFromTemplate(templateId, businessType, businessId, businessParams);
            if (!string.IsNullOrEmpty(fullExternalUrl))
            {
                taskData.Add("FullExternalUrl", fullExternalUrl);
            }

            // 业务参数不需要保存到Parms字段，Parms字段有其他作用

            return SaveTask(taskData);
        }

        /// <summary>
        /// 执行任务自动生成
        /// </summary>
        /// <returns></returns>
        public int ExecuteTaskGeneration()
        {
            int generatedCount = 0;

            try
            {
                // 获取当前需要执行的调度记录
                DataTable schedules = GetCurrentSchedules();

                foreach (DataRow row in schedules.Rows)
                {
                    string templateId = row["TemplateID"].ToString();
                    string templateName = row["TemplateName"].ToString();
                    string categoryId = row["CategoryID"].ToString();
                    string description = row["Description"].ToString();
                    string assignedTo = row["AssignedTo"] == DBNull.Value ? "" : row["AssignedTo"].ToString();
                    int priority = Convert.ToInt32(row["Priority"]);
                    decimal standardScore = Convert.ToDecimal(row["StandardScore"]);
                    int deadlineType = Convert.ToInt32(row["DeadlineType"]);
                    string deadlineValue = row["DeadlineValue"] == DBNull.Value ? null : row["DeadlineValue"].ToString();
                    int deadlineUnit = row["DeadlineUnit"] == DBNull.Value ? 0 : Convert.ToInt32(row["DeadlineUnit"]);

                    // 检查当天是否已生成过任务
                    if (HasGeneratedTaskToday(templateId))
                    {
                        continue; // 当天已生成过任务，跳过
                    }

                    // 计算截止时间
                    DateTime deadline = CalculateDeadline(deadlineType, deadlineValue, deadlineUnit);

                    // 生成任务名称
                    string taskName = string.Format("{0} - {1}", templateName, DateTime.Now.ToString("yyyy-MM-dd"));

                    // 生成任务
                if (GenerateTask(templateId, taskName, categoryId, description, assignedTo, priority, standardScore, deadline, null, null, null))
                {
                    generatedCount++;
                }
                }
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("ExecuteTaskGeneration error: " + ex.Message);
            }

            return generatedCount;
        }

        /// <summary>
        /// 外部触发任务生成
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public bool TriggerTaskGeneration(string templateId)
        {
            try
            {
                // 获取模板信息
                DataTable template = GetTaskTemplateById(templateId);
                if (template.Rows.Count == 0)
                {
                    return false;
                }

                DataRow row = template.Rows[0];
                string templateName = row["TemplateName"].ToString();
                string categoryId = row["CategoryID"].ToString();
                string description = row["Description"].ToString();
                string assignedTo = row["AssignedTo"] == DBNull.Value ? "" : row["AssignedTo"].ToString();
                int priority = Convert.ToInt32(row["Priority"]);
                decimal standardScore = Convert.ToDecimal(row["StandardScore"]);
                int deadlineType = 0;
                string deadlineValue = null;
                int deadlineUnit = 0;

                // 检查是否有截止时间配置
                if (row.Table.Columns.Contains("DeadlineType"))
                {
                    deadlineType = Convert.ToInt32(row["DeadlineType"]);
                    deadlineValue = row["DeadlineValue"] == DBNull.Value ? null : row["DeadlineValue"].ToString();
                    if (row.Table.Columns.Contains("DeadlineUnit"))
                    {
                        deadlineUnit = row["DeadlineUnit"] == DBNull.Value ? 0 : Convert.ToInt32(row["DeadlineUnit"]);
                    }
                }

                // 计算截止时间
                DateTime deadline = CalculateDeadline(deadlineType, deadlineValue, deadlineUnit);

                // 生成任务名称
                string taskName = string.Format("{0} - 手动触发 - {1}", templateName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                // 生成任务
                return GenerateTask(templateId, taskName, categoryId, description, assignedTo, priority, standardScore, deadline, null, null, null);
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("TriggerTaskGeneration error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 外部系统触发任务生成
        /// </summary>
        /// <param name="businessType">业务类型</param>
        /// <param name="businessId">业务ID</param>
        /// <param name="businessParams">业务参数</param>
        /// <returns></returns>
        public bool GenerateExternalTask(string businessType, string businessId, Dictionary<string, string> businessParams)
        {
            try
            {
                // 根据业务类型获取任务模板
                // 这里需要根据实际业务需求实现业务类型到模板的映射
                // 暂时使用一个简单的映射逻辑，实际项目中可能需要从配置或数据库中获取
                string templateId = GetTemplateIdByBusinessType(businessType);
                if (string.IsNullOrEmpty(templateId))
                {
                    return false;
                }

                // 获取模板信息
                DataTable template = GetTaskTemplateById(templateId);
                if (template.Rows.Count == 0)
                {
                    return false;
                }

                DataRow row = template.Rows[0];
                string templateName = row["TemplateName"].ToString();
                string categoryId = row["CategoryID"].ToString();
                string description = row["Description"].ToString();
                string assignedTo = row["AssignedTo"] == DBNull.Value ? "" : row["AssignedTo"].ToString();
                int priority = Convert.ToInt32(row["Priority"]);
                decimal standardScore = Convert.ToDecimal(row["StandardScore"]);
                int deadlineType = 0;
                string deadlineValue = null;
                int deadlineUnit = 0;

                // 检查是否有截止时间配置
                if (row.Table.Columns.Contains("DeadlineType"))
                {
                    deadlineType = Convert.ToInt32(row["DeadlineType"]);
                    deadlineValue = row["DeadlineValue"] == DBNull.Value ? null : row["DeadlineValue"].ToString();
                    if (row.Table.Columns.Contains("DeadlineUnit"))
                    {
                        deadlineUnit = row["DeadlineUnit"] == DBNull.Value ? 0 : Convert.ToInt32(row["DeadlineUnit"]);
                    }
                }

                // 计算截止时间
                DateTime deadline = CalculateDeadline(deadlineType, deadlineValue, deadlineUnit);

                // 获取用户昵称（用户备注）
                string userNickname = "";
                if (businessParams != null && businessParams.ContainsKey("uid"))
                {
                    string uid = businessParams["uid"];
                    userNickname = GetUserNickname(uid);
                }

                // 拼装其他信息
                string otherInfo = GetOtherInfo(businessType, businessId, businessParams);

                // 生成任务名称，格式：{用户昵称（用户备注）}-{其他信息}
                string taskName = string.Format("{0}-{1}", userNickname, otherInfo);

                // 生成任务描述，包含业务参数
                string taskDescription = description;
                if (businessParams != null && businessParams.Count > 0)
                {
                    taskDescription += "\n\n业务参数：";
                    foreach (var param in businessParams)
                    {
                        taskDescription += string.Format("\n{0}: {1}", param.Key, param.Value);
                    }
                }

                // 生成任务
                bool result = GenerateTask(templateId, taskName, categoryId, taskDescription, assignedTo, priority, standardScore, deadline, businessType, businessId, businessParams);

                if (result)
                {
                    // 记录外部系统触发的任务生成日志
                    CommonTool.WriteLog.Write(string.Format("External task generated: BusinessType={0}, BusinessId={1}", businessType, businessId));
                }

                return result;
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("GenerateExternalTask error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 根据用户ID获取用户昵称（用户备注）
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        private string GetUserNickname(string uid)
        {
            try
            {
                string strSql = string.Format("SELECT ISNULL(nick, '') as nick, ISNULL(remark, '') as remark FROM dbo.[user] WHERE uid = '{0}'", uid);
                DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    string nick = dt.Rows[0]["nick"].ToString();
                    string remark = dt.Rows[0]["remark"].ToString();
                    if (!string.IsNullOrEmpty(nick))
                    {
                        if (!string.IsNullOrEmpty(remark))
                        {
                            return string.Format("{0}({1})", nick, remark);
                        }
                        else
                        {
                            return nick;
                        }
                    }
                }
                return uid;
            }
            catch (Exception ex)
            {
                CommonTool.WriteLog.Write("GetUserNickname error: " + ex.Message);
                return uid;
            }
        }

        /// <summary>
        /// 拼装其他信息
        /// </summary>
        /// <param name="businessType">业务类型</param>
        /// <param name="businessId">业务ID</param>
        /// <param name="businessParams">业务参数</param>
        /// <returns></returns>
        private string GetOtherInfo(string businessType, string businessId, Dictionary<string, string> businessParams)
        {
            string otherInfo = businessId;
            if (businessParams != null)
            {
                // 根据业务类型和参数拼装其他信息
                switch (businessType)
                {
                    case "提现":
                        if (businessParams.ContainsKey("tx_mny"))
                        {
                            otherInfo = string.Format("提现【{0}元】", businessParams["tx_mny"]);
                        }
                        break;
                    case "用户反馈":
                        if (businessParams.ContainsKey("content"))
                        {
                            otherInfo = string.Format("用户反馈【{0}】", businessParams["content"]);
                        }
                        break;
                    case "聊天举报":
                        if (businessParams.ContainsKey("complaintType"))
                        {
                            otherInfo = string.Format("聊天举报【{0}】", businessParams["complaintType"]);
                        }
                        break;
                    case "礼物投诉":
                        if (businessParams.ContainsKey("reason"))
                        {
                            otherInfo = string.Format("发起礼物投诉【{0}】", businessParams["reason"]);
                        }
                        break;
                    case "头像昵称审核":
                        otherInfo = "修改了头像昵称";
                        break;
                    case "寻人区审核":
                        if (businessParams.ContainsKey("title"))
                        {
                            otherInfo = string.Format("发布寻人区【{0}】", businessParams["title"]);
                        }
                        break;
                    case "相册审核":
                        otherInfo = "上传了照片";
                        break;
                    case "消息预警":
                        //string level = businessParams.ContainsKey("level") ? businessParams["level"] : "";
                        //string msgs = businessParams.ContainsKey("msgs") ? businessParams["msgs"] : "";
                        //string words = businessParams.ContainsKey("words") ? businessParams["words"] : "";
                        //string msg_dtm = businessParams.ContainsKey("msg_dtm") ? businessParams["msg_dtm"] : "";
                        //otherInfo = string.Format("{0}-{1}-{2}-{3}", level, msgs, words, msg_dtm);
                        otherInfo = businessParams.ContainsKey("msgs") ? businessParams["msgs"] : "";
                        break;
                }
            }
            return otherInfo;
        }

        /// <summary>
        /// 根据业务类型获取任务模板ID
        /// </summary>
        /// <param name="businessType">业务类型</param>
        /// <returns></returns>
        private string GetTemplateIdByBusinessType(string businessType)
        {
            // 这里需要根据实际业务需求实现业务类型到模板的映射
            // 暂时使用一个简单的硬编码映射，实际项目中可能需要从配置或数据库中获取
            // 示例映射：
            switch (businessType)
            {
                case "提现":
                    return "91DF5A09-0D33-4740-9935-CCAB5FDFFAA9"; // 实际项目中应该替换为真实的模板ID
                case "相册审核":
                    return "82A1B9E7-7841-44FA-BBC2-E5BAD56D9E0F"; // 实际项目中应该替换为真实的模板ID
                case "头像昵称审核":
                    return "9181F262-4106-4C8A-BD22-D013ABDEDA45"; // 实际项目中应该替换为真实的模板ID
                case "用户反馈":
                    return "A9E5A1BC-A146-49CF-AD0F-FCD47FABC1F4"; // 实际项目中应该替换为真实的模板ID
                case "聊天举报":
                    return "60BFCEB3-AE48-4ECB-82C6-7ECAD7AFFBBF"; // 实际项目中应该替换为真实的模板ID
                case "礼物投诉":
                    return "50DFB518-8C6A-4E58-914E-54581F437A95"; // 实际项目中应该替换为真实的模板ID
                case "寻人区审核":
                    return "4E7DF9C8-615F-42ED-B662-BBF09FDE08D6"; // 实际项目中应该替换为真实的模板ID
                case "消息预警":
                    return "DF92A5E9-052D-4EFD-9060-019EB90F1596"; // 实际项目中应该替换为真实的模板ID
                default:
                    return null;
            }
        }
        #endregion
    }
}