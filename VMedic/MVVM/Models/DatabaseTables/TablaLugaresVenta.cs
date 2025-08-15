using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("lugaresventas")]
    public class TablaLugaresVenta : TableData
    {
        public string? CODIGO_LUGAR { get; set; }
        public string? DESCRIPCION { get; set; }
    }
}
