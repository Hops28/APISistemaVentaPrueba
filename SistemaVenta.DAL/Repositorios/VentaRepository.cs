using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbVentaContext;

        // Como hereda de GenericRepository y ya tiene un atributo igual a _dbVentaContext entonces se usa :base para usarlo desde el GenericRepository
        public VentaRepository(DbventaContext dbVentaContext):base(dbVentaContext)
        {
            _dbVentaContext = dbVentaContext;
        }

        public async Task<Venta> Registrar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbVentaContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in modelo.DetalleVenta)
                    {
                        Producto productoEncontrado = _dbVentaContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();

                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad;
                    }

                    await _dbVentaContext.SaveChangesAsync();

                    // Se genera un número de documento
                    NumeroDocumento correlativo = _dbVentaContext.NumeroDocumentos.First();

                    correlativo.UltimoNumero++;
                    correlativo.FechaRegistro = DateTime.Now;

                    _dbVentaContext.NumeroDocumentos.Update(correlativo);
                    await _dbVentaContext.SaveChangesAsync();

                    // Se genera el formato para el código
                    int cantidad_digitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantidad_digitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - cantidad_digitos, cantidad_digitos);

                    modelo.NumeroDocumento = numeroVenta;

                    await _dbVentaContext.AddAsync(modelo);
                    await _dbVentaContext.SaveChangesAsync();

                    ventaGenerada = modelo;

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                return ventaGenerada;
            };
        }
    }
}
