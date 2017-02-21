using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
	public class CallbackQueueModel_old //  : ModelBase<CallbackQueueModel>
	{

		//[IdentifierProperty]
		public long Id { get; set; }
		public long DeliveryId { get; set; }
		public bool IsProcessed { get; set; }
		public int RetryCount { get; set; }
		public bool InProcess { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? LastDateTried { get; set; }
		public bool Failed { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ResultMessage { get; set; }
	}
}
