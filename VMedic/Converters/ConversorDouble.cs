using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace VMedic.Converters
{
    public static partial class ConversorDouble
    {
        //funcion que invalida la conversion de double sin importar la region en que se ejecute la aplicacion
        public static double Parse(string? decim)
        {
            if (decim is not null)
            {
                var Decimal = decim.Contains(",") ? decim.Replace(",",".") : decim;
                return double.Parse(Decimal, CultureInfo.InvariantCulture);
            }
            else
            {
                return 0.0;
            }
        }
    }
}
