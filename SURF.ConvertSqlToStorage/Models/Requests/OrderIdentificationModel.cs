using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
	public class OrderIdentificationModel
	{
		public OrderIdentificationModel(string orderNumber, string orderLineNumber)
		{
			OrderNumber = orderNumber;
			OrderLineNumber = orderLineNumber;
		}
		public string OrderNumber { get; set; }
		public string OrderLineNumber { get; set; }
	}
}
