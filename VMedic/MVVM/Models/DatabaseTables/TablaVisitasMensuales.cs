using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("controlvisitasmensuales")]
    public class TablaVisitasMensuales : TableData
    {
        public int CODIGO_CONTROL_VISITAS { get; set; }
        public int CODIGO_DE_CLIENTE { get; set; }
        public int CODIGO_VENDEDOR { get; set; }
        public int SEMANA { get; set; }
        public int DIA { get; set; }
        public int SECUENCIA { get; set; }
    }
}
