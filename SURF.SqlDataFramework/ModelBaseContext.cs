using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SURF.SqlDataFramework
{
    public abstract class ModelBaseContext
    {
        private object _locker = new object();
        private SqlConnection _connection;
        private string _connectionString;

        public ModelBaseContext(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(connectionString);
        }

        public SqlConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        //public void InsertInto<T>(T item, bool clearTable = false) where T : IModelBase
        //{
        //    InsertInto<T>(new[] { item }, clearTable);
        //}

        public IEnumerable<long> InsertInto<T>(IEnumerable<T> dataList, bool clearTable = false) where T : IModelBase
        {
            var result = new List<long>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                if (clearTable)
                {
                    SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = ModelBaseHelper.CreateDeleteQuery<T>() };

                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                    {
                        result.Add((long)scalar);
                    }
                }

                var insertQuery = ModelBaseHelper.CreateInsertQuery<T>();
                foreach (var combiRow in dataList)
                {
                    SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = insertQuery };
                    combiRow.AddAsParametersToSqlCommand(cmd);
                    
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                    {
                        result.Add((long)scalar);
                    }
                }

                conn.Close();
            }

            return result;
        }

        //public void Update<T>(T item, Expression<Func<T, bool>> whereFunc = null, string[] fields = null) where T : IModelBase
        //{
        //    Update(new[] { item }, whereFunc, fields);
        //}
        public void Update<T>(IEnumerable<T> dataList, Expression<Func<T, bool>> whereFunc, string[] fields = null) where T : IModelBase
        {
            string where = null;
            if (whereFunc != null)
            {
                where = whereFunc.ToSqlString();
            }
            Update(dataList, where, fields);
        }

        public void Update<T>(IEnumerable<T> dataList, string where = null, string[] fields = null) where T : IModelBase
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var updateQuery = ModelBaseHelper.CreateUpdateQuery<T>(fields, where);
                foreach (var combiRow in dataList)
                {
                    SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = updateQuery };
                    combiRow.AddAsParametersToSqlCommand(cmd);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        /// <summary>
        /// Bulkcopies the list of data into the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void BulkCopy<T>(List<T> data) where T : IModelBase, new()
        {
            var columns = new T().GetFields().ToDictionary(p => p.Key, q => q.Value.GetType());

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                DataTable table = new DataTable();
                table.Columns.AddRange(columns.Select(c => new DataColumn(c.Key, c.Value)).ToArray());

                SqlBulkCopy bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, trans) { DestinationTableName = ModelBaseHelper.GetTableName<T>() };
                foreach (var column in columns)
                {
                    bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.Key, column.Key));
                }

                data.ForEach(row => table.Rows.Add(row.ToObjectArray()));

                bulk.WriteToServer(table);

                trans.Commit();
            }
        }

        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> whereFunc, string[] fields = null, bool distinct = false, int top = 0) where T : IModelBase, new()
        {
            var whereClause = whereFunc.ToSqlString();
            return Select<T>(whereClause, fields, distinct, top);
        }

        public IEnumerable<T> Select<T>(string where = null, string[] fields = null, bool distinct = false, int top = 0) where T : IModelBase, new()
        {
            List<T> result = new List<T>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var selectQuery = ModelBaseHelper.CreateSelectQuery<T>(fields, distinct, where, top);
                SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = selectQuery };
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(ModelBaseHelper.ReadFromSqlReader<T>(reader, fields));
                }

                conn.Close();
            }

            return result;
        }

        public T SelectSingle<T>(Expression<Func<T, bool>> whereFunc, string[] fields = null, bool distinct = false, int top = 0) where T : IModelBase, new()
        {
            var where = whereFunc.ToSqlString();
            return SelectSingle<T>(where, fields, distinct, top);
        }

        public T SelectSingle<T>(string where = null, string[] fields = null, bool distinct = false, int top = 0) where T : IModelBase, new()
        {
            T result = new T();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var selectQuery = ModelBaseHelper.CreateSelectQuery<T>(fields, distinct, where, 1);
                SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = selectQuery };
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = ModelBaseHelper.ReadFromSqlReader<T>(reader, fields);
                }

                conn.Close();
            }

            return result;
        }

        public int SelectValue(string query, Guid accountId)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = query };
                cmd.Parameters.Add(new SqlParameter("accountId", accountId));
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result = (int)reader.GetSqlInt32(0);
                }

                conn.Close();
            }

            return result;
        }

        public IEnumerable<Guid> SelectIds(string query)
        {
            var result = new List<Guid>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = query };
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add((Guid)reader.GetSqlGuid(0));
                }

                conn.Close();
            }

            return result;
        }

        public void Delete<T>(Expression<Func<T, bool>> whereFunc) where T : IModelBase
        {
            var where = whereFunc.ToSqlString();
            Delete<T>(where);
        }

        public void Delete<T>(string where = null, SqlTransaction transaction = null) where T : IModelBase
        {
            if (transaction != null)
            {
                var deleteQuery = ModelBaseHelper.CreateDeleteQuery<T>(where);
                SqlCommand cmd = new SqlCommand() { Connection = transaction.Connection, CommandText = deleteQuery, Transaction = transaction };
                cmd.ExecuteNonQuery();
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    var deleteQuery = ModelBaseHelper.CreateDeleteQuery<T>(where);
                    SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = deleteQuery };
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public void Create<T>() where T: IModelBase
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var query = ModelBaseHelper.CreateTableExistsCode(ModelBaseHelper.GetSchema<T>(), ModelBaseHelper.GetTableName<T>());
                query += "BEGIN\r\n";
                query += ModelBaseHelper.CreateTableCommand<T>() + ")\r\n";
                query += "END";

                SqlCommand cmd = new SqlCommand() { Connection = conn, CommandText = query };
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        public bool StartTransaction(ref SqlTransaction transaction, ref SqlConnection connection)
        {
            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                return false;
            }
        }

        public bool CommitTransaction(SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                if (transaction != null)
                {
                    transaction.Commit();
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                return false;
            }
        }

        public bool RollbackTransaction(SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

                return false;
            }
        }

    }

    public static class ExpressionExtensions
    {
        public static string ToSqlString(this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    var add = expression as BinaryExpression;
                    return add.Left.ToSqlString() + " + " + add.Right.ToSqlString();
                case ExpressionType.Constant:
                    var constant = expression as ConstantExpression;
                    if (constant.Type == typeof(string))
                        return "N'" + constant.Value.ToString().Replace("'", "''") + "'";
                    return constant.Value.ToString();
                case ExpressionType.Equal:
                    var equal = expression as BinaryExpression;
                    if (equal.Right.Type == typeof(string))
                        return string.Format("{0} like({1})", equal.Left.ToSqlString(), equal.Right.ToSqlString());
                    else
                        return equal.Left.ToSqlString() + " = " + equal.Right.ToSqlString();
                case ExpressionType.Lambda:
                    var l = expression as LambdaExpression;
                    return l.Body.ToSqlString();
                case ExpressionType.MemberAccess:
                    if (expression.GetType().Name == "FieldExpression")
                    {
                        return GetValueString(expression as MemberExpression);
                    }
                    else
                    {
                        var memberaccess = expression as MemberExpression;
                        // todo: if column aliases are used, look up ColumnAttribute.Name
                        return "[" + memberaccess.Member.Name + "]";
                    }
                case ExpressionType.OrElse:
                    var orElse = expression as BinaryExpression;
                    return orElse.Left.ToSqlString() + " or (" + orElse.Right.ToSqlString() + ")";
                case ExpressionType.Or:
                    var or = expression as BinaryExpression;
                    return or.Left.ToSqlString() + " or " + or.Right.ToSqlString();
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    var and = expression as BinaryExpression;
                    return and.Left.ToSqlString() + " and " + and.Right.ToSqlString();
            }
            throw new NotImplementedException(
              expression.GetType().ToString() + " " +
              expression.NodeType.ToString());
        }

        public static string GetValueString(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            var value = getter();
            if (IsNumber(value))
            {
                return value.ToString();
            }
            else
                if (value is bool)
            {
                return (bool)value ? "1" : "0";
            }
            else
            {
                return string.Format("'{0}'", value);
            }
        }

        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
