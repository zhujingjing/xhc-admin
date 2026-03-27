-- 测试新订单检测功能

-- 1. 首先检查任务模板是否存在
SELECT * FROM dbo.TaskTemplate WHERE TemplateName = '新订单提醒';

-- 2. 插入测试订单数据
INSERT INTO dbo.sys_order (
    order_id,
    uid,
    order_state,
    pay_mny,
    create_time
) VALUES (
    'TEST-ORDER-' + CONVERT(VARCHAR(36), NEWID()), -- 订单ID
    'test-user-001', -- 用户ID
    '付款成功', -- 订单状态
    100.00, -- 支付金额
    GETDATE() -- 创建时间
);

-- 3. 查看插入的订单
SELECT * FROM dbo.sys_order WHERE order_state = '付款成功' ORDER BY create_time DESC;

-- 4. 执行任务自动生成
-- 注意：这里需要通过应用程序调用 ExecuteTaskGeneration() 方法
-- 或者通过HTTP请求调用 /Task/ExecuteTaskGeneration 接口

-- 5. 查看生成的任务
SELECT * FROM dbo.Task WHERE BusinessType = '订单' ORDER BY CreateTime DESC;

-- 6. 验证任务信息是否完整
SELECT t.TaskID, t.TaskName, t.Description, t.BusinessType, t.BusinessId, t.CreateTime 
FROM dbo.Task t 
WHERE t.BusinessType = '订单' 
ORDER BY t.CreateTime DESC;