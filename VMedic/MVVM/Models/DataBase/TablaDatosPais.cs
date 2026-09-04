using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DataBase
{
    [Table("datospais")]
    public class TablaDatosPais : TableData
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public int? CodigoPais { get; set; }
        public int? IdParent { get; set; }
    }
}
