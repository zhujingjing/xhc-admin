# 任务管理板块时间显示修复计划

## 问题分析
任务管理板块中多个页面存在时间显示问题，主要原因是后台查询数据时，日期类型字段没有正确转换为字符串类型，导致前端展示异常。

## 修复任务列表

### [x] 任务1：分析现有时间相关字段和页面
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 识别任务管理板块中所有涉及时间显示的页面和字段
  - 分析当前后端查询方法中日期字段的处理方式
  - 确定需要修复的具体位置
- **Success Criteria**:
  - 完成所有时间相关页面和字段的清单
  - 明确当前后端处理方式的问题
- **Test Requirements**:
  - `programmatic` TR-1.1: 生成完整的时间字段和页面清单
  - `human-judgement` TR-1.2: 清单内容完整，覆盖所有相关页面

### [x] 任务2：修复BLL/Task.cs中的时间字段处理
- **Priority**: P0
- **Depends On**: 任务1
- **Description**:
  - 修改BLL/Task.cs中的查询方法，确保所有日期类型字段在返回前转换为字符串
  - 重点关注GetTaskById、GetTaskList等核心方法
  - 统一时间格式为"yyyy-MM-dd HH:mm:ss"
- **Success Criteria**:
  - 所有日期字段返回为格式化的字符串
  - 前端能够正确显示时间信息
- **Test Requirements**:
  - `programmatic` TR-2.1: 后端返回的时间字段为字符串类型
  - `programmatic` TR-2.2: 时间格式符合"yyyy-MM-dd HH:mm:ss"

### [x] 任务3：修复AuditTask页面时间显示
- **Priority**: P1
- **Depends On**: 任务2
- **Description**:
  - 在AuditTask.cshtml页面添加开始时间、结束时间、截止时间的显示
  - 确保页面能够正确接收和展示后端返回的时间数据
- **Success Criteria**:
  - 页面显示开始时间、结束时间、截止时间
  - 时间格式正确，显示清晰
- **Test Requirements**:
  - `human-judgement` TR-3.1: 页面能够正确显示所有时间字段
  - `human-judgement` TR-3.2: 时间格式美观易读

### [x] 任务4：修复Time和TimeAdd页面时间处理
- **Priority**: P1
- **Depends On**: 任务2
- **Description**:
  - 检查并修复Time.cshtml和TimeAdd.cshtml页面的时间显示问题
  - 确保时间选择和显示的一致性
- **Success Criteria**:
  - Time页面列表正确显示时间
  - TimeAdd页面正确处理时间输入和显示
- **Test Requirements**:
  - `human-judgement` TR-4.1: Time页面时间显示正确
  - `human-judgement` TR-4.2: TimeAdd页面时间处理正常

### [x] 任务5：修复其他相关页面时间显示
- **Priority**: P2
- **Depends On**: 任务2
- **Description**:
  - 检查并修复其他任务管理相关页面的时间显示问题
  - 包括Template、TemplateAdd等页面
- **Success Criteria**:
  - 所有相关页面的时间显示正常
  - 时间格式统一一致
- **Test Requirements**:
  - `human-judgement` TR-5.1: 所有相关页面时间显示正确
  - `human-judgement` TR-5.2: 时间格式统一美观

### [x] 任务6：测试和验证
- **Priority**: P1
- **Depends On**: 任务3、4、5
- **Description**:
  - 全面测试所有修复后的页面
  - 验证时间显示在不同场景下的正确性
  - 确保没有引入新的问题
- **Success Criteria**:
  - 所有页面时间显示正常
  - 没有出现时间相关的错误
- **Test Requirements**:
  - `human-judgement` TR-6.1: 所有页面时间显示正确
  - `human-judgement` TR-6.2: 操作流程中时间处理正常

## 技术方案
1. **后端处理**：在SQL查询中使用CONVERT函数将日期类型转换为字符串
2. **统一格式**：所有时间字段使用"yyyy-MM-dd HH:mm:ss"格式
3. **前端处理**：确保前端页面能够正确接收和展示字符串格式的时间
4. **测试验证**：全面测试所有相关页面和功能

## 预期成果
- 所有任务管理页面的时间显示正常
- 后端返回的时间数据格式统一
- 前端展示的时间清晰易读
- 避免因日期类型转换导致的显示问题

## 分析结果

### 涉及时间显示的页面和字段：

1. **AuditTask.cshtml**：
   - 现有：CreateTime
   - 缺失：StartTime、EndTime、Deadline

2. **Time.cshtml**：
   - StartTime、EndTime、CreateTime

3. **TimeAdd.cshtml**：
   - StartTime、EndTime

4. **其他页面**（如Detail.cshtml、Management.cshtml等）：
   - 可能包含时间字段

### 后端查询方法分析：

1. **GetTaskById**：
   - 问题：StartTime、EndTime、Deadline、CreateTime、UpdateTime字段未转换为字符串

2. **GetTaskList**：
   - 问题：StartTime、EndTime、Deadline、CreateTime、UpdateTime字段未转换为字符串

3. **GetTaskTimeList**：
   - 现状：StartTime、EndTime已转换为"HH:MM"格式
   - 问题：CreateTime、UpdateTime字段未转换为字符串

4. **GetTaskTimeById**：
   - 现状：StartTime、EndTime已转换为"HH:MM"格式
   - 问题：CreateTime、UpdateTime字段未转换为字符串

### 修复方案：
1. 修改GetTaskById和GetTaskList方法，将所有日期字段转换为"yyyy-MM-dd HH:mm:ss"格式
2. 修改GetTaskTimeList和GetTaskTimeById方法，将CreateTime、UpdateTime字段转换为"yyyy-MM-dd HH:mm:ss"格式
3. 在AuditTask.cshtml页面添加StartTime、EndTime、Deadline的显示
4. 确保所有前端页面能够正确处理字符串格式的时间数据