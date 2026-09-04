using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DataBase
{
    [Table("clientes")]
    public class TablaDoctores : TableData
    {
        public string? CODIGO_DE_CLIENTE { get; set; }
        public string? NOMBRE_COMERCIAL { get; set; }
        public string? CONTACTO_CLIENTE { get; set; }
        public string? DIRECCION_CLIENTE { get; set; }
        public string? TELEFONO_CLIENTE { get; set; }
        public string? DIRECCION_EMAIL { get; set; }
        public string? DUI_CLIENTE { get; set; }
        public string? JVPM { get; set; }
        public int? CODIGO_DE_PAIS { get; set; }
        public int? CODIGO_DEPARTAMENTO { get; set; }
        public int? CODIGO_MUNICIPIO { get; set; }
        public string? CODIGO_DE_CLASE { get; set; }
        public double LATITUD { get; set; }
        public double LONGITUD { get; set; }
        public string? COLOR { get; set; }
        public string? ESCALA_ADAPTACION { get; set; }
        public int? CATEGORIAID { get; set; }
    }
}
