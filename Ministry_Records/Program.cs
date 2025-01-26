var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Nastavení DbContext pro připojení k databázi pomocí SQL Serveru.
//propojena lokalni databaze MAMP MySql (pro spravne fungovani musi MAMP bezet)
// LOKALNI MAMP MySql DB
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("DBConnection"),new MySqlServerVersion(new Version(10,6, 28)));
// });



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();





app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) {
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}