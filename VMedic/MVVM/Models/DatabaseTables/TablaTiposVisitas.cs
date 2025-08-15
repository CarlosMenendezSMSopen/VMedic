using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("tiposvisitas")]
    public class TablaTiposVisitas : TableData
    {
        public string? CODIGO_TIPO_VISITA { get; set; }
        public string? DESCRIPCION { get; set; }
    }
}
