# 测试外部API接口的PowerShell脚本

# API接口URL
$apiUrl = "http://localhost:8080/Task/ExternalTaskGeneration"

# 测试参数 - 使用用户提供的参数格式
$testParams = @{
    businessType = "提现"
    businessId = "test_tx_20260321155826"
    parameters = @{
        userId = "test_user_id"
        amount = "100"
        score = "100000"
        status = "待审核"
        applyNo = "test_tx_20260321155826"
        applyTime = "2026-03-21 15:58:26"
    }
    timestamp = "2026-03-21 15:58:26"
}

# 将参数转换为JSON字符串
$jsonParams = $testParams | ConvertTo-Json

# 创建请求体
$body = @{
    businessParams = $jsonParams
}

# 发送POST请求
try {
    $response = Invoke-RestMethod -Uri $apiUrl -Method POST -Body $body -ContentType "application/x-www-form-urlencoded"
    Write-Host "测试结果:"
    Write-Host "状态: $($response.State)"
    Write-Host "消息: $($response.Msg)"
} catch {
    Write-Host "测试失败: $($_.Exception.Message)"
}

# 测试第二种格式 - 单独传递参数
Write-Host "`n测试第二种格式 - 单独传递参数:"
$body2 = @{
    businessType = "提现"
    businessId = "test_tx_20260321155826"
    businessParams = $testParams.parameters | ConvertTo-Json
}

try {
    $response2 = Invoke-RestMethod -Uri $apiUrl -Method POST -Body $body2 -ContentType "application/x-www-form-urlencoded"
    Write-Host "测试结果:"
    Write-Host "状态: $($response2.State)"
    Write-Host "消息: $($response2.Msg)"
} catch {
    Write-Host "测试失败: $($_.Exception.Message)"
}
