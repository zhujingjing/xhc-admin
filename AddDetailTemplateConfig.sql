-- 添加详情模板下拉框配置
-- 假设 chat_check_add_brife 中已经有以下选项：
-- 1. 广告
-- 2. 色情
-- 3. 暴力
-- 4. 诈骗

-- 为每个简述选项添加对应的详情模板

-- 广告相关模板
INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'ad_template_1', '多次发送广告信息，内容包含联系方式，严重影响用户体验。', 'chat_check_detail_template', '广告', 'chat_check_add_brife', 1, 1, '广告详情模板', GETDATE(), GETDATE());

INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'ad_template_2', '频繁发送推广链接，疑似广告行为。', 'chat_check_detail_template', '广告', 'chat_check_add_brife', 2, 1, '广告详情模板', GETDATE(), GETDATE());

-- 色情相关模板
INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'porn_template_1', '发送色情图片和视频，违反平台规定。', 'chat_check_detail_template', '色情', 'chat_check_add_brife', 1, 1, '色情详情模板', GETDATE(), GETDATE());

INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'porn_template_2', '使用挑逗性语言，进行色情暗示。', 'chat_check_detail_template', '色情', 'chat_check_add_brife', 2, 1, '色情详情模板', GETDATE(), GETDATE());

-- 暴力相关模板
INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'violence_template_1', '发送暴力血腥图片，严重影响用户体验。', 'chat_check_detail_template', '暴力', 'chat_check_add_brife', 1, 1, '暴力详情模板', GETDATE(), GETDATE());

INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'violence_template_2', '使用威胁性语言，进行人身攻击。', 'chat_check_detail_template', '暴力', 'chat_check_add_brife', 2, 1, '暴力详情模板', GETDATE(), GETDATE());

-- 诈骗相关模板
INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'scam_template_1', '以虚假身份进行诈骗，诱导用户转账。', 'chat_check_detail_template', '诈骗', 'chat_check_add_brife', 1, 1, '诈骗详情模板', GETDATE(), GETDATE());

INSERT INTO dbo.dropdown_config (id, [key], value, category, parent_key, parent_category, sort_order, status, comment, create_time, update_time)
VALUES (NEWID(), 'scam_template_2', '发送钓鱼链接，试图获取用户个人信息。', 'chat_check_detail_template', '诈骗', 'chat_check_add_brife', 2, 1, '诈骗详情模板', GETDATE(), GETDATE());