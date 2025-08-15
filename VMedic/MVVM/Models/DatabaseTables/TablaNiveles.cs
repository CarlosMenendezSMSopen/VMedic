using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("niveles")]
    public class TablaNiveles : TableData
    {
        public string? CODIGO_NIVEL { get; set; }
        public string? DESCRIPCION_NIVEL { get; set; }
        public int? ORDEN { get; set; }
    }
}
