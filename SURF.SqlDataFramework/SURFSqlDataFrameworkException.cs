using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.SqlDataFramework
{
    public class SURFSqlDataFrameworkException : Exception
    {
        public SURFSqlDataFrameworkExceptionCodes ResultCode { get; set; }

        public SURFSqlDataFrameworkException(SURFSqlDataFrameworkExceptionCodes code, Exception innerException = null) : base(code.ToString(), innerException)
        {
            ResultCode = code;
        }
    }

    public enum SURFSqlDataFrameworkExceptionCodes
    {
        // voor nu uit uitleverstraat gejat

        // OK 
        DeliveryAddedToDatabase = 1000,
        DeliveryStatusUpdated = 1001,
        DeliveryStatusSent = 1002,
        SuccesfullCallback = 1003,
        OfferinglistSent = 1004,
        SupplierNamesSent = 1005,
        DeliveryResultSent = 1006,

        //Error
        ErrorAddingOrder = 2000,
        SupplierNameIsUnknown = 2001,
        ErrorValidatingOrder = 2002,
        WebshopNameIsUnknown = 2003,
        InvalidPassword = 2004,
        ErrorUpdateStatus = 2005,
        ErrorRetrievingOrderStatus = 2006,
        StatusIsNotAllowed = 2007,
        InvalidWebshopCredentials = 2008,
        NoMatchingDataFound = 2009,
        FailedCallback = 2010,
        OrderRequestHasNoAuthentication = 2011,
        OrderRequestHasNoOrder = 2012,
        InvalidOrderRequest = 2013,
        OrderHasNoOrderLines = 2014,
        ErrorValidatingOrderLine = 2015,
        ErrorReadingConfig = 2016,
        DeliveryResultHasErrors = 2017,
        OrderWithSameOrderNumberExists = 2018,
        ForceResendOnlyPossibleWhenStatusIsToDeliver = 2019,
        DatabaseServerIsNotAvailable = 2020,
        ErrorStartingSchedule = 2021,
        ErrorStartingWebApi = 2022,
        ErrorCallback = 2023,
        ErrorPostingResult = 2024,
        FailedAddToCallbackQueue = 2025,
        ErrorPostingToWebservice = 2026,

        ErrorProcessingCSVDelivery = 3000,
        ErrorCheckingForSlimFysiekUdates = 3001,
        ErrorPlacingOrder = 3002,
        ErrorSettingDoneProcessing = 3003,
        ErrorCallingWebservice = 3004,
        ErrorConnectingToCRMDatabase = 3005,

        AddOrderPartiallySucceeded = 4001,

        // Warnings
        DeliveryCannotBeRetried = 5001,
        InvalidDeliveryAction = 5002,
        DeliveryCannotBeCanceled = 5003,

        UnexpectedError = 9999,
    }
}
