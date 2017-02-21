using Newtonsoft.Json;
using SURF.Delivery.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
	public class DeliveryRequestModel
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string SupplierName { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DeliveryStatus? Status { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? IncludeFailed { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? CheckForCallbackDone { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string OrderNumber { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> OrderLineNumbers { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? WebshopId { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? OrderDateFrom { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? OrderDateTo { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? Id { get; set; }
	}
}