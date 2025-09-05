using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("medicoprodpreferencia")]
    public class TablaMedicoProductoPreferencia : TableData
    {
        public int? CODIGO_DE_CLIENTE { get; set; }
        public int? ID_PRODUCTO_PREFERENCIA { get; set; }
    }
}
