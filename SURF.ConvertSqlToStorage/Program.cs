using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SURF.Delivery.Database.Models;
using SURF.Delivery.Database.Proxy;
using SURF.Delivery.Order.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.ConvertSqlToStorage
{
    class Program
    {
        private static Settings _settings;

        private static Source ParseSource(string source)
        {
            Source src = Source.Unknown;
            Enum.TryParse(source, out src);
            return src;
        }
        private static void CreateSettings(string[] args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                var split = arg.Split('=');
                result.Add(split[0].ToLower(), split[1]);
            }

            _settings = new Settings
            {
                ConnectionString = result.ContainsKey("connectionstring") ? result["connectionstring"] : null,
                Files = result.ContainsKey("files") ? result["files"] : null,
                ServiceUrl = result.ContainsKey("serviceurl") ? result["serviceurl"] : null,
                Source = result.ContainsKey("source") ? ParseSource(result["source"]) : Source.Unknown
            };
#if DEBUG
            if (string.IsNullOrEmpty(_settings.ConnectionString)) _settings.ConnectionString = "Data Source=Localhost;Initial Catalog=DeliveryDatabase;Trusted_Connection=True;";
            if (_settings.Source == Source.Unknown) _settings.Source = Source.Database;
            if (string.IsNullOrEmpty(_settings.Files)) _settings.Files = @"D:\Temp\Migrate";
            if (string.IsNullOrEmpty(_settings.ServiceUrl)) _settings.ServiceUrl= "http://localhost:8277/api/data";            
#endif
        }

        static void Main(string[] args)
        {
            CreateSettings(args);

            var log = new StringBuilder();

            //var argD = ArgsToDict(args);



            if (_settings.Source == Source.Database)
            {

                DeliveryContext context = new DeliveryContext(_settings.ConnectionString);

                PostToService(_settings.ServiceUrl, "account", context.Select<CacheDeliverers>().Select(d => new AccountModel { CrmName = d.Name, Name = d.Lmng_uitleveraarnaam, Mailadress = d.Emailaddress3 }));
                PostToService(_settings.ServiceUrl, "dashboardUser", context.Select<DashboardUser>());

                var deliveries = context.Select<DeliveryEntity>(top: 100);
                var statuses = context.Select<Delivery.Database.Models.DeliveryStatusHistory>().Where(s => deliveries.Any(d => s.DeliveryId == d.Id));

                PostToService(_settings.ServiceUrl, "delivery", deliveries);
                PostToService(_settings.ServiceUrl, "deliverystatus", statuses);

                PostToService(_settings.ServiceUrl, "webshop", context.Select<Webshop>());
                PostToService(_settings.ServiceUrl, "template", context.Select<CacheTemplates>().Select(t => new TemplateModel { Body = t.PresentationXml, Subject = t.SubjectPresentationXml, Title = t.Title }));
            }

            if (_settings.Source == Source.Json)
            {
                string[] entities = new[] { "account", "dashboardUser", "delivery", "deliveryStatus", "webshop", "template" };
                foreach (var entity in entities)
                {
                    var fileName = Path.Combine(_settings.Files, entity + ".json");
                    PostToService(_settings.ServiceUrl, entity, LoadJsonFromFile(fileName));
                }
            }

            Console.Write(_log.ToString());
            Console.ReadKey();
        }

        //private static void SaveToJson<T>(IEnumerable<T> data)
        //{
        //    var fileName = string.Empty;

        //    if (typeof(T) == typeof(AccountModel)) fileName = "account.json";
        //    if (typeof(T) == typeof(DashboardUser)) fileName = "dashboardUser.json";
        //    if (typeof(T) == typeof(DeliveryEntity)) fileName = "delivery.json";
        //    if (typeof(T) == typeof(Delivery.Database.Models.DeliveryStatusHistory)) fileName = "deliveryStatus.json";
        //    if (typeof(T) == typeof(Webshop)) fileName = "webshop.json";
        //    if (typeof(T) == typeof(TemplateModel)) fileName = "template.json";


        //    using (var stream = File.CreateText(Path.Combine(_settings.Files, fileName)))
        //    using (var writer = new JsonTextWriter(stream))
        //    {
        //        var jobj = JArray.FromObject(data);
        //        jobj.WriteTo(writer);
        //    }
        //}

        private static StringBuilder _log = new StringBuilder();
        private static void PostToService<T>(string url, string entity, IEnumerable<T> data) //, bool saveToJson = false)
        {
            try
            {
                //if (saveToJson)
                //{
                //    SaveToJson(data);
                //}

                var result = ServiceHelper.PostToService(url, entity, data);
                Console.WriteLine(result.ToString());
                _log.AppendLine(entity + ": succes");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _log.AppendLine(entity + ": failed");
            }
        }

        private static void PostToService<T>(string url, string entity, string json)
        {
            try
            {
                var result = ServiceHelper.PostToService(url, entity, json);
                Console.WriteLine(result.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static JArray LoadJsonFromFile(string fileName)
        {
            var file = new FileInfo(fileName);
            using (var stream = file.OpenText())
            using (var reader = new JsonTextReader(stream))
            {
                JArray result = (JArray)JArray.ReadFrom(reader);
                return result;
            }

            throw new Exception("Cannot load file");
        }
    }

    public enum Source
    {
        Unknown,
        Database,
        Json
    }

    public class Settings
    {
        public string ServiceUrl;
        public Source Source;        
        public string Files;
        public string ConnectionString;
    }
}
