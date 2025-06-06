-- 用户表创建脚本
-- 数据库：aspnet_demo

USE aspnet_demo;

-- 删除表（如果存在）
DROP TABLE IF EXISTS Users;

-- 创建用户表
CREATE TABLE Users (
    -- 主键ID，自增
    Id INT AUTO_INCREMENT PRIMARY KEY COMMENT '用户唯一标识',
    
    -- 用户名：唯一，长度3-20字符，非空
    Username VARCHAR(20) NOT NULL UNIQUE COMMENT '用户名，3-20字符，唯一',
    
    -- 密码：存储BCrypt哈希值，非空
    PasswordHash VARCHAR(255) NOT NULL COMMENT '密码哈希值（BCrypt加密）',
    
    -- 邮箱：唯一，符合邮箱格式，非空
    Email VARCHAR(100) NOT NULL UNIQUE COMMENT '用户邮箱，唯一',
    
    -- 注册时间：默认当前时间戳
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '注册时间',
    
    -- 更新时间：自动更新
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '最后更新时间',
    
    -- 是否激活（扩展字段）
    IsActive BOOLEAN DEFAULT TRUE COMMENT '账户是否激活'
) 
ENGINE=InnoDB 
DEFAULT CHARSET=utf8mb4 
COLLATE=utf8mb4_unicode_ci 
COMMENT='用户信息表';

-- 创建索引
CREATE INDEX idx_username ON Users(Username);
CREATE INDEX idx_email ON Users(Email);
CREATE INDEX idx_created_at ON Users(CreatedAt);

-- 添加字段约束检查
ALTER TABLE Users 
ADD CONSTRAINT chk_username_length CHECK (CHAR_LENGTH(Username) >= 3 AND CHAR_LENGTH(Username) <= 20),
ADD CONSTRAINT chk_email_format CHECK (Email REGEXP '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$');

-- 插入测试数据（可选）
-- INSERT INTO Users (Username, PasswordHash, Email) VALUES 
-- ('testuser', '$2a$11$test.hash.value.here', 'test@example.com');

SELECT 'Users table created successfully!' AS Result; 