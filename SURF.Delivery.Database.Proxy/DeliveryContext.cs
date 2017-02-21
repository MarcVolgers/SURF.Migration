using SURF.Delivery.Database.Models;
using SURF.SqlDataFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Database.Proxy
{
    public class DeliveryContext : ModelBaseContext
    {
        public DeliveryContext(string connectionString) : base(connectionString)
        {
        }

        public void UpdateCacheDeliverers(IEnumerable<CacheDeliverers> deliverers)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;

            try
            {
                if (StartTransaction(ref transaction, ref connection))
                {
                    Delete<CacheDeliverers>();
                    InsertInto(deliverers);

                    CommitTransaction(transaction, connection);
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction(transaction, connection);

                throw new Exception("Error adding deliverers to cache table", ex);
            }
        }

        public void UpdateCacheTemplates(IEnumerable<CacheTemplates> templates)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;

            try
            {
                if (StartTransaction(ref transaction, ref connection))
                {
                    Delete<CacheTemplates>();
                    InsertInto(templates);

                    CommitTransaction(transaction, connection);
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction(transaction, connection);

                throw new Exception("Error adding templates to cache table", ex);
            }
        }

        /// <summary>
        /// Add a delivery item to the database.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddDeliveryItem(DeliveryEntity item)
        {
            try
            {
                InsertInto(new[] { item });

                //_log.InfoFormat("Order {0} added under id {1}", item.OrderNumber, id);

                return item.Id;

            }
            catch (Exception ex)
            {
                throw new SURFSqlDataFrameworkException(SURFSqlDataFrameworkExceptionCodes.ErrorAddingOrder, ex);
            }
        }


        /// <summary>
        /// Get a single order to process. The item is set to InProcessing = 1.
        /// </summary>
        /// <param name="supplierName"></param>
        /// <param name="status"></param>
        /// <param name="includeFailed"></param>
        /// <returns></returns>
        public DeliveryEntity GetOrderToProcess(string supplierName, DeliveryStatus? status, bool? includeFailed, bool? checkForCallbackDone = null)
        {
            DeliveryEntity item = null;


            using (SqlConnection conn = Connection)
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = conn;

                //TODO parameteriseer(app.config) aantal uren retrycount voor de failed orders
                //Geef 1 item uit de queue terug met de Status=ToDeliver EN InProcessing=0(niemand doet er nu iets mee) gesorteerd op StatusDate ASC(FIFO)
                //OF waarbij de Status=Failed EN de StatusDate is langer dan [retryWaitInMinutes] minuuut geleden EN de RetryCount < [retryCount] EN InProcessing=0(niemand doet er nu iets mee).
                //Bij het teruggeven van dit item InProcessing=1 zetten

                string select = @"select top 1 Id 
                                        from
                                            Delivery
                                        where
                                            ([Status] = @status and InProcessing = 0 and SupplierName = @supplierName)";
                if (includeFailed.HasValue && includeFailed.Value)
                {
                    select += @"or 
                                            (
                                                ([Status] = 'Failed' and DATEDIFF(minute, [StatusDate], GETDATE()) >= @retryWaitInMinutes and RetryCount < @retryCount)
                                                and
                                                InProcessing = 0
                                                and
                                                SupplierName = @supplierName
                                            )";
                }
                else
                {
                    select += " AND DATEDIFF(minute, [LastDateTried], GETDATE()) >= @retryWaitInMinutes ";
                }

                if (checkForCallbackDone.HasValue)
                {
                    select += " AND WebshopCallbackDone = @callbackdone";
                }

                select += @" order by [StatusDate] ASC";

                cmd.CommandText = @"update Delivery set InProcessing = 1, LastDateTried = GetDate() output inserted.*
                                    where Id in
                                    (
                                        " + select + @"
                                    )";


                cmd.Parameters.Add(new SqlParameter("supplierName", supplierName));
                cmd.Parameters.Add(new SqlParameter("status", status.ToString()));
                cmd.Parameters.Add(new SqlParameter("retryCount", ConfigurationManager.AppSettings["RetryCount"]));
                cmd.Parameters.Add(new SqlParameter("retryWaitInMinutes", ConfigurationManager.AppSettings["RetryWaitInMinutes"]));
                if (checkForCallbackDone.HasValue)
                {
                    cmd.Parameters.Add(new SqlParameter("callbackdone", checkForCallbackDone.Value));
                }

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    item = DeliveryEntity.ReadFromSqlReader(reader);
                }

                reader.Close();
            }

            /* As-is solution Financiele koppeling
            item.FinanceEntity = LoadDeliveryFinanceEntity(item.Id);*/

            return item;
        }

        /// <summary>
        /// Get a single order to process. The item is set to InProcessing = 1.
        /// </summary>
        /// <param name="supplierName"></param>
        /// <param name="status"></param>
        /// <param name="includeFailed"></param>
        /// <returns></returns>
        public DeliveryEntity GetOrdersForResponseToProcess(string supplierName, DeliveryStatus status, bool includeFailed)
        {
            DeliveryEntity item = null;

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = conn;

                //TODO parameteriseer(app.config) aantal uren retrycount voor de failed orders
                //Geef 1 item uit de queue terug met de Status=ToDeliver EN InProcessing=0(niemand doet er nu iets mee) gesorteerd op StatusDate ASC(FIFO)
                //OF waarbij de Status=Failed EN de StatusDate is langer dan [retryWaitInMinutes] minuuut geleden EN de RetryCount < [retryCount] EN InProcessing=0(niemand doet er nu iets mee).
                //Bij het teruggeven van dit item InProcessing=1 zetten

                string select = @"SELECT TOP 1 Id 
                                  FROM Delivery
                                  WHERE ([Status] = @status AND InProcessing = 0 AND SupplierName = @supplierName)
                                  AND DATEDIFF(minute, [LastDateTried], GETDATE()) >= @retryWaitInMinutes
                                  ORDER BY [LastDateTried] ASC";

                cmd.CommandText = @"UPDATE Delivery SET InProcessing = 1, LastDateTried = GetDate() OUTPUT INSERTED.*
                                    WHERE Id IN
                                    (
                                        " + select + @"
                                    )";


                cmd.Parameters.Add(new SqlParameter("supplierName", supplierName));
                cmd.Parameters.Add(new SqlParameter("status", status.ToString()));
                cmd.Parameters.Add(new SqlParameter("retryWaitInMinutes", ConfigurationManager.AppSettings["RetryWaitInMinutes"]));

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    item = DeliveryEntity.ReadFromSqlReader(reader);
                }

                reader.Close();
            }

            /* As-is solution Financiele koppeling
            item.FinanceEntity = LoadDeliveryFinanceEntity(item.Id);*/

            return item;
        }

        public long AddDeliveryFinanceEntity(DeliveryFinanceEntity financeItem)
        {
            throw new NotImplementedException();

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = conn;
            //    var select = @"INSERT INTO [dbo].[DeliveryFinance]
            //                   ([DeliveryId]
            //                   ,[RemittanceDate]
            //                   ,[Price]
            //                   ,[RemittancePrice]
            //                   ,[PurchasePrice]
            //                   ,[Margin]
            //                   ,[Vat]
            //                   ,[VatPercentage]
            //                   ,[Multiplier]
            //                   ,[ProviderName]
            //                   ,[MediationAgreementName]
            //                   ,[ProductVariantName]
            //                   ,[ProductPlatform]
            //                   ,[ProviderSKU]
            //                   ,[Status])
            //             OUTPUT inserted.Id
            //             VALUES
            //                   (@DeliveryId,
            //                   @RemittanceDate,
            //                   @Price, 
            //                   @RemittancePrice, 
            //                   @PurchasePrice, 
            //                   @Margin, 
            //                   @Vat, 
            //                   @VatPercentage, 
            //                   @Multiplier,
            //                   @ProviderName,
            //                   @MediationAgreementName,
            //                   @ProductVariantName,
            //                   @ProductPlatform,
            //                   @ProviderSKU,
            //                   @Status)";
            //    cmd.CommandText = select;
            //    financeItem.AddAsParametersToSqlCommand(cmd);

            //    var id = (long)cmd.ExecuteScalar();

            //    _log.InfoFormat("Finance data added under id {0}", id);

            //    return id;
            //}
        }

        private void SaveDeliveryFinanceEntity(DeliveryFinanceEntity financeItem)
        {
            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = conn;
            //    var select = @"UPDATE [dbo].[DeliveryFinance]
            //                   SET [DeliveryId] = @DeliveryId
            //                      ,[RemittanceDate] = @RemittanceDate
            //                      ,[Price] = @Price
            //                      ,[RemittancePrice] = @RemittancePrice
            //                      ,[PurchasePrice] = @PurchasePrice
            //                      ,[Margin] = @Margin
            //                      ,[Vat] = @Vat
            //                      ,[VatPercentage] = @VatPercentage
            //                      ,[Multiplier] = @Multiplier
            //                      ,[ProviderName] = @ProviderName
            //                      ,[MediationAgreementName] = @MediationAgreementName
            //                      ,[ProductVariantName] = @ProductVariantName
            //                      ,[ProductPlatform] = @ProductPlatform
            //                      ,[ProviderSKU] = @ProviderSKU
            //                      ,[Status] = @Status
            //                 WHERE Id = @Id)";
            //    cmd.CommandText = select;
            //    financeItem.AddAsParametersToSqlCommand(cmd);
            //    cmd.ExecuteNonQuery();
            //}
        }

        private DeliveryFinanceEntity LoadDeliveryFinanceEntity(long deliveryId)
        {
            throw new NotImplementedException();

            //DeliveryFinanceEntity item = null;


            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = conn;
            //    var select = @"SELECT * FROM DeliveryFinance WHERE DeliveryId = @deliveryId";
            //    cmd.CommandText = select;
            //    cmd.Parameters.Add(new SqlParameter("deliveryId", deliveryId));
            //    SqlDataReader reader = cmd.ExecuteReader();
            //    item = DeliveryFinanceEntity.ReadFromSqlReader(reader);
            //}
            //return item;
        }

        private IEnumerable<DeliveryFinanceEntity> LoadDeliveryFinanceEntities(IEnumerable<long> deliveryIds)
        {
            throw new NotImplementedException();
            //List<DeliveryFinanceEntity> items = new List<DeliveryFinanceEntity>();


            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = conn;
            //    var select = @"select * from DeliveryFinance where DeliveryId IN(@deliveryIds)";
            //    cmd.CommandText = select;
            //    cmd.Parameters.Add(new SqlParameter("deliveryIds", string.Join(",", deliveryIds)));
            //    SqlDataReader reader = cmd.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        items.Add(DeliveryFinanceEntity.ReadFromSqlReader(reader));
            //    }
            //}
            //return items;
        }

        public IEnumerable<DeliveryEntity> GetOrderByOrderNumberAndOrderLineNumbers(string orderNumber, IEnumerable<string> orderLineNumbers)
        {
            throw new NotImplementedException();
            //List<DeliveryEntity> result = new List<DeliveryEntity>();


            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = conn;
            //    cmd.CommandText = @"SELECT * FROM Delivery
            //                        WHERE OrderNumber = @orderNumber
            //                        AND OrderLineNumber IN (" + string.Join(",", orderLineNumbers.Select(p => "'" + p + "'")) + ")";
            //    cmd.Parameters.Add(new SqlParameter("orderNumber", orderNumber));

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        result.Add(DeliveryEntity.ReadFromSqlReader(reader));
            //    }

            //    reader.Close();
            //}
            //return result;
        }

        /// <summary>
        /// Get multiple orders to process.
        /// </summary>
        /// <param name="delivererName"></param>
        /// <returns></returns>
        public IEnumerable<DeliveryEntity> GetOrdersToProcess(string delivererName)
        {
            List<DeliveryEntity> result = new List<DeliveryEntity>();


            using (SqlConnection conn = Connection)
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = conn;

                //TODO parameteriseer(app.config) aantal uren retrycount voor de failed orders
                //Geef 1 item uit de queue terug met de Status=ToDeliver EN InProcessing=0(niemand doet er nu iets mee) gesorteerd op StatusDate ASC(FIFO)
                //OF waarbij de Status=Failed EN de StatusDate is langer dan 1 uur geleden EN de RetryCount < 5 EN InProcessing=0(niemand doet er nu iets mee).
                //Bij het teruggeven van dit item InProcessing=1 zetten
                cmd.CommandText = @"update Delivery set InProcessing = 1 output inserted.*
                                    where Id in
                                    (
                                        select Id from Delivery
                                        where
                                            ([Status] = 'ToDeliver' AND InProcessing = 0 AND SupplierName = @supplierName)
                                            or 
                                            (
                                                ([Status] = 'Failed' and DATEDIFF(hour, [StatusDate], GETDATE()) >= 1 and RetryCount < 5)
                                                and
                                                InProcessing = 0
                                                and
                                                SupplierName = @supplierName
                                            )
                                    )";

                cmd.Parameters.Add(new SqlParameter("supplierName", delivererName));

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(DeliveryEntity.ReadFromSqlReader(reader));
                }

                reader.Close();
            }

            return result;
        }

        public IEnumerable<DeliveryEntity> GetOrdersToProcess(FinanceStatus financeStatus)
        {
            throw new NotImplementedException();
            //List<DeliveryEntity> result = new List<DeliveryEntity>();


            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;

            //    cmd.CommandText = @"UPDATE Delivery SET InProcessing = 1 OUTPUT INSERTED.*
            //                        WHERE Id IN
            //                        (
            //                            SELECT d.id FROM Delivery d
            //                            INNER JOIN DeliveryFinance df ON df.DeliveryId = d.Id
            //                            WHERE d.InProcessing = 0 AND df.Status = @status
            //                        )";

            //    cmd.Parameters.Add(new SqlParameter("status", financeStatus.ToString()));

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        result.Add(DeliveryEntity.ReadFromSqlReader(reader));
            //    }

            //    reader.Close();
            //}

            ///* As-is solution Financiele koppeling
            //var financeItems = LoadDeliveryFinanceEntities(result.Select(p => p.Id)).ToDictionary(p => p.DeliveryId, q => q);
            //foreach (var resultItem in result)
            //{
            //    resultItem.FinanceEntity = financeItems.ContainsKey(resultItem.Id) ? financeItems[resultItem.Id] : null;
            //}*/

            //return result;
        }


        /// <summary>
        /// Set the item to InProcessing = 0, set the status.
        /// </summary>
        /// <param name="deliveryItem"></param>
        /// <param name="newStatus"></param>
        public void DoneProcessing(DeliveryEntity deliveryItem, DeliveryStatus newStatus)
        {

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SetDeliveryStatus(deliveryItem, newStatus);

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "UPDATE Delivery SET InProcessing = 0 WHERE Id = @id";
            //    cmd.Parameters.Add(new SqlParameter("id", deliveryItem.Id));

            //    cmd.ExecuteNonQuery();
            //}
        }

        /// <summary>
        /// Set the item to InProcessing = 0
        /// </summary>
        /// <param name="deliveryItem"></param>
        /// <param name="newStatus"></param>
        public void DoneProcessing(DeliveryEntity deliveryItem)
        {

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "UPDATE Delivery SET InProcessing = 0 WHERE Id = @id";
            //    cmd.Parameters.Add(new SqlParameter("id", deliveryItem.Id));

            //    cmd.ExecuteNonQuery();
            //}
        }

        public void AddInvoiceLineQueueItem(DeliveryEntity deliveryItem)
        {
            //try
            //{
            //    var invoiceLineQueue = new InvoiceLineQueue()
            //    {
            //        DeliveryId = deliveryItem.Id,
            //        IsProcessed = false,
            //        RetryCount = 0,
            //        Failed = false,
            //        InProcess = false,
            //        LastDateTried = DateTime.Now,
            //        ResultMessage = string.Empty
            //    };


            //    using (SqlConnection conn = _deliveryConnection.Connection)
            //    {
            //        conn.Open();
            //        SqlCommand cmd = new SqlCommand();
            //        cmd.Connection = conn;
            //        var select = @"INSERT INTO [dbo].[InvoiceLineQueue]
            //                           ([DeliveryId]
            //                           ,[IsProcessed]
            //                           ,[RetryCount]
            //                           ,[InProcess]
            //                           ,[LastDateTried]
            //                           ,[Failed]
            //                           ,[ResultMessage])
            //                     VALUES
            //                           (@DeliveryId
            //                           ,@IsProcessed
            //                           ,@RetryCount
            //                           ,@InProcess
            //                           ,@LastDateTried
            //                           ,@Failed
            //                           ,@ResultMessage)";
            //        cmd.CommandText = select;
            //        invoiceLineQueue.AddAsParametersToSqlCommand(cmd);

            //        cmd.ExecuteScalar();

            //        _log.InfoFormat("InvoiceLineQueue item data added to queue for delivery {0}", deliveryItem.Id);
            //    }

            //    deliveryItem.AddedToInvoiceLineQueue = true;
            //    SaveDeliveryItem(deliveryItem);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(string.Format("Failed to add delivery {0} to callback queue.\r\nError:\r\n{1}", deliveryItem.Id, ex));
            //    //_log.ErrorFormat("Failed to add delivery {0} to callback queue.\r\nError:\r\n{1}", deliveryItem.Id, ex);
            //}

        }

        public void AddCallbackQueueItem(DeliveryEntity deliveryItem)
        {
            //try
            //{
            //    var callbackQueue = new CallbackQueue()
            //    {
            //        DeliveryId = deliveryItem.Id,
            //        IsProcessed = false,
            //        RetryCount = 0,
            //        Failed = false,
            //        InProcess = false,
            //        LastDateTried = DateTime.Now,
            //        ResultMessage = string.Empty
            //    };


            //    using (SqlConnection conn = _deliveryConnection.Connection)
            //    {
            //        conn.Open();
            //        SqlCommand cmd = new SqlCommand();
            //        cmd.Connection = conn;
            //        var select = @"INSERT INTO [dbo].[CallbackQueue]
            //                           ([DeliveryId]
            //                           ,[IsProcessed]
            //                           ,[RetryCount]
            //                           ,[InProcess]
            //                           ,[LastDateTried]
            //                           ,[Failed]
            //                           ,[ResultMessage])
            //                     VALUES
            //                           (@DeliveryId
            //                           ,@IsProcessed
            //                           ,@RetryCount
            //                           ,@InProcess
            //                           ,@LastDateTried
            //                           ,@Failed
            //                           ,@ResultMessage)";
            //        cmd.CommandText = select;
            //        callbackQueue.AddAsParametersToSqlCommand(cmd);

            //        cmd.ExecuteScalar();

            //        _log.InfoFormat("Callback item data added to queue for delivery {0}", deliveryItem.Id);
            //    }

            //    deliveryItem.AddedToCallbackQueue = true;
            //    SaveDeliveryItem(deliveryItem);
            //}
            //catch (Exception ex)
            //{
            //    throw new UitleverstraatException(UitleverstraatResultCode.FailedAddToCallbackQueue, string.Format("Failed to add delivery {0} to callback queue.\r\nError:\r\n{1}", deliveryItem.Id, ex));
            //    //_log.ErrorFormat("Failed to add delivery {0} to callback queue.\r\nError:\r\n{1}", deliveryItem.Id, ex);
            //}

        }

        public InvoiceLineQueue GetInvoiceLineQueueItem()
        {
            InvoiceLineQueue item = null;

            try
            {

                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();

                    cmd.Connection = conn;

                    string select = @"SELECT TOP 1 Id 
                                  FROM InvoiceLineQueue
                                  WHERE 
                                    ([Failed] = 0 AND [IsProcessed] = 0 AND InProcess = 0)
                                  OR
                                    ([Failed] = 1 and [RetryCount] < @retryCount AND DATEDIFF(minute, [LastDateTried], GETDATE()) >= @retryWaitInMinutes)
                                  ORDER BY [LastDateTried] ASC";

                    cmd.CommandText = @"UPDATE InvoiceLineQueue SET InProcess = 1, LastDateTried = GetDate() OUTPUT INSERTED.*
                                    WHERE Id IN
                                    (
                                        " + select + @"
                                    )";

                    cmd.Parameters.Add(new SqlParameter("retryWaitInMinutes", ConfigurationManager.AppSettings["CallbackRetryWaitInMinutes"]));
                    cmd.Parameters.Add(new SqlParameter("retryCount", ConfigurationManager.AppSettings["CallbackRetryCount"]));

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        item = InvoiceLineQueue.ReadFromSqlReader(reader);
                    }

                    reader.Close();
                }

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CallbackQueue GetCallbackItem()
        {
            CallbackQueue item = null;

            try
            {

                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();

                    cmd.Connection = conn;

                    string select = @"SELECT TOP 1 Id 
                                  FROM CallbackQueue
                                  WHERE 
                                    ([Failed] = 0 AND [IsProcessed] = 0 AND InProcess = 0)
                                  OR
                                    ([Failed] = 1 and [RetryCount] < @retryCount AND DATEDIFF(minute, [LastDateTried], GETDATE()) >= @retryWaitInMinutes)
                                  ORDER BY [LastDateTried] ASC";

                    cmd.CommandText = @"UPDATE CallbackQueue SET InProcess = 1, LastDateTried = GetDate() OUTPUT INSERTED.*
                                    WHERE Id IN
                                    (
                                        " + select + @"
                                    )";

                    cmd.Parameters.Add(new SqlParameter("retryWaitInMinutes", ConfigurationManager.AppSettings["RetryWaitInMinutes"]));
                    cmd.Parameters.Add(new SqlParameter("retryCount", ConfigurationManager.AppSettings["RetryCount"]));

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        item = CallbackQueue.ReadFromSqlReader(reader);
                    }

                    reader.Close();
                }

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Call the webshop service to send a message back.
        /// </summary>
        /// <param name="deliveryItem"></param>
        public bool CallWebshopService(DeliveryEntity deliveryItem, out string resultMessage)
        {
            throw new NotImplementedException();

            //bool result = false;
            //resultMessage = string.Empty;

            //if (string.IsNullOrEmpty(deliveryItem.WebshopCallbackUrl))
            //{
            //    throw new Exception("No callbackservice available");
            //}

            //// check status deliveryitem
            //if ((deliveryItem.DeliveryStatus == DeliveryStatus.SendFromDeliverer ||
            //     deliveryItem.DeliveryStatus == DeliveryStatus.Backorder ||
            //     deliveryItem.DeliveryStatus == DeliveryStatus.Unavailable ||
            //     deliveryItem.DeliveryStatus == DeliveryStatus.Cancelled) ||
            //    (deliveryItem.DeliveryStatus == DeliveryStatus.Failed &&
            //     deliveryItem.RetryCount >= Convert.ToInt32(ConfigurationManager.AppSettings["RetryCount"])))
            //{
            //    ResponseMessage<DeliveryResult> deliveryResult = new ResponseMessage<DeliveryResult>();
            //    try
            //    {
            //        var webshop = GetWebshop(deliveryItem.WebshopId);

            //        deliveryResult = CreateDeliveryResultResponse(deliveryItem);
            //        var json = JObject.FromObject(deliveryResult).ToString();

            //        // if CallbackUrlParameternam given, wrap the json into the parameter
            //        if (!string.IsNullOrEmpty(webshop.CallbackUrlParametername))
            //        {
            //            json = "{\"" + webshop.CallbackUrlParametername + "\":" + json + "}";
            //        }

            //        // use webclient for asmx, webrequest for other callbacks.
            //        bool postResult = webshop.CallbackUrlIsAsmx ?
            //                          PostToWebserviceWebClient(deliveryItem.WebshopCallbackUrl, webshop, json, out resultMessage) :
            //                          PostToWebservice(deliveryItem.WebshopCallbackUrl, webshop.CallbackSuccesMessage, json, out resultMessage);

            //        if (!postResult)
            //        {
            //            throw new UitleverstraatException(UitleverstraatResultCode.FailedCallback, resultMessage);
            //        }

            //        deliveryResult = new ResponseMessage<DeliveryResult>() { ResponseObject = deliveryResult.ResponseObject, ResponseMessageType = ResponseMessageType.OK };
            //        deliveryResult.ResultCode = UitleverstraatResultCode.SuccesfullCallback;
            //        deliveryItem.WebshopCallbackDone = true;
            //        SaveDeliveryItem(deliveryItem);
            //    }
            //    catch (UitleverstraatException ex)
            //    {
            //        deliveryResult = new ResponseMessage<DeliveryResult>()
            //        {
            //            ResultCode = UitleverstraatResultCode.FailedCallback,
            //            Exception = ex,
            //            ResponseMessageType = ResponseMessageType.Error,
            //            ResponseObject = deliveryResult.ResponseObject,
            //            CorrelationId = Guid.NewGuid()
            //        };
            //    }
            //    catch (Exception ex)
            //    {
            //        deliveryResult = new ResponseMessage<DeliveryResult>()
            //        {
            //            ResultCode = UitleverstraatResultCode.UnexpectedError,
            //            Exception = ex,
            //            ResponseMessageType = ResponseMessageType.Error,
            //            ResponseObject = deliveryResult.ResponseObject,
            //            CorrelationId = Guid.NewGuid()
            //        };
            //    }

            //    // log result
            //    var logMessage = new StringBuilder();
            //    logMessage.AppendFormat("ResultCode: {0}\r\n", deliveryResult.ResultCode);
            //    logMessage.AppendFormat("nResultDescription: {0}\r\n", deliveryResult.ResultDescription);
            //    logMessage.AppendFormat("Ordernumber: {0}\r\n", deliveryResult.ResponseObject.OrderNumber);
            //    logMessage.AppendFormat("Orderlinenumber: {0}\r\n", deliveryResult.ResponseObject.OrderLineNumber);
            //    logMessage.AppendFormat("CorrelationId: {0}\r\n", deliveryResult.CorrelationId);

            //    if (deliveryResult.ResponseMessageType == ResponseMessageType.OK)
            //    {
            //        _log.InfoFormat(logMessage.ToString());

            //        result = true;
            //    }
            //    else
            //        if (deliveryResult.ResponseMessageType == ResponseMessageType.Error)
            //    {
            //        if (deliveryResult.Exception != null)
            //        {
            //            logMessage.AppendFormat("Exception: {0}", deliveryResult.Exception);
            //        }

            //        LoggingHelper.LogError(UitleverstraatResultCode.ErrorCallingWebservice, new Exception(logMessage.ToString()));
            //        //log4net.ThreadContext.Properties["EventID"] = "3004";
            //        //_log.ErrorFormat(logMessage.ToString());

            //        result = false;
            //    }
            //}

            //return result;
        }


        /// <summary>
        /// Post json to a webclient.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="webshop"></param>
        /// <param name="json"></param>
        /// <param name="xErrorResultMessage"></param>
        /// <returns></returns>
        private bool PostToWebserviceWebClient(string url, Webshop webshop, string json, out string xErrorResultMessage)
        {
            throw new NotImplementedException();
            //_log.InfoFormat("Posting to {0}:\r\n{1}", url, json);

            //try
            //{
            //    var webClient = new WebClient()
            //    {
            //        BaseAddress = url,
            //        Credentials = new NetworkCredential(webshop.CallbackUrlUsername, webshop.CallbackUrlPassword)
            //    };

            //    using (webClient)
            //    {
            //        webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            //        xErrorResultMessage = webClient.UploadString(url, json);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    xErrorResultMessage = ex.ToString();
            //    LoggingHelper.LogError(UitleverstraatResultCode.ErrorPostingToWebservice, ex);
            //    //_log.Error(xErrorResultMessage);
            //    return false;
            //}

            //return true;
        }

        /// <summary>
        /// Post json sync.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        private bool PostToWebservice(string url, string succesMessage, string json, out string xErrorResultMessage)
        {
            throw new NotImplementedException();

            //_log.InfoFormat("Posting to {0}:\r\n{1}", url, json);

            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //httpWebRequest.ContentType = "text/json";
            //httpWebRequest.Method = "POST";

            //// adding timout creates tls/ssl issues on server...

            //// added time out.
            ////var timeOut = Convert.ToInt32(ConfigurationManager.AppSettings["CallbackTimeOut"]);
            ////httpWebRequest.Timeout = timeOut * 1000;

            ////ServicePointManager.SecurityProtocol =
            ////       SecurityProtocolType.Ssl3 |
            ////       SecurityProtocolType.Tls |
            ////       SecurityProtocolType.Tls11 |
            ////       SecurityProtocolType.Tls12;

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(json);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //}

            //xErrorResultMessage = string.Empty;
            //HttpWebResponse httpResponse = null;
            //try
            //{
            //    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //    xErrorResultMessage = GetXErrorMessage(httpResponse);
            //    return true;
            //}
            //catch (WebException ex)
            //{
            //    if (ex.Response != null)
            //    {
            //        xErrorResultMessage = GetXErrorMessage((HttpWebResponse)ex.Response);
            //    }

            //    if (string.IsNullOrWhiteSpace(xErrorResultMessage))
            //    {
            //        xErrorResultMessage = ex.ToString();
            //    }
            //    return false;
            //}
            //finally
            //{
            //    //added explicit close, so no time outs.
            //    if (httpResponse != null)
            //        httpResponse.Close();
            //}
        }


        /// <summary>
        /// Save the delivery item.
        /// </summary>
        /// <param name="item"></param>
        public void SaveDeliveryItem(DeliveryEntity item)
        {

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = @"UPDATE [dbo].[Delivery]
            //                           SET [OrderNumber] = @OrderNumber
            //                                ,[OrderLineNumber] = @OrderLineNumber
            //                                ,[OrderDate] = @OrderDate
            //                                ,[OrderAmount] = @OrderAmount
            //                                ,[ArticleCodeWebshop] = @ArticleCodeWebshop
            //                                ,[ArticleCodeSupplier] = @ArticleCodeSupplier
            //                                ,[ArticleName] = @ArticleName
            //                                ,[ArticleMedium] = @ArticleMedium
            //                                ,[ContactId] = @ContactId
            //                                ,[ContactName] = @ContactName
            //                                ,[ContactEmail] = @ContactEmail
            //                                ,[ContactInitials] = @ContactInitials
            //                                ,[ContactPrefix] = @ContactPrefix
            //                                ,[ContactStreet] = @ContactStreet
            //                                ,[ContactHousenumber] = @ContactHousenumber
            //                                ,[ContactZipCode] = @ContactZipCode
            //                                ,[ContactCity] = @ContactCity
            //                                ,[ContactCountry] = @ContactCountry
            //                                ,[ContactGender] = @ContactGender
            //                                ,[ContactInstitution] = @ContactInstitution
            //                                ,[ContactInstitutionDepartment] = @ContactInstitutionDepartment
            //                                ,[ContactInstitutionReference] = @ContactInstitutionReference
            //                                ,[SupplierName] = @SupplierName
            //                                ,[SupplierSerialNumber] = @SupplierSerialNumber
            //                                ,[SupplierDownloadlink] = @SupplierDownloadlink
            //                                ,[SupplierOrderNumber] = @SupplierOrderNumber
            //                                ,[SupplierVouchers] = @SupplierVouchers
            //                                ,[SupplierMessage] = @SupplierMessage
            //                                ,[SupplierErrors] = @SupplierErrors
            //                                ,[SupplierStatus] = @SupplierStatus
            //                                ,[SupplierStatusDate] = @SupplierStatusDate
            //                                ,[SupplierDeliveryDate] = @SupplierDeliveryDate
            //                                ,[SupplierInvoiceNumber] = @SupplierInvoiceNumber
            //                                ,[SupplierTrackAndTraceCode] = @SupplierTrackAndTraceCode
            //                                ,[StatusDate] = @StatusDate
            //                                ,[RetryCount] = @RetryCount
            //                                ,[InProcessing] = @InProcessing
            //                                ,[WebshopCallbackDone] = @WebshopCallbackDone
            //                                ,[Extra1] = @Extra1
            //                                ,[Extra2] = @Extra2
            //                                ,[TargetGroup] = @TargetGroup
            //                                ,[SendRegisteredMail] = @SendRegisteredMail
            //                                ,[AddedToCallbackQueue] = @addedToCallbackQueue
            //                        where Id = @id";

            //    item.AddAsParametersToSqlCommand(cmd);
            //    cmd.Parameters.Add(new SqlParameter("id", item.Id));

            //    cmd.ExecuteNonQuery();
            //}
        }

        /// <summary>
        /// Save the delivery item.
        /// </summary>
        /// <param name="item"></param>
        public void SaveCallbackQueueItem(CallbackQueue callbackQueue)
        {

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = @"UPDATE [dbo].[CallbackQueue]
            //                           SET [DeliveryId] = @deliveryId
            //                              ,[IsProcessed] = @isProcessed
            //                              ,[RetryCount] = @retryCount
            //                              ,[InProcess] = @inProcess
            //                              ,[LastDateTried] = @lastDateTried
            //                              ,[Failed] = @failed
            //                              ,[ResultMessage] = @resultMessage
            //                         WHERE Id = @id";

            //    callbackQueue.AddAsParametersToSqlCommand(cmd);
            //    cmd.Parameters.Add(new SqlParameter("id", callbackQueue.Id));

            //    cmd.ExecuteNonQuery();
            //}
        }

        /// <summary>
        /// Save the delivery item.
        /// </summary>
        /// <param name="item"></param>
        public void SaveInvoiceLineQueueItem(InvoiceLineQueue callbackQueue)
        {

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = @"UPDATE [dbo].[InvoiceLineQueue]
            //                           SET [DeliveryId] = @deliveryId
            //                              ,[IsProcessed] = @isProcessed
            //                              ,[RetryCount] = @retryCount
            //                              ,[InProcess] = @inProcess
            //                              ,[LastDateTried] = @lastDateTried
            //                              ,[Failed] = @failed
            //                              ,[ResultMessage] = @resultMessage
            //                         WHERE Id = @id";

            //    callbackQueue.AddAsParametersToSqlCommand(cmd);
            //    cmd.Parameters.Add(new SqlParameter("id", callbackQueue.Id));

            //    cmd.ExecuteNonQuery();
            //}
        }

        public List<DeliveryEntity> GetDeliveryItems()
        {
            //List<DeliveryEntity> result = new List<DeliveryEntity>();

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;

            //    cmd.CommandText = @"SELECT * FROM Delivery";

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        result.Add(DeliveryEntity.ReadFromSqlReader(reader));
            //    }

            //    reader.Close();
            //}

            //return result;
            throw new NotImplementedException();
        }

        public List<DeliveryEntity> GetDeliveryItems(DateTime startOrderDate, DateTime endOrderDate, bool cache = false, int? webshopId = null, bool specificTime = false)
        {
            throw new NotImplementedException();

            //if (!specificTime)
            //{
            //    startOrderDate = new DateTime(startOrderDate.Year, startOrderDate.Month, startOrderDate.Day, 0, 0, 0);
            //    endOrderDate = new DateTime(endOrderDate.Year, endOrderDate.Month, endOrderDate.Day, 23, 59, 59);
            //}

            //if (cache)
            //{
            //    if (_deliveryItems == null)
            //    {
            //        _deliveryItems = GetDeliveryItems(startOrderDate, endOrderDate, false);
            //    }

            //    return _deliveryItems.Where(p => p.OrderDate >= startOrderDate && p.OrderDate <= endOrderDate).ToList();
            //}

            //List<DeliveryEntity> result = new List<DeliveryEntity>();

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;

            //    startOrderDate = startOrderDate.ToUniversalTime();
            //    endOrderDate = endOrderDate.ToUniversalTime();

            //    var command = string.Format(@"SELECT * FROM Delivery WHERE OrderDate >= '{0:yyyy-MM-dd HH:mm:ss}' AND OrderDate <= '{1:yyyy-MM-dd HH:mm:ss}'", startOrderDate, endOrderDate);

            //    if (webshopId.HasValue)
            //    {
            //        command += string.Format(" AND WebshopId = {0}", webshopId.Value);
            //    }
            //    cmd.CommandText = command;

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        result.Add(DeliveryEntity.ReadFromSqlReader(reader));
            //    }

            //    reader.Close();
            //}

            //return result;
        }

        /// <summary>
        /// Gets the status from the queue of an order.
        /// </summary>
        /// <param name="orderNumber">The order number</param>
        /// <returns>The full record from the database</returns>
        public DeliveryEntity GetDeliveryItemByOrderNumber(string orderNumber, string orderLineNumber, long webshopId)
        {
            throw new NotImplementedException();

            //DeliveryEntity item = null;


            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT * FROM Delivery WHERE OrderNumber = @orderNumber AND OrderLineNumber = @orderLineNumber  AND WebshopId = @webshopId";

            //    cmd.Parameters.Add(new SqlParameter("orderNumber", orderNumber));
            //    cmd.Parameters.Add(new SqlParameter("orderLineNumber", orderLineNumber));
            //    cmd.Parameters.Add(new SqlParameter("webshopId", webshopId));
            //    SqlDataReader reader = cmd.ExecuteReader();


            //    if (reader.Read())
            //    {
            //        item = DeliveryEntity.ReadFromSqlReader(reader);
            //    }
            //}

            //if (item == null)
            //{
            //    throw new UitleverstraatException(UitleverstraatResultCode.NoMatchingDataFound, string.Format("For orderNumber {0}, orderLineNumber {1} and webshop {2}", orderNumber, orderLineNumber, webshopId));
            //}

            ///* As-is solution Financiele koppeling
            //item.FinanceEntity = this.LoadDeliveryFinanceEntity(item.Id);*/

            //return item;
        }

        public void CancelDelivery(int id)
        {
            //var deliveryItem = GetDeliveryItemById(id);
            //CancelDelivery(deliveryItem);
        }

        public void CancelDelivery(DeliveryEntity delivery)
        {
            //if (DeliveryCanBeRetried(delivery))
            //{
            //    //delivery.DeliveryStatus = DeliveryStatus.Cancelled;
            //    SetDeliveryStatus(delivery, DeliveryStatus.Cancelled);
            //}
            //else
            //{
            //    throw new UitleverstraatException(UitleverstraatResultCode.DeliveryCannotBeCanceled);
            //}
        }

        public void RetryDelivery(long id)
        {
            //var delivery = GetDeliveryItemById(id);

            //if (DeliveryCanBeRetried(delivery))
            //{

            //    delivery.RetryCount = 0;
            //    delivery.InProcessing = 0;
            //    delivery.AddedToCallbackQueue = false;
            //    delivery.WebshopCallbackDone = false;

            //    SaveDeliveryItem(delivery);

            //    SetDeliveryStatus(delivery, DeliveryStatus.ToDeliver);
            //}
            //else
            //{
            //    throw new UitleverstraatException(UitleverstraatResultCode.DeliveryCannotBeRetried);
            //}
        }

        public List<Suppliers> GetSuppliers()
        {
            throw new NotImplementedException();

            //List<Supplier> suppliers = new List<Supplier>();


            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT * FROM suppliers";

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        suppliers.Add(Supplier.ReadFromSqlReader(reader));
            //    }
            //}

            //if (suppliers == null)
            //{
            //    throw new UitleverstraatException(UitleverstraatResultCode.NoMatchingDataFound);
            //}

            //return suppliers;
        }

        /// <summary>
        /// Gets the status from the queue of an order.
        /// </summary>
        /// <param name="orderNumber">The order number</param>
        /// <returns>The full record from the database</returns>
        public DeliveryEntity GetDeliveryItemById(long id)
        {
            throw new NotImplementedException();

            //DeliveryEntity item = null;


            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT * FROM Delivery WHERE Id = @id";

            //    cmd.Parameters.Add(new SqlParameter("id", id));
            //    SqlDataReader reader = cmd.ExecuteReader();


            //    if (reader.Read())
            //    {
            //        item = DeliveryEntity.ReadFromSqlReader(reader);
            //    }
            //}

            //if (item == null)
            //{
            //    throw new UitleverstraatException(UitleverstraatResultCode.NoMatchingDataFound, " For Delivery Id " + id);
            //}

            ///* As-is solution Financiele koppeling
            //item.FinanceEntity = this.LoadDeliveryFinanceEntity(item.Id);*/

            //return item;
        }

        public List<Webshop> GetAllWebshops()
        {
            throw new NotImplementedException();
            //var result = new List<Webshop>();

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT * FROM Webshop";
            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        result.Add(Webshop.ReadFromSqlReader(reader));
            //    }

            //    return result;
            //}

            //throw new Exception("Error retrieving data");
        }

        public Webshop GetWebshop(long id)
        {

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT * FROM Webshop WHERE Id = @id";
            //    cmd.Parameters.Add(new SqlParameter("id", id));
            //    SqlDataReader reader = cmd.ExecuteReader();


            //    if (reader.Read())
            //    {
            //        return Webshop.ReadFromSqlReader(reader);
            //    }
            //}

            //throw new Exception("Error retrieving data for webshop Id: " + id);
            throw new NotImplementedException();
        }

        public Webshop GetWebshop(string name)
        {
            throw new NotImplementedException();
            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT * FROM Webshop WHERE Name = @name";
            //    cmd.Parameters.Add(new SqlParameter("name", name));
            //    SqlDataReader reader = cmd.ExecuteReader();


            //    if (reader.Read())
            //    {
            //        return Webshop.ReadFromSqlReader(reader);
            //    }
            //}

            //throw new Exception("Error retrieving data");
        }



        public DashboardUser GetDashboardUser(string loginName)
        {
            throw new NotImplementedException();

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    var splittedLogin = loginName.Split('\\');
            //    string userName = string.Empty;
            //    string domainName = string.Empty;

            //    if (splittedLogin.Length == 1)
            //    {
            //        userName = splittedLogin[0];
            //    }
            //    else if (splittedLogin.Length > 1)
            //    {
            //        domainName = splittedLogin[0];
            //        userName = splittedLogin[1];
            //    }
            //    else
            //    {
            //        userName = loginName;
            //    }

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT TOP 1 * FROM DashboardUser WHERE WindowsLoginName = @windowsLoginName AND WindowsDomainName = @windowsDomainName";
            //    cmd.Parameters.Add(new SqlParameter("windowsLoginName", userName));
            //    cmd.Parameters.Add(new SqlParameter("windowsDomainName", domainName));

            //    SqlDataReader reader = cmd.ExecuteReader();


            //    if (reader.Read())
            //    {
            //        return DashboardUser.ReadFromSqlReader(reader);
            //    }
            //}

            //return null;
        }

        public void UpdateDashboardUser(DashboardUser user)
        {
            throw new NotImplementedException();

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = @"UPDATE [dbo].[DashboardUser]
            //                           SET [Name] = @name
            //                              ,[WindowsLoginName] = @windowsLoginName
            //                              ,[WindowsDomainName] = @windowsDomainName
            //                              ,[Role] = @role
            //                              ,[DefaultWebshopId] = @defaultwebshopid
            //                         WHERE Id = @id";

            //    user.AddAsParametersToSqlCommand(cmd);
            //    cmd.Parameters.Add(new SqlParameter("id", user.Id));

            //    cmd.ExecuteNonQuery();
            //}
        }

        public void CreateDashboardUser(DashboardUser user)
        {
            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = @"INSERT INTO [dbo].[DashboardUser]
            //                            ([Name], [WindowsDomainName], [WindowsLoginName], [Role], [DefaultWebshopId] )
            //                           VALUES 
            //                            (@name, @windowsDomainName, @windowsLoginName, @role, @defaultwebshopid)";
            //    user.AddAsParametersToSqlCommand(cmd);

            //    cmd.ExecuteNonQuery();
            //}
        }

        public List<DashboardUser> GetDashboardUsers()
        {
            //List<DashboardUser> result = new List<DashboardUser>();

            //using (SqlConnection conn = _deliveryConnection.Connection)
            //{
            //    conn.Open();

            //    SqlCommand cmd = new SqlCommand();

            //    cmd.Connection = conn;
            //    cmd.CommandText = "SELECT * FROM DashboardUser";
            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        result.Add(DashboardUser.ReadFromSqlReader(reader));
            //    }
            //}

            //return result;
            throw new NotImplementedException();
        }

        public void DeleteDashboardUser(int id)
        {

            //try
            //{
            //    using (SqlConnection conn = _deliveryConnection.Connection)
            //    {
            //        conn.Open();

            //        SqlCommand cmd = new SqlCommand();
            //        cmd.Connection = conn;
            //        cmd.CommandText = @"DELETE FROM [dbo].[DashboardUser] WHERE Id = @id";

            //        cmd.Parameters.Add(new SqlParameter { ParameterName = "id", Value = id });

            //        cmd.ExecuteNonQuery();

            //        conn.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Error deleting user with id " + id, ex);
            //}
        }
    }
}
