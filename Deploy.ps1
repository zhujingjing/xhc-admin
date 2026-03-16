# 一键部署脚本
# 功能：编译BLL项目并将必要文件复制到服务器

# 配置参数
$localProjectPath = "d:\小火柴项目\code-admin"
$bllProjectPath = "$localProjectPath\BLL"
$webAppPath = "$localProjectPath\WebAppFrame"
$serverIP = "192.168.1.1"  # 替换为你的阿里云ECS服务器IP
$serverUser = "administrator"  # 替换为服务器用户名
$serverPassword = "password"  # 替换为服务器密码
$serverWebPath = "D:\WebApp\WebAppFrame"  # 替换为服务器上的Web应用路径
$serverBLLPath = "D:\WebApp\BLL"  # 替换为服务器上的BLL路径

# 编译BLL项目
Write-Host "正在编译BLL项目..."
msbuild "$bllProjectPath\BLL.csproj" /p:Configuration=Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "BLL项目编译失败！" -ForegroundColor Red
    exit 1
}

Write-Host "BLL项目编译成功！" -ForegroundColor Green

# 收集需要部署的文件
$filesToDeploy = @()

# 1. BLL文件
$filesToDeploy += "$bllProjectPath\bin\Release\BLL.dll"
$filesToDeploy += "$bllProjectPath\bin\Release\BLL.pdb"

# 2. 前端页面文件（修改过的页面）
$filesToDeploy += "$webAppPath\Views\Admin1\Chat_Check_Add.cshtml"
$filesToDeploy += "$webAppPath\Views\Admin1\Chat_Check_List.cshtml"
# 可以根据需要添加其他修改过的前端页面

# 3. 其他必要文件（如脚本文件）
$filesToDeploy += "$webAppPath\Scripts\public.js"

Write-Host "准备部署以下文件："
foreach ($file in $filesToDeploy) {
    Write-Host "- $file"
}

# 复制文件到服务器
Write-Host "正在复制文件到服务器..."
try {
    # 使用PowerShell的New-PSDrive创建网络驱动器
    $driveLetter = "Z"
    $sharePath = "\\$serverIP\c$"
    
    # 连接到服务器
    $securePassword = ConvertTo-SecureString $serverPassword -AsPlainText -Force
    $credential = New-Object System.Management.Automation.PSCredential ($serverUser, $securePassword)
    
    New-PSDrive -Name $driveLetter -PSProvider FileSystem -Root $sharePath -Credential $credential -Persist
    
    # 确保服务器上的目录存在
    New-Item -ItemType Directory -Path "$driveLetter:\WebApp\WebAppFrame\Views\Admin1" -Force
    New-Item -ItemType Directory -Path "$driveLetter:\WebApp\BLL" -Force
    New-Item -ItemType Directory -Path "$driveLetter:\WebApp\WebAppFrame\Scripts" -Force
    
    # 复制文件
    foreach ($file in $filesToDeploy) {
        $relativePath = $file.Replace($localProjectPath, "")
        $destinationPath = "$driveLetter:\WebApp\$relativePath"
        
        # 确保目标目录存在
        $destinationDir = Split-Path $destinationPath -Parent
        if (!(Test-Path $destinationDir)) {
            New-Item -ItemType Directory -Path $destinationDir -Force
        }
        
        Copy-Item -Path $file -Destination $destinationPath -Force
        Write-Host "已复制: $file -> $destinationPath"
    }
    
    # 断开网络驱动器
    Remove-PSDrive -Name $driveLetter
    
    Write-Host "部署完成！" -ForegroundColor Green
} catch {
    Write-Host "部署失败: $($_.Exception.Message)" -ForegroundColor Red
    # 尝试断开网络驱动器
    try {
        Remove-PSDrive -Name $driveLetter -ErrorAction SilentlyContinue
    } catch {}
    exit 1
}
