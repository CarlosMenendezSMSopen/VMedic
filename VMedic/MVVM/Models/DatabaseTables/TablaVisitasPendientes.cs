using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("visitas")]
    public class TablaVisitasPendientes : TableData
    {
        public string? CodCliente { get; set; }
        public string? CodLugar { get; set; }
        public string? IDTipoVisita { get; set; }
        public string? Comentarios { get; set; }
        public string? CodVendedor { get; set; }
        public string? FechaGPS { get; set; }
        public double? Latitud {  get; set; }
        public double? Longitud {  get; set; }
    }
}
