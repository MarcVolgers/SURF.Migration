using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SURF.SqlDataFramework;
using SURF.SqlDataFramework.Attributes;

namespace SURF.Delivery.Database.Models
{
    public class DeliveryFinanceEntity : ModelBase<DeliveryFinanceEntity>
    {
        [IdentifierProperty]
        public long Id { get; set; }
        public long DeliveryId { get; set; }
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
        public string Status { get; set; }
        public FinanceStatus FinanceStatus
        {
            get
            {
                return (FinanceStatus)Enum.Parse(typeof(FinanceStatus), this.Status);
            }
            set
            {
                this.Status = value.ToString();
            }
        }
    }
}
