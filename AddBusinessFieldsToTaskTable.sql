-- 添加BusinessType和BusinessId字段到Task表
ALTER TABLE dbo.Task
ADD BusinessType NVARCHAR(100) NULL,
    BusinessId NVARCHAR(200) NULL;

-- 创建索引以提高查询性能
CREATE INDEX IX_Task_BusinessType ON dbo.Task(BusinessType);
CREATE INDEX IX_Task_BusinessId ON dbo.Task(BusinessId);
