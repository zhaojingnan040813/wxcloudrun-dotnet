#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspnetapp;

/// <summary>
/// 计数器操作请求模型
/// </summary>
public class CounterRequest 
{
    /// <summary>
    /// 操作类型
    /// </summary>
    /// <example>inc</example>
    [Required(ErrorMessage = "操作类型不能为空")]
    public string action { get; set; }
}

/// <summary>
/// 计数器响应模型
/// </summary>
public class CounterResponse 
{
    /// <summary>
    /// 当前计数值
    /// </summary>
    /// <example>42</example>
    public int data { get; set; }
}

namespace aspnetapp.Controllers
{
    /// <summary>
    /// 计数器 API 控制器 - 提供计数器的查询和操作功能
    /// </summary>
    [Route("api/count")]
    [ApiController]
    [Produces("application/json")]
    public class CounterController : ControllerBase
    {
        private readonly CounterContext _context;

        public CounterController(CounterContext context)
        {
            _context = context;
        }

        private async Task<Counter> getCounterWithInit()
        {
            var counters = await _context.Counters.ToListAsync();
            if (counters.Count() > 0)
            {
                return counters[0];
            }
            else
            {
                var counter = new Counter { count = 0, createdAt = DateTime.Now, updatedAt = DateTime.Now };
                _context.Counters.Add(counter);
                await _context.SaveChangesAsync();
                return counter;
            }
        }

        /// <summary>
        /// 获取当前计数值
        /// </summary>
        /// <returns>返回当前的计数值</returns>
        /// <response code="200">成功返回计数值</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet]
        [ProducesResponseType(typeof(CounterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CounterResponse>> GetCounter()
        {
            var counter = await getCounterWithInit();
            return new CounterResponse { data = counter.count };
        }

        /// <summary>
        /// 执行计数器操作
        /// </summary>
        /// <param name="data">操作请求，包含操作类型：'inc' 表示增加1，'clear' 表示重置为0</param>
        /// <returns>操作后的计数值</returns>
        /// <response code="200">操作成功，返回新的计数值</response>
        /// <response code="400">请求参数无效</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost]
        [ProducesResponseType(typeof(CounterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CounterResponse>> PostCounter(CounterRequest data)
        {
            if (data?.action == "inc") 
            {
                var counter = await getCounterWithInit();
                counter.count += 1;
                counter.updatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new CounterResponse { data = counter.count };
            }
            else if (data?.action == "clear") 
            {
                var counter = await getCounterWithInit();
                counter.count = 0;
                counter.updatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new CounterResponse { data = counter.count };
            }
            else 
            {
                return BadRequest(new { 
                    error = "无效的操作类型", 
                    message = "支持的操作类型：'inc'（增加）或 'clear'（清零）",
                    received = data?.action ?? "null"
                });
            }
        }
    }
}
