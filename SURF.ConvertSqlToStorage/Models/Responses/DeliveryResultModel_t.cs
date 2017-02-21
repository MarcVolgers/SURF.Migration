using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
	public class DeliveryResultModel<T>
	{
		public DeliveryResultModel() { }
		public DeliveryResultModel(T item)
		{
			Data = new[] { item };
		}

		public DeliveryResultModel(IEnumerable<T> items)
		{
			Data = items;
		}
		public bool Succes { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Exception Exception { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<T> Data { get; set; }
	}
}
