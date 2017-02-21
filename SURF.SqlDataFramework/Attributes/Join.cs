using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.SqlDataFramework.Attributes
{
    public class Join : Attribute
    {
        public JoinType JoinType { get; set; }
        public string JoinTargetTable { get; set; }
        public string JoinSourceField { get; set; }
        public string JoinTargetField { get; set; }
    }

    public enum JoinType
    {
        Left,
        Inner
    }
}
