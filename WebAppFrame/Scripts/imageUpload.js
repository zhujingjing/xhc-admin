/**
 * 图片上传功能封装
 */

var ImageUploader = {
    // 上传图片
    upload: function(file, callback) {
        var formData = new FormData();
        formData.append('file', file);
        
        $.ajax({
            url: '/Common/UpLoadImage',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function(response) {
                if (callback) callback(response);
            },
            error: function() {
                mini.alert('图片上传失败');
                if (callback) callback(null);
            }
        });
    },
    
    // 批量上传图片
    uploadMultiple: function(files, callback) {
        var uploadedUrls = [];
        var uploadedCount = 0;
        
        if (files.length === 0) {
            if (callback) callback(uploadedUrls);
            return;
        }
        
        for (var i = 0; i < files.length; i++) {
            this.upload(files[i], function(url) {
                if (url) {
                    uploadedUrls.push(url);
                }
                uploadedCount++;
                
                if (uploadedCount === files.length) {
                    if (callback) callback(uploadedUrls);
                }
            });
        }
    },
    
    // 初始化上传区域
    initUploadArea: function(containerId, previewId, inputId, maxFiles) {
        var $container = $('#' + containerId);
        var $preview = $('#' + previewId);
        var $input = $('#' + inputId);
        
        if (!$container.length || !$preview.length || !$input.length) return;
        
        // 点击上传区域触发文件选择
        $container.on('click', function() {
            var fileInput = $('<input type="file" accept="image/*" multiple>');
            
            fileInput.on('change', function(e) {
                var files = e.target.files;
                if (files.length > 0) {
                    // 获取现有图片URLs
                    var existingUrls = $input.val() ? $input.val().split(',') : [];
                    
                    // 限制文件数量
                    if (maxFiles && (existingUrls.length + files.length) > maxFiles) {
                        mini.alert('最多只能上传' + maxFiles + '张图片，当前已有' + existingUrls.length + '张');
                        return;
                    }
                    
                    ImageUploader.uploadMultiple(files, function(newUrls) {
                        if (newUrls && newUrls.length > 0) {
                            // 合并现有URLs和新URLs
                            var allUrls = existingUrls.concat(newUrls);
                            // 更新预览
                            ImageUploader.updatePreview($preview[0], allUrls, maxFiles, inputId);
                            // 更新隐藏输入框
                            $input.val(allUrls.join(','));
                        }
                    });
                }
            });
            
            fileInput.click();
        });
        
        // 拖拽上传功能
        $container.on('dragover', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $container.addClass('upload-area-dragover');
        });
        
        $container.on('dragleave', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $container.removeClass('upload-area-dragover');
        });
        
        $container.on('drop', function(e) {
            e.preventDefault();
            e.stopPropagation();
            $container.removeClass('upload-area-dragover');
            
            var files = e.originalEvent.dataTransfer.files;
            if (files.length > 0) {
                // 过滤出图片文件
                var imageFiles = [];
                for (var i = 0; i < files.length; i++) {
                    if (files[i].type.match('image.*')) {
                        imageFiles.push(files[i]);
                    }
                }
                
                if (imageFiles.length > 0) {
                    // 获取现有图片URLs
                    var existingUrls = $input.val() ? $input.val().split(',') : [];
                    
                    // 限制文件数量
                    if (maxFiles && (existingUrls.length + imageFiles.length) > maxFiles) {
                        mini.alert('最多只能上传' + maxFiles + '张图片，当前已有' + existingUrls.length + '张');
                        return;
                    }
                    
                    ImageUploader.uploadMultiple(imageFiles, function(newUrls) {
                        if (newUrls && newUrls.length > 0) {
                            // 合并现有URLs和新URLs
                            var allUrls = existingUrls.concat(newUrls);
                            // 更新预览
                            ImageUploader.updatePreview($preview[0], allUrls, maxFiles, inputId);
                            // 更新隐藏输入框
                            $input.val(allUrls.join(','));
                        }
                    });
                }
            }
        });
    },
    
    // 更新图片预览
    updatePreview: function(previewContainer, imageUrls, maxFiles, inputId) {
        $(previewContainer).empty();
        
        $.each(imageUrls, function(index, url) {
            var item = $('<div class="upload-preview-item">');
            item.html(`
                <img src="${url}" alt="图片${index + 1}">
                <div class="remove-btn" onclick="ImageUploader.removeImage(this, '${url}', event)">×</div>
            `);
            $(previewContainer).append(item);
        });
        
        // 检查是否达到最大文件数量
        var hasReachedMax = maxFiles && imageUrls.length >= maxFiles;
        
        // 添加加号按钮（如果未达到最大数量）
        if (!hasReachedMax) {
            var addButton = $('<div class="upload-preview-item add-button">');
            addButton.html(`
                <div class="add-icon">+</div>
            `);
            $(previewContainer).append(addButton);
            
            // 为加号按钮添加点击事件
            addButton.on('click', function(e) {
                e.stopPropagation();
                // 直接创建文件输入并触发点击
                var fileInput = $('<input type="file" accept="image/*" multiple>');
                var $input = $('#' + inputId);
                
                fileInput.on('change', function(e) {
                    var files = e.target.files;
                    if (files.length > 0) {
                        // 获取现有图片URLs
                        var existingUrls = $input.val() ? $input.val().split(',') : [];
                        
                        // 限制文件数量
                        if (maxFiles && (existingUrls.length + files.length) > maxFiles) {
                            mini.alert('最多只能上传' + maxFiles + '张图片，当前已有' + existingUrls.length + '张');
                            return;
                        }
                        
                        ImageUploader.uploadMultiple(files, function(newUrls) {
                            if (newUrls && newUrls.length > 0) {
                                // 合并现有URLs和新URLs
                                var allUrls = existingUrls.concat(newUrls);
                                // 更新预览
                                ImageUploader.updatePreview(previewContainer, allUrls, maxFiles, inputId);
                                // 更新隐藏输入框
                                $input.val(allUrls.join(','));
                            }
                        });
                    }
                });
                
                fileInput.click();
            });
        }
    },
    
    // 移除图片
    removeImage: function(button, imageUrl, event) {
        // 阻止事件冒泡，防止触发上传区域的点击事件
        if (event) {
            event.stopPropagation();
        }
        
        var $item = $(button).parent();
        var $preview = $item.parent();
        var $input = $('#inputImages');
        
        $item.remove();
        
        // 更新隐藏输入框
        if ($input.length) {
            var urls = $input.val().split(',');
            var newUrls = urls.filter(function(url) {
                return url !== imageUrl;
            });
            $input.val(newUrls.join(','));
        }
    },
    
    // 从显示区域获取图片路径
    getImagePathsFromDisplay: function(displayElement) {
        var paths = [];
        $(displayElement).find('img').each(function() {
            paths.push($(this).attr('src'));
        });
        
        return paths;
    }
};
