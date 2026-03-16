# 确保文件路径正确
$filePath = "d:\小火柴项目\code-admin\WebAppFrame\Views\Admin1\User_Record_Add.cshtml"

# 确保目录存在
$directory = [System.IO.Path]::GetDirectoryName($filePath)
if (-not (Test-Path -Path $directory)) {
    New-Item -ItemType Directory -Path $directory -Force
}

# 定义文件内容
$content = @'
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8" />
    <title>新增用户记录</title>
    <script src="~/Scripts/jquery.min.js"></script>
    <script src="~/Scripts/miniui/boot.js" type="text/javascript"></script>
    <script src="~/Scripts/Global.js"></script>
    <link href="~/Styles/Style_chart.css" rel="stylesheet" />
    <link href="~/Styles/public.css" rel="stylesheet" />
    <style type="text/css">
        body {
            font-family: tahoma, arial, sans-serif;
            font-size: 12px;
            margin: 5px;
        }
        .form-container {
            width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 5px;
            background-color: #f9f9f9;
        }
        .form-item {
            margin-bottom: 15px;
        }
        .form-label {
            display: inline-block;
            width: 100px;
            text-align: right;
            margin-right: 10px;
            font-weight: bold;
        }
        .mini-textbox, .mini-combobox, .mini-textarea, .mini-datepicker {
            width: 400px;
            border: 1px solid #b3d9ff;
            border-radius: 3px;
        }
        .mini-textbox {
            height: 35px;
        }
        .mini-textarea {
            height: 80px;
        }
        .button-container {
            display: flex;
            justify-content: flex-end;
            margin-top: 20px;
            width: 510px;
        }
        .mini-button {
            width: 100px;
            margin-left: 10px;
            text-align: center;
            padding: 8px 0;
        }
    </style>
    <script type="text/javascript">
        var form;
        var uid = '@ViewData["uid"]';
        var id = '@ViewData["id"]';
        
        $(function () {
            mini.parse();
            form = new mini.Form("form1");
            
            // 根据是否有id来设置页面标题
            var pageTitle = document.getElementById("pageTitle");
            if (id) {
                pageTitle.textContent = "修改用户记录";
            }
            
            // 设置日期默认值为当前日期（仅在新增模式下）
            var recordDate = mini.get("record_date");
            if (recordDate && !id) {
                recordDate.setValue(new Date());
            }
            
            // 如果是编辑模式，加载数据
            if (id) {
                loadData(id);
            }
        });
        
        function loadData(id) {
            $.ajax({
                url: "/Admin1/GetUserRecordById",
                type: "GET",
                data: { id: id },
                dataType: "json",
                success: function (result) {
                    if (result) {
                        // 确保日期字段正确处理
                        if (result.record_date) {
                            var recordDate = mini.get("record_date");
                            if (recordDate) {
                                // 如果是字符串格式，转换为Date对象
                                if (typeof result.record_date === "string") {
                                    // 处理yyyy-MM-dd格式的日期字符串
                                    var dateParts = result.record_date.split("-");
                                    if (dateParts.length === 3) {
                                        var year = parseInt(dateParts[0]);
                                        var month = parseInt(dateParts[1]) - 1; // 月份从0开始
                                        var day = parseInt(dateParts[2]);
                                        result.record_date = new Date(year, month, day);
                                    }
                                }
                                recordDate.setValue(result.record_date);
                            }
                        }
                        form.setData(result);
                    }
                },
                error: function (xhr, status, error) {
                    alert("加载数据失败: " + error);
                }
            });
        }
        
        function saveData() {
            form.validate();
            if (form.isValid() === false) return;
            
            var data = form.getData();
            data.uid = uid;
            // 从框架页面顶部获取操作人信息
            var operator_name = "";
            if (window.parent.frames["topFrame"]) {
                operator_name = $(window.parent.frames["topFrame"].document).find("#sys_user_name").val();
            }
            if (window.parent.parent.frames["topFrame"]) {
                operator_name = $(window.parent.parent.frames["topFrame"].document).find("#sys_user_name").val();
            }
            data.operator_name = operator_name;
            // 在编辑模式下添加id
            if (id) {
                data.id = id;
            }
            var json = mini.encode(data);
            
            var url = "/Admin1/AddUserRecord";
            if (id) {
                url = "/Admin1/UpdateUserRecord";
            }
            
            $.ajax({
                url: url,
                type: "POST",
                data: { send: json },
                dataType: "json",
                success: function (result) {
                    try {
                        if (result.state == "1" || result.state == 1) {
                            alert("保存成功");
                            CloseWindow("ok");
                        } else {
                            alert("保存失败: " + (result.msg || "未知错误"));
                        }
                    } catch (e) {
                        alert("解析返回结果失败: " + e.message);
                    }
                },
                error: function (xhr, status, error) {
                    alert("请求错误: " + error);
                }
            });
        }
        
        function cancel() {
            CloseWindow();
        }
        
        function CloseWindow(action) {
            if (window.CloseOwnerWindow) {
                window.CloseOwnerWindow(action);
            } else {
                window.parent.CloseWindow(action);
            }
        }
    </script>
</head>
<body>
    <div class="form-container">
        <h2 style="text-align: center; margin-bottom: 20px;"><span id="pageTitle">新增用户记录</span></h2>
        <form id="form1">
            <div class="form-item">
                <label class="form-label">记录日期：</label>
                <input id="record_date" name="record_date" class="mini-datepicker" format="yyyy-MM-dd" required="true" />
            </div>
            <div class="form-item">
                <label class="form-label">评价：</label>
                <select name="evaluation" class="mini-combobox" style="width: 400px;">
                    <option value="">请选择</option>
                    <option value="很好">很好</option>
                    <option value="好">好</option>
                    <option value="一般">一般</option>
                    <option value="差">差</option>
                    <option value="很差">很差</option>
                </select>
            </div>
            <div class="form-item">
                <label class="form-label">处理结果：</label>
                <select name="processing_result" class="mini-combobox" style="width: 400px;">
                    <option value="">请选择</option>
                    <option value="警告并在线客服告知">警告并在线客服告知</option>
                    <option value="封号并在线客服告知">封号并在线客服告知</option>
                    <option value="在线客服告知">在线客服告知</option>

                    <option value="警告">警告</option>
                    <option value="封号">封号</option>
                    <option value="在线客服沟通">在线客服沟通</option>
                    <option value="客服微信沟通">客服微信沟通</option>
                    <option value="风险标记">风险标记</option>
                    <option value="去掉风险标记">去掉风险标记</option>
                    <option value="其他">其他</option>
                </select>
            </div>
            <div class="form-item">
                <label class="form-label">来源：</label>
                <select name="source" class="mini-combobox" style="width: 400px;">
                    <option value="">请选择</option>
                    <option value="提现">提现</option>
                    <option value="充值">充值</option>
                    <option value="投诉举报">投诉举报</option>
                    <option value="新用户">新用户</option>
                    <option value="其他">其他</option>
                </select>
            </div>
            <div class="form-item">
                <label class="form-label">简述：</label>
                <textarea name="summary" class="mini-textarea" rows="3" required="true"></textarea>
            </div>
            <div class="form-item">
                <label class="form-label">详情：</label>
                <textarea name="details" class="mini-textarea" rows="3"></textarea>
            </div>
            
            <div class="form-item">
                <label class="form-label">备注：</label>
                <textarea name="remark" class="mini-textarea" rows="3"></textarea>
            </div>
            <div class="button-container">
                <a class="mini-button" onclick="saveData">保存</a>
                <a class="mini-button" onclick="cancel">取消</a>
            </div>
        </form>
    </div>
</body>
</html>
'@

# 创建UTF-8 BOM编码的文件
$encoding = New-Object System.Text.UTF8Encoding $true
[System.IO.File]::WriteAllText($filePath, $content, $encoding)

Write-Host "文件已保存为UTF-8 BOM格式" -ForegroundColor Green

# 验证文件编码
$bytes = Get-Content $filePath -Encoding Byte -TotalCount 3
Write-Host "文件BOM: $($bytes[0].ToString('X2')) $($bytes[1].ToString('X2')) $($bytes[2].ToString('X2'))" -ForegroundColor Yellow