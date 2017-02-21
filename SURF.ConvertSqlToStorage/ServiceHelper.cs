using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SURF.ConvertSqlToStorage
{
    public static class ServiceHelper
    {
        public static JObject GetFromService<T>(string service, string action, Dictionary<string, string> queryParams = null)
        {
            HttpClient client = new HttpClient();
            var requestUri = string.Format("{0}/{1}", service, action);
            if (queryParams != null && queryParams.Any())
            {
                requestUri += string.Format("?{0}", string.Join("&", queryParams.Select(q => string.Format("{0}={1}", q.Key, q.Value))));

            }
            var response = client.GetAsync(requestUri);

            var responseResult = response.Result.Content.ReadAsStringAsync().Result;

            var result = JObject.Parse(responseResult);

            return result;
        }

        public static JObject DeleteFromService<T>(string service, string action, Dictionary<string, string> queryParams = null)
        {
            HttpClient client = new HttpClient();
            var requestUri = string.Format("{0}/{1}", service, action);
            if (queryParams != null && queryParams.Any())
            {
                requestUri += string.Format("?{0}", string.Join("&", queryParams.Select(q => string.Format("{0}={1}", q.Key, q.Value))));

            }
            var response = client.DeleteAsync(requestUri);

            var responseResult = response.Result.Content.ReadAsStringAsync().Result;

            var result = JObject.Parse(responseResult);

            return result;
        }

        public static JObject PostToService<T>(string service, string action, T obj)
        {
            return PostToService<T>(service, action, GetJsonString(obj));
        }

        public static JObject PostToService<T>(string service, string action, IEnumerable<T> obj)
        {
            return PostToService<T>(service, action, GetJsonString(obj));
        }

        public static JObject PostToService<T, T2>(string service, string action, T2 obj)
        {
            return PostToService<T>(service, action, GetJsonString(obj));
        }

        public static JObject PostToService<T, T2>(string service, string action, IEnumerable<T2> obj)
        {
            return PostToService<T>(service, action, GetJsonString(obj));
        }

        public static JObject PostToService<T>(string service, string action, string body)
        {
            HttpClient client = new HttpClient();
            var requestUri = string.Format("{0}/{1}", service, action);

            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = client.PostAsync(requestUri, content);

            var responseResult = response.Result.Content.ReadAsStringAsync().Result;

            var result = JObject.Parse(responseResult);

            return result;
        }

        public static string GetJsonString<T>(T obj)
        {
            var json = string.Empty;

            if (IsEnumerableType(typeof(T)))
            {
                json = JArray.FromObject(obj).ToString();
            }
            else
            {
                json = JObject.FromObject(obj).ToString();
            }

            return json;
        }

        public static bool IsEnumerableType(Type type)
        {
            return type.GetInterfaces().Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                   type.GetInterfaces().Any(i => i.Name.Contains("IEnumerable"));
        }
    }
}
