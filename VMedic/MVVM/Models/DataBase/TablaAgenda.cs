using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DataBase
{
    [Table("Agenda")]
    public class TablaAgenda : TableData
    {
        public int? CodigoControlVisita { get; set; }
        public int? CodigoVendedor { get; set; }
        public string? Vendedor { get; set; }
        public int TipoControl { get; set; }
        public int? Semana { get; set; }
        public int? Dia { get; set; }
        public int? CodigoCliente { get; set; }
        public string? Cliente { get; set; }
        public int? CodigoCompania { get; set; }
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public int? Estado { get; set; }
        public string? Fecha { get; set; }
        public string? FechaFinal { get; set; }
        public string? HoraLlegada { get; set; }
    }
}
