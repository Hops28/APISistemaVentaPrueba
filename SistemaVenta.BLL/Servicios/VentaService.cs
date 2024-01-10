﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Model;

namespace SistemaVenta.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepositorio;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepositorio, 
            IGenericRepository<DetalleVenta> detalleVentaRepositorio, 
            IMapper mapper)
        {
            _ventaRepositorio = ventaRepositorio;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _mapper = mapper;
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepositorio.Registrar(_mapper.Map<Venta>(modelo));

                if (ventaGenerada.IdVenta == 0)
                {
                    throw new TaskCanceledException("No se pudo crear");
                }

                return _mapper.Map<VentaDTO>(ventaGenerada);

            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar();
            var ListaResultado = new List<Venta>();

            try
            {
                // buscarPor = fecha, numeroVenta
                if (buscarPor == "fecha")
                {
                    DateTime inicioFecha = DateTime.ParseExact(fechaInicio, "dd/mm/yyyy", new CultureInfo("es-NI"));
                    DateTime finFecha = DateTime.ParseExact(fechaFin, "dd/mm/yyyy", new CultureInfo("es-NI"));

                    ListaResultado = await query.Where(v => 
                        v.FechaRegistro.Value.Date >= inicioFecha.Date &&
                        v.FechaRegistro.Value.Date <= finFecha.Date
                    ).Include(dv => dv.DetalleVenta)
                    .ThenInclude(p => p.IdProductoNavigation)
                    .ToListAsync();
                }
                else
                {
                    ListaResultado = await query.Where(v =>
                        v.NumeroDocumento == numeroVenta
                    ).Include(dv => dv.DetalleVenta)
                    .ThenInclude(p => p.IdProductoNavigation)
                    .ToListAsync();
                }
            }
            catch 
            {

                throw;
            }

            return _mapper.Map<List<VentaDTO>>(ListaResultado);
        }

        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Consultar();
            var listaResultado = new List<DetalleVenta>();


            try
            {
                DateTime inicioFecha = DateTime.ParseExact(fechaInicio, "dd/mm/yyyy", new CultureInfo("es-NI"));
                DateTime finFecha = DateTime.ParseExact(fechaFin, "dd/mm/yyyy", new CultureInfo("es-NI"));

                listaResultado = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv =>
                        dv.IdVentaNavigation.FechaRegistro.Value.Date <= inicioFecha.Date &&
                        dv.IdVentaNavigation.FechaRegistro.Value.Date <= finFecha.Date
                    ).ToListAsync();
            }
            catch
            {
                throw;
            }

            return _mapper.Map<List<ReporteDTO>>(listaResultado);
        }
    }
}
