using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SURF.Delivery.Order.Models.Attributes;
using System.ComponentModel;

namespace SURF.Delivery.Order.Models
{
    public class ModelBase<T> : TableEntity
    {        
        public ModelBase()
        {
            PartitionKey = CreatePartitionKey();
            RowKey = CreateRowKey();
        }

        public string CreatePartitionKey()
        {
            Type t = typeof(T);

            var partKey = t.CustomAttributes.FirstOrDefault(c => c.AttributeType == typeof(PartitionKey));

            if (partKey != null)
            {
                partKey.NamedArguments.FirstOrDefault(a => a.MemberName == "Name").TypedValue.ToString();
            }

            return t.Name;
        }

        public string CreateRowKey()
        {
            Type t = typeof(T);

            var rowKeys = t.GetProperties().Where(p => p.GetCustomAttribute<RowKey>() is RowKey);

            var key = rowKeys.FirstOrDefault(); //.GetValue(this).ToString();

            if (key != null)
            {
                var val = key.GetValue(this);

                if (val != null)
                {
                    return val.ToString();
                }
                //throw new Exception("No RowKey defined.");
            }

            return null;
        }

        public string CreateCombinedRowKey(string[] values)
        {
            return string.Join(";", values);
        }
    }
}
