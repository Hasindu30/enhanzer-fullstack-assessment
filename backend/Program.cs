using backend.Models;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure strongly typed options for external API
builder.Services.Configure<EnhanzerApiOptions>(
    builder.Configuration.GetSection(EnhanzerApiOptions.SectionName));

// Register AuthService with typed HttpClient
builder.Services.AddHttpClient<IAuthService, AuthService>();

// Configure CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.MapControllers();

app.Run();

