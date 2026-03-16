CREATE TABLE [dbo].[chat_check](
    [id] [varchar](32) NOT NULL,
    [dtm_chat] [datetime] NOT NULL,
    [uid_from] [varchar](32) NOT NULL,
    [uid_to] [varchar](32) NOT NULL,
    [coin_used] [decimal](18, 2) NOT NULL,
    [score_earned] [decimal](18, 2) NOT NULL,
    [send_count] [int] NOT NULL,
    [receive_count] [int] NOT NULL,
    [chat_depth] [int] NOT NULL,
    [evaluation] [nvarchar](255) NULL,
    [details] [nvarchar](max) NULL,
    [source] [nvarchar](50) NULL,
    [create_time] [datetime] NOT NULL,
    [update_time] [datetime] NOT NULL,
    [operator_name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_chat_check] PRIMARY KEY CLUSTERED 
(
    [id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[chat_check] ADD  CONSTRAINT [DF_chat_check_create_time]  DEFAULT (getdate()) FOR [create_time]
GO

ALTER TABLE [dbo].[chat_check] ADD  CONSTRAINT [DF_chat_check_update_time]  DEFAULT (getdate()) FOR [update_time]
GO

ALTER TABLE [dbo].[chat_check]  WITH CHECK ADD  CONSTRAINT [FK_chat_check_user_from] FOREIGN KEY([uid_from])
REFERENCES [dbo].[user] ([uid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chat_check] CHECK CONSTRAINT [FK_chat_check_user_from]
GO

ALTER TABLE [dbo].[chat_check]  WITH CHECK ADD  CONSTRAINT [FK_chat_check_user_to] FOREIGN KEY([uid_to])
REFERENCES [dbo].[user] ([uid])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chat_check] CHECK CONSTRAINT [FK_chat_check_user_to]
GO

-- 创建索引
CREATE INDEX [IX_chat_check_uid_from] ON [dbo].[chat_check] ([uid_from])
GO

CREATE INDEX [IX_chat_check_uid_to] ON [dbo].[chat_check] ([uid_to])
GO

CREATE INDEX [IX_chat_check_dtm_chat] ON [dbo].[chat_check] ([dtm_chat])
GO

CREATE INDEX [IX_chat_check_source] ON [dbo].[chat_check] ([source])
GO

CREATE INDEX [IX_chat_check_uid_from_uid_to_dtm_chat] ON [dbo].[chat_check] ([uid_from], [uid_to], [dtm_chat])
GO