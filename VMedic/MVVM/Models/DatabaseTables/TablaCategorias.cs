using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("categorias")]
    public class TablaCategorias : TableData
    {
        public string? CODIGO_DIVISION { get; set; }
        public string? DESCRIPCION_DIVISION { get; set; }
    }
}
