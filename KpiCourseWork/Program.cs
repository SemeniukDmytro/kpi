var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()  
            .AllowAnyHeader(); 
    });
});

builder.Services.AddControllers();

var app = builder.Build();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();