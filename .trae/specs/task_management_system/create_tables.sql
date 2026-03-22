-- 小火柴任务管理系统 - 数据库表创建脚本

-- 创建任务分类表
CREATE TABLE TaskCategory (
    CategoryID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CategoryName NVARCHAR(100) NOT NULL,
    ParentID UNIQUEIDENTIFIER NULL REFERENCES TaskCategory(CategoryID),
    Description NVARCHAR(500) NULL,
    CreateTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME DEFAULT GETDATE()
);

-- 创建任务模板表
CREATE TABLE TaskTemplate (
    TemplateID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TemplateName NVARCHAR(100) NOT NULL,
    CategoryID UNIQUEIDENTIFIER REFERENCES TaskCategory(CategoryID),
    Description NVARCHAR(500) NULL,
    Priority INT NOT NULL DEFAULT 0,
    StandardScore DECIMAL(10,2) NOT NULL DEFAULT 100,
    ExecutionCycle INT NOT NULL DEFAULT 0,
    CycleValue NVARCHAR(50) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME DEFAULT GETDATE()
);

-- 创建时间段设置表
CREATE TABLE Task_Time (
    SettingID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CategoryID UNIQUEIDENTIFIER NULL REFERENCES TaskCategory(CategoryID),
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME DEFAULT GETDATE()
);

-- 创建任务表
CREATE TABLE Task (
    TaskID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskName NVARCHAR(100) NOT NULL,
    TemplateID UNIQUEIDENTIFIER NULL REFERENCES TaskTemplate(TemplateID),
    CategoryID UNIQUEIDENTIFIER REFERENCES TaskCategory(CategoryID),
    Description NVARCHAR(1000) NULL,
    AssignedTo VARCHAR(256) NOT NULL,
    Priority INT NOT NULL DEFAULT 0,
    Status INT NOT NULL DEFAULT 0,
    StandardScore DECIMAL(10,2) NOT NULL DEFAULT 100,
    ActualScore DECIMAL(10,2) NULL,
    StartTime DATETIME DEFAULT GETDATE(),
    EndTime DATETIME NULL,
    Deadline DATETIME NULL,
    Creator VARCHAR(256) NOT NULL,
    CreateTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME DEFAULT GETDATE()
);

-- 创建任务审核表
CREATE TABLE TaskAudit (
    AuditID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskID UNIQUEIDENTIFIER REFERENCES Task(TaskID),
    Auditor VARCHAR(256) NOT NULL,
    AuditResult BIT NOT NULL,
    AuditOpinion NVARCHAR(500) NULL,
    AuditTime DATETIME DEFAULT GETDATE()
);

-- 创建任务操作日志表
CREATE TABLE TaskOperationLog (
    LogID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskID UNIQUEIDENTIFIER REFERENCES Task(TaskID),
    Operator VARCHAR(256) NOT NULL,
    OperationType NVARCHAR(50) NOT NULL,
    OperationContent NVARCHAR(1000) NULL,
    OperationTime DATETIME DEFAULT GETDATE()
);

-- 创建任务统计视图
CREATE VIEW TaskStatisticsView AS
SELECT 
    t.AssignedTo AS UserName,
    COUNT(*) AS TotalTasks,
    SUM(CASE WHEN t.Status >= 2 THEN 1 ELSE 0 END) AS CompletedTasks,
    SUM(CASE WHEN t.Status = 3 THEN 1 ELSE 0 END) AS AuditedTasks,
    ISNULL(SUM(t.ActualScore), 0) AS TotalScore,
    ISNULL(AVG(t.ActualScore), 0) AS AverageScore,
    CASE WHEN COUNT(*) > 0 THEN CAST(SUM(CASE WHEN t.Status >= 2 THEN 1 ELSE 0 END) AS DECIMAL) / COUNT(*) * 100 ELSE 0 END AS CompletionRate
FROM 
    Task t
GROUP BY 
    t.AssignedTo;

-- 创建索引

-- Task表索引
CREATE INDEX IX_Task_AssignedTo ON Task(AssignedTo);
CREATE INDEX IX_Task_Status ON Task(Status);
CREATE INDEX IX_Task_Priority ON Task(Priority);
CREATE INDEX IX_Task_Deadline ON Task(Deadline);

-- TaskTemplate表索引
CREATE INDEX IX_TaskTemplate_CategoryID ON TaskTemplate(CategoryID);

-- TaskCategory表索引
CREATE INDEX IX_TaskCategory_ParentID ON TaskCategory(ParentID);

-- Task_Time表索引
CREATE INDEX IX_Task_Time_CategoryID ON Task_Time(CategoryID);

-- 插入初始数据

-- 插入默认任务分类
INSERT INTO TaskCategory (CategoryName, Description) VALUES
('提现审核', '处理用户提现申请'),
('信息审核', '审核用户昵称、头像等信息'),
('礼物投诉', '处理礼物相关投诉'),
('每日数据', '每日需要收集的数据任务'),
('每周任务', '每周需要完成的任务'),
('每月任务', '每月需要完成的任务'),
('微信客服', '微信平台的客服消息'),
('其他任务', '其他类型的任务');

-- 插入默认时间段设置（全局设置，工作时间8:00-20:00）
INSERT INTO Task_Time (StartTime, EndTime, IsActive) VALUES
('08:00:00', '20:00:00', 1);

-- 插入默认任务模板
INSERT INTO TaskTemplate (TemplateName, CategoryID, Description, Priority, StandardScore, ExecutionCycle) VALUES
('每日提现审核', (SELECT CategoryID FROM TaskCategory WHERE CategoryName = '提现审核'), '每天审核用户提现申请', 1, 100, 1),
('每日信息审核', (SELECT CategoryID FROM TaskCategory WHERE CategoryName = '信息审核'), '每天审核用户昵称、头像等信息', 1, 100, 1),
('每日礼物投诉处理', (SELECT CategoryID FROM TaskCategory WHERE CategoryName = '礼物投诉'), '每天处理礼物相关投诉', 2, 120, 1),
('微信公众号流量数据', (SELECT CategoryID FROM TaskCategory WHERE CategoryName = '每日数据'), '每天截图微信公众号流量数据', 0, 80, 1),
('每周工作总结', (SELECT CategoryID FROM TaskCategory WHERE CategoryName = '每周任务'), '每周提交工作总结', 1, 100, 2),
('每月绩效自评', (SELECT CategoryID FROM TaskCategory WHERE CategoryName = '每月任务'), '每月提交绩效自评', 1, 100, 3);