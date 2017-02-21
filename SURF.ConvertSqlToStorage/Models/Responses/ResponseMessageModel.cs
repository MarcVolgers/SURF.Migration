using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
	public class ResponseMessageModel<T>
	{
		public ResponseMessageType ResponseMessageType
		{
			get
			{
				ResponseMessageType result;
				if (Enum.TryParse(this.Type, out result))
				{
					return result;
				}
				else
				{
					throw new Exception(string.Format("Cannot parse type {0}", this.Type));
				}
			}
			set
			{
				Type = value.ToString();
			}
		}
		public string Type { get; set; }
        [JsonIgnore]
		//public UitleverstraatResultCode ResultCode { get; set; }
		//public string ResultDescription
		//{
		//	get
		//	{
		//		return UitleverstraatResultCodeHelper.GetDescription(ResultCode);
		//	}
		//}
		public string Message { get; set; }
		public Guid CorrelationId { get; set; }
		public Exception Exception { get; set; }
		public T ResponseObject { get; set; }
	}
}
