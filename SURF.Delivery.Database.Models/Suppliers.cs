using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SURF.SqlDataFramework.Attributes;
using SURF.SqlDataFramework;

namespace SURF.Delivery.Database.Models
{
    public class Suppliers : ModelBase<Suppliers>
    {
        [IdentifierProperty]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
