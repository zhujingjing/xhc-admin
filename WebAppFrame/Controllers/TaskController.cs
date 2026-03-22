using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using BLL;

namespace WebAppFrame.Controllers
{
    public class TaskController : Controller
    {
        BLL.Task taskBll = new BLL.Task();
        BLL.Common commonBll = new BLL.Common();

        // 任务面板
        public ActionResult Panel()
        {
            return View();
        }

        // 任务管理
        public ActionResult Management()
        {
            return View();
        }

        // 任务模板管理
        public ActionResult Template()
        {
            return View();
        }

        // 任务分类管理
        public ActionResult Category()
        {
            return View();
        }

        // 时间段设置
        public ActionResult Time()
        {
            return View();
        }

        // 任务审核
        public ActionResult Audit()
        {
            return View();
        }

        // 任务审核页面
        public ActionResult AuditTask(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        // 任务统计
        public ActionResult Statistics()
        {
            return View();
        }

        // 任务详情
        public ActionResult Detail(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        // 新增/编辑任务
        public ActionResult ManagementAdd()
        {
            return View();
        }

        // 新增/编辑任务模板
        public ActionResult TemplateAdd()
        {
            return View();
        }

        // 新增/编辑任务分类
        public ActionResult CategoryAdd()
        {
            return View();
        }

        // 新增/编辑时间段设置
        public ActionResult TimeAdd()
        {
            return View();
        }

        // 获取任务分类列表
        public string GetTaskCategoryList(string categoryName)
        {
            DataTable dt = taskBll.GetTaskCategoryList(categoryName);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 根据ID获取任务分类
        public string GetTaskCategoryById(string id)
        {
            DataTable dt = taskBll.GetTaskCategoryById(id);
            if (dt.Rows.Count > 0)
            {
                return CommonTool.JsonHelper.DataTableToJSON(dt);
            }
            else
            {
                CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
                info.State = "0";
                info.Msg = "分类不存在";
                return info.ToString();
            }
        }

        // 保存任务分类
        public string SaveTaskCategory(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            bool tag = taskBll.SaveTaskCategory(dicParm);
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

        // 删除任务分类
        public string DeleteTaskCategory(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            bool tag = taskBll.DeleteTaskCategory(id);
            if (tag)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "该分类已被任务、模板或时间段设置引用，无法删除";
            }
            return info.ToString();
        }

        // 获取任务模板列表
        public string GetTaskTemplateList()
        {
            DataTable dt = taskBll.GetTaskTemplateList();
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 根据ID获取任务模板
        public string GetTaskTemplateById(string id)
        {
            DataTable dt = taskBll.GetTaskTemplateById(id);
            if (dt.Rows.Count > 0)
            {
                return CommonTool.JsonHelper.DataTableToJSON(dt);
            }
            else
            {
                CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
                info.State = "0";
                info.Msg = "模板不存在";
                return info.ToString();
            }
        }

        // 保存任务模板
        public string SaveTaskTemplate(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            bool tag = taskBll.SaveTaskTemplate(dicParm);
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

        // 删除任务模板
        public string DeleteTaskTemplate(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            bool tag = taskBll.DeleteTaskTemplate(id);
            if (tag)
            {
                info.State = "1";
                info.Msg = "成功";
            }
            else
            {
                info.State = "0";
                info.Msg = "该模板已被任务引用，无法删除";
            }
            return info.ToString();
        }

        // 获取时间段设置列表
        public string GetTaskTimeList(string categoryId)
        {
            DataTable dt = taskBll.GetTaskTimeList(categoryId);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 根据ID获取时间段设置
        public string GetTaskTimeById(string id)
        {
            DataTable dt = taskBll.GetTaskTimeById(id);
            if (dt.Rows.Count > 0)
            {
                return CommonTool.JsonHelper.DataTableToJSON(dt);
            }
            else
            {
                CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
                info.State = "0";
                info.Msg = "设置不存在";
                return info.ToString();
            }
        }

        // 保存时间段设置
        public string SaveTaskTime(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            bool tag = taskBll.SaveTaskTime(dicParm);
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

        // 删除时间段设置
        public string DeleteTaskTime(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            bool tag = taskBll.DeleteTaskTime(id);
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

        // 获取任务列表
        public string GetTaskList(string assignedTo, string status, string priority, string categoryId)
        {
            // 权限检查：普通客服只能查看自己的任务
            BLL.Data_Sys_User user = (BLL.Data_Sys_User)Session["Data_Sys_User"];
            if (user != null && user.Type != "平台管理员") {
                assignedTo = user.User_Name;
            }
            
            DataTable dt = taskBll.GetTaskList(assignedTo, status, priority, categoryId);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 根据ID获取任务
        public string GetTaskById(string id)
        {
            DataTable dt = taskBll.GetTaskById(id);
            return CommonTool.JsonHelper.DataTableToJSON(dt);
        }

        // 保存任务
        public string SaveTask(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            
            // 使用前端传入的操作人信息，如果没有则使用系统
            if (!dicParm.ContainsKey("Operator"))
            {
                string operatorName = "系统";
                BLL.Data_Sys_User user = (BLL.Data_Sys_User)Session["Data_Sys_User"];
                if (user != null)
                {
                    operatorName = user.User_Name;
                }
                dicParm.Add("Operator", operatorName);
            }
            
            // 直接保存，不做权限检查
            bool tag = taskBll.SaveTask(dicParm);
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

        // 删除任务
        public string DeleteTask(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            
            // 直接删除，不做权限检查
            bool tag = taskBll.DeleteTask(id);
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

        // 更新任务状态
        public string UpdateTaskStatus(string id, string status, string operatorName = "")
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            
            // 优先使用前端传入的操作人信息
            if (string.IsNullOrEmpty(operatorName))
            {
                operatorName = "系统";
                BLL.Data_Sys_User user = (BLL.Data_Sys_User)Session["Data_Sys_User"];
                if (user != null)
                {
                    operatorName = user.User_Name;
                }
            }
            
            bool tag = taskBll.UpdateTaskStatus(id, Convert.ToInt32(status), operatorName);
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

        // 批量更新任务状态
        public string BatchUpdateTaskStatus(string ids, string status, string operatorName = "")
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            
            // 优先使用前端传入的操作人信息
            if (string.IsNullOrEmpty(operatorName))
            {
                operatorName = "系统";
                BLL.Data_Sys_User user = (BLL.Data_Sys_User)Session["Data_Sys_User"];
                if (user != null)
                {
                    operatorName = user.User_Name;
                }
            }
            
            bool tag = taskBll.BatchUpdateTaskStatus(ids.Split(','), Convert.ToInt32(status), operatorName);
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

        // 保存任务审核
        public string SaveTaskAudit(string taskId, string auditor, string auditResult, string auditOpinion)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            
            // 创建参数字典
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("TaskID", taskId);
            dic.Add("Auditor", auditor);
            dic.Add("AuditResult", auditResult);
            dic.Add("AuditOpinion", auditOpinion);
            
            bool tag = taskBll.SaveTaskAudit(dic);
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

        // 批量审核任务
        public string BatchAuditTasks(string taskIds, string auditor, string auditResult, string auditOpinion)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            bool tag = taskBll.BatchAuditTasks(taskIds.Split(','), auditor, Convert.ToBoolean(auditResult), auditOpinion);
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

        // 获取任务操作日志
        public string GetTaskOperationLog(string taskId)
        {
            DataTable dt = taskBll.GetTaskOperationLog(taskId);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 获取任务统计数据
        public string GetTaskStatistics()
        {
            DataTable dt = taskBll.GetTaskStatistics();
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 获取用户任务统计
        public string GetUserTaskStatistics(string userName)
        {
            DataTable dt = taskBll.GetUserTaskStatistics(userName);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 获取任务审核信息
        public string GetTaskAudit(string taskId)
        {
            DataTable dt = taskBll.GetTaskAudit(taskId);
            return CommonTool.JsonHelper.DataTableToJSON(dt);
        }

        // 执行任务自动生成
        public string ExecuteTaskGeneration()
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            try
            {
                int generatedCount = taskBll.ExecuteTaskGeneration();
                info.State = "1";
                info.Msg = "成功生成【" + generatedCount + "】个任务";
                //info.Data = generatedCount.ToString();
            }
            catch (Exception ex)
            {
                info.State = "0";
                info.Msg = "执行失败: " + ex.Message;
            }
            return info.ToString();
        }

        // 外部触发任务生成
        [HttpPost]
        public string TriggerTaskGeneration(string templateId)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            try
            {
                bool result = taskBll.TriggerTaskGeneration(templateId);
                if (result)
                {
                    info.State = "1";
                    info.Msg = "成功";
                }
                else
                {
                    info.State = "0";
                    info.Msg = "触发失败";
                }
            }
            catch (Exception ex)
            {
                info.State = "0";
                info.Msg = "触发失败: " + ex.Message;
            }
            return info.ToString();
        }

        // 外部系统触发任务生成API
        [HttpPost]
        public string ExternalTaskGeneration()
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            try
            {
                // 读取请求体
                string requestBody;
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }
                
                if (string.IsNullOrEmpty(requestBody))
                {
                    info.State = "0";
                    info.Msg = "请求体不能为空";
                    return info.ToString();
                }
                
                // 解析JSON请求体
                Dictionary<string, string> fullParams = CommonTool.JsonHelper.GetParms2(requestBody);
                
                // 提取业务类型
                string businessType = "";
                if (fullParams.ContainsKey("businessType"))
                {
                    businessType = fullParams["businessType"];
                }
                
                // 提取业务ID
                string businessId = "";
                if (fullParams.ContainsKey("businessId"))
                {
                    businessId = fullParams["businessId"];
                }
                
                // 提取业务参数
                string businessParams = "";
                if (fullParams.ContainsKey("parameters"))
                {
                    businessParams = fullParams["parameters"];
                }
                
                // 打印解析到的参数
                CommonTool.WriteLog.Write(string.Format("ExternalTaskGeneration - businessType: {0}, businessId: {1}, parameters: {2}", businessType, businessId, businessParams));
                
                // 参数验证
                if (string.IsNullOrEmpty(businessType))
                {
                    info.State = "0";
                    info.Msg = "业务类型不能为空";
                    return info.ToString();
                }
                
                if (string.IsNullOrEmpty(businessId))
                {
                    info.State = "0";
                    info.Msg = "业务ID不能为空";
                    return info.ToString();
                }
                
                // 解析业务参数
                Dictionary<string, string> businessParamsDict = null;
                if (!string.IsNullOrEmpty(businessParams))
                {
                    try
                    {
                        businessParamsDict = CommonTool.JsonHelper.GetParms2(businessParams);
                    }
                    catch (Exception ex)
                    {
                        info.State = "0";
                        info.Msg = "业务参数格式错误: " + ex.Message;
                        return info.ToString();
                    }
                }
                
                // 调用BLL方法生成任务
                bool result = taskBll.GenerateExternalTask(businessType, businessId, businessParamsDict);
                if (result)
                {
                    info.State = "1";
                    info.Msg = "任务生成成功";
                }
                else
                {
                    info.State = "0";
                    info.Msg = "任务生成失败";
                }
            }
            catch (Exception ex)
            {
                info.State = "0";
                info.Msg = "任务生成失败: " + ex.Message;
            }
            
            // 直接返回标准格式
            return info.ToString();
        }

        // 调度配置页面
        public ActionResult Schedule()
        {
            return View();
        }

        // 新增/编辑调度页面
        public ActionResult ScheduleAdd()
        {
            return View();
        }

        // 获取调度列表
        public string GetTaskTemplateScheduleList(string templateId)
        {
            DataTable dt = taskBll.GetTaskTemplateScheduleList(templateId);
            string data = CommonTool.JsonHelper.DataTableToJSON(dt);
            return commonBll.GetMiniUIData2(dt.Rows.Count.ToString(), data);
        }

        // 根据ID获取调度
        public string GetTaskTemplateScheduleById(string id)
        {
            DataTable dt = taskBll.GetTaskTemplateScheduleById(id);
            if (dt.Rows.Count > 0)
            {
                return CommonTool.JsonHelper.DataTableToJSON(dt);
            }
            else
            {
                return "";
            }
        }

        // 保存调度配置
        public string SaveTaskTemplateSchedule(string send)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            Dictionary<string, string> dicParm = CommonTool.JsonHelper.GetParms2(send);
            bool tag = taskBll.SaveTaskTemplateSchedule(dicParm);
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

        // 删除调度配置
        public string DeleteTaskTemplateSchedule(string id)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            bool tag = taskBll.DeleteTaskTemplateSchedule(id);
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

        // 更新调度状态
        public string UpdateTaskTemplateScheduleStatus(string id, string isActive)
        {
            CommonTool.ServerRtnInfo info = new CommonTool.ServerRtnInfo();
            bool tag = taskBll.UpdateTaskTemplateScheduleStatus(id, Convert.ToBoolean(isActive));
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
    }
}