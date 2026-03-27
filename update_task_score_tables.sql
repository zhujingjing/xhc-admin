-- 修改 TaskTemplate 表，添加得分规则相关字段
ALTER TABLE dbo.TaskTemplate
ADD 
    DelayPenaltyRule NVARCHAR(MAX) NULL, -- 超时惩罚规则，JSON格式
    EarlyRewardRule NVARCHAR(MAX) NULL,  -- 提前奖励规则，JSON格式
    MaxDelayPenalty DECIMAL(10,2) NULL,  -- 最大超时惩罚分数
    MaxEarlyReward DECIMAL(10,2) NULL;   -- 最大提前奖励分数

-- 修改 Task 表，添加审核得分和最终得分字段
ALTER TABLE dbo.Task
ADD 
    AuditScore DECIMAL(10,2) NULL,  -- 审核得分
    FinalScore DECIMAL(10,2) NULL;  -- 最终得分

-- 修改 TaskAudit 表，添加审核得分字段
ALTER TABLE dbo.TaskAudit
ADD 
    AuditScore DECIMAL(10,2) NULL;  -- 审核得分

-- 添加索引以提高查询性能
CREATE INDEX IX_Task_AuditScore ON dbo.Task(AuditScore);
CREATE INDEX IX_Task_FinalScore ON dbo.Task(FinalScore);
CREATE INDEX IX_TaskAudit_AuditScore ON dbo.TaskAudit(AuditScore);