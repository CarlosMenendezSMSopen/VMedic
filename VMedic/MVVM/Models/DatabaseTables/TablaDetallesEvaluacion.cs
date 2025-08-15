using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("evaluaciondetalles")]
    public class TablaDetallesEvaluacion : TableData
    {
        public string? IdCliente { get; set; }
        public string? IdProducto { get; set; }
        public string? Observaciones { get; set; }
        public string? Cantidad { get; set; }
        public string? Presentacion { get; set; }
    }
}
