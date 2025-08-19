using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("especialidad")]
    public class TablaClasesEspecializaciones : TableData
    {
        public string? CODIGO_DE_CLASE { get; set; }
        public string? DESCRIPCION_CLASE { get; set; }
    }
}
