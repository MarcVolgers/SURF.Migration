using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
    public class TemplateModel : ModelBase<TemplateModel>
	{
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                RowKey = _title = value;
            }
        }       
		public string Body { get; set; }
		public string Subject { get; set; }
		
	}
}
