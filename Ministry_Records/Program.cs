using Microsoft.EntityFrameworkCore;
using Ministry_Records;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// EXTERNI ASP.NET Mariadb
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("MonsterAspDbConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MonsterAspDbConnection"))
    );
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {
    options.AddPolicy("MyCorsPolicy", builder => {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MinistryRecords API V1");
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