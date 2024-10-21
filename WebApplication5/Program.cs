
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Entities;
namespace WebApplication5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDistributedMemoryCache();

            // This was missing:
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "MyAllowSpecificOrigins",
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000", "http://localhost:3000/signup", "http://localhost:3000/signin", "http://localhost:3000/Home", "http://localhost:3000/blog/");
                                      policy.AllowAnyHeader();
                                      policy.AllowAnyMethod();
                                      policy.AllowCredentials();
                                  });
            });
            builder.Services.AddAutoMapper(typeof(Program).Assembly);
            builder.Services.AddScoped<PasswordHasher<User>>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            app.MapControllers();
            app.UseCors("MyAllowSpecificOrigins");
            app.UseSession();
            app.Run();
        }
    }
}
