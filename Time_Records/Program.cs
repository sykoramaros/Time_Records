using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Time_Records;
using Time_Records.Models;
using Time_Records.Services;
using HotChocolate.AspNetCore;
using Time_Records.GraphQL;
using Time_Records.GraphQL.Users.Queries;
using Time_Records.GraphQL.Users.Mutations;
using HotChocolate;
using HotChocolate.Types;
using Time_Records.GraphQL.Types;
using Time_Records.GraphQL.Types.Inputs;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IGoogleAccountService, GoogleAccountService>();
builder.Services.AddScoped<GoogleAccountService>();
builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<RecordCreditTimeService>();
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
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options => {
        // povolene znaky a mezera pro username
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        options.User.RequireUniqueEmail = true;
    })
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

// povoleni anotaci ve Swaggeru
builder.Services.AddSwaggerGen(options => {
    options.EnableAnnotations();
});

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])) ?? throw new InvalidOperationException("JWT:SecretKey not found")
        };
    });

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<UserQueries>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<UserMutations>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("MyCorsPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeRecords API V1");
    });
}

if (app.Environment.IsDevelopment()) {
    app.MapGraphQL("/graphql")
        .WithOptions(new GraphQLServerOptions {
            Tool = { Enable = true }
        });
} else {
    app.MapGraphQL("/graphql");
}

app.Run();
