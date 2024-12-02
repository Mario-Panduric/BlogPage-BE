
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication5.Authorization;
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
            builder.Services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.Cookie.Name = "token";

            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "localhost.com",
                    ValidAudience = "localhost.com",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyThatIsVeryVeryLong12345")
                    )
                    
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["token"];
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddScoped<TokenGenerator>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDistributedMemoryCache();

        
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "MyAllowSpecificOrigins",
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000", "http://localhost:3000/signup", "http://localhost:3000/signin", "http://localhost:3000/home", "http://localhost:3000/blog/");
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
            
            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseCors("MyAllowSpecificOrigins");

            app.Run();
        }
    }
}
