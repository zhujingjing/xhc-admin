# 用户交易分析功能 - 实现计划

## 1. 功能需求分析

**需求**：在聊天对话页面的评价按钮后添加一个新功能，用于查看两个用户之间的金币和积分交易情况。

**核心功能**：

* 统计两个用户之间的总金币交易金额（分类汇总）

* 统计两个用户之间的总积分交易金额（分类汇总）

* 显示第一次交易的日期

* 显示最后一次交易的日期

* 计算总跨度时间（xx天）

* 计算最长中断天数（xx天）

## 2. 数据库表结构分析

从代码中分析得到以下表结构：

### 2.1 dbo.coin 表

* **用途**：金币交易记录

* **主要字段**：

  * uid\_from: 发起用户ID

  * uid\_to: 接收用户ID

  * amount: 金额

  * type: 类型（增加/减少）

  * type\_dtl: 详细类型（如聊天金币、送礼物金币等）

  * dtm: 交易时间

  * comment: 备注

### 2.2 dbo.score 表

* **用途**：积分交易记录

* **主要字段**：

  * uid\_from: 发起用户ID

  * uid\_to: 接收用户ID

  * amount: 金额

  * type: 类型（增加/减少）

  * type\_dtl: 详细类型

  * dtm: 交易时间

  * comment: 备注

  * state: 状态

## 3. 实现方案

### 3.1 界面设计

**新页面：用户交易分析**

* **入口**：聊天对话页面评价按钮后添加"历史分析"按钮

* **布局**：

  * 中部：统计信息区域

    * 金币交易统计：

      * A->B的金币消耗

      * B->A的金币消耗

    * 积分交易统计：

      * A->B的积分增加

      * B->A的积分增加

    * 交易时间统计（第一次交易日期、最后一次交易日期、总跨度时间、最长中断天数）

  * 底部：交易记录表格

    * 表格列：日期、交易类型、方向、金额、详细类型、备注

    * 支持表格排序和分页

### 3.2 后端实现

**新增控制器方法**：

1. `Admin1Controller.cs` 中添加 `UserTransactionAnalysis` 方法
2. 添加 `GetUserTransactionStats` 方法获取交易统计数据
3. 添加 `GetUserTransactionDetails` 方法获取交易明细数据

**SQL 查询设计**：

* 金币交易统计：查询两个用户之间的金币消耗记录，按方向分类

* 积分交易统计：查询两个用户之间的积分增加记录，按方向分类

* 交易时间统计：计算第一次交易日期、最后一次交易日期、总跨度时间、最长中断天数

### 3.3 前端实现

**新增视图**：`UserTransactionAnalysis.cshtml`

* 使用 Bootstrap 布局

* 简单的统计卡片展示总体数据

* 表格展示交易明细数据

* 不需要时间范围选择器，显示全部数据

* 支持表格排序和分页

## 4. 实现步骤

### 4.1 后端实现

1. **添加控制器方法**：
   - `UserTransactionAnalysis`：页面入口
   - `GetUserTransactionStats`：获取交易统计数据
   - `GetUserTransactionDetails`：获取交易明细数据
   - `GetUserTransactionTableData`：获取表格统计数据

2. **实现数据查询逻辑**：
   - 金币交易统计：按方向和类型分类汇总金币交易金额
   - 积分交易统计：按方向和类型分类汇总积分交易金额
   - 交易时间统计：计算交易时间相关指标
   - 表格统计：按日期和类型分类展示交易数据

### 4.2 前端实现

1. **创建视图文件**：`UserTransactionAnalysis.cshtml`
2. **设计页面布局**：用户信息、统计信息区域、交易记录表格
3. **实现数据展示**：展示统计数据和交易明细数据
4. **添加交互功能**：表格排序和分页

### 4.3 集成到聊天对话页面

1. **修改聊天对话页面**：在评价按钮后添加"交易分析"按钮
2. **添加跳转逻辑**：点击按钮跳转到用户交易分析页面

## 5. 技术实现细节

### 5.1 后端数据查询

**金币交易统计查询**：

```sql
-- A->B的金币消耗
SELECT 
    'A->B' as direction,
    type_dtl as transaction_type,
    SUM(amount) as amount
FROM dbo.coin
WHERE uid_from = @uid1 AND uid_to = @uid2 AND type = '减少'
GROUP BY type_dtl
ORDER BY amount DESC

-- B->A的金币消耗
SELECT 
    'B->A' as direction,
    type_dtl as transaction_type,
    SUM(amount) as amount
FROM dbo.coin
WHERE uid_from = @uid2 AND uid_to = @uid1 AND type = '减少'
GROUP BY type_dtl
ORDER BY amount DESC
```

**积分交易统计查询**：

```sql
-- A->B的积分增加
SELECT 
    'A->B' as direction,
    type_dtl as transaction_type,
    SUM(amount) as amount
FROM dbo.score
WHERE uid_from = @uid1 AND uid_to = @uid2 AND type = '增加'
GROUP BY type_dtl
ORDER BY amount DESC

-- B->A的积分增加
SELECT 
    'B->A' as direction,
    type_dtl as transaction_type,
    SUM(amount) as amount
FROM dbo.score
WHERE uid_from = @uid2 AND uid_to = @uid1 AND type = '增加'
GROUP BY type_dtl
ORDER BY amount DESC
```

**交易时间统计查询**：
```sql
-- 第一次和最后一次交易日期
SELECT 
    MIN(dtm) as first_transaction_date,
    MAX(dtm) as last_transaction_date
FROM (
    SELECT dtm FROM dbo.coin WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
    UNION
    SELECT dtm FROM dbo.score WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
) t

-- 计算最长中断天数
WITH transaction_dates AS (
    SELECT DISTINCT CONVERT(date, dtm) as transaction_date
    FROM (
        SELECT dtm FROM dbo.coin WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
        UNION
        SELECT dtm FROM dbo.score WHERE (uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)
    ) t
    ORDER BY transaction_date
),
date_gaps AS (
    SELECT 
        transaction_date,
        LEAD(transaction_date) OVER (ORDER BY transaction_date) as next_date,
        DATEDIFF(day, transaction_date, LEAD(transaction_date) OVER (ORDER BY transaction_date)) as gap_days
    FROM transaction_dates
)
SELECT 
    ISNULL(MAX(gap_days), 0) as max_gap_days
FROM date_gaps
```

**表格统计查询**：
```sql
-- 按日期和类型分类的交易数据
SELECT 
    CONVERT(char(10), dtm, 120) as date,
    '金币' as transaction_type,
    CASE 
        WHEN uid_from = @uid1 AND uid_to = @uid2 AND type = '减少' THEN 'A->B' 
        WHEN uid_from = @uid2 AND uid_to = @uid1 AND type = '减少' THEN 'B->A' 
        ELSE NULL 
    END as direction,
    amount,
    type_dtl as detail_type,
    comment
FROM dbo.coin
WHERE ((uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)) AND type = '减少'

UNION ALL

SELECT 
    CONVERT(char(10), dtm, 120) as date,
    '积分' as transaction_type,
    CASE 
        WHEN uid_from = @uid1 AND uid_to = @uid2 AND type = '增加' THEN 'A->B' 
        WHEN uid_from = @uid2 AND uid_to = @uid1 AND type = '增加' THEN 'B->A' 
        ELSE NULL 
    END as direction,
    amount,
    type_dtl as detail_type,
    comment
FROM dbo.score
WHERE ((uid_from = @uid1 AND uid_to = @uid2) OR (uid_from = @uid2 AND uid_to = @uid1)) AND type = '增加'

ORDER BY date DESC, transaction_type, direction
```

### 5.2 前端数据展示

* **统计信息**：使用卡片展示金币和积分的分类汇总数据

* **交易时间统计**：使用卡片展示交易时间相关指标

* **交易明细**：使用表格展示详细的交易记录

* **排序功能**：表格支持按日期、交易类型、金额排序

* **分页功能**：表格支持分页，默认每页显示10条数据

## 6. 测试计划

1. **功能测试**：验证所有功能是否正常工作
2. **性能测试**：测试大量数据下的加载速度
3. **兼容性测试**：测试在不同浏览器中的表现

## 7. 预期效果

* 用户可以通过聊天对话页面快速查看两个用户之间的金币和积分交易情况

* 直观展示金币和积分的分类汇总数据

* 提供交易时间相关的统计指标

* 界面简洁，操作简单，提升用户体验

## 8. 风险评估

1. **数据量较大时的性能问题**：通过分页查询和数据缓存解决
2. **查询复杂度过高导致的性能问题**：优化SQL查询，使用适当的索引
3. **前端渲染大量数据时的性能问题**：使用分页和虚拟滚动解决

## 9. 后续优化方向

1. 添加导出功能，支持导出统计数据为Excel
2. 增加更多维度的统计分析，如交易时段分析
3. 添加交易趋势分析，展示交易金额变化趋势
4. 集成机器学习模型，预测交易趋势

