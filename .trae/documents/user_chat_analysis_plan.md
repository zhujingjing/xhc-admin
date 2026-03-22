# 用户聊天和交易分析功能 - 实现计划

## 1. 功能需求分析

**需求**：在聊天对话页面的评价按钮后添加一个新功能，用于查看两个用户之间的历史聊天情况和金币积分交易情况。

**核心功能**：

* 统计两个用户之间的聊天记录数据

* 显示金币交易情况

* 显示积分交易情况

* 提供历史数据查询

## 2. 数据库表结构分析

从代码中分析得到以下表结构：

### 2.1 dbo.sum\_user 表

* **用途**：用户聊天统计数据

* **主要字段**：

  * uid: 用户ID

  * dtm: 日期时间

  * c\_login: 登录次数

  * c\_match: 匹配次数

  * c\_msg\_suc: 成功消息数

  * c\_msg\_suc\_: 成功消息数（其他类型）

  * c\_msg\_fail: 失败消息数

  * c\_msg\_fail\_: 失败消息数（其他类型）

### 2.2 dbo.coin 表

* **用途**：金币交易记录

* **主要字段**：

  * uid\_from: 发起用户ID

  * amount: 金额

  * type: 类型（增加/减少）

  * dtm: 交易时间

### 2.3 dbo.score 表

* **用途**：积分交易记录

* **主要字段**：

  * uid\_from: 发起用户ID

  * amount: 金额

  * type: 类型（增加/减少）

  * type\_dtl: 详细类型

  * dtm: 交易时间

  * comment: 备注

  * state: 状态

## 3. 实现方案

### 3.1 界面设计

**新页面：用户关系分析**

* **入口**：聊天对话页面评价按钮后添加"历史分析"按钮

* **布局**：

  * 顶部：用户信息展示（双方头像、昵称）

  * 中部：总体统计数据区域

    * 显示两个用户之间的总聊天次数、总金币交易金额（这里要做分类汇总，比如聊天金币，送礼物金币等根据实际数据来分）、总积分交易金额（做分类汇总）、第一次聊天日期，最后一次聊天日期，聊天总跨度时间（xx天），最长中断天数（xx天）

    * 使用简单的数字

  * 底部：按日期统计表格

    * 表格列：日期、聊天情况（消息数量）、金币情况（收支金额）、积分情况（收支金额）

    * 支持表格排序和分页

### 3.2 后端实现

**新增控制器方法**：

1. `Admin1Controller.cs` 中添加 `UserRelationshipAnalysis` 方法
2. 添加 `GetUserRelationshipStats` 方法获取总体统计数据
3. 添加 `GetUserRelationshipDetails` 方法获取按日期的详细统计数据

**SQL 查询设计**：

* 总体统计：查询两个用户之间的总聊天次数、总金币交易金额、总积分交易金额

* 按日期统计：查询每天的聊天次数、金币交易金额、积分交易金额

### 3.3 前端实现

**新增视图**：`UserRelationshipAnalysis.cshtml`

* 使用 Bootstrap 布局

* 简单的统计卡片展示总体数据

* 表格展示按日期的详细统计数据

* 不需要时间范围选择器，显示全部数据

* 支持表格排序和分页

## 4. 实现步骤

### 4.1 后端实现

1. **添加控制器方法**：

   * `UserRelationshipAnalysis`：页面入口

   * `GetUserRelationshipStats`：获取总体统计数据

   * `GetUserRelationshipDetails`：获取按日期的详细统计数据

2. **实现数据查询逻辑**：

   * 总体统计：计算两个用户之间的总聊天次数、总金币交易金额、总积分交易金额

   * 按日期统计：按日期分组计算每天的聊天次数、金币交易金额、积分交易金额

### 4.2 前端实现

1. **创建视图文件**：`UserRelationshipAnalysis.cshtml`
2. **设计页面布局**：用户信息、总体统计卡片、详细统计表格
3. **实现数据展示**：展示总体统计数据和按日期的详细统计数据
4. **添加交互功能**：表格排序和分页

### 4.3 集成到聊天对话页面

1. **修改聊天对话页面**：在评价按钮后添加"历史分析"按钮
2. **添加跳转逻辑**：点击按钮跳转到用户关系分析页面

## 5. 技术实现细节

### 5.1 后端数据查询

**总体统计查询**：

```sql
-- 聊天统计
SELECT 
    SUM(c_msg_suc) as total_chat_count
FROM dbo.sum_user
WHERE uid IN (@uid1, @uid2)

-- 金币交易统计
SELECT 
    SUM(CASE WHEN type = '增加' THEN amount ELSE -amount END) as total_coin_amount
FROM dbo.coin
WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)

-- 积分交易统计
SELECT 
    SUM(CASE WHEN type = '增加' THEN amount ELSE -amount END) as total_score_amount
FROM dbo.score
WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
```

**按日期统计查询**：

```sql
WITH date_range AS (
    SELECT DISTINCT CONVERT(char(10), dtm, 120) as date
    FROM (
        SELECT dtm FROM dbo.sum_user WHERE uid IN (@uid1, @uid2)
        UNION
        SELECT dtm FROM dbo.coin WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
        UNION
        SELECT dtm FROM dbo.score WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
    ) t
)
SELECT 
    d.date,
    ISNULL(c.chat_count, 0) as chat_count,
    ISNULL(co.coin_amount, 0) as coin_amount,
    ISNULL(s.score_amount, 0) as score_amount
FROM date_range d
LEFT JOIN (
    SELECT 
        CONVERT(char(10), dtm, 120) as date,
        SUM(c_msg_suc) as chat_count
    FROM dbo.sum_user
    WHERE uid IN (@uid1, @uid2)
    GROUP BY CONVERT(char(10), dtm, 120)
) c ON d.date = c.date
LEFT JOIN (
    SELECT 
        CONVERT(char(10), dtm, 120) as date,
        SUM(CASE WHEN type = '增加' THEN amount ELSE -amount END) as coin_amount
    FROM dbo.coin
    WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
    GROUP BY CONVERT(char(10), dtm, 120)
) co ON d.date = co.date
LEFT JOIN (
    SELECT 
        CONVERT(char(10), dtm, 120) as date,
        SUM(CASE WHEN type = '增加' THEN amount ELSE -amount END) as score_amount
    FROM dbo.score
    WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
    GROUP BY CONVERT(char(10), dtm, 120)
) s ON d.date = s.date
ORDER BY d.date DESC
```

### 5.2 前端数据展示

* **总体统计**：使用卡片展示三个关键指标

* **详细统计**：使用表格展示按日期的详细数据

* **排序功能**：表格支持按日期、聊天次数、金币金额、积分金额排序

* **分页功能**：表格支持分页，默认每页显示10条数据

## 6. 测试计划

1. **功能测试**：验证所有功能是否正常工作
2. **性能测试**：测试大量数据下的加载速度
3. **兼容性测试**：测试在不同浏览器中的表现

## 7. 预期效果

* 用户可以通过聊天对话页面快速查看两个用户之间的历史聊天情况

* 直观展示金币和积分的交易记录

* 提供按日期的详细统计数据，帮助用户了解每天的互动情况

* 界面简洁，操作简单，提升用户体验

## 8. 风险评估

1. **数据量较大时的性能问题**：通过分页查询和数据缓存解决
2. **查询复杂度过高导致的性能问题**：优化SQL查询，使用适当的索引
3. **前端渲染大量数据时的性能问题**：使用分页和虚拟滚动解决

## 9. 后续优化方向

1. 添加导出功能，支持导出统计数据为Excel
2. 增加更多维度的统计分析，如聊天时段分析
3. 添加用户互动热度分析，展示互动频率变化趋势
4. 集成机器学习模型，预测用户互动趋势

