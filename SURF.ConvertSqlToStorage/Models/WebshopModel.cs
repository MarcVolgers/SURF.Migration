using SURF.Delivery.Order.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
    public class WebshopModel : ModelBase <WebshopModel>
    {
        private string _name;
        [RowKey]
        public string Name
        {
            get { return _name; }
            set
            {
                RowKey = _name = value;
            }
        }
        public int Id { get; set; }
        //public string Name { get; set; }
        public string Password { get; set; }
        public string CallbackUrl { get; set; }
        public string CallbackSuccesMessage { get; set; }
        public string CallbackUrlUsername { get; set; }
        public string CallbackUrlPassword { get; set; }
        public string CallbackUrlDomain { get; set; }
        public string CallbackUrlParametername { get; set; }
        public bool CallbackUrlIsAsmx { get; set; }
        public bool UseExtendedError { get; set; }

    }
}
