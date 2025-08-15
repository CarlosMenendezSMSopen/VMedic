using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.MVVM.Models.Data
{
    public class TableData
    {
        [PrimaryKey, AutoIncrement]
        public int TableID { get; set; }
    }
}
