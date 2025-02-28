using JessyProject;
using JessyProject.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:3000") // Permet l'acc�s � partir de localhost:3002 (votre frontend)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<ClassementDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ClassementDbContext>(options =>
    options.UseNpgsql("postgresql://classement_db_5uxe_user:TwsMEpVPtpK3l3bfawZuMg39uvnddw6s@dpg-cv0sc5tsvqrc738v8s60-a/classement_db_5uxe"));

builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ListenAnyIP(int.Parse(
        Environment.GetEnvironmentVariable("PORT") ?? "8080"
    ));
});

var app = builder.Build();

app.UseCors("AllowLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
