# 一键部署脚本（使用Robocopy）
# 功能：编译BLL项目并使用Robocopy复制必要文件到服务器

# 配置参数
$localProjectPath = "d:\小火柴项目\code-admin"
$bllProjectPath = "$localProjectPath\BLL"
$webAppPath = "$localProjectPath\WebAppFrame"
$serverIP = "192.168.1.1"  # 替换为你的阿里云ECS服务器IP
$serverShare = "WebApp"  # 替换为服务器上的共享文件夹名称
$serverUser = "administrator"  # 替换为服务器用户名
$serverPassword = "password"  # 替换为服务器密码

# 编译BLL项目
Write-Host "正在编译BLL项目..."
msbuild "$bllProjectPath\BLL.csproj" /p:Configuration=Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "BLL项目编译失败！" -ForegroundColor Red
    exit 1
}

Write-Host "BLL项目编译成功！" -ForegroundColor Green

# 构建服务器路径
$serverPath = "\\$serverIP\$serverShare"

# 映射网络驱动器
Write-Host "正在连接到服务器..."
try {
    $securePassword = ConvertTo-SecureString $serverPassword -AsPlainText -Force
    $credential = New-Object System.Management.Automation.PSCredential ($serverUser, $securePassword)
    
    # 使用net use命令映射网络驱动器
    net use Z: $serverPath /user:$serverUser $serverPassword
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "连接服务器失败！" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "连接服务器成功！" -ForegroundColor Green
    
    # 复制BLL文件
    Write-Host "正在复制BLL文件..."
    Robocopy "$bllProjectPath\bin\Release" "Z:\BLL" BLL.dll BLL.pdb /E /Z /COPYALL /R:3 /W:1
    
    # 复制前端页面文件
    Write-Host "正在复制前端页面文件..."
    Robocopy "$webAppPath\Views\Admin1" "Z:\WebAppFrame\Views\Admin1" Chat_Check_Add.cshtml Chat_Check_List.cshtml /E /Z /COPYALL /R:3 /W:1
    
    # 复制脚本文件
    Write-Host "正在复制脚本文件..."
    Robocopy "$webAppPath\Scripts" "Z:\WebAppFrame\Scripts" public.js /E /Z /COPYALL /R:3 /W:1
    
    # 断开网络驱动器
    net use Z: /delete
    
    Write-Host "部署完成！" -ForegroundColor Green
} catch {
    Write-Host "部署失败: $($_.Exception.Message)" -ForegroundColor Red
    # 尝试断开网络驱动器
    try {
        net use Z: /delete /y 2>$null
    } catch {}
    exit 1
}
