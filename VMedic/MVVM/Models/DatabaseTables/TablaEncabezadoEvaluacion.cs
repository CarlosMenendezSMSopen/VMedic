using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("evaluacionencabezado")]
    public class TablaEncabezadoEvaluacion : TableData
    {
        public string? IdCliente { get; set; }
        public string? Base64Image { get; set; }
    }
}
