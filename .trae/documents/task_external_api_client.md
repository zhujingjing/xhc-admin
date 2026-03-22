# 任务管理系统外部API调用指南

## 1. API接口说明

### 接口路径
`/Task/ExternalTaskGeneration`

### 接口方法
`POST`

### 接口参数
- `businessType`：业务类型（字符串）
- `businessId`：业务ID（字符串）
- `businessParams`：业务参数（JSON字符串）

### 接口响应
标准JSON格式，包含状态码和消息：
```json
{
  "State": "1", // 1表示成功，0表示失败
  "Msg": "任务生成成功" // 响应消息
}
```

## 2. C#客户端调用代码

### 使用HttpClient调用

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class TaskApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public TaskApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// 生成外部任务
    /// </summary>
    /// <param name="businessType">业务类型</param>
    /// <param name="businessId">业务ID</param>
    /// <param name="businessParams">业务参数</param>
    /// <returns></returns>
    public async Task<ApiResponse> GenerateExternalTask(string businessType, string businessId, Dictionary<string, string> businessParams)
    {
        try
        {
            // 构建请求参数
            var parameters = new Dictionary<string, string>
            {
                { "businessType", businessType },
                { "businessId", businessId }
            };

            // 如果有业务参数，将其转换为JSON字符串
            if (businessParams != null && businessParams.Count > 0)
            {
                parameters["businessParams"] = JsonConvert.SerializeObject(businessParams);
            }

            // 创建表单内容
            var content = new FormUrlEncodedContent(parameters);

            // 发送请求
            var response = await _httpClient.PostAsync($"{_baseUrl}/Task/ExternalTaskGeneration", content);

            // 读取响应内容
            var responseContent = await response.Content.ReadAsStringAsync();

            // 解析响应
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
            return apiResponse;
        }
        catch (Exception ex)
        {
            return new ApiResponse { State = "0", Msg = "调用失败: " + ex.Message };
        }
    }
}

public class ApiResponse
{
    public string State { get; set; }
    public string Msg { get; set; }
}

// 使用示例
class Program
{
    static async Task Main(string[] args)
    {
        // 初始化客户端
        var client = new TaskApiClient("http://localhost:8080");

        // 准备业务参数
        var businessParams = new Dictionary<string, string>
        {
            { "orderId", "123456" },
            { "customerName", "张三" },
            { "amount", "1000" },
            { "orderStatus", "待处理" }
        };

        // 调用API
        var response = await client.GenerateExternalTask("Order", "ORD-2023-001", businessParams);

        // 处理响应
        Console.WriteLine($"状态: {response.State}");
        Console.WriteLine($"消息: {response.Msg}");
    }
}
```

## 3. JavaScript客户端调用代码

### 使用fetch API调用

```javascript
// 调用外部任务生成API
async function generateExternalTask(businessType, businessId, businessParams) {
    try {
        // 构建请求参数
        const formData = new FormData();
        formData.append('businessType', businessType);
        formData.append('businessId', businessId);
        
        // 如果有业务参数，将其转换为JSON字符串
        if (businessParams && Object.keys(businessParams).length > 0) {
            formData.append('businessParams', JSON.stringify(businessParams));
        }

        // 发送请求
        const response = await fetch('/Task/ExternalTaskGeneration', {
            method: 'POST',
            body: formData
        });

        // 解析响应
        const result = await response.json();
        return result;
    } catch (error) {
        console.error('调用API失败:', error);
        return { State: '0', Msg: '调用失败: ' + error.message };
    }
}

// 使用示例
async function testApi() {
    // 准备业务参数
    const businessParams = {
        orderId: '123456',
        customerName: '张三',
        amount: '1000',
        orderStatus: '待处理'
    };

    // 调用API
    const response = await generateExternalTask('Order', 'ORD-2023-001', businessParams);

    // 处理响应
    console.log('状态:', response.State);
    console.log('消息:', response.Msg);

    // 显示结果
    if (response.State === '1') {
        alert('任务生成成功');
    } else {
        alert('任务生成失败: ' + response.Msg);
    }
}

// 调用测试函数
testApi();
```

### 使用axios调用

```javascript
// 引入axios
// npm install axios
// 或使用CDN: <script src="https://cdn.jsdelivr.net/npm/axios/dist/axios.min.js"></script>

// 调用外部任务生成API
async function generateExternalTask(businessType, businessId, businessParams) {
    try {
        // 构建请求参数
        const params = {
            businessType: businessType,
            businessId: businessId
        };

        // 如果有业务参数，将其转换为JSON字符串
        if (businessParams && Object.keys(businessParams).length > 0) {
            params.businessParams = JSON.stringify(businessParams);
        }

        // 发送请求
        const response = await axios.post('/Task/ExternalTaskGeneration', params);

        // 返回响应数据
        return response.data;
    } catch (error) {
        console.error('调用API失败:', error);
        return { State: '0', Msg: '调用失败: ' + error.message };
    }
}

// 使用示例
async function testApi() {
    // 准备业务参数
    const businessParams = {
        orderId: '123456',
        customerName: '张三',
        amount: '1000',
        orderStatus: '待处理'
    };

    // 调用API
    const response = await generateExternalTask('Order', 'ORD-2023-001', businessParams);

    // 处理响应
    console.log('状态:', response.State);
    console.log('消息:', response.Msg);

    // 显示结果
    if (response.State === '1') {
        alert('任务生成成功');
    } else {
        alert('任务生成失败: ' + response.Msg);
    }
}

// 调用测试函数
testApi();
```

## 4. 业务类型与任务模板映射

当前系统中的业务类型与任务模板映射关系如下：

| 业务类型 | 任务模板ID | 说明 |
|---------|-----------|------|
| Order   | 模板ID1   | 订单相关任务 |
| Customer | 模板ID2  | 客户相关任务 |
| Product | 模板ID3   | 产品相关任务 |

**注意**：实际项目中，需要根据系统中的真实模板ID替换上述映射关系。

## 5. 错误处理

API可能返回的错误信息：

| 错误消息 | 说明 |
|---------|------|
| 业务类型不能为空 | businessType参数为空 |
| 业务ID不能为空 | businessId参数为空 |
| 业务参数格式错误 | businessParams不是有效的JSON格式 |
| 任务生成失败 | 内部处理失败，可能是模板不存在或其他原因 |

## 6. 安全性考虑

1. **API密钥验证**：建议在生产环境中添加API密钥验证，防止未授权调用
2. **参数验证**：客户端和服务端都应该对参数进行验证
3. **错误处理**：不要在错误消息中暴露敏感信息
4. **日志记录**：记录API调用日志，便于排查问题

## 7. 性能优化

1. **批量处理**：如果需要同时生成多个任务，考虑实现批量接口
2. **缓存**：对业务类型与模板的映射关系进行缓存，提高性能
3. **异步处理**：对于耗时的任务生成操作，考虑使用异步处理

## 8. 测试建议

1. **单元测试**：测试API的参数验证和错误处理
2. **集成测试**：测试API与数据库的交互
3. **负载测试**：测试API在高并发情况下的性能
4. **安全测试**：测试API的安全性，防止注入攻击等