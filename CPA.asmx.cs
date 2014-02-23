using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Services;
using HtmlAgilityPack;

namespace CPAServices
{
    /// <summary>
    /// Summary description for CPA
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class CPA : System.Web.Services.WebService
    {
        #region Members

        private static Dictionary<string, string> _Provincias = null;
        private static Dictionary<string, Dictionary<int, string>> _Localidades = null;

        #endregion

        [WebMethod]
        public String GetCPAById(string idProvincia, int idLocalidad, String calle, String altura)
        {
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["localidad"] = idLocalidad.ToString();
                data["calle"] = calle;
                data["altura"] = altura;
                data["ct_captcha"] = GetCaptchaText();
                data["action"] = "cpa";
                var response = wb.UploadValues(GetCPAUrl(), "POST", data);

                var html = System.Text.Encoding.UTF8.GetString(response);

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                string value = null;
                if (htmlDoc.DocumentNode.SelectSingleNode("//h1/span") != null)
                {
                    if (htmlDoc.DocumentNode.ChildNodes.Where(x => x.Name.Equals("h1",StringComparison.CurrentCultureIgnoreCase)).Count() > 1)
                    {
                        value = null;
                    }
                    else
                    {
                        value = htmlDoc.DocumentNode.SelectSingleNode("//h1/span").InnerHtml.Trim();
                    }
                }
                else
                {
                    value = htmlDoc.DocumentNode.SelectSingleNode("//div").ChildNodes.Last().InnerHtml.Trim();
                }
                return value;
            }
        }
        
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        private string GetCaptchaText()
        {
            var queryString = random.NextDouble().ToString().Replace(",", ".");
            var captchaUrl = "http://www.correoargentino.com.ar/sites/all/modules/custom/ca_forms/libs/Captcha/securimage_show.php?" + queryString;

            try
            {
                using (WebClient client = new WebClient())
                {
                    return OCRHelper.GetText(client.DownloadData(captchaUrl)).ToUpper();
                }

            }
            catch (Exception)
            {
                return RandomString(6).ToUpper();
            }
        }


        private static string GetCPAUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["formUrl"];
        }

        [WebMethod]
        public String GetCPAByNombre(string idProvincia, String nombreLocalidad, String calle, String altura)
        {
            int? idLocalidad = GetIdLocalidad(idProvincia, nombreLocalidad);
            if (!idLocalidad.HasValue)
            {
                return null;
            }
            return GetCPAById(idProvincia, idLocalidad.Value, calle, altura);
        }

        [WebMethod]
        public String GetIdProvincia(String nombreProvincia)
        {
            Func<KeyValuePair<string, string>, bool> condition = x => x.Value.Equals(nombreProvincia, StringComparison.InvariantCultureIgnoreCase);

            var provincias = GetProvincias();
            var provincia = provincias.Any(condition);

            return provincia ? provincias.First(condition).Value : null;
        }


        private IEnumerable<KeyValuePair<String, String>> GetProvincias()
        {
            if (_Provincias == null)
            {
                LoadProvincias();
            }
            return _Provincias;
        }

        private void LoadProvincias()
        {
            _Provincias = new Dictionary<string, string>();
            foreach (String key in System.Configuration.ConfigurationManager.AppSettings) {
                if (key.Length == 1)
                {
                    _Provincias.Add(key, System.Configuration.ConfigurationManager.AppSettings[key]);
                }
            }
        }

        private void LoadLocalidadesByProvincia(String idProvincia)
        {
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["provincia"] = idProvincia;
                data["action"] = "localidades";

                var response = wb.UploadValues(GetCPAUrl(), "POST", data);

                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(System.Text.Encoding.UTF8.GetString(response));

                var locs = new Dictionary<int, String>();
                foreach (var localidad in result)
                {
                    int idLocalidad = 0;
                    if (int.TryParse(localidad.id.ToString(), out idLocalidad))
                    {
                        locs.Add(idLocalidad, (String)localidad.nombre);
                    }
                }

                _Localidades.Add(idProvincia, locs);
            }
        }

        private IEnumerable<KeyValuePair<int, String>> GetLocalidades(String idProvincia)
        {
            LoadProvincias();
            if (!_Provincias.Keys.Any(x => x.Equals(idProvincia, StringComparison.InvariantCultureIgnoreCase)))
            {
                return null;
            }
            LoadLocalidadesByProvincia(idProvincia);
            if (_Localidades.Any(x => x.Key.Equals(idProvincia, StringComparison.InvariantCultureIgnoreCase)))
            {
                return _Localidades.First(x => x.Key.Equals(idProvincia, StringComparison.InvariantCultureIgnoreCase)).Value;
            }
            return null;

        }

        [WebMethod]
        public int? GetIdLocalidad(String idProvincia, String nombreLocalidad)
        {
            if (_Provincias == null || !_Provincias.Any())
            {
                LoadProvincias();
            }
            if (_Localidades == null)
            {
                _Localidades = new Dictionary<string, Dictionary<int, string>>();
            }
            if (!_Provincias.Keys.Any(x => x.Equals(idProvincia, StringComparison.InvariantCultureIgnoreCase)))
            {
                return null;
            }
            else
            {
                if (!_Localidades.Keys.Any(x => x.Equals(idProvincia, StringComparison.InvariantCultureIgnoreCase)))
                {
                    LoadLocalidadesByProvincia(idProvincia);
                }
                var locs = _Localidades.First(x => x.Key.Equals(idProvincia, StringComparison.InvariantCultureIgnoreCase)).Value;

                if (locs.Any(x => x.Value.Equals(nombreLocalidad, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return locs.First(x => x.Value.Equals(nombreLocalidad, StringComparison.InvariantCultureIgnoreCase)).Key;
                }
                return null;
            }
        }

    }
}
