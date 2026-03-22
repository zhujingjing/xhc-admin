# 任务自动生成系统技术实现指南

## 1. 系统概述

任务自动生成系统是在现有任务管理系统基础上扩展的功能，用于根据预设的时间规则自动生成任务，支持每天、每周、每月的调度规则，并提供外部API接口用于手动触发任务生成。

## 2. 数据库表结构设计

### 2.1 现有表结构回顾

| 表名 | 作用 |
|------|------|
| TaskCategory | 任务分类（支持多级） |
| TaskTemplate | 任务模板（定义任务的基本属性） |
| Task | 任务实例（存储每个具体任务） |
| Task_Time | 自动触发时间段设置（全局/按分类） |
| TaskAudit | 审核记录 |
| TaskOperationLog | 操作日志 |

### 2.2 新增调度表：TaskTemplateSchedule

此表用于存储每个任务模板的详细执行时间点（支持每天、每周、每月规则）。

```sql
CREATE TABLE dbo.TaskTemplateSchedule (
    ScheduleID NVARCHAR(50) NOT NULL PRIMARY KEY,
    TemplateID NVARCHAR(50) NOT NULL,
    ScheduleType INT NOT NULL, -- 1=每天, 2=每周, 3=每月
    DayOfWeek INT NULL, -- 当ScheduleType=2时有效：1=周一, 2=周二, ..., 7=周日
    DayOfMonth INT NULL, -- 当ScheduleType=3时有效：1~31
    ExecuteTime TIME NOT NULL, -- 具体执行时间，例如 '09:00:00'
    IsActive BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_TaskTemplateSchedule_TaskTemplate FOREIGN KEY (TemplateID) REFERENCES dbo.TaskTemplate(TemplateID)
);

-- 创建索引
CREATE INDEX IX_TaskTemplateSchedule_TemplateID ON dbo.TaskTemplateSchedule(TemplateID);
CREATE INDEX IX_TaskTemplateSchedule_IsActive ON dbo.TaskTemplateSchedule(IsActive);
```

### 2.3 任务模板表扩展字段

在 TaskTemplate 中添加以下字段，用于定义每个模板的截止时间计算规则。

```sql
ALTER TABLE dbo.TaskTemplate
ADD 
    DeadlineType INT NOT NULL DEFAULT 0,       -- 0=无截止, 1=相对生成时间, 2=固定时间点, 3=当天结束, 4=次日开始工作
    DeadlineValue NVARCHAR(50) NULL,            -- 根据类型存储（如数值、时间字符串）
    DeadlineUnit INT NULL;                       -- 当DeadlineType=1时有效：1=分钟, 2=小时, 3=天
```

## 3. 核心功能实现

### 3.1 程序职责

- 每分钟运行一次（已写好框架代码，到时候调用现在的代码即可）
- 查询所有需要在当前时刻生成的任务模板调度记录
- 对每个满足条件的记录，检查当天是否已生成过（防重复）
- 计算截止时间
- 插入任务记录到 Task 表
- 记录日志

### 3.2 核心方法实现

#### 3.2.1 获取当前需要执行的调度记录

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

#### 3.2.2 防重复检查

```csharp
public bool HasGeneratedTaskToday(string templateId)
{
    string today = DateTime.Now.ToString("yyyy-MM-dd");
    string strSql = string.Format(@"SELECT COUNT(*) 
                                 FROM dbo.Task 
                                 WHERE TemplateID = '{0}'
                                 AND CONVERT(VARCHAR(10), CreateTime, 120) = '{1}'", templateId, today);

    string countStr = DBHelper.SqlHelper.GetDataItemString(strSql);
    int count = 0;
    if (!string.IsNullOrEmpty(countStr))
    {
        count = Convert.ToInt32(countStr);
    }
    return count > 0;
}
```

#### 3.2.3 计算截止时间

```csharp
public DateTime? CalculateDeadline(int deadlineType, string deadlineValue, int? deadlineUnit)
{
    DateTime now = DateTime.Now;

    switch (deadlineType)
    {
        case 0: // 无截止
            return null;
        case 1: // 相对生成时间
            if (int.TryParse(deadlineValue, out int value) && deadlineUnit.HasValue)
            {
                switch (deadlineUnit.Value)
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

    return null;
}
```

#### 3.2.4 执行任务自动生成

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
            string assignedTo = row["AssignedTo"].ToString();
            int priority = Convert.ToInt32(row["Priority"]);
            decimal standardScore = Convert.ToDecimal(row["StandardScore"]);
            int deadlineType = Convert.ToInt32(row["DeadlineType"]);
            string deadlineValue = row["DeadlineValue"]?.ToString();
            int? deadlineUnit = row["DeadlineUnit"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["DeadlineUnit"]);

            // 检查当天是否已生成过任务
            if (HasGeneratedTaskToday(templateId))
            {
                continue;
            }

            // 计算截止时间
            DateTime? deadline = CalculateDeadline(deadlineType, deadlineValue, deadlineUnit);

            // 生成任务名称
            string taskName = $"{templateName} - {DateTime.Now.ToString("yyyy-MM-dd")}";

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

## 4. API接口设计

### 4.1 执行任务自动生成

**URL**: GET /Task/ExecuteTaskGeneration

**返回格式**:
```json
{
  "state": "1",
  "msg": "成功",
  "data": "5" // 生成的任务数量
}
```

### 4.2 外部触发任务生成

**URL**: POST /Task/TriggerTaskGeneration

**参数**:
- templateId: 任务模板ID

**返回格式**:
```json
{
  "state": "1",
  "msg": "成功"
}
```

## 5. 部署和使用说明

### 5.1 数据库部署

1. 执行 `CreateTaskScheduleTables.sql` 脚本创建和修改数据库表结构

### 5.2 任务调度配置

1. 在 `TaskTemplateSchedule` 表中添加调度记录
2. 在 `TaskTemplate` 表中设置截止时间配置

### 5.3 定时任务配置

将 `ExecuteTaskGeneration` 方法配置为每分钟执行一次，可使用Windows任务计划程序或其他定时任务工具。

### 5.4 外部系统集成

外部系统可以通过调用 `POST /Task/TriggerTaskGeneration?templateId={templateId}` 接口触发任务生成。

## 6. 测试和验证方案

### 6.1 功能测试

1. **调度规则测试**:
   - 测试每天规则
   - 测试每周规则
   - 测试每月规则

2. **截止时间计算测试**:
   - 测试无截止时间
   - 测试相对生成时间
   - 测试固定时间点
   - 测试当天结束
   - 测试次日开始工作

3. **防重复测试**:
   - 确保每天只生成一次任务

4. **API接口测试**:
   - 测试执行任务自动生成接口
   - 测试外部触发接口

### 6.2 性能测试

- 测试大量调度记录的处理性能
- 测试并发触发的稳定性

## 7. 代码结构

### 7.1 BLL层

- `BLL/Task.cs` - 包含任务自动生成的核心逻辑

### 7.2 Controller层

- `WebAppFrame/Controllers/TaskController.cs` - 包含API接口

### 7.3 数据库脚本

- `CreateTaskScheduleTables.sql` - 数据库表结构脚本

## 8. 注意事项

1. 确保数据库连接正常
2. 确保定时任务正确配置
3. 定期检查任务生成日志
4. 合理设置调度规则，避免任务生成过于频繁

## 9. 扩展建议

1. 可以添加任务生成历史记录，便于追溯
2. 可以添加任务生成失败通知机制
3. 可以添加更复杂的调度规则，如工作日、节假日等
4. 可以添加任务生成模板的复制功能
