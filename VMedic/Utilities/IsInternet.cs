using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VMedic;

namespace VMedic.Utilities
{
    public static class IsInternet
    {
        public static bool Avilable()
        {
            try
            {
                IEnumerable<ConnectionProfile> profiles = Connectivity.Current.ConnectionProfiles;
                if (!profiles.Contains(ConnectionProfile.WiFi) && !profiles.Contains(ConnectionProfile.Cellular))
                {
                    return false;
                }

                WebRequest tRequest = WebRequest.Create("https://www.google.com/");
                tRequest.Method = "GET";
                tRequest.Timeout = 120000;
                tRequest.ContentType = "application/json";
                using (HttpWebResponse response = (HttpWebResponse)tRequest.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageMaker.Make("Error de conexión", ex.ToString(), ex.Message, App.Current?.MainPage);
                return false;
            }
        }
    }
}
