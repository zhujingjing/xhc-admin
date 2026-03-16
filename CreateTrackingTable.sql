-- 创建用户跟踪记录表
CREATE TABLE dbo.user_tracking (
    id VARCHAR(36) PRIMARY KEY NOT NULL,
    uid VARCHAR(36) NOT NULL,
    tracking_type VARCHAR(50) NOT NULL,  -- 跟踪类型（如：登录、修改信息、操作记录等）
    description NVARCHAR(200) NULL,       -- 跟踪描述
    ip VARCHAR(50) NULL,                  -- 操作IP地址
    operator_uid VARCHAR(36) NOT NULL,    -- 操作人UID
    operator_name NVARCHAR(50) NOT NULL,  -- 操作人姓名
    create_time DATETIME NOT NULL,        -- 创建时间
    FOREIGN KEY (uid) REFERENCES dbo.[user](uid),
    FOREIGN KEY (operator_uid) REFERENCES dbo.system_user(uid)
);

-- 创建索引
CREATE INDEX IX_user_tracking_uid ON dbo.user_tracking(uid);
CREATE INDEX IX_user_tracking_create_time ON dbo.user_tracking(create_time);
CREATE INDEX IX_user_tracking_tracking_type ON dbo.user_tracking(tracking_type);
CREATE INDEX IX_user_tracking_operator_uid ON dbo.user_tracking(operator_uid);
