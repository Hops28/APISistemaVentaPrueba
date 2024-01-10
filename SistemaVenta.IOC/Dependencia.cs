using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SistemaVenta.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.Repositorios;

using SistemaVenta.Utility;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.BLL.Servicios;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        // Método de extensión que fué agregado a la clase "IServiceCollection"
        public static void InyectarDependencias (this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<DbventaContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            /*
             Configura la inyección de dependencias para un servicio transitorio (AddTransient). 
            Asocia la interfaz genérica IGenericRepository<> con su implementación genérica GenericRepository<>. 
            Esto significa que cada vez que se solicita IGenericRepository<T>, 
            se crea una nueva instancia de GenericRepository<T>.
             
            Está configurando la inyección de dependencias 
            para que cuando alguien solicite un servicio del tipo IGenericRepository<T>, 
            ASP.NET Core cree y devuelva una nueva instancia de GenericRepository<T>.
             */
            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            /*
             Configura la inyección de dependencias para un servicio de ámbito (AddScoped). 
            Asocia la interfaz IVentaRepository con su implementación VentaRepository. 
            En este caso, la instancia de VentaRepository se crea una vez por cada solicitud de ámbito 
            (por ejemplo, una solicitud web en una aplicación ASP.NET Core).
             */
            service.AddScoped<IVentaRepository, VentaRepository>();

            // Dependencia de Automapper con todos los mapeos
            service.AddAutoMapper(typeof(AutomapperProfile));

            // Dependencias de todos estos servicios
            service.AddScoped<IRolService, RolService>();
            service.AddScoped<IUsuarioService, UsuarioService>();
            service.AddScoped<ICategoriaService,CategoriaService>();
            service.AddScoped<IProductoService, ProductoService>();
            service.AddScoped<IVentaService, VentaService>();
            service.AddScoped<IDashBoardService, DashBoardService>();
            service.AddScoped<IMenuService, MenuService>();
        }
    }
}
