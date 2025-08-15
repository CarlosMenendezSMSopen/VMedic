using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("SKUproductos")]
    public class TablaSKUProducto : TableData
    {
        public string? CODIGO_UNIDAD_VENTA { get; set; }
        public string? PRODUCTO { get; set; }
    }
}
