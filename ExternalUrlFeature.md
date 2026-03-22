# 外部链接功能使用说明

## 1. 功能概述

外部链接功能允许在任务模板中配置外部URL，当任务生成时，系统会根据模板配置自动生成完整的外部链接，并存储在任务的FullExternalUrl字段中。客服可以通过点击任务详情页面中的链接跳转到外部系统进行工作。

## 2. 配置步骤

### 2.1 在任务模板中配置外部链接

1. 打开任务模板管理页面
2. 点击"新增模板"或"编辑模板"
3. 在模板编辑页面中，找到外部链接相关配置：
   - **外部URL**：输入外部系统的基础地址，例如 `https://example.com/task`
   - **URL参数**：输入URL参数模板，支持动态参数，例如 `id={BusinessId}&type={BusinessType}`
   - **启用外部URL**：选择是否启用外部链接功能
4. 保存模板配置

### 2.2 支持的动态参数

在URL参数模板中，可以使用以下动态参数：
- `{BusinessType}`：替换为任务的业务类型
- `{BusinessId}`：替换为任务的业务ID

## 3. 任务生成流程

1. 当任务通过模板生成时，系统会检查模板是否启用了外部链接
2. 如果启用了外部链接，系统会根据模板配置生成完整的外部URL
3. 系统会动态替换URL参数模板中的变量
4. 生成的完整URL会存储在任务的FullExternalUrl字段中

## 4. 使用方法

1. 打开任务详情页面
2. 在"任务基本信息"部分，找到"外部链接"字段
3. 点击"点击访问外部链接"链接
4. 系统会在新窗口打开完整的外部URL

## 5. 示例

### 5.1 配置示例

**任务模板配置：**
- 外部URL：`https://example.com/task`
- URL参数：`id={BusinessId}&type={BusinessType}`
- 启用外部URL：是

**任务生成时：**
- BusinessType：`order`
- BusinessId：`12345`

**生成的外部URL：**
`https://example.com/task?id=12345&type=order`

### 5.2 前端显示

在任务详情页面中，外部链接会显示为：
```html
<a href="https://example.com/task?id=12345&type=order" target="_blank" style="color: #409eff; text-decoration: underline;">点击访问外部链接</a>
```

## 6. 技术实现

### 6.1 数据库修改

- 在TaskTemplate表中添加了以下字段：
  - ExternalUrl (nvarchar(2000))：外部URL基础地址
  - ExternalUrlParams (nvarchar(1000))：URL参数模板
  - ExternalUrlEnabled (bit)：是否启用外部URL

- 在Task表中保留了FullExternalUrl字段，删除了其他外部链接相关字段

### 6.2 代码修改

- 修改了BLL/Task.cs中的任务模板相关方法，支持外部链接配置
- 添加了GenerateExternalUrlFromTemplate方法，用于从任务模板生成外部链接
- 修改了任务生成逻辑，在任务创建时生成并存储完整的外部URL
- 修改了前端页面，在任务模板编辑页面添加了外部链接配置字段，在任务详情页面显示外部链接

## 7. 注意事项

1. 外部URL必须是有效的URL格式
2. URL参数模板中的动态参数必须使用正确的格式：`{参数名}`
3. 任务生成时，业务类型和业务ID必须正确传递，才能生成正确的外部链接
4. 如果任务模板没有启用外部链接，任务详情页面不会显示外部链接