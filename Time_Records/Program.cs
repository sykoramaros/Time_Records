using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Time_Records;
using Time_Records.Models;
using Time_Records.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<GoogleAccountService>();
builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<RecordTimeService>();
builder.Services.AddScoped<StudyService>();
builder.Services.AddHttpContextAccessor();

// EXTERNI ASP.NET Mariadb
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("MonsterAspDbConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MonsterAspDbConnection"))
    );
});

// builder.Services.AddIdentity<AppUser, IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();

// podpora GUID
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.Name = "AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/forbidden";
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Nesmíš použít AllowAnyOrigin() spolu s AllowCredentials(), protože to prohlížeč nepovolí.
builder.Services.AddCors(options => {
    options.AddPolicy("MyCorsPolicy", builder => {
        builder.WithOrigins(
                "https://sykoramaros.github.io",
                "http://localhost:3000",
                "http://localhost:5113",
                "https://localhost:7081",
                "https://recordsapi.runasp.net" )
        // builder.AllowAnyOrigin()
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

// autentifikace pro Google
// builder.Services.AddAuthentication(options => {
//         options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//     })
//     .AddCookie(options => {
//         options.Cookie.HttpOnly = true;
//         options.Cookie.SameSite = SameSiteMode.Lax;
//         options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//     })
//     .AddGoogle(options => {
//         options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//         options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//         options.CallbackPath = "/api/Account/Google-login-callback";
//         options.SaveTokens = true;
//     });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeRecords API V1");
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors("MyCorsPolicy");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();