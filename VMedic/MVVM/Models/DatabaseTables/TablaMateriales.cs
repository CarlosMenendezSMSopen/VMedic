using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("materiales")]
    public class TablaMateriales : TableData
    {
        public string? CODIGO_MATERIAL { get; set; }
        public string? NOMBRE_MATERIAL { get; set; }
        public int CODIGO_COMPANIA { get; set; }
    }
}
