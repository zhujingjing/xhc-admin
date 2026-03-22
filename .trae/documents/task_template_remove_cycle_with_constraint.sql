-- 先删除ExecutionCycle列的默认约束
ALTER TABLE dbo.TaskTemplate DROP CONSTRAINT DF__TaskTempl__Execu__49E3F248;

-- 再删除执行周期和周期值字段
ALTER TABLE dbo.TaskTemplate DROP COLUMN ExecutionCycle;
ALTER TABLE dbo.TaskTemplate DROP COLUMN CycleValue;