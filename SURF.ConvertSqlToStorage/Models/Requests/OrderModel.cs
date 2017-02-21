using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
    public class OrderModel
    {
        public string OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ContactId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactPrefix { get; set; }
        public string ContactLastName { get; set; }
        public string ContactStreet { get; set; }
        public string ContactHousenumber { get; set; }
        public string ContactZipCode { get; set; }
        public string ContactCity { get; set; }
        public string ContactCountry { get; set; }
        public string ContactGender { get; set; }
        public string ContactInstitution { get; set; }
        public List<OrderLineModel> OrderLines { get; set; }
        public bool ForceResend { get; set; }
    }
}
