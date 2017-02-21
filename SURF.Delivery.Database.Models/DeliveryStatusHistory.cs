using SURF.SqlDataFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SURF.SqlDataFramework.Attributes;

namespace SURF.Delivery.Database.Models
{
    public class DeliveryStatusHistory : ModelBase<DeliveryStatusHistory>
    {
        [IdentifierProperty]
        public long Id { get; set; }
        public long DeliveryId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public int RetryCount { get; set; }
    }
}
