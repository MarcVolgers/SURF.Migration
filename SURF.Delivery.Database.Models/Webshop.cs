using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SURF.SqlDataFramework;
using SURF.SqlDataFramework.Attributes;

namespace SURF.Delivery.Database.Models
{
    public class Webshop : ModelBase<Webshop>
    {
        [IdentifierProperty]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string CallbackUrl { get; set; }
        public string CallbackSuccesMessage { get; set; }
        public string CallbackUrlUsername { get; set; }
        public string CallbackUrlPassword { get; set; }
        public string CallbackUrlDomain { get; set; }
        public string CallbackUrlParametername { get; set; }
        public bool CallbackUrlIsAsmx { get; set; }
        public bool UseExtendedError { get; set; }

        /*public void AddAsParametersToSqlCommand(SqlCommand cmd)
        {
            Type t = typeof(DeliveryEntity);

            foreach (PropertyInfo pi in t.GetProperties())
            {
                try
                {
                    if (pi.Name != "Id")
                    {
                        if (pi.GetValue(this) == null)
                            cmd.Parameters.Add(new SqlParameter(pi.Name, DBNull.Value));
                        else
                            cmd.Parameters.Add(new SqlParameter(pi.Name, pi.GetValue(this)));
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static Webshop ReadFromSqlReader(SqlDataReader reader)
        {
            Type t = typeof(Webshop);

            Webshop item = new Webshop();

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

            }

            return item;
        }*/
    }
}
