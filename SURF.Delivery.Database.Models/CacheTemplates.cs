using SURF.SqlDataFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Database.Models
{
    public class CacheTemplates : ModelBase<CacheTemplates>
    {
        public string Title { get; set; }
        public string PresentationXml { get; set; }
        public string SubjectPresentationXml { get; set; }
    }
}
