-- 在TaskTemplate表中增加AssignedTo字段
ALTER TABLE dbo.TaskTemplate ADD AssignedTo NVARCHAR(100) NULL;

-- 添加字段描述
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'任务分配给的用户', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'TaskTemplate', 
    @level2type = N'COLUMN', @level2name = N'AssignedTo';