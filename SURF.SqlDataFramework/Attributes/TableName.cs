using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.SqlDataFramework.Attributes
{
    public class TableName : Attribute
    {
        public string Name { get; set; }
    }

    public class Schema : Attribute
    {
        public string Name { get; set; }
    }
}
