using aspnetapp;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<CounterContext>();

// 添加控制器支持
builder.Services.AddControllers();

// 配置 OpenAPI/Swagger 服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AspNetApp API",
        Version = "v1",
        Description = "一个基于 .NET 6 的计数器 API，支持增加和清零操作",
        Contact = new OpenApiContact
        {
            Name = "开发团队",
            Email = "dev@example.com"
        }
    });
    
    // 启用 XML 注释支持
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    // 开发环境下启用 Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetApp API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "AspNetApp API 文档";
        c.DefaultModelsExpandDepth(-1); // 隐藏模型
    });
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
