-- 测试用例：验证任务日志中的操作人信息

-- 1. 测试任务创建时的操作人信息
-- 场景1：前端传入操作人信息
INSERT INTO dbo.Task (TaskID, TaskName, Description, AssignedTo, Priority, Status, StandardScore, Creator, CreateTime, UpdateTime)
VALUES ('test-task-1', '测试任务1', '测试任务描述', 'user1', 1, 0, 100, 'admin', GETDATE(), GETDATE());

-- 插入操作日志（模拟前端传入操作人信息的情况）
INSERT INTO dbo.TaskOperationLog (LogID, TaskID, Operator, OperationType, OperationContent, OperationTime)
VALUES ('log-1', 'test-task-1', '前端传入的操作人', '创建', '创建任务: 测试任务1', GETDATE());

-- 场景2：前端未传入操作人信息，使用系统默认值
INSERT INTO dbo.Task (TaskID, TaskName, Description, AssignedTo, Priority, Status, StandardScore, Creator, CreateTime, UpdateTime)
VALUES ('test-task-2', '测试任务2', '测试任务描述', 'user2', 1, 0, 100, 'admin', GETDATE(), GETDATE());

-- 插入操作日志（模拟前端未传入操作人信息的情况）
INSERT INTO dbo.TaskOperationLog (LogID, TaskID, Operator, OperationType, OperationContent, OperationTime)
VALUES ('log-2', 'test-task-2', '系统', '创建', '创建任务: 测试任务2', GETDATE());

-- 2. 测试任务状态变更时的操作人信息
-- 场景1：从待处理变更为进行中
UPDATE dbo.Task SET Status = 1, UpdateTime = GETDATE() WHERE TaskID = 'test-task-1';

-- 插入操作日志
INSERT INTO dbo.TaskOperationLog (LogID, TaskID, Operator, OperationType, OperationContent, OperationTime)
VALUES ('log-3', 'test-task-1', 'user1', '状态变更', '任务状态从 【待处理】 变更为 【进行中】 (测试任务1)', GETDATE());

-- 场景2：从进行中变更为已完成
UPDATE dbo.Task SET Status = 2, EndTime = GETDATE(), UpdateTime = GETDATE() WHERE TaskID = 'test-task-1';

-- 插入操作日志
INSERT INTO dbo.TaskOperationLog (LogID, TaskID, Operator, OperationType, OperationContent, OperationTime)
VALUES ('log-4', 'test-task-1', 'user1', '状态变更', '任务状态从 【进行中】 变更为 【已完成】 (测试任务1)', GETDATE());

-- 3. 测试任务审核时的操作人信息
-- 场景1：审核通过
INSERT INTO dbo.TaskAudit (AuditID, TaskID, Auditor, AuditResult, AuditOpinion, AuditTime)
VALUES ('audit-1', 'test-task-1', 'admin', 1, '审核通过', GETDATE());

UPDATE dbo.Task SET Status = 3, ActualScore = 100, UpdateTime = GETDATE() WHERE TaskID = 'test-task-1';

-- 插入操作日志
INSERT INTO dbo.TaskOperationLog (LogID, TaskID, Operator, OperationType, OperationContent, OperationTime)
VALUES ('log-5', 'test-task-1', 'admin', '审核', '任务审核通过 (测试任务1)，意见：审核通过', GETDATE());

-- 场景2：审核拒绝
UPDATE dbo.Task SET Status = 0, UpdateTime = GETDATE() WHERE TaskID = 'test-task-2';

-- 插入操作日志
INSERT INTO dbo.TaskOperationLog (LogID, TaskID, Operator, OperationType, OperationContent, OperationTime)
VALUES ('log-6', 'test-task-2', 'admin', '审核', '任务审核拒绝 (测试任务2)，意见：任务未完成', GETDATE());

-- 4. 验证操作人信息是否正确记录
SELECT t.TaskID, t.TaskName, l.Operator, l.OperationType, l.OperationContent, l.OperationTime
FROM dbo.Task t
JOIN dbo.TaskOperationLog l ON t.TaskID = l.TaskID
ORDER BY t.TaskID, l.OperationTime;

-- 5. 清理测试数据
DELETE FROM dbo.TaskOperationLog WHERE TaskID IN ('test-task-1', 'test-task-2');
DELETE FROM dbo.TaskAudit WHERE TaskID IN ('test-task-1', 'test-task-2');
DELETE FROM dbo.Task WHERE TaskID IN ('test-task-1', 'test-task-2');
