using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DataBase
{
    [Table("presentaciones")]
    public class TablaPresentaciones : TableData
    {
        public string? CODIGO_UNIDAD_VENTA { get; set; }
        public string? PRODUCTO { get; set; }
        public string? DESCRP_UNIDAD_VENTA { get; set; }
        public int FACTOR_DE_CONVERSION { get; set; }
        public double PRECIO { get; set; }
        public string? NIVEL_PRECIO { get; set; }
    }
}
