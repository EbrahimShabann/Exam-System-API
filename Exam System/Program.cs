using Exam_System.Models;
using Exam_System.Services;
using Exam_System.Services.DbInitializer;
using Exam_System.Services.ReposService.IRepos;
using Exam_System.Services.ReposService.Repos;
using ExamSystem.Models;
using ExamSystem.Services;
using ExamSystem.Services.AuthServices;
using ExamSystem.Services.credintailsService;
using ExamSystem.Services.EmailService;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Exam_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddOpenApi();

            //register hangfire
            builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("cs")));
            builder.Services.AddHangfireServer();


            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                //to solve cycle problem of navigation probertites when Serialization
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            }); 
            //register dependencies
            builder.Services.AddScoped<IDbInitializer,DbInitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICredintailsService, CredintailsService>();
            builder.Services.AddScoped<BGJobsService>();



            //register memory cache
            builder.Services.AddMemoryCache();


            var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

            builder.Services.AddSingleton<JwtOptions>(jwt);
            // Add and configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Add JWT Bearer support to Swagger UI
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOi...\""
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            //enable CORS
            builder.Services.AddCors(options =>
            {
               
                // Add CORS policy for Angular frontend
                options.AddPolicy("AllowAngular",
                    builder => builder
                        .WithOrigins("http://localhost:4200") //WithOrigins("https://ebrahimshabann.github.io") 
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });

            // Configure JWT Authentication           
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                };
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            using(var scope= app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await initializer.Initialize();
            }

            //if (app.Environment.IsDevelopment())
            //{
                app.MapOpenApi();
                //app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    //options.SwaggerEndpoint("/openapi/v1.json", "v1");
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

                });


            //}

            app.UseHttpsRedirection();
            app.UseSession();
           

            // CORS must be before UseAuthorization and MapControllers
            app.UseCors("AllowAngular");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/dashboard");

            app.MapControllers();

            app.Run();
        }
    }
}
/*{
    "title": "Programming Fundamentals Quiz",
  "description": "Test your basic programming knowledge with 5 multiple-choice questions",
  "duration": 15,
  "questions": [
    {
        "questionText": "What does 'HTML' stand for?",
      "questionType": "MCQ",
      "choices": [
        {
            "choiceText": "Hyper Trainer Marking Language",
          "isCorrect": false
        },
        {
            "choiceText": "Hyper Text Markup Language",
          "isCorrect": true
        },
        {
            "choiceText": "Hyper Transfer Markup Language",
          "isCorrect": false
        }
      ]
    },
    {
        "questionText": "Which of these is NOT a programming paradigm?",
      "questionType": "MCQ",
      "choices": [
        {
            "choiceText": "Object-Oriented Programming",
          "isCorrect": false
        },
        {
            "choiceText": "Functional Programming",
          "isCorrect": false
        },
        {
            "choiceText": "Lateral Programming",
          "isCorrect": true
        }
      ]
    },
    {
        "questionText": "What will 'Boolean(0)' return in JavaScript?",
      "questionType": "MCQ",
      "choices": [
        {
            "choiceText": "true",
          "isCorrect": false
        },
        {
            "choiceText": "false",
          "isCorrect": true
        },
        {
            "choiceText": "undefined",
          "isCorrect": false
        }
      ]
    },
    {
        "questionText": "Which language uses 'print()' for console output?",
      "questionType": "MCQ",
      "choices": [
        {
            "choiceText": "Java",
          "isCorrect": false
        },
        {
            "choiceText": "Python",
          "isCorrect": true
        },
        {
            "choiceText": "C++",
          "isCorrect": false
        }
      ]
    },
    {
        "questionText": "What is the main purpose of CSS?",
      "questionType": "MCQ",
      "choices": [
        {
            "choiceText": "Adding interactivity to web pages",
          "isCorrect": false
        },
        {
            "choiceText": "Styling web page elements",
          "isCorrect": true
        },
        {
            "choiceText": "Creating database schemas",
          "isCorrect": false
        }
      ]
    }
  ]
}*/