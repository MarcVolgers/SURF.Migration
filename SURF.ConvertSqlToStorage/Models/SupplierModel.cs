using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
    [Obsolete]
	public class SupplierModel : ModelBase<SupplierModel>
	{
		//[IdentifierProperty]
		public int Id { get; set; }
        private string _name;
		public string Name
        {
            get { return _name; }
            set
            {
                RowKey = _name = value;
            }
        }
	}
}
