using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("categorias_medico")]
    public class TablaCategoriasMedico : TableData
    {
        public int? CATEGORIAID { get; set; }
        public string? CATEGORIA { get; set; }
    }
}
