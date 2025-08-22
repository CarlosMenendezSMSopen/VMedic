using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("producto_preferencia")]
    public class TablaProductoPreferencia : TableData
    {
        public int? ID_PRODUCTO_PREFERENCIA { get; set; }
        public string? DESCRIPCION_PROD_PREFERENCIA { get; set; }
    }
}
