using EF;
using Microsoft.EntityFrameworkCore;
using Model.AppSettings;
using Service.Bookkeeping;
using Service.EventService;
using Service.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<BookkeepingContext>(option => option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

builder.Services.Configure<LineBot>(builder.Configuration.GetSection("LineBot"));

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEventService, EventService>();
builder.Services.AddTransient<IBookkeepingService, BookkeepingService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
