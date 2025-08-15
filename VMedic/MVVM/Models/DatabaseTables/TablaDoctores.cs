using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.MVVM.Models.DatabaseTables
{
    [Table("clientes")]
    public class TablaDoctores : TableData
    {
        public string? CODIGO_DE_CLIENTE { get; set; }
        public string? NOMBRE_COMERCIAL { get; set; }
        public string? CONTACTO_CLIENTE { get; set; }
        public string? DIRECCION_CLIENTE { get; set; }
        public string? DIRECCION_ENVIO { get; set; }
        public string? TELEFONO_CLIENTE { get; set; }
        public string? DIRECCION_EMAIL { get; set; }
        public string? JVPM { get; set; }
        public int? LIMITE_CRED_CLIENTE { get; set; }
        public int? SALDO_CLIENTE { get; set; }
        public string? NIVEL_PRECIO { get; set; }
        public string? CODIGO_CONDICION { get; set; }
        public string? CODIGO_TIPO_PAGO { get; set; }
        public string? GIRO_DE_NEGOCIO { get; set; }
        public string? NUMERO_REGISTRO { get; set; }
        public string? CODIGO_DE_MONEDA { get; set; }
        public string? CODIGO_RUTA_DESPACHO { get; set; }
        public string? PRECIOS_CON_IVA { get; set; }
        public double LATITUD { get; set; }
        public double LONGITUD { get; set; }
        public string? CODIGO_DE_CLASE { get; set; }
        public string? COLOR { get; set; }
        public string? SEGMENTACION { get; set; }
        public string? ESCALA_ADAPTACION { get; set; }
        public int? Visitas { get; set; }
        public int? CATEGORIAID { get; set; }
    }
}
