using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Global;
using VMedic.Interfaces;
using VMedic.Utilidades;

namespace VMedic.Servicios
{
    public class RestService : IRestService
    {
        private string DomainExtension { get; } = "wapiidc/query/smsdadaadmin/4811a970b1ee42edc719c9675e757313/";
        public async Task<IList<T>?> ResultadoGET<T>(string? domain, string consulta, Func<string[], T>? map)
        {
            try
            {
                if (IsInternet.Avilable())
                {
                    DatosCompartidos.ErrorResponseValue = null;
 
                    string urlRequest = domain + DomainExtension + consulta;
                    Debug.WriteLine("URL Request " + urlRequest);
                    using HttpClient client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    string jsonResponse = await client.GetStringAsync(urlRequest);
                    if (jsonResponse.Contains("{\"value\":\""))
                    {
                        DatosCompartidos.ErrorResponseValue = new Dictionary<int, string> { { 1, jsonResponse.Split("\":")[1].Split(",\"")[0].Replace("\"", "") } };
                        return null;
                    }

                    if (map is not null)
                    {
                        IList<Dictionary<string, string>>? datos = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonResponse);

                        var resultado = new List<T>();

                        if (datos is not null)
                            foreach (var item in datos)
                            {
                                // Tomamos los valores en orden
                                var valores = item.Values.ToArray();

                                // Usamos la función map para convertir array de strings en T
                                var obj = map(valores);

                                resultado.Add(obj);
                            }

                        return resultado;
                    }
                    else
                    {
                        IList<T>? datos = JsonConvert.DeserializeObject<List<T>>(jsonResponse);

                        return datos;
                    }
                }
                else
                {
                    DatosCompartidos.ErrorResponseValue = new Dictionary<int, string> { { 2, "No hay conexión de Internet, verifique su plan de datos o Wi-fi e inténtelo de nuevo" } };
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Resultado " + ex);
                PressedPreferences.EndPressed();
                if (!ex.Message.Contains("The operation has timed out."))
                {
                    DatosCompartidos.ErrorResponseValue = new Dictionary<int, string> { { 3, "Ha ocurrido un error inesperado" } };
                    ExceptionMessageMaker.Make("Error resultado al obtener datos", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
                }

                return null;
            }
        }

        public async Task<IList<T>?> ResultadoPOST<T>(string? operacionID, string? parametros, Func<string[], T>? map)
        {
            try
            {
                if (IsInternet.Avilable())
                {
                    using (var client = new HttpClient())
                    {
                        Debug.WriteLine($"URL Request {(await URL.GetDomain())}{DomainExtension}{operacionID}/{parametros}");
                        string strConsulta = (await URL.GetDomain()) + "wapiidc/query/consultav5";

                        var jsonData = new
                        {
                            usuario = "smsdadaadmin",
                            accesskey = "4811a970b1ee42edc719c9675e757313",
                            operacionid = $"{operacionID}",
                            paramsvalue = parametros
                        };

                        client.Timeout = TimeSpan.FromSeconds(10);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        var content = new StringContent(
                            System.Text.Json.JsonSerializer.Serialize(jsonData),
                            Encoding.UTF8,
                            "application/json");

                        var responseTask = client.PostAsync(strConsulta, content);
                        responseTask.Wait();
                        var response = await responseTask;
                        response.EnsureSuccessStatusCode(); // Lanza excepción si no es 200-299

                        var responseBody = await response.Content.ReadAsStringAsync();

                        if (map is not null)
                        {
                            string jsonArray = responseBody.Replace("{", "[").Replace("}", "]").Replace("\"\":", "");

                            var datos = JsonConvert.DeserializeObject<List<List<string>>>(jsonArray);

                            return datos?.Select(item => map(item.ToArray())).ToList();
                        }
                        else
                        {
                            IList<T>? datos = JsonConvert.DeserializeObject<List<T>>(responseBody);
                            return datos;
                        }
                    }
                }
                else
                {
                    DatosCompartidos.ErrorResponseValue = new Dictionary<int, string> { { 2, "No hay conexión de Internet, verifique su plan de datos o Wi-fi e inténtelo de nuevo" } };
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Resultado " + ex);
                PressedPreferences.EndPressed();
                if (!ex.Message.Contains("The operation has timed out."))
                {
                    DatosCompartidos.ErrorResponseValue = new Dictionary<int, string> { { 3, "Ha ocurrido un error inesperado" } };
                    ExceptionMessageMaker.Make("Error resultado envío", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
                }
                return null;
            }
        }
    }
}
