using aspnetapp.Models;

namespace aspnetapp.Services
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>认证响应</returns>
        Task<AuthResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// 验证JWT令牌
        /// </summary>
        /// <param name="token">JWT令牌</param>
        /// <returns>用户信息</returns>
        Task<UserInfo?> ValidateTokenAsync(string token);

        /// <summary>
        /// 生成JWT令牌
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>JWT令牌</returns>
        string GenerateJwtToken(User user);
    }
} 