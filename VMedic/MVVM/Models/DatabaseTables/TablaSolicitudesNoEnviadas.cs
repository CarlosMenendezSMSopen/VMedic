using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("SOLICITUDES_NO_ENVIADAS")]
    public class TablaSolicitudesNoEnviadas : TableData
    {
        public string? OperacionID { get; set; }
        public string? Parametros { get; set; }
        public int ClavesVacias { get; set; } //0 == NO \\ 1 == SI
        public int TipoRestService { get; set; }
        public string? CodigoCliente { get; set; }
    }
}
