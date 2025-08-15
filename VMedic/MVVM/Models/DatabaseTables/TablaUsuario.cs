using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("Usuario")]
    public class TablaUsuario : TableData
    {
        public int CodVendedor { get; set; }
        public string? UsuarioName { get; set; }
        public string? Contraseña { get; set; }
        public int Remember { get; set; }
        public int UbicacionRequerida { get; set; }
        public int? CodPortafolio { get; set; }
        public string? IDLicencia { get; set; }
        public string? LicenciaName { get; set; }
        public string? LicenciaPass { get; set; }
        public string? Logo { get; set; }
        public int? CompanyID { get; set; }
    }
}
