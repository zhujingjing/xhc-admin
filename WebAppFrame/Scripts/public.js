

function show_trend(tie_no, start, end) {
    var parentHeight = $(window.parent.document).find("#rightFrame").height();
    var parentWidth = $(window.parent.document).find("#rightFrame").width();

    mini.open({
        url: "/Admin1/Tie_ReplyTrend/" + tie_no + "?start=" + start + "&end=" + end,
        showMaxButton: false,
        title: "回复量-变化趋势",
        width: parentWidth - 200,
        height: 520,
        ondestroy: function (action) {
            if (action == "ok") {

            }
            else if (action == "cancel") {

            }
        }
    });
}


function UserAllInfo(uid, h, w) {
    var parentHeight = "100%";
    var parentWidth = "100%";
    var user_type = "普通用户";
    if (window.parent.frames["topFrame"]) {
        user_type = $(window.parent.frames["topFrame"].document).find("#sys_user_type").val();
    }

    if (window.parent.parent.frames["topFrame"]) {
        user_type = $(window.parent.parent.frames["topFrame"].document).find("#sys_user_type").val();
    }
        
    mini.open({
        url: "/Admin1/UserAll/" + uid + "?user_type=" + user_type,
        showMaxButton: false,
        title: "全貌",
        width: parentWidth,
        height: parentHeight,
        ondestroy: function (action) {
            if (action == "ok") {

            }
            else if (action == "cancel") {

            }
        }
    });
}

function ChartAB(dtm, a, b) {
    mini.open({
        url: "/Admin1/User_Chart_AB/?dtm=" + dtm + "&uid_from=" + a + "&uid_to=" + b,
        showMaxButton: false,
        title: "聊天详情",
        width: 550,
        height: 780,
        ondestroy: function (action) {
        }
    });
}

function DateDiff_SS(start, end) {
    var rtn = '';
    var dtm1 = new Date(start);
    var dtm2 = new Date(end);
    var sp = dtm2.getTime() - dtm1.getTime();
    var ss = Math.floor(sp / 1000);
    rtn = ss;
    return rtn;
}

function DateDiff_Txt(start, end) {
    var txt = "";
    var tst = DateDiff_SS(start, end);

    var min = tst / 60;
    var hour = 0;
    var day = 0;
    //console.log(tst + '---' + min);
    if (min < 1) {
        txt = tst + "秒";
    }
    else {
        if (min >= 60) {
            hour = min / 60;
            if (hour >= 24) {
                day = hour / 24;
                if (day >= 1) {
                    txt = parseInt(day) + "天";
                }
            }
            else {
                txt = parseInt(hour) + "小时";
            }
        }
        else {
            txt = parseInt(min) + "分钟";
        }
    }
    

    return txt;
}

/**
 * 缓存管理对象
 */
var dropdownCache = {
    // 获取缓存
    get: function(category) {
        var cache = localStorage.getItem('dropdown_cache');
        if (!cache) return null;
        
        var data = JSON.parse(cache);
        var now = new Date().getTime();
        
        // 检查是否过期（24小时）
        if (now - data.timestamp > 24 * 60 * 60 * 1000) {
            this.clear();
            return null;
        }
        
        return category ? data.data[category] : data.data;
    },
    
    // 设置缓存
    set: function(data) {
        var cache = {
            timestamp: new Date().getTime(),
            data: data
        };
        localStorage.setItem('dropdown_cache', JSON.stringify(cache));
    },
    
    // 清除缓存
    clear: function(category) {
        if (category) {
            var cache = this.get();
            if (cache) {
                delete cache[category];
                this.set(cache);
            }
        } else {
            localStorage.removeItem('dropdown_cache');
        }
    }
};

/**
 * 批量加载下拉框数据
 * @param {Array} categories - 下拉框分类数组
 * @param {Function} callback - 回调函数
 */
function loadBatchDropdownOptions(categories, callback) {
    console.log('开始批量加载下拉框数据，分类:', categories);
    // 检查缓存
    var cachedData = {};
    var needLoad = [];
    
    // 检查每个分类的缓存
    for (var i = 0; i < categories.length; i++) {
        var category = categories[i];
        var cached = dropdownCache.get(category);
        if (cached) {
            console.log('从缓存获取分类数据:', category, cached);
            cachedData[category] = cached;
        } else {
            needLoad.push(category);
        }
    }
    
    console.log('需要从服务器加载的分类:', needLoad);
    console.log('从缓存获取的分类:', Object.keys(cachedData));
    
    // 如果所有数据都在缓存中，直接使用
    if (needLoad.length === 0) {
        console.log('所有数据都在缓存中，直接使用');
        if (callback) callback(cachedData);
        return;
    }
    
    // 否则请求数据
    console.log('开始请求服务器数据');
    $.ajax({
        url: "/Admin1/GetBatchDropdownOptions",
        type: "post",
        data: { categories: needLoad.join(",") },
        success: function(data) {
            console.log('服务器返回数据:', data);
            try {
                var result = JSON.parse(data);
                console.log('解析后的数据:', result);
                
                // 合并缓存数据和新数据
                for (var category in result) {
                    cachedData[category] = result[category];
                    console.log('合并分类数据:', category, result[category]);
                }
                
                // 更新缓存
                dropdownCache.set(cachedData);
                console.log('更新缓存完成');
                
                // 执行回调
                if (callback) callback(cachedData);
            } catch (error) {
                console.error('解析服务器返回数据失败:', error);
                if (callback) callback(cachedData);
            }
        },
        error: function(xhr, status, error) {
            console.error('批量加载下拉框选项失败:', error);
            console.error('请求状态:', status);
            console.error('响应内容:', xhr.responseText);
            if (callback) callback(cachedData);
        }
    });
}

/**
 * 加载单个下拉框选项
 * @param {string} category - 下拉框分类
 * @param {string} selectId - 下拉框元素ID
 * @param {boolean} useKeyAsValue - 是否使用key作为value，默认false（value和text一致）
 * @param {Function} callback - 回调函数，参数为选项数据
 */
function loadDropdownOptions(category, selectId, useKeyAsValue, callback) {
    // 先检查缓存
    var cached = dropdownCache.get(category);
    if (cached) {
        updateDropdown(selectId, cached, useKeyAsValue);
        if (callback) callback(cached);
        return;
    }
    
    // 缓存中没有，请求数据
    $.ajax({
        url: "/Admin1/GetDropdownOptions",
        type: "get",
        data: { category: category },
        success: function(data) {
            var options = JSON.parse(data);
            
            // 更新缓存
            var cacheData = dropdownCache.get() || {};
            cacheData[category] = options;
            dropdownCache.set(cacheData);
            
            // 更新下拉框
            updateDropdown(selectId, options, useKeyAsValue);
            
            // 执行回调
            if (callback) callback(options);
        },
        error: function() {
            console.error("加载下拉框选项失败");
            if (callback) callback([]);
        }
    });
}

/**
 * 更新单个下拉框
 * @param {string} selectId - 下拉框元素ID
 * @param {Array} options - 选项数据
 * @param {boolean} useKeyAsValue - 是否使用key作为value
 */
function updateDropdown(selectId, options, useKeyAsValue) {
    var selectElement = $("#" + selectId);
    
    // 清空原有选项，保留第一个默认选项（"全部"）
    var firstOption = selectElement.find("option:first");
    selectElement.empty();
    if (firstOption.length > 0) {
        selectElement.append(firstOption);
    } else {
        selectElement.append($('<option value="">全部</option>'));
    }
    
    // 添加选项
    for (var i = 0; i < options.length; i++) {
        var optionValue = useKeyAsValue ? options[i].key : options[i].value;
        selectElement.append($('<option value="' + optionValue + '">' + options[i].value + '</option>'));
    }
}

/**
 * 加载级联下拉框选项
 * @param {string} category - 子级分类名称
 * @param {string} selectId - 子级下拉框元素ID
 * @param {string} parentKey - 父级选中的 key 值
 * @param {string} parentCategory - 父级分类名称
 * @param {boolean} useKeyAsValue - 是否使用key作为value，默认false
 */
function loadCascadingDropdownOptions(category, selectId, parentKey, parentCategory, useKeyAsValue) {
    // 清空下拉框
    var selectElement = $("#" + selectId);
    selectElement.empty();
    selectElement.append($('<option value="">请选择</option>'));
    
    // 如果父级未选择，直接返回
    if (!parentKey) {
        return;
    }
    
    // 检查缓存
    var cacheKey = category + "_" + parentKey;
    var cached = dropdownCache.get(cacheKey);
    if (cached) {
        updateDropdown(selectId, cached, useKeyAsValue);
        return;
    }
    
    // 请求数据
    $.ajax({
        url: "/Admin1/GetCascadingDropdownOptions",
        type: "post",
        data: { 
            category: category, 
            parentKey: parentKey,
            parentCategory: parentCategory
        },
        success: function(data) {
            var options = JSON.parse(data);
            
            // 更新缓存
            var cacheData = dropdownCache.get() || {};
            cacheData[cacheKey] = options;
            dropdownCache.set(cacheData);
            
            // 更新下拉框
            updateDropdown(selectId, options, useKeyAsValue);
        },
        error: function() {
            console.error("加载级联下拉框选项失败");
        }
    });
}

/**
 * 刷新下拉框缓存
 * @param {string} category - 可选，指定分类，不指定则刷新所有
 */
function refreshDropdownCache(category) {
    dropdownCache.clear(category);
    if (!category) {
        mini.alert("缓存已刷新，下次加载页面时将获取最新数据");
    }
}

/**
 * 格式化日期时间为 yyyy-MM-dd HH:mm:ss 格式
 * @param {string|Date} date - 日期对象或日期字符串
 * @returns {string} 格式化后的日期时间字符串
 */
function formatDate(date) {
    if (!date) return "";
    if (typeof date === 'string') {
        // 尝试将字符串转换为日期对象
        var dateObj = new Date(date);
        if (!isNaN(dateObj.getTime())) {
            return dateObj.getFullYear() + "-" + 
                   (dateObj.getMonth() + 1).toString().padStart(2, '0') + "-" + 
                   dateObj.getDate().toString().padStart(2, '0') + " " + 
                   dateObj.getHours().toString().padStart(2, '0') + ":" + 
                   dateObj.getMinutes().toString().padStart(2, '0') + ":" + 
                   dateObj.getSeconds().toString().padStart(2, '0');
        }
        return date;
    }
    if (date instanceof Date) {
        return date.getFullYear() + "-" + 
               (date.getMonth() + 1).toString().padStart(2, '0') + "-" + 
               date.getDate().toString().padStart(2, '0') + " " + 
               date.getHours().toString().padStart(2, '0') + ":" + 
               date.getMinutes().toString().padStart(2, '0') + ":" + 
               date.getSeconds().toString().padStart(2, '0');
    }
    return String(date);
}
