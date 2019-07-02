using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CS;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.SM;
using Branch = PX.SM.Branch;
using PX.Objects;
using PX.Objects.AP;
using PX.Objects.CR.MassProcess;
using PX.Data.ReferentialIntegrity.Attributes;

namespace PX.Objects.AP
{
  public class VendorMaint_Extension : PXGraphExtension<VendorMaint>
    {
        [PXDBString(32, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Vendor Name", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        protected virtual void VendorR_AcctName_CacheAttached(PXCache cache) { }

        [PXDBString(32, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Address Line 1", Visibility = PXUIVisibility.SelectorVisible)]
        [PXMassMergableField]
        protected virtual void Address_AddressLine1_CacheAttached(PXCache cache) { }

        [PXDBString(32, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Address Line 2", Visibility = PXUIVisibility.SelectorVisible)]
        [PXMassMergableField]
        protected virtual void Address_AddressLine2_CacheAttached(PXCache cache) { }

        [PXDBEmail]
        [PXUIField(DisplayName = "Email", Visibility = PXUIVisibility.SelectorVisible)]
        [PXMassMergableField]
        [PXDefault()]
        protected virtual void Contact_EMail_CacheAttached(PXCache cache) { }

        [PXDBString(16, IsUnicode = true)]
        [PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault()]
        [PhoneValidation()]
        [PXMassMergableField]
        protected virtual void Contact_Phone1_CacheAttached(PXCache cache) { }

        [PXDBString(32, IsUnicode = true)]
        [PXUIField(DisplayName = "City", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault()]
        [PXMassMergableField]
        protected virtual void Address_City_CacheAttached(PXCache cache) { }

        [PXDBString(10)]
        [PXUIField(DisplayName = "Postal Code")]
        [PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask), countryIdField: typeof(Address.countryID))]
        [PXDynamicMask(typeof(Search<Country.zipCodeMask, Where<Country.countryID, Equal<Current<Address.countryID>>>>))]
        [PXDefault()]
        [PXMassMergableField]
        protected virtual void Address_PostalCode_CacheAttached(PXCache cache) { }

        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.vendor>, Or<Terms.visibleTo, Equal<TermsVisibleTo.all>>>>), DescriptionField = typeof(Terms.descr), CacheGlobal = true)]
        [PXDefault(typeof(Select<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>>), SourceField = typeof(VendorClass.termsID))]
        [PXUIField(DisplayName = "Terms")]
        //[PXForeignReference(typeof(Field<Vendor.termsID>.IsRelatedTo<Terms.termsID>))]
        protected virtual void VendorR_TermsID_CacheAttached(PXCache cache) { }

        [PXDBString(5, IsUnicode = true)]
        [PXSelector(typeof(Search<CurrencyList.curyID, Where<CurrencyList.isFinancial, Equal<True>>>), CacheGlobal = true)]
        [PXDefault(typeof(Select<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>>), SourceField = typeof(VendorClass.curyID))]
        [PXUIField(DisplayName = "Currency ID")]
        protected virtual void VendorR_CuryID_CacheAttached(PXCache cache) { }

        [PXDBString(6, IsUnicode = true)]
        [PXSelector(typeof(CurrencyRateType.curyRateTypeID))]
        [PXDefault(typeof(Select<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>>), SourceField = typeof(VendorClass.curyRateTypeID))]
        [PXUIField(DisplayName = "Curr. Rate Type ")]
        protected virtual void VendorR_CuryRateTypeID_CacheAttached(PXCache cache) { }

        protected void Vendor_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {

            var row = (Vendor)e.Row;

            string vClass = row.VendorClassID;

            if(vClass == "FORWARDER")
            {
                AutoNumberAttribute.SetNumberingId<Vendor.acctCD>(cache, "FORWARDER");
            }
            

        }

        public virtual void Vendor_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
        {
            Vendor vendor = e.Row as Vendor;
            if (vendor != null && e.Operation == PXDBOperation.Insert && e.TranStatus == PXTranStatus.Completed)
            {
                PXLongOperation.StartOperation(Base, delegate ()
                {
                    // Add your webservice call here
                });
            }
        }

        protected void VendorR_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            Vendor row = (Vendor)e.Row;
            var str = row.AcctCD.ToString().Trim();
            int value = str.Length;

            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^[a-zA-Z0-9]+$"))
            {
                if (value != 6)
                {
                    sender.RaiseExceptionHandling<Vendor.acctCD>(e.Row, row.AcctCD, new PXSetPropertyException("Cannot Be Less Or More Than 6 Characters"));
                    //throw new PXRowPersistingException(typeof(VendorR.acctCD).Name, null, "Cannot Be Less Or More Than 6 Characters");
                }
            }
            else
            {
                sender.RaiseExceptionHandling<Vendor.acctCD>(e.Row, row.AcctCD, new PXSetPropertyException("is NOT an alphanumeric string. Please enter only letters or numbers."));
            }
            
        }
        
    }
}