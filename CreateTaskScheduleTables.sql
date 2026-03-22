-- 创建任务模板调度表
CREATE TABLE dbo.TaskTemplateSchedule (
    ScheduleID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TemplateID UNIQUEIDENTIFIER NOT NULL,
    ScheduleType INT NOT NULL, -- 1=每天, 2=每周, 3=每月
    DayOfWeek INT NULL, -- 当ScheduleType=2时有效：1=周一, 2=周二, ..., 7=周日
    DayOfMonth INT NULL, -- 当ScheduleType=3时有效：1~31
    ExecuteTime TIME NOT NULL, -- 具体执行时间，例如 '09:00:00'
    IsActive BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME DEFAULT GETDATE(),
    UpdateTime DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_TaskTemplateSchedule_TaskTemplate FOREIGN KEY (TemplateID) REFERENCES dbo.TaskTemplate(TemplateID)
);

-- 为TaskTemplate表添加截止时间配置字段
ALTER TABLE dbo.TaskTemplate
ADD 
    DeadlineType INT NOT NULL DEFAULT 0,       -- 0=无截止, 1=相对生成时间, 2=固定时间点, 3=当天结束, 4=次日开始工作
    DeadlineValue NVARCHAR(50) NULL,            -- 根据类型存储（如数值、时间字符串）
    DeadlineUnit INT NULL;                       -- 当DeadlineType=1时有效：1=分钟, 2=小时, 3=天

-- 创建索引
CREATE INDEX IX_TaskTemplateSchedule_TemplateID ON dbo.TaskTemplateSchedule(TemplateID);
CREATE INDEX IX_TaskTemplateSchedule_IsActive ON dbo.TaskTemplateSchedule(IsActive);
