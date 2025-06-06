using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspnetapp.Models
{
    /// <summary>
    /// 用户实体模型
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 用户名，3-20字符，唯一
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-20个字符之间")]
        [Column("Username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码哈希值（BCrypt加密）
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 用户邮箱，唯一
        /// </summary>
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 注册时间
        /// </summary>
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 账户是否激活
        /// </summary>
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;
    }
} 