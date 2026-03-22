-- 从TaskTemplate表中删除执行周期和周期值字段
ALTER TABLE dbo.TaskTemplate DROP COLUMN ExecutionCycle;
ALTER TABLE dbo.TaskTemplate DROP COLUMN CycleValue;