using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("subcategorias")]
    public class TablaSubCategorias : TableData
    {
        public string? CODIGO_MARCA { get; set; }
        public string? CODIGO_DIVISION { get; set; }
        public string? DESCRIPCION_MARCA { get; set; }
    }
}
