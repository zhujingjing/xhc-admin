-- 为任务表添加新字段
ALTER TABLE Task
ADD 
    -- 结果（多选）
    Result NVARCHAR(500) NULL,
    -- 备注
    Remarks TEXT NULL,
    -- 图片（多张，存储路径，用逗号分隔）
    Images NVARCHAR(MAX) NULL,
    -- 参数（JSON格式）
    Parms NVARCHAR(MAX) NULL;
