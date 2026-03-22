# 小火柴任务管理系统 - 数据库表结构设计

## 1. 任务分类表 (TaskCategory)

| 字段名 | 数据类型 | 约束 | 描述 |
| :--- | :--- | :--- | :--- |
| `CategoryID` | `UNIQUEIDENTIFIER` | `PRIMARY KEY DEFAULT NEWID()` | 分类ID |
| `CategoryName` | `NVARCHAR(100)` | `NOT NULL` | 分类名称 |
| `ParentID` | `UNIQUEIDENTIFIER` | `NULL REFERENCES TaskCategory(CategoryID)` | 父分类ID，用于多级分类 |
| `Description` | `NVARCHAR(500)` | `NULL` | 分类描述 |
| `CreateTime` | `DATETIME` | `DEFAULT GETDATE()` | 创建时间 |
| `UpdateTime` | `DATETIME` | `DEFAULT GETDATE()` | 更新时间 |

## 2. 任务模板表 (TaskTemplate)

| 字段名 | 数据类型 | 约束 | 描述 |
| :--- | :--- | :--- | :--- |
| `TemplateID` | `UNIQUEIDENTIFIER` | `PRIMARY KEY DEFAULT NEWID()` | 模板ID |
| `TemplateName` | `NVARCHAR(100)` | `NOT NULL` | 模板名称 |
| `CategoryID` | `UNIQUEIDENTIFIER` | `REFERENCES TaskCategory(CategoryID)` | 分类ID |
| `Description` | `NVARCHAR(500)` | `NULL` | 模板描述 |
| `Priority` | `INT` | `NOT NULL DEFAULT 0` | 优先级（0-低，1-中，2-高） |
| `StandardScore` | `DECIMAL(10,2)` | `NOT NULL DEFAULT 100` | 标准得分 |
| `ExecutionCycle` | `INT` | `NOT NULL DEFAULT 0` | 执行周期（0-手动，1-每天，2-每周，3-每月） |
| `CycleValue` | `NVARCHAR(50)` | `NULL` | 周期值（如每周几，每月几号） |
| `IsActive` | `BIT` | `NOT NULL DEFAULT 1` | 是否激活 |
| `CreateTime` | `DATETIME` | `DEFAULT GETDATE()` | 创建时间 |
| `UpdateTime` | `DATETIME` | `DEFAULT GETDATE()` | 更新时间 |

## 3. 时间段设置表 (Task_Time)

| 字段名 | 数据类型 | 约束 | 描述 |
| :--- | :--- | :--- | :--- |
| `SettingID` | `UNIQUEIDENTIFIER` | `PRIMARY KEY DEFAULT NEWID()` | 设置ID |
| `CategoryID` | `UNIQUEIDENTIFIER` | `REFERENCES TaskCategory(CategoryID)` | 分类ID，可为NULL表示全局设置 |
| `StartTime` | `TIME` | `NOT NULL` | 开始时间 |
| `EndTime` | `TIME` | `NOT NULL` | 结束时间 |
| `IsActive` | `BIT` | `NOT NULL DEFAULT 1` | 是否激活 |
| `CreateTime` | `DATETIME` | `DEFAULT GETDATE()` | 创建时间 |
| `UpdateTime` | `DATETIME` | `DEFAULT GETDATE()` | 更新时间 |

## 4. 任务表 (Task)

| 字段名 | 数据类型 | 约束 | 描述 |
| :--- | :--- | :--- | :--- |
| `TaskID` | `UNIQUEIDENTIFIER` | `PRIMARY KEY DEFAULT NEWID()` | 任务ID |
| `TaskName` | `NVARCHAR(100)` | `NOT NULL` | 任务名称 |
| `TemplateID` | `UNIQUEIDENTIFIER` | `REFERENCES TaskTemplate(TemplateID)` | 模板ID，可为NULL |
| `CategoryID` | `UNIQUEIDENTIFIER` | `REFERENCES TaskCategory(CategoryID)` | 分类ID |
| `Description` | `NVARCHAR(1000)` | `NULL` | 任务描述 |
| `AssignedTo` | `VARCHAR(256)` | `NOT NULL` | 分配给的用户名，关联sys_user表中的user_name |
| `Priority` | `INT` | `NOT NULL DEFAULT 0` | 优先级（0-低，1-中，2-高） |
| `Status` | `INT` | `NOT NULL DEFAULT 0` | 状态（0-待处理，1-处理中，2-已完成，3-已审核） |
| `StandardScore` | `DECIMAL(10,2)` | `NOT NULL DEFAULT 100` | 标准得分 |
| `ActualScore` | `DECIMAL(10,2)` | `NULL` | 实际得分 |
| `StartTime` | `DATETIME` | `DEFAULT GETDATE()` | 开始时间 |
| `EndTime` | `DATETIME` | `NULL` | 结束时间 |
| `Deadline` | `DATETIME` | `NULL` | 截止时间 |
| `Creator` | `VARCHAR(256)` | `NOT NULL` | 创建者用户名，关联sys_user表中的user_name |
| `CreateTime` | `DATETIME` | `DEFAULT GETDATE()` | 创建时间 |
| `UpdateTime` | `DATETIME` | `DEFAULT GETDATE()` | 更新时间 |

## 5. 任务审核表 (TaskAudit)

| 字段名 | 数据类型 | 约束 | 描述 |
| :--- | :--- | :--- | :--- |
| `AuditID` | `UNIQUEIDENTIFIER` | `PRIMARY KEY DEFAULT NEWID()` | 审核ID |
| `TaskID` | `UNIQUEIDENTIFIER` | `REFERENCES Task(TaskID)` | 任务ID |
| `Auditor` | `VARCHAR(256)` | `NOT NULL` | 审核人用户名，关联sys_user表中的user_name |
| `AuditResult` | `BIT` | `NOT NULL` | 审核结果（1-通过，0-拒绝） |
| `AuditOpinion` | `NVARCHAR(500)` | `NULL` | 审核意见 |
| `AuditTime` | `DATETIME` | `DEFAULT GETDATE()` | 审核时间 |

## 6. 任务操作日志表 (TaskOperationLog)

| 字段名 | 数据类型 | 约束 | 描述 |
| :--- | :--- | :--- | :--- |
| `LogID` | `UNIQUEIDENTIFIER` | `PRIMARY KEY DEFAULT NEWID()` | 日志ID |
| `TaskID` | `UNIQUEIDENTIFIER` | `REFERENCES Task(TaskID)` | 任务ID |
| `Operator` | `VARCHAR(256)` | `NOT NULL` | 操作人用户名，关联sys_user表中的user_name |
| `OperationType` | `NVARCHAR(50)` | `NOT NULL` | 操作类型（创建、分配、开始、完成、审核等） |
| `OperationContent` | `NVARCHAR(1000)` | `NULL` | 操作内容 |
| `OperationTime` | `DATETIME` | `DEFAULT GETDATE()` | 操作时间 |

## 7. 任务统计视图 (TaskStatisticsView)

**说明**：这是一个视图，用于统计任务完成情况和绩效计算

| 字段名 | 数据类型 | 描述 |
| :--- | :--- | :--- |
| `UserName` | `VARCHAR(256)` | 用户名，关联sys_user表中的user_name |
| `TotalTasks` | `INT` | 总任务数 |
| `CompletedTasks` | `INT` | 已完成任务数 |
| `AuditedTasks` | `INT` | 已审核任务数 |
| `TotalScore` | `DECIMAL(10,2)` | 总得分 |
| `AverageScore` | `DECIMAL(10,2)` | 平均得分 |
| `CompletionRate` | `DECIMAL(5,2)` | 完成率 |

## 8. 索引设计

1. **Task表索引**：
   - `IX_Task_AssignedTo`：非聚集索引，加速按分配人查询
   - `IX_Task_Status`：非聚集索引，加速按状态查询
   - `IX_Task_Priority`：非聚集索引，加速按优先级查询
   - `IX_Task_Deadline`：非聚集索引，加速按截止时间查询

2. **TaskTemplate表索引**：
   - `IX_TaskTemplate_CategoryID`：非聚集索引，加速按分类查询

3. **TaskCategory表索引**：
   - `IX_TaskCategory_ParentID`：非聚集索引，加速查询子分类

4. **TimeSlotSetting表索引**：
   - `IX_TimeSlotSetting_CategoryID`：非聚集索引，加速按分类查询

## 9. 数据关系图

```
TaskCategory ──┐
              │
              ▼
TaskTemplate ──→ Task ──→ TaskAudit
              │
              └─→ TaskOperationLog
              
TimeSlotSetting
```

## 10. 备注

1. 以上表结构设计基于SQL Server数据库
2. 与现有系统的集成通过sys_user表的user_name字段关联实现
3. 任务自动触发功能通过API接口实现，现有系统可以调用该接口创建任务
4. **TimeSlotSetting表的使用说明**：
   - 用于设置任务自动触发的时间段
   - CategoryID为空时表示全局设置，适用于所有任务类型
   - CategoryID不为空时表示特定分类的时间段设置，优先级高于全局设置
   - 应用层在处理任务触发请求时，会查询该表获取对应分类的时间段设置
   - 如果当前时间在设置的时间段内，则立即生成任务
   - 如果当前时间不在设置的时间段内，则将任务信息存储到临时表，等待后续处理
5. 定期任务的自动生成通过外部桌面程序执行，每天到点自动生成
6. 非工作时间的任务请求会存储到临时表，由外部桌面程序在工作时间开始时处理