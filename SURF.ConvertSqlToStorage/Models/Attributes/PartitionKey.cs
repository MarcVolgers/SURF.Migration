using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models.Attributes
{
    public class PartitionKey : Attribute
    {
        public string Name { get; set; }
    }
}
