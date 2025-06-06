using System.ComponentModel.DataAnnotations;

namespace aspnetapp.Models
{
    /// <summary>
    /// 用户注册请求模型
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <example>testuser</example>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-20个字符之间")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        /// <example>Password123!</example>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱地址
        /// </summary>
        /// <example>test@example.com</example>
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// 用户登录请求模型
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <example>testuser</example>
        [Required(ErrorMessage = "用户名不能为空")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        /// <example>Password123!</example>
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 认证响应模型
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        /// <example>true</example>
        public bool Success { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        /// <example>登录成功</example>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// JWT 令牌（仅登录成功时返回）
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string? Token { get; set; }

        /// <summary>
        /// 用户信息（仅登录成功时返回）
        /// </summary>
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// 用户信息模型（不包含敏感信息）
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        /// <example>testuser</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱地址
        /// </summary>
        /// <example>test@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 注册时间
        /// </summary>
        /// <example>2024-01-01T12:00:00</example>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        /// <example>true</example>
        public bool IsActive { get; set; }
    }
} 