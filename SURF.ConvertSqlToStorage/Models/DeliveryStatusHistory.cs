using SURF.Delivery.Order.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
	public class DeliveryStatusHistory : ModelBase<DeliveryStatusHistory>
	{
        private long _id;
		//[IdentifierProperty]
        [RowKey]
		public long Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                RowKey = value.ToString();
            }
        }
		public long DeliveryId { get; set; }
		public DateTime Date { get; set; }
		public string Status { get; set; }
		public int RetryCount { get; set; }
	}
}
