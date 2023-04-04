var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleansClient((context, clientBuilder) =>
    {
        clientBuilder.UseLocalhostClustering();
        clientBuilder.AddMemoryStreams("StreamProvider");
    })
    .ConfigureLogging(x =>
    {
        x.AddConsole();
        x.SetMinimumLevel(LogLevel.Warning);
    });

// Add services to the container.

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
