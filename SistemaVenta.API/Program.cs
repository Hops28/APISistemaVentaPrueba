using SistemaVenta.IOC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Se usó un método de extensión, el cual lo que hace es agregar el método "InyectarDependencias" a la clase
// IServiceColletion
builder.Services.InyectarDependencias(builder.Configuration);

// Para evitar conflictos de relación se activan las Cors
// De esta manera se puede utilizar la API desde cualquier otro consumidor
builder.Services.AddCors(options =>
{
    // Se configuran las políticas
    options.AddPolicy("NuevaPolitica", app =>
    {
        // Se permite cualquier origen, cabecera y método
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Se activan las configuraciones del Cors que hicimos antes
app.UseCors("NuevaPolitica");

app.UseAuthorization();

app.MapControllers();

app.Run();
