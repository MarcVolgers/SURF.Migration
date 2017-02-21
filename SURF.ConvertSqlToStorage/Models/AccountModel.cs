using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SURF.Delivery.Order.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SURF.Delivery.Order.Models
{  
    [PartitionKey(Name="Account")]
    public class AccountModel : ModelBase<AccountModel>
    {
        private string _name;  
             
        public string Mailadress { get; set; }
     
        [RowKey]
        public string Name
        {
            get { return _name; }
            set
            {
                RowKey = _name = value;
            }
        }

        public string CrmName { get; set; }
        public string CrmId { get; set; }

        //public string RowKey
        //{
        //    get { return Name; }
        //    set { Name = value; }
        //}

        //public string PartitionKey
        //{
        //    get { return "Account"; }
        //    set { base.PartitionKey = "Account"; }
        //}

        //public int CreatedByDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string CreatedByName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string CreatedByYomiName { get; set; }
        //public int crmrd_bikcodeidDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string crmrd_bikcodeidName { get; set; }
        //public int crmrd_countrypidDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string crmrd_countrypidName { get; set; }
        //public int crmrd_countryvidDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string crmrd_countryvidName { get; set; }
        //public int DefaultPriceLevelIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string DefaultPriceLevelIdName { get; set; }
        //public int lmng_accountimageidDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string lmng_accountimageidName { get; set; }
        //public int lmng_othervatcodeidDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string lmng_othervatcodeidName { get; set; }
        //public int MasterAccountIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string MasterAccountIdName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string MasterAccountIdYomiName { get; set; }
        //public int ModifiedByDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string ModifiedByName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string ModifiedByYomiName { get; set; }
        //public int OriginatingLeadIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string OriginatingLeadIdName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string OriginatingLeadIdYomiName { get; set; }
        //public int ParentAccountIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string ParentAccountIdName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string ParentAccountIdYomiName { get; set; }
        //public int PreferredEquipmentIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string PreferredEquipmentIdName { get; set; }
        //public int PreferredServiceIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string PreferredServiceIdName { get; set; }
        //public int PreferredSystemUserIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string PreferredSystemUserIdName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string PreferredSystemUserIdYomiName { get; set; }
        //public int PrimaryContactIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string PrimaryContactIdName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string PrimaryContactIdYomiName { get; set; }
        //public int surf_categorieidDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string surf_categorieidName { get; set; }
        //public int surf_eerstecontactpersooncontentidDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string surf_eerstecontactpersooncontentidName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string surf_eerstecontactpersooncontentidYomiName { get; set; }
        //public int TerritoryIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string TerritoryIdName { get; set; }
        //public int TransactionCurrencyIdDsc { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string TransactionCurrencyIdName { get; set; }
        //public Guid Address1_AddressId { get; set; }
        //public int Address1_AddressTypeCode { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Name { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_PrimaryContactName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Line1 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Line2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Line3 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_City { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_StateOrProvince { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_County { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Country { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_PostOfficeBox { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_PostalCode { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public int Address1_UTCOffset { get; set; }
        //public int Address1_FreightTermsCode { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_UPSZone { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Telephone1 { get; set; }
        //public int Address1_ShippingMethodCode { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Telephone2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Telephone3 { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address1_Fax { get; set; }
        //public int Address1_TimeZoneRuleVersionNumber { get; set; }
        //public DateTime Address1_OverriddenCreatedOn { get; set; }
        //public int Address1_UTCConversionTimeZoneCode { get; set; }
        //public int Address1_ImportSequenceNumber { get; set; }
        //public Guid Address2_AddressId { get; set; }
        //public int Address2_AddressTypeCode { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Name { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_PrimaryContactName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Line1 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Line2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Line3 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_City { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_StateOrProvince { get; set; }
        //public string Address2_County { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Country { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_PostOfficeBox { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_PostalCode { get; set; }

        //public int Address2_UTCOffset { get; set; }
        //public int Address2_FreightTermsCode { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        //public string Address2_UPSZone { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Telephone1 { get; set; }
        //public int Address2_ShippingMethodCode { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Telephone2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Telephone3 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Address2_Fax { get; set; }
        //public int Address2_TimeZoneRuleVersionNumber { get; set; }
        //public DateTime Address2_OverriddenCreatedOn { get; set; }
        //public int Address2_UTCConversionTimeZoneCode { get; set; }
        //public int Address2_ImportSequenceNumber { get; set; }
        //public Guid OwnerId { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string OwnerIdName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string OwnerIdYomiName { get; set; }
        //public int OwnerIdDsc { get; set; }
        //public int OwnerIdType { get; set; }
        //public Guid AccountId { get; set; }
        //public int AccountCategoryCode { get; set; }
        //public Guid TerritoryId { get; set; }
        //public Guid DefaultPriceLevelId { get; set; }
        //public int CustomerSizeCode { get; set; }
        //public int PreferredContactMethodCode { get; set; }
        //public int CustomerTypeCode { get; set; }
        //public int AccountRatingCode { get; set; }
        //public int IndustryCode { get; set; }
        //public int TerritoryCode { get; set; }
        //public int AccountClassificationCode { get; set; }
        //public int DeletionStateCode { get; set; }
        //public int BusinessTypeCode { get; set; }
        //public Guid OwningBusinessUnit { get; set; }
        //public Guid OwningTeam { get; set; }
        //public Guid OwningUser { get; set; }
        //public Guid OriginatingLeadId { get; set; }
        //public int PaymentTermsCode { get; set; }
        //public int ShippingMethodCode { get; set; }
        //public Guid PrimaryContactId { get; set; }
        //public bool ParticipatesInWorkflow { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Name { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string AccountNumber { get; set; }
        //public int NumberOfEmployees { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Description { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SIC { get; set; }
        //public int OwnershipCode { get; set; }
        //public int SharesOutstanding { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string TickerSymbol { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string StockExchange { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string WebSiteURL { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string FtpSiteURL { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string EMailAddress1 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string EMailAddress2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string EMailAddress3 { get; set; }

        //public bool DoNotPhone { get; set; }
        //public bool DoNotFax { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Telephone1 { get; set; }
        //public bool DoNotEMail { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Telephone2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Fax { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Telephone3 { get; set; }
        //public bool DoNotPostalMail { get; set; }
        //public bool DoNotBulkEMail { get; set; }
        //public bool DoNotBulkPostalMail { get; set; }
        //public bool CreditOnHold { get; set; }
        //public bool IsPrivate { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public Guid CreatedBy { get; set; }
        //public DateTime ModifiedOn { get; set; }
        //public Guid ModifiedBy { get; set; }
        //public Guid ParentAccountId { get; set; }
        //public int StateCode { get; set; }
        //public int StatusCode { get; set; }
        //public int PreferredAppointmentDayCode { get; set; }
        //public Guid PreferredSystemUserId { get; set; }
        //public int PreferredAppointmentTimeCode { get; set; }
        //public bool Merged { get; set; }
        //public bool DoNotSendMM { get; set; }
        //public Guid MasterId { get; set; }
        //public DateTime LastUsedInCampaign { get; set; }
        //public Guid PreferredServiceId { get; set; }
        //public Guid PreferredEquipmentId { get; set; }
        //public int UTCConversionTimeZoneCode { get; set; }
        //public DateTime OverriddenCreatedOn { get; set; }
        //public int TimeZoneRuleVersionNumber { get; set; }
        //public int ImportSequenceNumber { get; set; }
        //public Guid TransactionCurrencyId { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string YomiName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad1_area { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad1_building { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad1_extra1 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad1_extra2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad1_extra3 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad1_extra4 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad1_extra5 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad2_area { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad2_building { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad2_extra1 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad2_extra2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad2_extra3 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad2_extra4 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_ad2_extra5 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_address1_block { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_address2_block { get; set; }
        //public bool Crmrd_BusinessPartner { get; set; }
        //public bool Crmrd_copyaddress { get; set; }
        //public bool Crmrd_Customer { get; set; }
        //public bool Crmrd_DataValidated { get; set; }
        //public bool Crmrd_Investor { get; set; }
        //public bool CRMRD_IsOwnCompany { get; set; }
        //public bool Crmrd_Press { get; set; }
        //public bool Crmrd_Reseller { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_Searchname { get; set; }
        //public bool Crmrd_Supplier { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Crmrd_taxnumber { get; set; }
        //public bool Lmng_AccessionAgreementSigned { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Lmng_othercostcenter { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Lmng_PictureUrl { get; set; }
        //public bool Lmng_ShowonSURFdienstennl { get; set; }
        //public bool Lmng_Tenderofferneverrequired { get; set; }
        //public bool Lmng_VATremittancerequired { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_Afkorting { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_AuthenticationPassword { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_authenticationusername { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_BTWnummer { get; set; }
        //public int SURF_Consortium { get; set; }
        //public int SURF_debiteurnr { get; set; }
        //public bool SURF_emailuitleverbestellingen { get; set; }
        //public bool SURF_IsBranchorganisatie { get; set; }
        //public bool SURF_IsFamiliepartner { get; set; }
        //public bool SURF_IsHuisendienstenleverancier { get; set; }
        //public bool SURF_IsICTaanbiederuitgever { get; set; }
        //public bool SURF_IsInstelling { get; set; }
        //public bool SURF_IsOverig { get; set; }
        //public bool SURF_IsReseller { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_loginnaam { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_loginpassword { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_Logintoelichting { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_ss_authenticatie_hostname2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Surf_ss_authentication_aselectorg { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_ss_authentication_aselectprov { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public bool SURF_ss_authentication_checkdomain { get; set; }
        //public bool SURF_ss_authentication_cleandomain { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_ss_authentication_domain { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_ss_authentication_hostname { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_ss_authentication_hostname2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        //public string SURF_ss_authentication_hostname3 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_ss_authentication_path { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string SURF_ss_authentication_secret { get; set; }

        //public bool SURF_ss_authentication_stripdomain { get; set; }
        //public int SURF_ss_authenticationtype { get; set; }
        //public bool SURF_Uitleveraar { get; set; }
        //public Guid lmng_accountimageid { get; set; }
        //public Guid crmrd_bikcodeid { get; set; }
        //public Guid crmrd_countrypid { get; set; }
        //public Guid crmrd_countryvid { get; set; }
        //public Guid surf_eerstecontactpersooncontentid { get; set; }
        //public Guid surf_categorieid { get; set; }
        //public Guid lmng_othervatcodeid { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Lmng_surfconext_entityid { get; set; }
        //public bool Lmng_nationalprocedureapplicable { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Lmng_schacHomeOrganization { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string Lmng_uitleveraarnaam { get; set; }
    }

}