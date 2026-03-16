CREATE TABLE [dbo].[user_record](
    [id] [varchar](32) NOT NULL,
    [uid] [varchar](32) NOT NULL,
    [record_date] [date] NOT NULL,
    [summary] [nvarchar](255) NOT NULL,
    [details] [nvarchar](max) NULL,
    [remark] [nvarchar](max) NULL,
    [evaluation] [nvarchar](255) NULL,
    [processing_result] [nvarchar](255) NULL,
    [source] [nvarchar](255) NULL,
    [create_time] [datetime] NOT NULL,
    [update_time] [datetime] NOT NULL,
    [operator_name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_user_record] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[user_record] ADD  CONSTRAINT [DF_user_record_create_time]  DEFAULT (getdate()) FOR [create_time]
GO

ALTER TABLE [dbo].[user_record] ADD  CONSTRAINT [DF_user_record_update_time]  DEFAULT (getdate()) FOR [update_time]
GO

ALTER TABLE [dbo].[user_record]  WITH CHECK ADD  CONSTRAINT [FK_user_record_user] FOREIGN KEY([uid])
REFERENCES [dbo].[user] ([uid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[user_record] CHECK CONSTRAINT [FK_user_record_user]
GO

-- 创建索引
CREATE INDEX [IX_user_record_uid] ON [dbo].[user_record] ([uid])
GO

CREATE INDEX [IX_user_record_record_date] ON [dbo].[user_record] ([record_date])
GO

-- 添加来源字段（用于已有表）
ALTER TABLE [dbo].[user_record] ADD [source] [nvarchar](255) NULL
GO

-- 添加违规情况字段（用于已有表）
ALTER TABLE [dbo].[user_record] ADD [violation] [nvarchar](max) NULL
GO