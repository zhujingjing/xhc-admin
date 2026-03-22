-- 创建任务操作日志表
CREATE TABLE [dbo].[TaskOperationLog] (
    [LogID] [varchar](50) NOT NULL PRIMARY KEY,
    [TaskID] [varchar](50) NOT NULL,
    [Operator] [varchar](50) NOT NULL,
    [OperationType] [varchar](20) NOT NULL,
    [OperationContent] [nvarchar](500) NOT NULL,
    [OperationTime] [datetime] NOT NULL,
    CONSTRAINT [FK_TaskOperationLog_Task] FOREIGN KEY ([TaskID]) REFERENCES [dbo].[Task] ([TaskID])
);

-- 创建索引以提高查询性能
CREATE INDEX [IX_TaskOperationLog_TaskID] ON [dbo].[TaskOperationLog] ([TaskID]);
CREATE INDEX [IX_TaskOperationLog_OperationTime] ON [dbo].[TaskOperationLog] ([OperationTime] DESC);

-- 插入测试数据
INSERT INTO [dbo].[TaskOperationLog] ([LogID], [TaskID], [Operator], [OperationType], [OperationContent], [OperationTime])
VALUES 
('1', 'test-task-1', 'admin', '创建', '创建任务: 测试任务1', GETDATE()),
('2', 'test-task-1', 'user1', '状态变更', '任务状态从 ''待处理'' 变更为 ''进行中''', GETDATE()),
('3', 'test-task-1', 'user1', '状态变更', '任务状态从 ''进行中'' 变更为 ''已完成''', GETDATE()),
('4', 'test-task-1', 'admin', '审核', '任务审核通过', GETDATE());