using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("muestra")]
    public class TablaMuestras : TableData
    {
        public string? CODIGO_MUESTRA { get; set; }
        public string? DESCRIPCION_MUESTRA { get; set; }
        public double? PRECIO_SUGERIDO { get; set; }
        public int CANT_DISPONIBLE { get; set; }
    }
}
