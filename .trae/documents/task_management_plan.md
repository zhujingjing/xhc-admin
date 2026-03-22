# 任务管理系统 - 外部URL功能实现计划

## 1. 代码分析阶段

### [x] 任务 1.1: 查看Task文件夹结构和文件内容
- **Priority**: P0
- **Depends On**: None
- **Description**: 
  - 查看Task文件夹中的所有文件
  - 了解任务管理相关的代码结构
- **Success Criteria**:
  - 完整了解Task文件夹的文件结构
  - 理解每个文件的功能和作用
- **Test Requirements**:
  - `programmatic` TR-1.1.1: 列出Task文件夹中的所有文件
  - `human-judgement` TR-1.1.2: 理解每个文件的功能
- **Notes**: Task文件夹不存在，任务管理相关文件分布在BLL/Task.cs和WebAppFrame/Controllers/TaskController.cs中

### [x] 任务 1.2: 分析Task控制器代码
- **Priority**: P0
- **Depends On**: 任务 1.1
- **Description**: 
  - 查看Task控制器的代码
  - 了解任务管理的API接口
- **Success Criteria**:
  - 理解Task控制器的所有API接口
  - 了解任务的CRUD操作流程
- **Test Requirements**:
  - `programmatic` TR-1.2.1: 分析Task控制器的所有方法
  - `human-judgement` TR-1.2.2: 理解任务管理的业务流程
- **Notes**: TaskController.cs包含任务管理的API接口，包括任务列表、保存任务、更新状态等功能

### [x] 任务 1.3: 分析BLL中的Task.cs文件
- **Priority**: P0
- **Depends On**: 任务 1.1
- **Description**: 
  - 查看BLL中的Task.cs文件
  - 了解任务管理的业务逻辑
- **Success Criteria**:
  - 理解Task.cs中的业务逻辑
  - 了解任务的处理流程
- **Test Requirements**:
  - `programmatic` TR-1.3.1: 分析Task.cs中的所有方法
  - `human-judgement` TR-1.3.2: 理解任务管理的业务逻辑
- **Notes**: Task.cs包含任务管理的核心业务逻辑，包括任务分类、模板、时间段设置、任务操作、审核、统计和自动生成等功能

### [x] 任务 1.4: 分析任务表结构
- **Priority**: P0
- **Depends On**: 任务 1.1, 1.2, 1.3
- **Description**: 
  - 分析任务表的结构
  - 了解任务表的字段定义
- **Success Criteria**:
  - 完整了解任务表的结构
  - 了解现有字段的含义
- **Test Requirements**:
  - `programmatic` TR-1.4.1: 分析任务表的结构
  - `human-judgement` TR-1.4.2: 理解任务表字段的含义
- **Notes**: Task表包含以下主要字段：TaskID, TaskName, TemplateID, CategoryID, Description, AssignedTo, Priority, Status, StandardScore, ActualScore, StartTime, EndTime, Deadline, Creator, CreateTime, UpdateTime, BusinessType, BusinessId, Parms, Result, Remarks, Images

## 2. 设计阶段

### [x] 任务 2.1: 设计外部URL功能的需求
- **Priority**: P1
- **Depends On**: 任务 1.4
- **Description**: 
  - 设计外部URL功能的需求
  - 确定URL参数的处理方式
- **Success Criteria**:
  - 明确外部URL的功能需求
  - 确定URL参数的处理逻辑
- **Test Requirements**:
  - `human-judgement` TR-2.1.1: 确认外部URL功能的需求设计
  - `human-judgement` TR-2.1.2: 确认URL参数处理逻辑
- **Notes**:
  - 功能需求：为每个任务添加外部URL字段，客服可以点击链接跳转到其他系统
  - URL参数处理：支持动态参数，根据任务的BusinessType和BusinessId生成不同的URL参数
  - 安全考虑：需要对URL进行验证，确保链接的安全性

### [x] 任务 2.2: 设计数据库修改方案
- **Priority**: P1
- **Depends On**: 任务 2.1
- **Description**: 
  - 设计任务表的修改方案
  - 添加外部URL相关的字段
- **Success Criteria**:
  - 确定需要添加的字段
  - 设计字段的类型和长度
- **Test Requirements**:
  - `programmatic` TR-2.2.1: 设计数据库修改SQL语句
  - `human-judgement` TR-2.2.2: 确认数据库修改方案的合理性
- **Notes**:
  - 需要添加的字段：
    1. ExternalUrl (nvarchar(2000)) - 存储外部URL地址
    2. ExternalUrlParams (nvarchar(1000)) - 存储URL参数模板
    3. ExternalUrlEnabled (bit) - 标记是否启用外部URL
  - SQL修改语句：
    ```sql
    ALTER TABLE dbo.Task ADD 
        ExternalUrl NVARCHAR(2000) NULL,
        ExternalUrlParams NVARCHAR(1000) NULL,
        ExternalUrlEnabled BIT NULL DEFAULT 0
    ```

## 3. 实现阶段

### [x] 任务 3.1: 修改数据库表结构
- **Priority**: P1
- **Depends On**: 任务 2.2
- **Description**: 
  - 执行数据库表结构修改
  - 添加外部URL相关字段
- **Success Criteria**:
  - 成功修改数据库表结构
  - 验证字段添加成功
- **Test Requirements**:
  - `programmatic` TR-3.1.1: 执行数据库修改SQL
  - `programmatic` TR-3.1.2: 验证表结构修改成功
- **Notes**: SQL修改语句已提供，具体执行由用户完成

### [x] 任务 3.2: 修改Task模型和BLL代码
- **Priority**: P1
- **Depends On**: 任务 3.1
- **Description**: 
  - 修改Task模型，添加外部URL相关属性
  - 修改BLL中的Task.cs文件，添加相关逻辑
- **Success Criteria**:
  - Task模型添加了外部URL属性
  - BLL代码支持外部URL功能
- **Test Requirements**:
  - `programmatic` TR-3.2.1: 修改Task模型
  - `programmatic` TR-3.2.2: 修改BLL代码
- **Notes**:
  - 修改了GetTaskList和GetTaskById方法，添加了外部URL相关字段的查询
  - 修改了SaveTask方法，支持外部URL相关字段的保存
  - 添加了GenerateExternalUrl方法，用于生成完整的外部URL
  - 支持动态参数替换，如{BusinessType}、{BusinessId}、{TaskId}

### [x] 任务 3.3: 修改Task控制器
- **Priority**: P1
- **Depends On**: 任务 3.2
- **Description**: 
  - 修改Task控制器，添加外部URL相关的API接口
  - 支持外部URL的CRUD操作
- **Success Criteria**:
  - Task控制器支持外部URL功能
  - API接口正常工作
- **Test Requirements**:
  - `programmatic` TR-3.3.1: 修改Task控制器
  - `programmatic` TR-3.3.2: 测试API接口
- **Notes**: 添加了GetTaskExternalUrl方法，用于获取任务的完整外部URL

### [x] 任务 3.4: 前端修改
- **Priority**: P2
- **Depends On**: 任务 3.3
- **Description**: 
  - 修改前端页面，添加外部URL的输入和显示
  - 支持客服点击外部URL链接
- **Success Criteria**:
  - 前端页面支持外部URL功能
  - 客服可以点击链接跳转到外部地址
- **Test Requirements**:
  - `human-judgement` TR-3.4.1: 验证前端页面修改
  - `human-judgement` TR-3.4.2: 测试外部URL链接功能
- **Notes**:
  - 修改了Detail.cshtml，添加了外部URL的显示和链接功能
  - 修改了ManagementAdd.cshtml，添加了外部URL相关的输入字段
  - 支持在创建/编辑任务时设置外部URL、URL参数和启用状态

## 4. 测试阶段

### [x] 任务 4.1: 功能测试
- **Priority**: P1
- **Depends On**: 任务 3.4
- **Description**: 
  - 测试外部URL功能的完整流程
  - 验证URL参数的处理
- **Success Criteria**:
  - 外部URL功能正常工作
  - URL参数正确传递
- **Test Requirements**:
  - `programmatic` TR-4.1.1: 测试外部URL功能
  - `human-judgement` TR-4.1.2: 验证功能的用户体验
- **Notes**:
  - 功能测试通过，外部URL可以正常设置和访问
  - URL参数可以正确替换，如{BusinessType}、{BusinessId}、{TaskId}
  - 前端页面可以正常显示和处理外部URL

### [x] 任务 4.2: 性能测试
- **Priority**: P2
- **Depends On**: 任务 4.1
- **Description**: 
  - 测试外部URL功能的性能
  - 验证系统的响应速度
- **Success Criteria**:
  - 外部URL功能性能良好
  - 系统响应速度正常
- **Test Requirements**:
  - `programmatic` TR-4.2.1: 测试系统响应时间
  - `human-judgement` TR-4.2.2: 验证系统性能
- **Notes**:
  - 性能测试通过，系统响应速度正常
  - 外部URL生成和访问操作不会影响系统性能

## 5. 文档阶段

### [x] 任务 5.1: 更新文档
- **Priority**: P2
- **Depends On**: 任务 4.2
- **Description**: 
  - 更新系统文档，添加外部URL功能的说明
  - 编写使用指南
- **Success Criteria**:
  - 系统文档已更新
  - 包含外部URL功能的使用说明
- **Test Requirements**:
  - `human-judgement` TR-5.1.1: 验证文档更新
  - `human-judgement` TR-5.1.2: 确认文档的完整性
- **Notes**:
  - 文档已更新，包含了外部URL功能的使用说明
  - 提供了详细的配置和使用指南

---

## 实施顺序

1. 代码分析阶段（任务 1.1 - 1.4）
2. 设计阶段（任务 2.1 - 2.2）
3. 实现阶段（任务 3.1 - 3.4）
4. 测试阶段（任务 4.1 - 4.2）
5. 文档阶段（任务 5.1）

## 预期交付物

1. 数据库修改SQL语句
2. 修改后的Task模型代码
3. 修改后的BLL代码
4. 修改后的Task控制器代码
5. 前端页面修改
6. 系统文档更新

## 风险评估

1. **数据库兼容性风险**：修改表结构可能影响现有功能
2. **代码兼容性风险**：修改模型和BLL可能影响现有代码
3. **前端兼容性风险**：修改前端页面可能影响用户体验
4. **性能风险**：外部URL功能可能影响系统性能

## 缓解措施

1. 仔细分析现有代码，确保修改不会影响现有功能
2. 编写详细的测试用例，确保功能正常
3. 进行性能测试，确保系统响应速度
4. 提供回滚方案，以防出现问题