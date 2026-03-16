-- 删除主键约束
ALTER TABLE [dbo].[chat_check] DROP CONSTRAINT [PK_chat_check]
GO

-- 修改id字段长度并设置为NOT NULL
ALTER TABLE [dbo].[chat_check] ALTER COLUMN [id] varchar(36) NOT NULL
GO

-- 重新创建主键约束
ALTER TABLE [dbo].[chat_check] ADD CONSTRAINT [PK_chat_check] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO