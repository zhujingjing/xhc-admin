# User_Coin页面金币来源和消耗统计完善方案

## 一、现状分析

### 1.1 当前页面结构
**顶部统计区域** (第229-233行):
- 总金币: `@ViewData["coin_total"]`
- 可用金币: `@ViewData["coin_canuse"]`
- 已用金币: `@ViewData["coin_used"]`

**数据表格区域**:
- 左侧表格: 金币增加记录 (type='增加')
- 右侧表格: 金币减少记录 (type='减少')

### 1.2 数据表结构
**coin表主要字段**:
- `id`: 主键
- `dtm`: 操作时间
- `uid_from`: 来源用户ID
- `uid_to`: 目标用户ID
- `amount`: 金币数量
- `type`: 类型 ('增加' 或 '减少')
- `type_dtl`: 类型详情 (如'购买'、'签到'、'发送礼物'、'发送语音'等)
- `comment`: 备注
- `rel_id`: 关联ID

### 1.3 现有金币来源和消耗类型
**金币来源类型** (type='增加'):
- 购买
- 签到
- 任务奖励-会员
- 积分兑换

**金币消耗类型** (type='减少'):
- 发送礼物
- 发送语音
- 添加好友
- 消息支出

## 二、需求说明

在顶部统计区域增加两个新板块:
1. **金币来源统计**: 显示金币增加的前3位来源,按金币数量由高到低排序
2. **金币消耗统计**: 显示金币消耗的前3位类型,按金币数量由高到低排序

## 三、实施方案

### 3.1 后端实现

#### 3.1.1 在Admin1Controller.cs中新增统计方法

**位置**: `d:\小火柴项目\code-admin\WebAppFrame\Controllers\Admin1Controller.cs`

在User_Coin方法中增加统计逻辑:

```csharp
// 在User_Coin方法中,第3706行后添加以下代码:

// 获取金币来源统计(前3位)
DataTable coinSourceStats = bll.GetCoinSourceStats(id);
ViewData["coin_source_stats"] = coinSourceStats;

// 获取金币消耗统计(前3位)
DataTable coinConsumeStats = bll.GetCoinConsumeStats(id);
ViewData["coin_consume_stats"] = coinConsumeStats;
```

#### 3.1.2 在BLL.cs中新增业务逻辑方法

**位置**: `d:\小火柴项目\code-admin\BLL\BLL.cs`

在GetUserCoin_Used方法后添加以下两个方法:

```csharp
// 获取金币来源统计(前3位)
public DataTable GetCoinSourceStats(string uid)
{
    string strSql = @"
        SELECT TOP 3
            type_dtl as source_type,
            SUM(amount) as total_amount,
            COUNT(*) as count
        FROM dbo.coin
        WHERE uid_from = '{0}' AND type = '增加'
        GROUP BY type_dtl
        ORDER BY total_amount DESC
    ";
    strSql = string.Format(strSql, uid);
    DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
    return dt;
}

// 获取金币消耗统计(前3位)
public DataTable GetCoinConsumeStats(string uid)
{
    string strSql = @"
        SELECT TOP 3
            type_dtl as consume_type,
            SUM(amount) as total_amount,
            COUNT(*) as count
        FROM dbo.coin
        WHERE uid_from = '{0}' AND type = '减少'
        GROUP BY type_dtl
        ORDER BY total_amount DESC
    ";
    strSql = string.Format(strSql, uid);
    DataTable dt = DBHelper.SqlHelper.GetDataTable(strSql);
    return dt;
}
```

### 3.2 前端实现

#### 3.2.1 修改User_Coin.cshtml页面

**位置**: `d:\小火柴项目\code-admin\WebAppFrame\Views\Admin1\User_Coin.cshtml`

**修改点1**: 在CSS样式中添加新的样式类(第58行后):

```css
.stats-box {
    display: inline-block;
    vertical-align: top;
    min-width: 150px;
    padding: 10px;
    margin: 0 5px;
    border: 1px solid #e0e0e0;
    border-radius: 5px;
    background-color: #f9f9f9;
}

.stats-title {
    font-size: 16px;
    font-weight: bold;
    color: #333;
    margin-bottom: 8px;
    text-align: center;
}

.stats-item {
    font-size: 14px;
    margin: 5px 0;
    padding: 3px 0;
    border-bottom: 1px dashed #ddd;
}

.stats-item:last-child {
    border-bottom: none;
}

.stats-label {
    color: #666;
}

.stats-value {
    color: #ff6a00;
    font-weight: bold;
    float: right;
}
```

**修改点2**: 修改顶部统计区域(第229-233行),增加两个新板块:

```html
<div id="desc" style="padding: 15px; margin-bottom: 5px; border: solid 1px #dcdcdc;">
    <!-- 原有的三个统计项 -->
    <span class="mny">总金币:<br /><span class="gray">@ViewData["coin_total"]</span></span>
    <span class="mny">可用:<br /><span class="gray">@ViewData["coin_canuse"]</span></span>
    <span class="mny">已用:<br /><span class="gray">@ViewData["coin_used"]</span></span>

    <!-- 新增:金币来源统计 -->
    <div class="stats-box">
        <div class="stats-title">金币来源(前3)</div>
        @{
            var coinSourceStats = ViewData["coin_source_stats"] as System.Data.DataTable;
            if (coinSourceStats != null && coinSourceStats.Rows.Count > 0)
            {
                for (int i = 0; i < coinSourceStats.Rows.Count; i++)
                {
                    var row = coinSourceStats.Rows[i];
                    <div class="stats-item">
                        <span class="stats-label">@row["source_type"]</span>
                        <span class="stats-value">@row["total_amount"]</span>
                    </div>
                }
            }
            else
            {
                <div class="stats-item" style="text-align:center; color:#999;">暂无数据</div>
            }
        }
    </div>

    <!-- 新增:金币消耗统计 -->
    <div class="stats-box">
        <div class="stats-title">金币消耗(前3)</div>
        @{
            var coinConsumeStats = ViewData["coin_consume_stats"] as System.Data.DataTable;
            if (coinConsumeStats != null && coinConsumeStats.Rows.Count > 0)
            {
                for (int i = 0; i < coinConsumeStats.Rows.Count; i++)
                {
                    var row = coinConsumeStats.Rows[i];
                    <div class="stats-item">
                        <span class="stats-label">@row["consume_type"]</span>
                        <span class="stats-value">@row["total_amount"]</span>
                    </div>
                }
            }
            else
            {
                <div class="stats-item" style="text-align:center; color:#999;">暂无数据</div>
            }
        }
    </div>
</div>
```

**修改点3**: 调整页面高度计算(第69-70行),因为顶部区域变高了:

```javascript
// 修改前:
$("#datagrid1").height(parentHeight - 196);
$("#datagrid2").height(parentHeight - 196);

// 修改后:
$("#datagrid1").height(parentHeight - 280);
$("#datagrid2").height(parentHeight - 280);
```

## 四、实现步骤

### 步骤1: 修改BLL.cs
- 在`d:\小火柴项目\code-admin\BLL\BLL.cs`中添加`GetCoinSourceStats`和`GetCoinConsumeStats`方法

### 步骤2: 修改Admin1Controller.cs
- 在`d:\小火柴项目\code-admin\WebAppFrame\Controllers\Admin1Controller.cs`的`User_Coin`方法中调用新增的BLL方法
- 将统计结果传递给ViewData

### 步骤3: 修改User_Coin.cshtml
- 添加新的CSS样式类
- 修改顶部统计区域HTML结构,增加金币来源和消耗统计板块
- 调整页面高度计算

### 步骤4: 测试验证
- 测试页面显示是否正常
- 验证统计数据是否正确
- 确认排序是否正确(按金币数量由高到低)
- 检查前3位数据是否正确显示

## 五、SQL语句说明

### 5.1 金币来源统计SQL
```sql
SELECT TOP 3
    type_dtl as source_type,
    SUM(amount) as total_amount,
    COUNT(*) as count
FROM dbo.coin
WHERE uid_from = '{uid}' AND type = '增加'
GROUP BY type_dtl
ORDER BY total_amount DESC
```

### 5.2 金币消耗统计SQL
```sql
SELECT TOP 3
    type_dtl as consume_type,
    SUM(amount) as total_amount,
    COUNT(*) as count
FROM dbo.coin
WHERE uid_from = '{uid}' AND type = '减少'
GROUP BY type_dtl
ORDER BY total_amount DESC
```

## 六、注意事项

1. **编码格式**: 所有代码文件必须使用UTF-8格式保存
2. **页面顶部**: 确保页面顶部没有实体字符,避免出现空行
3. **数据为空处理**: 当用户没有金币记录时,显示"暂无数据"
4. **样式一致性**: 新增的统计板块样式要与现有页面风格保持一致
5. **性能考虑**: 使用TOP 3限制查询结果,避免全表扫描

## 七、预期效果

修改后的页面顶部统计区域将显示:
- 总金币、可用、已用 (原有)
- 金币来源统计(前3位) - 显示来源类型和对应金币数量
- 金币消耗统计(前3位) - 显示消耗类型和对应金币数量

所有数据按金币数量由高到低排序,便于管理员快速了解用户的金币使用情况。
