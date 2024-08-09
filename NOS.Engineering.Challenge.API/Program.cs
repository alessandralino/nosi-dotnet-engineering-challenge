using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.API.Extensions;
using NOS.Engineering.Challenge.Database;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

// Add dbcontext to the container 
builder.Services.AddMySqlDatabase(builder.Configuration);
builder.Services.AddOutputCache();
var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();

app.UseOutputCache();
    
app.Run();