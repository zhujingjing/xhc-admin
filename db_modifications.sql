-- 向TaskTemplate表添加外部链接相关字段
ALTER TABLE dbo.TaskTemplate ADD ExternalUrl NVARCHAR(2000) NULL, ExternalUrlParams NVARCHAR(1000) NULL, ExternalUrlEnabled BIT NULL DEFAULT 0

-- 从Task表删除外部链接相关字段，保留FullExternalUrl字段
ALTER TABLE dbo.Task DROP COLUMN ExternalUrl, ExternalUrlParams, ExternalUrlEnabled
