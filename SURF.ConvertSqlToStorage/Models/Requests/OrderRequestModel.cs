using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
    public class OrderRequestModel
    {
        public AuthenticationModel Authentication { get; set; }
        public OrderModel Order { get; set; }

        public object JsonEncode()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
