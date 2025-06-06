using aspnetapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace aspnetapp.Services
{
    /// <summary>
    /// 认证服务实现
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly CounterContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            CounterContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // 检查用户名是否已存在
                var existingUserByUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username);
                if (existingUserByUsername != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "用户名已存在"
                    };
                }

                // 检查邮箱是否已存在
                var existingUserByEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUserByEmail != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "邮箱已被注册"
                    };
                }

                // 创建新用户
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("用户注册成功: {Username}", request.Username);

                return new AuthResponse
                {
                    Success = true,
                    Message = "注册成功",
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt,
                        IsActive = user.IsActive
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注册失败: {Username}", request.Username);
                return new AuthResponse
                {
                    Success = false,
                    Message = "注册失败，请稍后重试"
                };
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                // 查找用户
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "用户名或密码错误"
                    };
                }

                // 验证密码
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "用户名或密码错误"
                    };
                }

                // 生成JWT令牌
                var token = GenerateJwtToken(user);

                _logger.LogInformation("用户登录成功: {Username}", request.Username);

                return new AuthResponse
                {
                    Success = true,
                    Message = "登录成功",
                    Token = token,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt,
                        IsActive = user.IsActive
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户登录失败: {Username}", request.Username);
                return new AuthResponse
                {
                    Success = false,
                    Message = "登录失败，请稍后重试"
                };
            }
        }

        /// <summary>
        /// 验证JWT令牌
        /// </summary>
        public async Task<UserInfo?> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "YourSecretKeyHere");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // 查找用户
                var user = await _context.Users.FindAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return null;
                }

                return new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 生成JWT令牌
        /// </summary>
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "YourSecretKeyHere");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username),
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), // 1小时有效期
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
} 