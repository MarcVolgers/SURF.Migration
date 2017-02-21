using Newtonsoft.Json;
using SURF.SqlDataFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SURF.SqlDataFramework
{
    [TableName(Name = null)]
    public interface IModelBase
    {
        [JsonIgnore]
        [IgnoreProperty]
        object this[string fieldName] { get; set; }
        Dictionary<string, PropertyInfo> GetFields();
        object[] ToObjectArray();
        void AddAsParametersToSqlCommand(SqlCommand cmd);
        List<string> GetFieldValues(string[] fields, string encapsulateString = null);
    }
}
