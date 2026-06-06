using DapperApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Khai báo dịch vụ xử lý Controller
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Khai báo dịch vụ tạo tài liệu Swagger

// Đăng ký Repository vào DI container
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Kích hoạt giao diện đồ họa của Swagger để có thể nhấn "Try it out" và test API

}

// Tạm thời tắt chuyển hướng HTTPS vì dự án đang lắng nghe trực tiếp trên cổng http (5175)
// app.UseHttpsRedirection();

app.UseAuthorization();

// ĐÂY LÀ DÒNG QUAN TRỌNG NHẤT BỊ THIẾU: Ánh xạ các API từ StudentsController vào hệ thống
app.MapControllers();

// Giữ lại hoặc xóa đoạn WeatherForecast mặc định tùy nhu cầu của bạn
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}