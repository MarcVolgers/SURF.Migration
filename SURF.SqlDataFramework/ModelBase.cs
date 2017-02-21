using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SURF.SqlDataFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SURF.SqlDataFramework
{
    [Serializable]
    public abstract class ModelBase<T> : IModelBase
       where T : new()
    {
        private PropertyInfo[] _properties;

        [JsonIgnore]
        [IgnoreProperty]
        private PropertyInfo[] Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = typeof(T).GetProperties();
                }
                return _properties;
            }

        }

        #region properties

        [JsonIgnore]
        [IgnoreProperty]
        public object this[string fieldName]
        {
            get
            {
                var property = Properties.FirstOrDefault(p => p.Name == fieldName);

                if (property != null)
                {
                    return property.GetValue(this);
                }
                else
                {
                    if (!GetType().IsValueType || (Nullable.GetUnderlyingType(GetType()) != null))
                    {
                        return null;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            set
            {
                var property = Properties.FirstOrDefault(p => p.Name == fieldName);

                if (property != null)
                {
                    property.SetValue(this, value);
                }
            }
        }

        #endregion

        #region public methods

        public JObject ToJObject()
        {
            return JObject.FromObject(this);
        }

        public static T LoadJObject(JObject jObject)
        {
            return LoadJson((jObject.ToString()));
        }
        public static T LoadJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public override string ToString()
        {
            var toStringProperties = Properties.Where(p => p.GetCustomAttributes().Any(q => q.GetType() == typeof(ToStringProperty)));
            if (toStringProperties.Any())
                return string.Join(", ", toStringProperties.Select(p => p.GetValue(this)));

            return base.ToString();
        }

        /// <summary>
        /// Adds al Properties as parameters to the SqlCommand.
        /// Properties marked as [IgnoreProperty] are ignored.
        /// </summary>
        /// <param name="cmd">The SqlCommand</param>
        public void AddAsParametersToSqlCommand(SqlCommand cmd)
        {
            Type t = typeof(T);

            foreach (PropertyInfo pi in Properties)
            {
                try
                {
                    if (!pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IgnoreProperty)))
                    {
                        if (pi.GetValue(this) == null)
                            cmd.Parameters.Add(new SqlParameter(pi.Name, DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter(pi.Name, pi.GetValue(this)));
                    }
                }
                catch { }
            }
        }

        public static T ReadFromSqlReader(SqlDataReader reader)
        {
            Type t = typeof(T);

            T item = new T();

            try
            {
                for (int a = 0; a < reader.FieldCount; a++)
                {
                    PropertyInfo pi = t.GetProperty(reader.GetName(a));

                    if (pi != null)
                    {
                        if (!reader.IsDBNull(a))
                        {
                            pi.SetValue(item, reader.GetValue(a));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //_log.WarnFormat("Cannot read object of type {0}.\r\nError: {1}", t.Name, ex);
            }

            return item;
        }

        public object[] ToObjectArray()
        {
            List<object> objectRow = new List<object>();

            foreach (var field in GetFields())
            {
                objectRow.Add(this[field.Key]);
            }

            return objectRow.ToArray();
        }

        public Dictionary<string, PropertyInfo> GetFields()
        {
            return Properties.Where(p => p.GetCustomAttributes().Any(s => s.GetType() == typeof(IgnoreProperty))).ToDictionary(q => q.Name, r => r);
        }

        /// <summary>
        /// Get a list of values of the current object for given fields.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="encapsulateString"></param>
        /// <returns></returns>
        public List<string> GetFieldValues(string[] fields = null, string encapsulateString = null)
        {
            List<string> result = new List<string>();

            //Type t = typeof(T);

            foreach (PropertyInfo pi in Properties)
            {
                try
                {
                    if (!pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IgnoreProperty)) &&
                        (fields == null || (fields != null && fields.Any(f => f == pi.Name))))
                    {
                        object fieldValue = pi.GetValue(this);

                        string value = string.Empty;

                        if (fieldValue != null)
                        {
                            value = fieldValue.ToString();
                        }

                        if (encapsulateString != null)
                        {
                            value = string.Format("{0}{1}{0}", encapsulateString, value);
                        }

                        result.Add(value);
                    }
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public T Clone()
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(this, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        #endregion               
    }
}
