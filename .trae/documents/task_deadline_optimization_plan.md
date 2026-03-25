# 任务截止时间优化计划

## 问题分析

当前任务管理系统存在以下问题：
- 非上班时间生成的任务，截止时间设置会导致很多任务在工作时间开始的时候就已经过期
- 工作时间：上午9点到12点半，下午2点到6点半
- 例如：如果任务在晚上生成，设置为1小时后截止，那么截止时间会是晚上的某个时间，而不是第二天的工作时间

## 核心原因

通过分析 `BLL/Task.cs` 中的 `CalculateDeadline` 方法，发现当前的截止时间计算逻辑没有考虑工作时间因素：
- 对于相对时间类型（类型1），直接在当前时间基础上增加指定的时间量
- 对于固定时间点（类型2），直接使用当天的指定时间
- 对于当天结束（类型3），使用当天23:59:59
- 对于次日开始工作（类型4），使用次日9:00:00

## 解决方案

### 1. 工作时间配置

工作时间在 `Task_Time` 表中配置，支持按分类设置和全局设置（CategoryID为NULL）。

### 2. 时间计算优化

#### 2.1 任务开始时间优化
- 当生成任务时，检查当前时间是否在工作时间内
- 如果不在，将开始时间调整到下一个工作时间
- 当任务状态从待处理变为进行中时，同样检查并调整开始时间

#### 2.2 截止时间计算优化

修改 `CalculateDeadline` 方法，增加工作时间判断逻辑：

1. **相对时间类型（类型1）**：
   - 计算基础截止时间
   - 检查截止时间是否在工作时间内
   - 如果不在，调整到下一个工作时间

2. **固定时间点（类型2）**：
   - 检查指定时间是否在工作时间内
   - 如果不在，推移到当天23:59:59

3. **当天结束（类型3）**：
   - 使用当天23:59:59

4. **次日开始工作（类型4）**：
   - 使用次日工作开始时间（从Task_Time表获取）

### 3. 核心算法

1. **GetWorkingHours** 方法：获取工作时间配置
2. **IsInWorkingHours** 方法：判断给定时间是否在工作时间内
3. **GetNextWorkingTime** 方法：获取下一个工作时间点
4. **AdjustToWorkingHours** 方法：将时间调整到工作时间内

### 4. 核心逻辑伪代码

#### 4.1 GetWorkingHours 方法
```csharp
public List<WorkingHour> GetWorkingHours(string categoryId)
{
    // 从Task_Time表获取工作时间配置
    // 优先获取指定分类的配置，如果没有则获取全局配置
    // 返回工作时间列表
}
```

#### 4.2 IsInWorkingHours 方法
```csharp
public bool IsInWorkingHours(DateTime time, string categoryId)
{
    // 获取工作时间配置
    // 检查给定时间是否在工作时间范围内
    // 返回判断结果
}
```

#### 4.3 GetNextWorkingTime 方法
```csharp
public DateTime GetNextWorkingTime(DateTime currentTime, string categoryId)
{
    // 获取工作时间配置
    // 计算下一个工作时间点
    // 考虑跨天、跨周末的情况
    // 返回下一个工作时间
}
```

#### 4.4 AdjustToWorkingHours 方法
```csharp
public DateTime AdjustToWorkingHours(DateTime time, string categoryId)
{
    // 检查时间是否在工作时间内
    // 如果不在，调整到下一个工作时间
    // 返回调整后的时间
}
```

#### 4.5 CalculateDeadline 方法修改
```csharp
public DateTime CalculateDeadline(int deadlineType, string deadlineValue, int deadlineUnit, string categoryId)
{
    DateTime now = DateTime.Now;
    DateTime deadline = DateTime.MinValue;
    
    switch (deadlineType)
    {
        case 0: // 无截止
            return DateTime.MinValue;
        case 1: // 相对生成时间
            // 计算基础截止时间
            // 检查是否在工作时间内
            // 如果不在，调整到下一个工作时间
            break;
        case 2: // 固定时间点
            // 解析固定时间点
            // 检查是否在工作时间内
            // 如果不在，推移到当天23:59:59
            break;
        case 3: // 当天结束
            // 使用当天23:59:59
            return new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
        case 4: // 次日开始工作
            // 使用次日工作开始时间（从Task_Time表获取）
            break;
    }
    
    return deadline;
}
```

#### 4.6 任务开始时间优化
```csharp
// 任务生成时的开始时间优化
public DateTime GetAdjustedStartTime(string categoryId)
{
    DateTime now = DateTime.Now;
    if (IsInWorkingHours(now, categoryId))
    {
        return now;
    }
    else
    {
        return GetNextWorkingTime(now, categoryId);
    }
}

// 任务状态变为进行中时的开始时间（允许非工作时间）
public DateTime GetCurrentTime()
{
    return DateTime.Now;
}
```

### 5. 任务生命周期中关于时间的处理

#### 5.1 任务生成阶段
1. **生成时间**：系统当前时间
2. **开始时间**：
   - 检查当前时间是否在工作时间内
   - 如果不在，调整到下一个工作时间
3. **截止时间**：
   - 根据任务模板的截止时间配置计算
   - 相对时间类型：如果不在工作时间内，调整到下一个工作时间
   - 固定时间点类型：如果不在工作时间内，推移到当天23:59:59
   - 当天结束类型：使用当天23:59:59
   - 次日开始工作类型：使用次日工作开始时间

#### 5.2 任务执行阶段
1. **开始时间**：
   - 当任务状态从待处理变为进行中时设置
   - 允许在非工作时间工作，使用当前操作时间
2. **结束时间**：任务完成时的系统当前时间（允许在非工作时间完成）

#### 5.3 任务审核阶段
1. **审核时间**：任务审核时的系统当前时间（允许在非工作时间审核）

#### 5.4 时间相关的状态转换
- **待处理**：任务生成时的状态，开始时间已调整到工作时间
- **进行中**：任务开始执行时的状态，使用当前操作时间（允许非工作时间）
- **已完成**：任务完成时的状态，记录结束时间（允许非工作时间）
- **已审核**：任务审核通过时的状态，记录审核时间（允许非工作时间）

## 实施步骤

### [ ] 任务1：添加工作时间配置获取方法
- **Priority**: P1
- **Depends On**: None
- **Description**:
  - 在 `BLL/Task.cs` 中添加 `GetWorkingHours` 方法，用于从Task_Time表获取工作时间配置
  - 支持按分类获取和全局获取（CategoryID为NULL）
- **Success Criteria**:
  - 方法能够正确从Task_Time表获取工作时间配置
- **Test Requirements**:
  - `programmatic` TR-1.1: 测试获取不同分类的工作时间配置
  - `human-judgement` TR-1.2: 代码逻辑清晰，符合表结构设计

### [ ] 任务2：添加工作时间判断方法
- **Priority**: P1
- **Depends On**: 任务1
- **Description**:
  - 在 `BLL/Task.cs` 中添加 `IsInWorkingHours` 方法，用于判断给定时间是否在工作时间内
  - 使用从Task_Time表获取的工作时间配置
- **Success Criteria**:
  - 方法能够正确判断时间是否在工作时间内
- **Test Requirements**:
  - `programmatic` TR-2.1: 测试不同时间点的判断结果
  - `human-judgement` TR-2.2: 代码逻辑清晰，符合工作时间定义

### [ ] 任务3：添加下一个工作时间计算方法
- **Priority**: P1
- **Depends On**: 任务2
- **Description**:
  - 在 `BLL/Task.cs` 中添加 `GetNextWorkingTime` 方法，用于获取下一个工作时间点
  - 考虑跨天、跨周末的情况
  - 使用从Task_Time表获取的工作时间配置
- **Success Criteria**:
  - 方法能够正确计算下一个工作时间点
- **Test Requirements**:
  - `programmatic` TR-3.1: 测试不同时间点的下一个工作时间计算
  - `human-judgement` TR-3.2: 代码逻辑清晰，考虑边界情况

### [ ] 任务4：修改 CalculateDeadline 方法
- **Priority**: P0
- **Depends On**: 任务3
- **Description**:
  - 修改 `CalculateDeadline` 方法，增加工作时间判断和调整逻辑
  - 对于相对时间类型，检查并调整截止时间到工作时间内
  - 对于固定时间点类型，检查并调整到工作时间内
  - 对于当天结束类型，使用当天工作结束时间（从Task_Time表获取）
  - 对于次日开始工作类型，使用次日工作开始时间（从Task_Time表获取）
- **Success Criteria**:
  - 方法能够正确计算考虑工作时间的截止时间
- **Test Requirements**:
  - `programmatic` TR-4.1: 测试不同时间点、不同类型的截止时间计算
  - `human-judgement` TR-4.2: 代码逻辑清晰，符合业务需求

### [ ] 任务5：优化任务开始时间设置
- **Priority**: P1
- **Depends On**: 任务3
- **Description**:
  - 修改任务生成时的开始时间设置逻辑
  - 修改任务状态从待处理变为进行中时的开始时间设置逻辑
  - 确保开始时间在工作时间范围内
- **Success Criteria**:
  - 任务开始时间被正确调整到工作时间内
- **Test Requirements**:
  - `programmatic` TR-5.1: 测试不同时间点生成任务时的开始时间设置
  - `human-judgement` TR-5.2: 代码逻辑清晰，符合业务需求

### [ ] 任务6：测试验证
- **Priority**: P1
- **Depends On**: 任务4, 任务5
- **Description**:
  - 测试非上班时间生成的任务开始时间和截止时间计算
  - 测试不同截止时间类型的计算结果
  - 验证任务不会在工作时间开始时就过期
- **Success Criteria**:
  - 非上班时间生成的任务开始时间和截止时间被正确调整到工作时间内
  - 任务不会在工作时间开始时就过期
- **Test Requirements**:
  - `programmatic` TR-6.1: 测试不同场景下的开始时间和截止时间计算
  - `human-judgement` TR-6.2: 测试结果符合预期，任务不会过期

## 预期效果

- 非上班时间生成的任务，开始时间会被调整到下一个工作时间
- 非上班时间生成的任务，截止时间会被调整到下一个工作时间
- 任务不会在工作时间开始时就过期
- 提高任务管理的合理性和可执行性
- 减少因时间设置不合理导致的任务过期问题

## 风险评估

- **风险1**: 调整逻辑可能会影响现有的任务截止时间计算
  - **缓解措施**: 仔细测试各种场景，确保修改不会破坏现有功能
- **风险2**: 跨天、跨周末的情况可能会导致计算错误
  - **缓解措施**: 完善 `GetNextWorkingTime` 方法，考虑各种边界情况
- **风险3**: 性能影响
  - **缓解措施**: 优化算法，确保计算效率

## 结论

通过优化 `CalculateDeadline` 方法，考虑工作时间因素，可以有效解决非上班时间生成的任务截止时间问题，提高任务管理的合理性和可执行性。