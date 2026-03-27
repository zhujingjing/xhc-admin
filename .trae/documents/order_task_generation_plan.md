# 订单任务自动生成 - 实现计划

## 任务背景
需要在任务管理系统中增加一个自动检测新订单并生成客服提醒任务的功能。当有新的订单生成时，系统应该自动创建一个任务，提醒客服有新订单需要关注。

## 现有系统分析

### 任务管理系统架构
1. **BLL层**：`BLL/Task.cs` 包含任务管理的核心逻辑
2. **控制器层**：`WebAppFrame/Controllers/TaskController.cs` 处理HTTP请求
3. **数据层**：使用 `DBHelper` 访问数据库

### 自动生成任务流程
1. `ExecuteTaskGeneration()` 方法：获取当前需要执行的调度记录
2. 检查当天是否已生成过任务
3. 计算截止时间
4. 生成任务
5. 保存任务到数据库

### 现有业务类型映射
系统已经支持多种业务类型的任务生成，如提现、用户反馈、聊天举报等，通过 `GetTemplateIdByBusinessType()` 方法进行映射。

## 实现计划

### [x] 任务1：创建订单任务模板
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 在系统中创建一个新的任务模板，用于新订单提醒
  - 模板名称："新订单提醒"
  - 分类：客服任务
  - 优先级：中等
  - 描述：提醒客服有新的订单产生，需要关注
- **Success Criteria**:
  - 任务模板创建成功，ID可在代码中使用
- **Test Requirements**:
  - `programmatic` TR-1.1: 模板在数据库中存在且状态为激活
  - `human-judgement` TR-1.2: 模板配置正确，包含必要信息

### [x] 任务2：添加订单业务类型映射
- **Priority**: P0
- **Depends On**: 任务1
- **Description**:
  - 在 `GetTemplateIdByBusinessType()` 方法中添加 "订单" 业务类型的映射
  - 关联到新创建的订单任务模板ID
- **Success Criteria**:
  - 业务类型映射添加成功
- **Test Requirements**:
  - `programmatic` TR-2.1: 方法能正确返回订单模板ID
  - `human-judgement` TR-2.2: 映射关系正确配置

### [x] 任务3：实现订单信息拼装逻辑
- **Priority**: P0
- **Depends On**: 任务2
- **Description**:
  - 在 `GetOtherInfo()` 方法中添加订单业务类型的信息拼装逻辑
  - 包含订单金额、订单号等关键信息
- **Success Criteria**:
  - 订单信息能正确拼装到任务名称中
- **Test Requirements**:
  - `programmatic` TR-3.1: 方法能正确拼装订单信息
  - `human-judgement` TR-3.2: 拼装的信息清晰明了

### [x] 任务4：实现新订单检测逻辑
- **Priority**: P0
- **Depends On**: 任务3
- **Description**:
  - 在 `BLL/Task.cs` 中添加 `CheckNewOrders()` 方法
  - 检测 `sys_order` 表中新增的订单
  - 为每个新订单生成任务
- **Success Criteria**:
  - 能正确检测到新订单
  - 能为每个新订单生成任务
- **Test Requirements**:
  - `programmatic` TR-4.1: 方法能正确检测新订单
  - `programmatic` TR-4.2: 能为新订单生成任务
  - `human-judgement` TR-4.3: 生成的任务信息完整准确

### [x] 任务5：集成到自动任务生成流程
- **Priority**: P0
- **Depends On**: 任务4
- **Description**:
  - 在 `ExecuteTaskGeneration()` 方法中调用 `CheckNewOrders()` 方法
  - 确保新订单检测成为自动任务生成的一部分
- **Success Criteria**:
  - 新订单检测集成到自动任务生成流程中
- **Test Requirements**:
  - `programmatic` TR-5.1: 自动任务生成能检测新订单
  - `programmatic` TR-5.2: 能为新订单生成任务

### [ ] 任务6：测试验证
- **Priority**: P1
- **Depends On**: 任务5
- **Description**:
  - 测试新订单检测功能
  - 验证任务生成是否正确
  - 检查任务信息是否完整
- **Success Criteria**:
  - 功能测试通过
  - 任务生成正确
- **Test Requirements**:
  - `programmatic` TR-6.1: 新订单能触发任务生成
  - `human-judgement` TR-6.2: 生成的任务信息准确完整

## 技术实现细节

### 数据库表结构（基于现有代码分析）
- **sys_order表**：
  - uid: 用户ID
  - order_state: 订单状态（包含'付款成功'）
  - pay_mny: 支付金额
  - 其他字段：订单号、创建时间等

### 实现步骤
1. **创建任务模板**：通过系统界面创建新订单提醒模板
2. **修改代码**：
   - 在 `GetTemplateIdByBusinessType()` 中添加订单业务类型映射
   - 在 `GetOtherInfo()` 中添加订单信息拼装逻辑
   - 添加 `CheckNewOrders()` 方法检测新订单
   - 在 `ExecuteTaskGeneration()` 中集成新订单检测

3. **测试**：
   - 模拟新订单生成
   - 运行自动任务生成
   - 验证任务是否正确生成

## 预期效果
当有新的订单生成时，系统会自动创建一个任务，提醒客服有新订单需要关注。任务包含订单的关键信息，如订单金额、订单号等，方便客服快速了解订单情况。