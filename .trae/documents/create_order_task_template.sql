-- 创建新订单提醒任务模板
INSERT INTO dbo.TaskTemplate (
    TemplateID,
    TemplateName,
    CategoryID,
    Description,
    Priority,
    StandardScore,
    IsActive,
    AssignedTo,
    DeadlineType,
    DeadlineValue,
    DeadlineUnit,
    ExternalUrl,
    ExternalUrlParams,
    ExternalUrlEnabled,
    CreateTime,
    UpdateTime
) VALUES (
    'ORDER-TEMPLATE-001', -- 模板ID
    '新订单提醒', -- 模板名称
    NULL, -- 分类ID，可根据实际情况设置
    '提醒客服有新的订单产生，需要关注', -- 描述
    2, -- 优先级：2-中等
    10, -- 标准得分
    1, -- 激活状态：1-激活
    NULL, -- 分配给，可根据实际情况设置
    3, -- 截止时间类型：3-当天结束
    NULL, -- 截止时间值
    0, -- 截止时间单位
    NULL, -- 外部URL
    NULL, -- 外部URL参数
    0, -- 外部URL启用状态
    GETDATE(), -- 创建时间
    GETDATE() -- 更新时间
);

-- 查看创建的模板
SELECT * FROM dbo.TaskTemplate WHERE TemplateName = '新订单提醒';