# 任务管理系统详细文档

## 1. 系统架构

### 1.1 技术栈
- **前端**：ASP.NET MVC + MiniUI
- **后端**：C#
- **数据库**：SQL Server
- **BLL层**：业务逻辑层，处理核心业务逻辑
- **控制器**：处理HTTP请求，调用BLL层方法
- **视图**：展示数据和用户交互界面

### 1.2 核心文件结构
- **BLL/Task.cs**：任务管理核心业务逻辑
- **Controllers/TaskController.cs**：任务管理控制器
- **Views/Task/**：任务管理相关视图

## 2. 核心功能模块

### 2.1 任务分类管理
- **功能**：管理任务分类，支持层级结构
- **核心字段**：
  - CategoryID：分类ID
  - CategoryName：分类名称
  - ParentID：父分类ID
  - Description：描述
- **核心方法**：
  - GetTaskCategoryList：获取分类列表
  - SaveTaskCategory：保存分类
  - DeleteTaskCategory：删除分类

### 2.2 任务模板管理
- **功能**：定义任务模板，作为自动生成任务的基础
- **核心字段**：
  - TemplateID：模板ID
  - TemplateName：模板名称
  - CategoryID：分类ID
  - Priority：优先级
  - StandardScore：标准得分
  - IsActive：是否激活
  - AssignedTo：指派给
  - DeadlineType：截止时间类型
  - DeadlineValue：截止时间值
  - DeadlineUnit：截止时间单位
- **核心方法**：
  - GetTaskTemplateList：获取模板列表
  - SaveTaskTemplate：保存模板
  - DeleteTaskTemplate：删除模板

### 2.3 时间段设置
- **功能**：设置任务生成的时间段
- **核心字段**：
  - SettingID：设置ID
  - CategoryID：分类ID（可为空，表示全局设置）
  - StartTime：开始时间
  - EndTime：结束时间
  - IsActive：是否激活
- **核心方法**：
  - GetTaskTimeList：获取时间段设置列表
  - SaveTaskTime：保存时间段设置
  - IsInTimeSlot：检查当前时间是否在设置的时间段内

### 2.4 任务调度配置
- **功能**：配置任务自动生成的调度规则
- **核心字段**：
  - ScheduleID：调度ID
  - TemplateID：模板ID
  - ScheduleType：调度类型（1-每天，2-每周，3-每月）
  - DayOfWeek：星期几（调度类型为每周时使用）
  - DayOfMonth：每月几号（调度类型为每月时使用）
  - ExecuteTime：执行时间
  - IsActive：是否激活
- **核心方法**：
  - GetTaskTemplateScheduleList：获取调度列表
  - SaveTaskTemplateSchedule：保存调度配置
  - UpdateTaskTemplateScheduleStatus：更新调度状态

## 3. 自动生成任务逻辑

### 3.1 核心流程

1. **获取当前需要执行的调度记录**
   - 调用 `GetCurrentSchedules()` 方法
   - 条件：
     - 调度状态为激活（IsActive=1）
     - 模板状态为激活（IsActive=1）
     - 当前时间等于调度执行时间
     - 调度类型匹配（每天/每周/每月）

2. **检查当天是否已生成任务**
   - 调用 `HasGeneratedTaskToday()` 方法
   - 防止重复生成任务

3. **计算截止时间**
   - 调用 `CalculateDeadline()` 方法
   - 根据模板配置的截止时间类型计算
   - 截止时间类型：
     - 0：无截止
     - 1：相对生成时间（分钟/小时/天）
     - 2：固定时间点
     - 3：当天结束
     - 4：次日开始工作

4. **生成任务**
   - 调用 `GenerateTask()` 方法
   - 生成任务名称：模板名称 + 当前日期
   - 设置任务状态为待处理（0）
   - 记录创建时间和创建者（系统）

5. **保存任务**
   - 调用 `SaveTask()` 方法
   - 记录操作日志

### 3.2 执行入口

- **定时执行**：通过外部定时任务调用 `ExecuteTaskGeneration()` 方法
- **手动触发**：通过 `TriggerTaskGeneration()` 方法手动触发特定模板的任务生成

### 3.3 代码实现细节

#### 3.3.1 获取当前需要执行的调度记录
```csharp
public DataTable GetCurrentSchedules()
{
    DateTime now = DateTime.Now;
    string currentTime = now.ToString("HH:mm:ss");
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
                                 AND CONVERT(VARCHAR(8), s.ExecuteTime, 108) = '{0}'
                                 AND (
                                     (s.ScheduleType = 1) -- 每天
                                     OR (s.ScheduleType = 2 AND s.DayOfWeek = {1})
                                     OR (s.ScheduleType = 3 AND s.DayOfMonth = {2})
                                 )", currentTime, currentDayOfWeek, currentDayOfMonth);

    return DBHelper.SqlHelper.GetDataTable(strSql);
}
```

#### 3.3.2 执行任务自动生成
```csharp
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
            if (GenerateTask(templateId, taskName, categoryId, description, assignedTo, priority, standardScore, deadline))
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
```

## 4. 任务状态管理和审核流程

### 4.1 任务状态
- 0：待处理
- 1：进行中
- 2：已完成
- 3：已审核

### 4.2 审核流程
1. 任务完成后（状态为2），提交审核
2. 审核人审核任务
3. 审核通过：状态变为3，计算实际得分
4. 审核拒绝：状态变为0，需要重新处理

### 4.3 得分计算
- 标准得分：模板中设置的StandardScore
- 实际得分：根据完成时间计算
  - 提前完成：加分（最多加20%）
  - 超时完成：减分（最多减30%）

## 5. 任务操作日志和统计功能

### 5.1 操作日志
- 记录任务的创建、修改、状态变更、审核等操作
- 包含操作人、操作类型、操作内容、操作时间等信息

### 5.2 统计功能
- 任务统计：总任务数、已完成任务数、已审核任务数、总得分、平均得分、完成率
- 用户任务统计：按用户统计任务完成情况

## 6. 数据库表结构

### 6.1 核心表
- **TaskCategory**：任务分类表
- **TaskTemplate**：任务模板表
- **Task**：任务表
- **Task_Time**：时间段设置表
- **TaskTemplateSchedule**：任务调度表
- **TaskAudit**：任务审核表
- **TaskOperationLog**：任务操作日志表

### 6.2 视图
- **TaskStatisticsView**：任务统计视图

## 7. 系统工作流程

### 7.1 自动生成任务流程
1. 管理员配置任务分类
2. 管理员创建任务模板，设置优先级、得分、截止时间等
3. 管理员配置任务调度规则（每天/每周/每月）
4. 系统定时执行任务生成
5. 系统检查当前时间是否符合调度规则
6. 系统检查当天是否已生成任务
7. 系统计算截止时间
8. 系统生成任务并保存
9. 任务指派给指定用户
10. 用户处理任务
11. 用户完成任务
12. 审核人审核任务
13. 系统计算实际得分
14. 系统记录操作日志

### 7.2 手动触发任务流程
1. 管理员选择任务模板
2. 管理员点击触发按钮
3. 系统获取模板信息
4. 系统计算截止时间
5. 系统生成任务并保存
6. 任务指派给指定用户
7. 用户处理任务
8. 用户完成任务
9. 审核人审核任务
10. 系统计算实际得分
11. 系统记录操作日志

## 8. 核心API

### 8.1 控制器API
- **ExecuteTaskGeneration**：执行任务自动生成
- **TriggerTaskGeneration**：手动触发任务生成
- **GetTaskList**：获取任务列表
- **SaveTask**：保存任务
- **UpdateTaskStatus**：更新任务状态
- **SaveTaskAudit**：保存任务审核

### 8.2 BLL方法
- **GetCurrentSchedules**：获取当前需要执行的调度记录
- **HasGeneratedTaskToday**：检查当天是否已生成任务
- **CalculateDeadline**：计算截止时间
- **GenerateTask**：生成任务
- **ExecuteTaskGeneration**：执行任务自动生成
- **TriggerTaskGeneration**：手动触发任务生成

## 9. 系统优化建议

1. **增加任务依赖关系**：支持任务之间的依赖关系，确保任务按顺序执行
2. **优化任务调度**：增加更灵活的调度规则，如工作日调度、特定日期调度
3. **增强任务提醒**：添加任务到期提醒功能
4. **改进统计分析**：增加更详细的任务统计和分析功能
5. **优化数据库性能**：为常用查询添加索引
6. **增加任务批量操作**：支持批量创建、修改、删除任务

## 10. 总结

任务管理系统是一个功能完整的任务管理平台，支持任务的自动生成、状态管理、审核流程和统计分析。其中，自动生成任务功能通过调度配置实现了任务的定时自动创建，大大提高了工作效率。系统的核心优势在于：

1. **灵活的调度配置**：支持每天、每周、每月的调度规则
2. **智能的截止时间计算**：支持多种截止时间类型
3. **完善的状态管理**：从创建到审核的完整流程
4. **详细的操作日志**：记录所有任务操作
5. **全面的统计分析**：提供任务完成情况的统计数据

系统架构清晰，代码结构合理，功能完善，为企业的任务管理提供了有力的支持。