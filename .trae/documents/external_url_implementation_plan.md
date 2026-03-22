# 外部链接功能实现计划

## 1. 代码分析阶段

### [ ] 任务 1.1: 分析当前外部链接相关代码
- **Priority**: P0
- **Depends On**: None
- **Description**: 
  - 分析当前BLL/Task.cs中外部链接相关的代码
  - 分析前端页面中外部链接相关的代码
  - 分析数据库表结构中的外部链接字段
- **Success Criteria**:
  - 完整了解当前外部链接功能的实现
  - 识别需要删除和修改的代码
- **Test Requirements**:
  - `programmatic` TR-1.1.1: 分析BLL/Task.cs中的外部链接相关代码
  - `programmatic` TR-1.1.2: 分析前端页面中的外部链接相关代码
  - `programmatic` TR-1.1.3: 分析数据库表结构中的外部链接字段

### [ ] 任务 1.2: 分析任务模板相关代码
- **Priority**: P0
- **Depends On**: 任务 1.1
- **Description**: 
  - 分析任务模板的数据库表结构
  - 分析任务模板的BLL代码
  - 分析任务模板的前端页面
- **Success Criteria**:
  - 完整了解任务模板的结构和功能
  - 确定如何在任务模板中添加外部链接配置
- **Test Requirements**:
  - `programmatic` TR-1.2.1: 分析任务模板的数据库表结构
  - `programmatic` TR-1.2.2: 分析任务模板的BLL代码
  - `programmatic` TR-1.2.3: 分析任务模板的前端页面

## 2. 设计阶段

### [x] 任务 2.1: 设计数据库修改方案
- **Priority**: P1
- **Depends On**: 任务 1.2
- **Description**: 
  - 设计任务模板表的修改方案，添加外部链接相关字段
  - 设计任务表的修改方案，保留FullExternalUrl字段，删除其他外部链接相关字段
- **Success Criteria**:
  - 确定任务模板表需要添加的字段
  - 确定任务表需要保留和删除的字段
- **Test Requirements**:
  - `programmatic` TR-2.1.1: 设计任务模板表的修改SQL语句
  - `programmatic` TR-2.1.2: 设计任务表的修改SQL语句
- **Notes**:
  - 任务模板表需要添加的字段：
    - ExternalUrl (nvarchar(2000)) - 外部URL基础地址
    - ExternalUrlParams (nvarchar(1000)) - URL参数模板
    - ExternalUrlEnabled (bit) - 是否启用外部URL
  - 任务表需要删除的字段：
    - ExternalUrl
    - ExternalUrlParams
    - ExternalUrlEnabled
  - 任务表需要保留的字段：
    - FullExternalUrl

### [x] 任务 2.2: 设计外部链接生成逻辑
- **Priority**: P1
- **Depends On**: 任务 2.1
- **Description**: 
  - 设计任务生成时的外部链接生成逻辑
  - 设计参数动态拼接的实现方式
- **Success Criteria**:
  - 明确外部链接生成的完整流程
  - 确定参数动态拼接的具体实现
- **Test Requirements**:
  - `human-judgement` TR-2.2.1: 确认外部链接生成逻辑的设计
  - `human-judgement` TR-2.2.2: 确认参数动态拼接的实现方式
- **Notes**:
  - 外部链接生成流程：
    1. 在任务模板中配置外部链接相关信息（ExternalUrl、ExternalUrlParams、ExternalUrlEnabled）
    2. 任务生成时，从任务模板获取外部链接配置
    3. 如果启用了外部链接，根据模板配置生成完整的外部URL
    4. 动态替换URL参数模板中的变量（如{BusinessType}、{BusinessId}、{TaskId}）
    5. 将生成的完整URL存储在任务的FullExternalUrl字段中
  - 参数动态拼接实现：
    - 支持的动态参数：{BusinessType}、{BusinessId}、{TaskId}
    - 在任务生成时，根据任务的实际值替换这些参数
    - 确保URL参数的正确编码

## 3. 实现阶段

### [ ] 任务 3.1: 修改数据库表结构
- **Priority**: P1
- **Depends On**: 任务 2.1
- **Description**: 
  - 执行任务模板表的修改，添加外部链接相关字段
  - 执行任务表的修改，保留FullExternalUrl字段，删除其他外部链接相关字段
- **Success Criteria**:
  - 任务模板表成功添加外部链接相关字段
  - 任务表成功修改，保留FullExternalUrl字段
- **Test Requirements**:
  - `programmatic` TR-3.1.1: 执行任务模板表的修改SQL
  - `programmatic` TR-3.1.2: 执行任务表的修改SQL

### [ ] 任务 3.2: 修改任务模板相关代码
- **Priority**: P1
- **Depends On**: 任务 3.1
- **Description**: 
  - 修改BLL/Task.cs中的任务模板相关方法，支持外部链接配置
  - 修改任务模板的前端页面，添加外部链接配置字段
- **Success Criteria**:
  - BLL代码支持任务模板的外部链接配置
  - 前端页面支持外部链接配置的输入和显示
- **Test Requirements**:
  - `programmatic` TR-3.2.1: 修改BLL/Task.cs中的任务模板相关方法
  - `programmatic` TR-3.2.2: 修改任务模板的前端页面

### [ ] 任务 3.3: 修改任务生成逻辑
- **Priority**: P1
- **Depends On**: 任务 3.2
- **Description**: 
  - 修改任务生成逻辑，从任务模板获取外部链接配置
  - 实现任务生成时的外部链接动态拼接
  - 确保完整的外部链接存储在任务的FullExternalUrl字段中
- **Success Criteria**:
  - 任务生成时能正确获取任务模板的外部链接配置
  - 能正确动态拼接外部链接参数
  - 完整的外部链接能正确存储在任务中
- **Test Requirements**:
  - `programmatic` TR-3.3.1: 修改任务生成逻辑
  - `programmatic` TR-3.3.2: 测试任务生成时的外部链接生成

### [ ] 任务 3.4: 清理和修改前端页面
- **Priority**: P2
- **Depends On**: 任务 3.3
- **Description**: 
  - 删除任务管理页面中的外部链接配置字段
  - 修改任务详情页面，确保正确显示外部链接
- **Success Criteria**:
  - 任务管理页面不再显示外部链接配置字段
  - 任务详情页面能正确显示从任务模板生成的外部链接
- **Test Requirements**:
  - `programmatic` TR-3.4.1: 修改任务管理页面
  - `programmatic` TR-3.4.2: 修改任务详情页面

## 4. 测试阶段

### [ ] 任务 4.1: 功能测试
- **Priority**: P1
- **Depends On**: 任务 3.4
- **Description**: 
  - 测试任务模板的外部链接配置功能
  - 测试任务生成时的外部链接生成
  - 测试任务详情页面的外部链接显示
- **Success Criteria**:
  - 任务模板的外部链接配置功能正常
  - 任务生成时能正确生成外部链接
  - 任务详情页面能正确显示外部链接
- **Test Requirements**:
  - `programmatic` TR-4.1.1: 测试任务模板的外部链接配置
  - `programmatic` TR-4.1.2: 测试任务生成时的外部链接生成
  - `programmatic` TR-4.1.3: 测试任务详情页面的外部链接显示

### [ ] 任务 4.2: 性能测试
- **Priority**: P2
- **Depends On**: 任务 4.1
- **Description**: 
  - 测试外部链接生成的性能
  - 验证系统的响应速度
- **Success Criteria**:
  - 外部链接生成性能良好
  - 系统响应速度正常
- **Test Requirements**:
  - `programmatic` TR-4.2.1: 测试外部链接生成的性能
  - `human-judgement` TR-4.2.2: 验证系统响应速度

## 5. 文档阶段

### [ ] 任务 5.1: 更新文档
- **Priority**: P2
- **Depends On**: 任务 4.2
- **Description**: 
  - 更新系统文档，添加外部链接功能的说明
  - 编写使用指南
- **Success Criteria**:
  - 系统文档已更新
  - 包含外部链接功能的使用说明
- **Test Requirements**:
  - `human-judgement` TR-5.1.1: 验证文档更新
  - `human-judgement` TR-5.1.2: 确认文档的完整性

---

## 实施顺序

1. 代码分析阶段（任务 1.1 - 1.2）
2. 设计阶段（任务 2.1 - 2.2）
3. 实现阶段（任务 3.1 - 3.4）
4. 测试阶段（任务 4.1 - 4.2）
5. 文档阶段（任务 5.1）

## 预期交付物

1. 数据库修改SQL语句
2. 修改后的任务模板相关代码
3. 修改后的任务生成逻辑代码
4. 修改后的前端页面
5. 系统文档更新

## 风险评估

1. **数据库兼容性风险**：修改表结构可能影响现有功能
2. **代码兼容性风险**：修改任务生成逻辑可能影响现有代码
3. **前端兼容性风险**：修改前端页面可能影响用户体验

## 缓解措施

1. 仔细分析现有代码，确保修改不会影响现有功能
2. 编写详细的测试用例，确保功能正常
3. 进行性能测试，确保系统响应速度
4. 提供回滚方案，以防出现问题