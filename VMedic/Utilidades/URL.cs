using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.Utilidades
{
    public static class URL
    {
        private static string DomainCOM { get; } = "https://bluefenyx.com/";
        private static string DomainNET { get; } = "https://bluefenyx.net/";
        private static string DomainExtension { get; } = "wapiidc/query/smsdadaadmin/4811a970b1ee42edc719c9675e757313/VMedicA001";
        private static bool Val { get; set; }

        public static async Task<string> GetDomain()
        {
            if (await ValidateDomain())
            {
                return DomainCOM;
            }
            else if (await ValidatealternDomain())
            {
                return DomainNET;
            }
            else
            {
                return DomainNET;
            }
        }

        //Metodo para validar el funcionamiento de ambos dominios, si la extencion .com da un error de 404 o 500, el valor será falso, de lo contrario, será verdadero
        public static async Task<bool> ValidateDomain()
        {
            try
            {
                using (HttpClient client = new())
                {
                    HttpResponseMessage response = await client.GetAsync(DomainCOM + DomainExtension);
                    if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Debug.WriteLine("Domain Verdadero");
                        return true;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Debug.WriteLine("Domain No encontrado");
                        return false;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        Debug.WriteLine("Error de Servidor");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error validar dominio: " + ex.ToString());
                return false;
            }

            return false;
        }


        //Metodo para validar el funcionamiento de ambos dominios, si la extencion .com da un error de 404 o 500, el valor será falso, de lo contrario, será verdadero
        public static async Task<bool> ValidatealternDomain()
        {
            try
            {
                using (HttpClient client = new())
                {
                    HttpResponseMessage response = await client.GetAsync(DomainNET + DomainExtension);
                    if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Debug.WriteLine("Domain Verdadero");
                        return true;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Debug.WriteLine("Domain No encontrado");
                        return false;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        Debug.WriteLine("Error de Servidor");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                var MainPage = App.Current?.Windows[0].Page;
                if (MainPage is not null && !ex.Message.Contains("Connection failure") && !ex.Message.Contains("not be found"))
                {
                    await MainPage.DisplayAlert("Caída de Servidor al obtener el dominio", ex.ToString(), "OK");
                }
                Debug.WriteLine("Error validar dominio alterno: " + ex.ToString());
                return false;
            }

            return false;
        }
    }
}
