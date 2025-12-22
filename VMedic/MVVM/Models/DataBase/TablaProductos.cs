using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DataBase
{
    [Table("productos")]
    public class TablaProductos : TableData
    {
        public string? PRODUCTO { get; set; }
        public string? DESCRIPCION_PROD { get; set; }
        public double PRECIOU { get; set; }
        public int CANTIDAD { get; set; }
        public string? CATEGORIA { get; set; }
    }
}
