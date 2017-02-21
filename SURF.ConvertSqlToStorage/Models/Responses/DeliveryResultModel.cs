using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
    public class DeliveryResultModel
    {
        public string OrderNumber { get; set; }
        public string OrderLineNumber { get; set; }
        public string OrderStatus { get; set; }
        public string ArticleCodeWebshop { get; set; }
        public string SupplierSerialNumber { get; set; }
        public string SupplierDownloadLink { get; set; }
        public string SupplierOrderNumber { get; set; }
        public string SupplierVouchers { get; set; }
        public string SupplierMessage { get; set; }
        public string SupplierErrors { get; set; }
        public string SupplierStatus { get; set; }
        public DateTime? SupplierStatusDate { get; set; }
        public DateTime? SupplierDeliveryDate { get; set; }
        public string SupplierTrackAndTraceCode { get; set; }
        public DateTime? SupplierLicenceExpirationDate { get; set; }
    }


}
