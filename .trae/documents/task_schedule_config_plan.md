# 任务调度配置实现方案

## 一、现有系统分析

### 1. 系统架构
- **BLL层**：`BLL/Task.cs` 包含任务管理的核心业务逻辑
- **控制器**：`TaskController.cs` 处理前端请求
- **前端页面**：`WebAppFrame/Views/Task/` 目录下的多个页面
- **数据库**：已存在 `TaskTemplateSchedule` 表用于存储调度信息

### 2. 现有功能
- 任务分类管理
- 任务模板管理
- 任务管理
- 任务审核
- 任务统计
- 时间段设置
- 任务自动生成（基于调度）

### 3. 缺少功能
- 调度配置的前端管理页面
- 调度配置的CRUD操作接口
- 调度配置的管理功能

## 二、实现方案

### 1. 页面设计

#### 1.1 调度配置页面 (`Schedule.cshtml`)
- **页面名称**：调度配置
- **菜单名称**：调度配置
- **功能**：
  - 显示所有调度记录列表
  - 支持按模板筛选
  - 支持启用/禁用调度
  - 支持编辑和删除调度
  - 支持添加新调度

#### 1.2 调度配置编辑页面 (`ScheduleAdd.cshtml`)
- **页面名称**：新增/编辑调度
- **功能**：
  - 选择任务模板
  - 设置调度类型（每天、每周、每月）
  - 根据调度类型设置相应的日期参数
  - 设置执行时间
  - 设置启用状态

### 2. 功能实现

#### 2.1 BLL层扩展
在 `BLL/Task.cs` 中添加以下方法：
- `GetTaskTemplateScheduleList()`：获取调度列表
- `GetTaskTemplateScheduleById()`：根据ID获取调度
- `SaveTaskTemplateSchedule()`：保存调度配置
- `DeleteTaskTemplateSchedule()`：删除调度配置
- `UpdateTaskTemplateScheduleStatus()`：更新调度状态

#### 2.2 控制器扩展
在 `TaskController.cs` 中添加以下接口：
- `Schedule()`：调度配置页面
- `ScheduleAdd()`：新增/编辑调度页面
- `GetTaskTemplateScheduleList()`：获取调度列表
- `GetTaskTemplateScheduleById()`：根据ID获取调度
- `SaveTaskTemplateSchedule()`：保存调度配置
- `DeleteTaskTemplateSchedule()`：删除调度配置
- `UpdateTaskTemplateScheduleStatus()`：更新调度状态

#### 2.3 前端页面实现
- 创建 `Schedule.cshtml` 页面，使用MiniUI表格展示调度列表
- 创建 `ScheduleAdd.cshtml` 页面，使用表单进行调度配置
- 集成到现有菜单系统

### 3. 数据库集成
- 使用现有的 `TaskTemplateSchedule` 表
- 确保与 `TaskTemplate` 表的外键关系

## 三、技术实现细节

### 1. 调度类型处理
- **每天**：不设置 `DayOfWeek` 和 `DayOfMonth`
- **每周**：设置 `DayOfWeek`（1-7，对应周一到周日）
- **每月**：设置 `DayOfMonth`（1-31）

### 2. 执行时间格式
- 使用 `TIME` 类型，格式为 `HH:mm:ss`

### 3. 状态管理
- `IsActive` 字段控制调度是否启用
- 只有启用的调度才会在自动生成任务时被考虑

### 4. 与现有系统集成
- 在任务模板管理页面添加调度配置入口
- 在任务自动生成逻辑中使用调度配置

## 四、实现步骤

1. **扩展BLL层**：在 `Task.cs` 中添加调度相关方法
2. **扩展控制器**：在 `TaskController.cs` 中添加调度相关接口
3. **创建前端页面**：创建 `Schedule.cshtml` 和 `ScheduleAdd.cshtml`
4. **更新菜单**：在左侧菜单中添加调度配置选项
5. **测试验证**：确保调度配置功能正常工作

## 五、预期效果

- 管理员可以通过调度配置页面管理所有任务模板的执行时间
- 支持灵活的调度规则（每天、每周、每月）
- 与现有任务自动生成系统无缝集成
- 提供直观的用户界面进行调度管理
