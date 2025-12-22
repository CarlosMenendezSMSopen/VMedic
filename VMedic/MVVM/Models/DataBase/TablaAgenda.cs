using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DataBase
{
    [Table("Agenda")]
    public class TablaAgenda : TableData
    {
        public int CODIGO_CONTROL_VISITAS { get; set; }
        public int? CODIGO_DE_PROGRAMACION { get; set; }
        public int? TIPO_DE_PROGRAMACION { get; set; }
        public int CODIGO_DE_CLIENTE { get; set; }
        public string? FECHA_INICIAL { get; set; }
        public string? FECHA_FINAL { get; set; }
        public int SEMANA { get; set; }
        public int DIA { get; set; }
    }
}
