using SURF.SqlDataFramework.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SURF.SqlDataFramework
{
    public static class ModelBaseHelper
    {
        public static string CreateTableExistsCode(string schema, string tableName)
        {
            string code = string.Format(
                             @"IF (NOT EXISTS (SELECT * 
                             FROM INFORMATION_SCHEMA.TABLES 
                             WHERE TABLE_SCHEMA = '{0}' 
                             AND  TABLE_NAME = '{1}'))", schema, tableName);
            return code;
        }

        public static string CreateTableCommand<T>()
        {
            var result = new StringBuilder();
            var t = typeof(T);

            result.AppendLine(string.Format("CREATE TABLE {0}(", GetTableName<T>()));

            List<string> fieldList = new List<string>();

            var properties = t.GetProperties();            
            foreach (PropertyInfo pi in t.GetProperties())
            {
                try
                {
                    if (!pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IgnoreProperty)))
                    {
                        // IDENTITY(1,1) for IdentityPropery
                        var type = pi.PropertyType;
                        string isNullable = Nullable.GetUnderlyingType(pi.PropertyType) != null ? "NULL" : string.Empty;
                        string identity = pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IdentifierProperty)) ? "IDENTITY(1,1)" : string.Empty;
                        string columnRow = string.Format("[{0}] {1} {2} {3}", pi.Name, GetSqlType(pi.PropertyType.ToString()), identity, isNullable);

                        if(pi != properties.Last())
                        {
                            columnRow += ",";
                        }

                        result.AppendLine(columnRow);
                    }

                    
                }
                catch { }
            }

            return result.ToString();
        }

        public static string GetSqlType(string type)
        {
            switch (type)
            {                
                case "System.Int32": return "[int]";
                case "System.Int64": return "[bigint]";
                case "System.String": return "[nvarchar](max)";
                case "System.Boolean": return "[bit]";
                default: return type;
            }
        }

        /// <summary>
        /// Creates a deletequery.
        /// </summary>
        /// <param name="where">Optional where clause.</param>
        /// <returns></returns>
        public static string CreateDeleteQuery<T>(string where = null)
        {
            var t = typeof(T);
            return string.Format("DELETE FROM {0}{1}", GetTableName<T>(), where != null ? " WHERE " + where : "");
        }

        /// <summary>
        /// Creates an update query.
        /// </summary>
        /// <param name="fields">Optional array of fields. When left empty all fields without attribute [IgnoreProperty] are used.</param>
        /// <returns></returns>
        public static string CreateInsertQuery<T>(string[] fields = null, bool includingIdentifier = false)
        {
            Type t = typeof(T);

            List<string> fieldList = new List<string>();
            List<string> paramsList = new List<string>();
            var identifierName = string.Empty;

            foreach (PropertyInfo pi in t.GetProperties())
            {
                try
                {
                    if (!pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IgnoreProperty)) &&
                        (includingIdentifier ? true : !pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IdentifierProperty))) &&
                       (fields == null || (fields != null && fields.Any(f => f == pi.Name))))
                    {
                        fieldList.Add(string.Format("[{0}]", pi.Name));
                        paramsList.Add(string.Format("@{0}", pi.Name));
                    }

                    if(pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IdentifierProperty)))
                    {
                        identifierName = pi.Name;
                    }
                }
                catch { }
            }

            var insertedId = string.IsNullOrEmpty(identifierName) ? string.Empty : " OUTPUT inserted." + identifierName;


            string query = string.Format("INSERT INTO {0} ({1}) {3} VALUES ({2})", GetTableName<T>(), string.Join(",", fieldList), string.Join(",", paramsList), insertedId);

            return query;
        }

        /// <summary>
        /// Creates an update query.
        /// </summary>
        /// <param name="fields">Optional array of fields. When left empty all fields without attribute [IgnoreProperty] are used.</param>
        /// <param name="where">Optional where clause (without WHERE). When left empty [IdentifierProperty] fields are used.</param>
        /// <returns>Update statement with params (e.g. UPDATE table SET [value] = @value WHERE [id] = @id)</returns>
        public static string CreateUpdateQuery<T>(string[] fields = null, string where = null)
        {
            var t = typeof(T);
            List<string> fieldParamsList = new List<string>();
            
            if (where != null)
            {
                where = where.ToLower().StartsWith("where") ? Regex.Replace(where, "where", string.Empty, RegexOptions.IgnoreCase) : where;
            }
            else
            {
                where = CreateWhereIdentifierClause<T>();
            }

            foreach (PropertyInfo pi in t.GetProperties())
            {
                try
                {
                    if (!pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IgnoreProperty)) &&
                        !pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IdentifierProperty)) &&
                        (fields == null || (fields != null && fields.Any(f => f == pi.Name))))
                    {
                        fieldParamsList.Add(string.Format("[{0}] = @{0}", pi.Name));
                    }
                }
                catch { }
            }

            string query = string.Format("UPDATE {0} SET {1} WHERE {2}", GetTableName<T>(),
                                                                         string.Join(",", fieldParamsList),
                                                                         where);

            return query;
        }

        public static string CreateWhereIdentifierClause<T>()
        {
            var t = typeof(T);
            List<string> whereIdList = new List<string>();

            foreach (PropertyInfo pi in t.GetProperties())
            {
                try
                {
                    if (pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IdentifierProperty)))
                    {
                        whereIdList.Add(string.Format("[{0}] = @{0}", pi.Name));
                    }
                }
                catch { }
            }

            return string.Join(" AND ", whereIdList);
        }

        /// <summary>
        /// Creates a select query.
        /// </summary>
        /// <param name="fields">Optional array of fields. When left empty all fields without attribute [IgnoreProperty] are used.</param>
        /// <param name="distinct">When true the distinct operator is added.</param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string CreateSelectQuery<T>(string[] fields = null, bool distinct = false, string where = null, int top = 0)
        {
            var t = typeof(T);
            List<string> fieldList = new List<string>();
            List<string> joinList = new List<string>();
            
            foreach (PropertyInfo pi in t.GetProperties())
            {
                try
                {
                    if (!pi.GetCustomAttributes().Any(p => p.GetType() == typeof(IgnoreProperty)) && 
                        (fields == null || (fields != null && fields.Any(f => f.ToLower() == pi.Name.ToLower()))))
                    {
                        fieldList.Add(string.Format("s.[{0}]", pi.Name));
                    }
                    if(pi.GetCustomAttributes().Any(p => p.GetType() == typeof(Join)))
                    {
                        var joinAttribute = (Join)pi.GetCustomAttributes().FirstOrDefault(p => p.GetType() == typeof(Join));
                        int joinNumber = joinList.Count() + 1;

                        joinList.Add(string.Format(" {0} JOIN {1} t{2} ON t{2}.{3} = s.{4}", joinAttribute.JoinType, joinAttribute.JoinTargetTable, joinNumber, joinAttribute.JoinTargetField, joinAttribute.JoinSourceField));
                    }
                }
                catch(Exception ex)
                {
                }
            }

            string query = string.Format("SELECT {4}{2}{0} FROM {1} s {5} {3}", string.Join(",", fieldList), 
                                                                         GetTableName<T>(), 
                                                                         distinct ? "DISTINCT " : "", 
                                                                         where == null ? "" : " WHERE " + where,
                                                                         top == 0 ? "" : "TOP "+top+" ",
                                                                         string.Join(" ", joinList));

            return query;
        }

        public static string GetTableName<T>()
        {
            var t = typeof(T);
            var tableNameAttribute = (TableName)t.GetCustomAttribute(typeof(TableName));
            if (tableNameAttribute != null && tableNameAttribute.Name != null)
                return tableNameAttribute.Name;

            return t.Name;
        }

        public static string GetSchema<T>()
        {
            var t = typeof(T);
            var tableNameAttribute = (Schema)t.GetCustomAttribute(typeof(Schema));
            if (tableNameAttribute != null && tableNameAttribute.Name != null)
                return tableNameAttribute.Name;

            return "dbo"; // return default schema
        }

        /// <summary>
        /// Reads a value from the reader and converts to T.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T ReadFromSqlReader<T>(SqlDataReader reader, string[] fields = null) where T : IModelBase, new()
        {
            Type t = typeof(T);

            T item = new T();

            try
            {
                for (int a = 0; a < reader.FieldCount; a++)
                {
                    PropertyInfo pi = t.GetProperty(reader.GetName(a));

                    if (pi != null && (fields == null || (fields != null && fields.Any(f => f.ToLower() == pi.Name.ToLower()))))
                    {
                        var name = pi.Name;

                        if (!reader.IsDBNull(a))
                        {
                            try
                            {
                                pi.SetValue(item, reader.GetValue(a));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return item;
        }
    }
}
