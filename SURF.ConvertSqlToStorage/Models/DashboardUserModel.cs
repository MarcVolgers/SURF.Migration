using SURF.Delivery.Order.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{
    public class DashboardUserModel : ModelBase<DashboardUserModel>
	{
        private int _id;

		//[IdentifierProperty]
        [RowKey]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RowKey = _id.ToString();
            }
        }
		public string Name { get; set; }

		public string WindowsLoginName { get; set; }
		public string WindowsDomainName { get; set; }

		//[IgnoreProperty]
		public string EscapedWindowsLoginName
		{
			get
			{
				return WindowsLoginName.Replace("/", @"//");
			}
		}

		public string Role { get; set; }

		//[IgnoreProperty]
		public DashboardUserRole DashboardUserRole
		{
			get
			{
				return (DashboardUserRole)Enum.Parse(typeof(DashboardUserRole), this.Role);
			}
			set
			{
				this.Role = value.ToString();
			}
		}

		public int DefaultWebshopId { get; set; }
	}
}