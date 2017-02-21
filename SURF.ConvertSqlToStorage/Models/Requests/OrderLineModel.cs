using Newtonsoft.Json;
using SURF.Delivery.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
	public class OrderLineModel
	{
		public string OrderLineNumber { get; set; }
		public int OrderAmount { get; set; }
		public string ArticleCodeWebshop { get; set; }
		public string ArticleCodeSupplier { get; set; }
		public string ArticleName { get; set; }
		public string ArticleMedium { get; set; }
		public string SupplierName { get; set; }
		public string Status { get; set; }
		public bool SendRegisteredMail { get; set; }
		public string Serialnumber { get; set; }

		#region Fields CSV
		public string Extra1 { get; set; }
		public string Extra2 { get; set; }
		public string TargetGroup { get; set; }
		#endregion

		#region Fields financial Export
		public DateTime RemittanceDate { get; set; }
		public float Price { get; set; }
		public float RemittancePrice { get; set; }
		public float PurchasePrice { get; set; }
		public float Margin { get; set; }
		public float Vat { get; set; }
		public float VatPercentage { get; set; }
		public int Multiplier { get; set; }
		public string ProviderName { get; set; }
		public string MediationAgreementName { get; set; }
		public string ProductvariantName { get; set; }
		public float ProductPlatform { get; set; }
		public string ProviderSKU { get; set; }
		#endregion

		[JsonIgnore]
		public DeliveryStatus DeliveryStatus
		{
			get
			{
				DeliveryStatus status;
				if (Enum.TryParse(this.Status, out status))
				{
					return status;
				}

				//throw new Exception(string.Format("Invalid status {0}", this.Status));
				return DeliveryStatus.Unknown;
			}
		}
	}
}
