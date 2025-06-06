using Microsoft.AspNetCore.Mvc;
using aspnetapp.Models;
using aspnetapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace aspnetapp.Controllers
{
    /// <summary>
    /// 用户认证 API 控制器
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求信息</param>
        /// <returns>注册结果</returns>
        /// <response code="201">注册成功</response>
        /// <response code="400">请求参数无效</response>
        /// <response code="409">用户名或邮箱已存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "请求参数无效",
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                if (result.Message.Contains("已存在") || result.Message.Contains("已被注册"))
                {
                    return Conflict(result);
                }
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(Register), result);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求信息</param>
        /// <returns>登录结果，包含JWT令牌</returns>
        /// <response code="200">登录成功</response>
        /// <response code="400">请求参数无效</response>
        /// <response code="401">用户名或密码错误</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "请求参数无效"
                });
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                if (result.Message.Contains("用户名或密码错误"))
                {
                    return Unauthorized(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 获取当前用户信息（需要认证）
        /// </summary>
        /// <returns>当前用户信息</returns>
        /// <response code="200">获取成功</response>
        /// <response code="401">未授权</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserInfo>> GetCurrentUser()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userInfo = await _authService.ValidateTokenAsync(token);

                if (userInfo == null)
                {
                    return Unauthorized();
                }

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取当前用户信息失败");
                return Unauthorized();
            }
        }

        /// <summary>
        /// 用户登出（可选实现）
        /// </summary>
        /// <returns>登出结果</returns>
        /// <response code="200">登出成功</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public ActionResult<AuthResponse> Logout()
        {
            // 在实际应用中，这里可以将JWT添加到黑名单
            // 或者实现token撤销机制
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "登出成功"
            });
        }
    }
} 