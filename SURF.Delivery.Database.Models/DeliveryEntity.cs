using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SURF.SqlDataFramework;
using SURF.SqlDataFramework.Attributes;
using Newtonsoft.Json;

namespace SURF.Delivery.Database.Models
{
    [TableName(Name="Delivery")]
    public class DeliveryEntity : ModelBase<DeliveryEntity>
    {
        [IdentifierProperty]
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public string OrderLineNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public int OrderAmount { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ArticleCodeWebshop { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ArticleCodeSupplier { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ArticleName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ArticleMedium { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactEmail { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactInitials { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactPrefix { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactStreet { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactHousenumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactZipCode { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactCity { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactCountry { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactGender { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactInstitution { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactInstitutionDepartment { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactInstitutionReference { get; set; }
        public string SupplierName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierSerialNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierDownloadlink { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierOrderNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierVouchers { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierErrors { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierStatus { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SupplierStatusDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SupplierDeliveryDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierInvoiceNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SupplierTrackAndTraceCode { get; set; }        
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public int RetryCount { get; set; }
        public int InProcessing { get; set; }
        public long WebshopId { get; set; }
        public bool SendRegisteredMail { get; set; }
        /*public string WebshopCallbackUrl
        {
            get
            {
                UitleverstraatRepository uitleverstraatRepository = new UitleverstraatRepository();
                var webshop = uitleverstraatRepository.GetWebshop(this.WebshopId);
                return webshop.CallbackUrl;
            }
        }*/
        public bool WebshopCallbackDone { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TargetGroup { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Extra1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Extra2 { get; set; }
        [IgnoreProperty]
        public string ContactFullName
        {
            get
            {
                return string.Format("{0} {1}{2}{3}",
                                     this.ContactInitials,
                                     this.ContactPrefix,
                                     string.IsNullOrEmpty(this.ContactPrefix) ? string.Empty : " ",                                     
                                     this.ContactName);
            }
        }
        [IgnoreProperty]
        public string ContactFullLastName
        {
            get
            {
                return string.Format("{0}{1}{2}",
                                     this.ContactPrefix,                                     
                                     string.IsNullOrEmpty(this.ContactPrefix) ? string.Empty : " ",
                                     this.ContactName);
            }
        }
        [IgnoreProperty]
        public DeliveryStatus DeliveryStatus
        {
            get
            {
                return (DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), this.Status);
            }
            set
            {
                this.Status = value.ToString();
            }
        }
        [IgnoreProperty]
        public DeliveryFinanceEntity FinanceEntity { get; set; }

        public bool AddedToCallbackQueue { get; set; }
        [IgnoreProperty]
        public bool AddedToInvoiceLineQueue { get; set; }
        [IgnoreProperty]
        public string StatusDutchDescription
        {
            get
            {
                return StatusDescription(this.DeliveryStatus, "NL");
            }
        }
        [IgnoreProperty]
        public static string StatusDescription(string status, string language)
        {
            return StatusDescription((DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), status), language);
        }
        [IgnoreProperty]
        public static string StatusDescription(DeliveryStatus status, string language)
        {
            switch (language)
            {
                case "NL":
                    switch (status)
                    {
                        case DeliveryStatus.Backorder: return "Backorder";
                        case DeliveryStatus.Failed: return "Mislukt";
                        case DeliveryStatus.OfferedToDeliverer: return "Aangeboden aan uitleveraar";
                        case DeliveryStatus.SendFromDeliverer: return "Verzonden door uitleveraar";
                        case DeliveryStatus.ToDeliver: return "Uit te leveren";
                        case DeliveryStatus.Unavailable: return "Niet beschikbaar";
                        case DeliveryStatus.Cancelled: return "Geannuleerd";
                    }
                    break;
            }

            return status.ToString();
        }        
    }
}
