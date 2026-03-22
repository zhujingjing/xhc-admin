-- 在TaskTemplate表中添加截止时间配置字段
ALTER TABLE dbo.TaskTemplate ADD 
    DeadlineType INT NOT NULL DEFAULT 0,       -- 0=无截止, 1=相对生成时间, 2=固定时间点, 3=当天结束, 4=次日开始工作
    DeadlineValue NVARCHAR(50) NULL,            -- 根据类型存储（如数值、时间字符串）
    DeadlineUnit INT NULL;                       -- 当DeadlineType=1时有效：1=分钟, 2=小时, 3=天

-- 添加字段描述
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'截止时间类型：0=无截止, 1=相对生成时间, 2=固定时间点, 3=当天结束, 4=次日开始工作', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'TaskTemplate', 
    @level2type = N'COLUMN', @level2name = N'DeadlineType';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'截止时间值：根据类型存储（如数值、时间字符串）', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'TaskTemplate', 
    @level2type = N'COLUMN', @level2name = N'DeadlineValue';

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'时间单位：当DeadlineType=1时有效：1=分钟, 2=小时, 3=天', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'TaskTemplate', 
    @level2type = N'COLUMN', @level2name = N'DeadlineUnit';