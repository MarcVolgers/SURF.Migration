using SURF.SqlDataFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Database.Models
{
    public class CacheDeliverers : ModelBase<CacheDeliverers>
    {
        public string Name { get; set; }
        public string Lmng_uitleveraarnaam { get; set; }
        public string Emailaddress3 { get; set; }
    }
}
