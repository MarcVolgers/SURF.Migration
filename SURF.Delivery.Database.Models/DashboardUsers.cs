using SURF.SqlDataFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SURF.SqlDataFramework.Attributes;

namespace SURF.Delivery.Database.Models
{
    public enum DashboardUserRole
    {
        Admin,
        Reader,
        Inactive
    }

    public class DashboardUser : ModelBase<DashboardUser>
    {
        [IdentifierProperty]
        public int Id { get; set; }
        public string Name { get; set; }

        public string WindowsLoginName { get; set; }
        public string WindowsDomainName { get; set; }        

        [IgnoreProperty]
        public string EscapedWindowsLoginName
        {
            get
            {
                return WindowsLoginName.Replace("/", @"//");
            }
        }

        public string Role { get; set; }

        [IgnoreProperty]
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
