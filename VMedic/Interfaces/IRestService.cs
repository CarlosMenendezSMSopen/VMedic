using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.Interfaces
{
    public interface IRestService
    {
        Task<IList<T>?> ResultadoGET<T>(string consulta, Func<string[], T>? map);
        Task<IList<T>?> ResultadoPOST<T>(string? operacionID, string? parametros, Func<string[], T>? map);
    }
}
