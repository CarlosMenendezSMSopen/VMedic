using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DataBase
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
        public string? FECHA { get; set; }
        public string? FECHAFINAL { get; set; }
        public string? HORA_LLEGADA { get; set; }
        public int? TIPO_CONTROL { get; set; }
        public int? ESTADO { get; set; }
    }

    public class TablaSemanasDias
    {
        public int SEMANA { get; set; }
        public int DIA { get; set; }
    }
}
