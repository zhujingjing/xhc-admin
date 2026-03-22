# 用户交易情况分析 - 实现计划

## 任务分解和优先级

### [/] 任务1: 分析表结构和数据模型
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 分析 dbo.coin 和 dbo.score 表的结构
  - 理解字段含义和数据关系
  - 确定交易类型和分类方式
- **Success Criteria**:
  - 明确表结构和字段含义
  - 确定交易类型分类标准
- **Test Requirements**:
  - `programmatic` TR-1.1: 验证表结构字段存在性
  - `human-judgement` TR-1.2: 确认交易类型分类合理
- **Notes**:
  - 从现有代码中分析表结构
  - 重点关注 uid_from, uid_to, type, type_dtl, amount, dtm 字段

### [ ] 任务2: 开发金币交易统计功能
- **Priority**: P1
- **Depends On**: 任务1
- **Description**:
  - 实现从用户A到用户B的金币消耗统计
  - 实现从用户B到用户A的金币消耗统计
  - 按交易类型（聊天、礼物等）分类统计
- **Success Criteria**:
  - 正确计算金币总消耗
  - 准确分类聊天和礼物等消耗
- **Test Requirements**:
  - `programmatic` TR-2.1: 验证统计数据准确性
  - `human-judgement` TR-2.2: 确认分类逻辑合理
- **Notes**:
  - 使用 type = '减少' 作为消耗记录
  - 按 type_dtl 字段分类

### [ ] 任务3: 开发积分交易统计功能
- **Priority**: P1
- **Depends On**: 任务1
- **Description**:
  - 实现从用户A到用户B的积分活动统计
  - 实现从用户B到用户A的积分活动统计
  - 按交易类型（聊天、礼物等）分类统计
- **Success Criteria**:
  - 正确计算积分总额
  - 准确分类聊天和礼物等积分
- **Test Requirements**:
  - `programmatic` TR-3.1: 验证统计数据准确性
  - `human-judgement` TR-3.2: 确认分类逻辑合理
- **Notes**:
  - 使用 type = '增加' 作为积分记录
  - 按 type_dtl 字段分类

### [ ] 任务4: 开发日期表格统计功能
- **Priority**: P1
- **Depends On**: 任务2, 任务3
- **Description**:
  - 实现按日期统计金币消耗和积分产生
  - 生成日期、金币消耗、积分产生的表格
- **Success Criteria**:
  - 正确按日期分组统计
  - 表格数据完整准确
- **Test Requirements**:
  - `programmatic` TR-4.1: 验证日期分组正确性
  - `human-judgement` TR-4.2: 确认表格展示合理
- **Notes**:
  - 使用 CONVERT(char(10), dtm, 120) 格式化日期
  - 合并金币和积分数据

### [ ] 任务5: 集成到聊天对话页面
- **Priority**: P2
- **Depends On**: 任务2, 任务3, 任务4
- **Description**:
  - 在聊天对话页面添加交易情况入口
  - 实现点击按钮查看交易统计
  - 集成前端展示界面
- **Success Criteria**:
  - 入口位置正确（评价按钮后）
  - 点击后能正确显示统计数据
- **Test Requirements**:
  - `programmatic` TR-5.1: 验证入口功能正常
  - `human-judgement` TR-5.2: 确认界面美观易用
- **Notes**:
  - 考虑页面布局和用户体验

### [ ] 任务6: 测试和优化
- **Priority**: P2
- **Depends On**: 任务5
- **Description**:
  - 测试统计功能的准确性
  - 优化查询性能
  - 处理边界情况
- **Success Criteria**:
  - 所有统计功能正确
  - 查询性能良好
  - 边界情况处理合理
- **Test Requirements**:
  - `programmatic` TR-6.1: 验证各种场景下的正确性
  - `human-judgement` TR-6.2: 确认系统稳定性
- **Notes**:
  - 测试无交易记录的情况
  - 优化大数量级数据的查询性能

## 技术方案

### 数据库查询方案

#### 1. 金币交易统计
- **从A到B的金币消耗**:
  ```sql
  SELECT 
      'A->B' as direction,
      type_dtl as transaction_type,
      SUM(amount) as amount
  FROM dbo.coin
  WHERE uid_from = 'userA' AND uid_to = 'userB' AND type = '减少'
  GROUP BY type_dtl
  ORDER BY amount DESC
  ```

- **从B到A的金币消耗**:
  ```sql
  SELECT 
      'B->A' as direction,
      type_dtl as transaction_type,
      SUM(amount) as amount
  FROM dbo.coin
  WHERE uid_from = 'userB' AND uid_to = 'userA' AND type = '减少'
  GROUP BY type_dtl
  ORDER BY amount DESC
  ```

#### 2. 积分交易统计
- **从A到B的积分活动**:
  ```sql
  SELECT 
      'A->B' as direction,
      type_dtl as transaction_type,
      SUM(amount) as amount
  FROM dbo.score
  WHERE uid_from = 'userA' AND uid_to = 'userB' AND type = '增加'
  GROUP BY type_dtl
  ORDER BY amount DESC
  ```

- **从B到A的积分活动**:
  ```sql
  SELECT 
      'B->A' as direction,
      type_dtl as transaction_type,
      SUM(amount) as amount
  FROM dbo.score
  WHERE uid_from = 'userB' AND uid_to = 'userA' AND type = '增加'
  GROUP BY type_dtl
  ORDER BY amount DESC
  ```

#### 3. 日期表格统计
```sql
WITH all_transactions AS (
    SELECT 
        CONVERT(char(10), dtm, 120) as date,
        '金币' as transaction_type,
        CASE 
            WHEN uid_from = 'userA' AND uid_to = 'userB' AND type = '减少' THEN amount
            WHEN uid_from = 'userB' AND uid_to = 'userA' AND type = '减少' THEN amount
            ELSE 0 
        END as coin_amount,
        0 as score_amount
    FROM dbo.coin
    WHERE ((uid_from = 'userA' AND uid_to = 'userB') OR (uid_from = 'userB' AND uid_to = 'userA')) AND type = '减少'
    
    UNION ALL
    
    SELECT 
        CONVERT(char(10), dtm, 120) as date,
        '积分' as transaction_type,
        0 as coin_amount,
        CASE 
            WHEN uid_from = 'userA' AND uid_to = 'userB' AND type = '增加' THEN amount
            WHEN uid_from = 'userB' AND uid_to = 'userA' AND type = '增加' THEN amount
            ELSE 0 
        END as score_amount
    FROM dbo.score
    WHERE ((uid_from = 'userA' AND uid_to = 'userB') OR (uid_from = 'userB' AND uid_to = 'userA')) AND type = '增加'
)
SELECT 
    date,
    SUM(coin_amount) as 金币消耗,
    SUM(score_amount) as 积分产生
FROM all_transactions
GROUP BY date
ORDER BY date DESC
```

### 实现步骤
1. 首先实现数据库查询功能
2. 开发后端API接口
3. 集成到前端聊天对话页面
4. 测试和优化

### 预期交付
- 完整的交易统计功能
- 集成到聊天对话页面的入口
- 详细的统计数据展示
- 按日期的交易记录表格