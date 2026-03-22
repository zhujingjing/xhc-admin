# 任务面板优化计划

## 任务分析

### 当前功能结构
- **任务面板**（Panel.cshtml）：显示任务列表，支持按状态和优先级筛选，包含批量操作功能
- **任务控制器**（TaskController.cs）：处理任务相关的后端逻辑，包括获取任务列表、更新任务状态等
- **任务业务逻辑**（BLL/Task.cs）：实现任务的核心业务逻辑，包括任务状态更新、批量操作等

### 优化需求
1. **状态和优先级下拉框默认选择全部**：确保页面加载时默认显示所有任务
2. **优化批量操作框**：将"开始任务"和"完成任务"从下拉框改为两个可点击的按钮，并且互斥

## 实施计划

### [ ] 任务1：验证状态和优先级下拉框默认值设置
- **Priority**: P1
- **Depends On**: None
- **Description**: 
  - 检查当前Panel.cshtml中状态和优先级下拉框的默认值设置
  - 确保默认值为空字符串，对应"全部"选项
- **Success Criteria**:
  - 页面加载时，状态和优先级下拉框默认显示"全部状态"和"全部优先级"
- **Test Requirements**:
  - `programmatic` TR-1.1: 页面加载后检查下拉框默认值
  - `human-judgement` TR-1.2: 视觉上确认默认选项显示正确

### [ ] 任务2：优化批量操作框
- **Priority**: P0
- **Depends On**: 任务1
- **Description**:
  - 修改batchOperation函数，将下拉框选择改为两个互斥的按钮
  - 确保两个按钮只能选择一个
  - 保持批量操作的核心逻辑不变
- **Success Criteria**:
  - 点击"批量操作"按钮后，显示两个互斥的按钮选项
  - 只能选择其中一个操作
  - 操作执行后正确更新任务状态
- **Test Requirements**:
  - `programmatic` TR-2.1: 批量操作弹窗显示两个互斥按钮
  - `programmatic` TR-2.2: 只能选择一个操作并正确执行
  - `human-judgement` TR-2.3: 操作界面直观易用

### [ ] 任务3：测试优化效果
- **Priority**: P1
- **Depends On**: 任务2
- **Description**:
  - 测试状态和优先级默认值是否正确
  - 测试批量操作功能是否正常工作
  - 测试任务状态更新是否正确
- **Success Criteria**:
  - 所有功能正常工作
  - 界面美观易用
- **Test Requirements**:
  - `programmatic` TR-3.1: 所有功能测试通过
  - `human-judgement` TR-3.2: 界面体验良好

## 技术实现要点

1. **状态和优先级默认值**：
   - 确认下拉框默认值为空字符串，对应"全部"选项

2. **批量操作优化**：
   - 修改batchOperation函数，使用自定义弹窗替代prompt
   - 实现两个互斥的按钮选项
   - 保持与后端API的兼容性

3. **代码变更范围**：
   - 主要修改文件：WebAppFrame/Views/Task/Panel.cshtml
   - 不需要修改后端代码，保持API调用不变
