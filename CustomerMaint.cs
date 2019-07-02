using PX.Data.EP;
using PX.Objects.AP;
using PX.Objects.CR.MassProcess;
using PX.Objects.GL;
using PX.Objects.TX;
using PX.Data.ReferentialIntegrity.Attributes;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PX.Common;
using PX.Data;
using PX.SM;
using PX.Objects.AR.CCPaymentProcessing;
using PX.Objects.AR.Repositories;
using PX.Objects.Common;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.SO;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using CashAccountAttribute = PX.Objects.GL.CashAccountAttribute;
using PX.Objects;
using PX.Objects.AR;

namespace PX.Objects.AR
{
  public class CustomerMaint_Extension : PXGraphExtension<CustomerMaint>
  {
    #region Event Handlers
    
    
        [PXDBString(32, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Customer Name", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        protected virtual void Customer_AcctName_CacheAttached(PXCache cache) { }

        [PXDBEmail]
        [PXUIField(DisplayName = "Email", Visibility = PXUIVisibility.SelectorVisible)]
        [PXMassMergableField]
        [PXDefault()]
        protected virtual void Contact_EMail_CacheAttached(PXCache cache) { }

        [PXDBString(16, IsUnicode = true)]
        [PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
        [PhoneValidation()]
        [PXDefault()]
        [PXMassMergableField]
        protected virtual void Contact_Phone1_CacheAttached(PXCache cache)
        { }

        [PXDBString(32, IsUnicode = true)]
        [PXUIField(DisplayName = "Address Line 1", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault()]
        [PXMassMergableField]
        protected virtual void Address_AddressLine1_CacheAttached(PXCache cache)
        { }

        [PXDBString(32, IsUnicode = true)]
        [PXUIField(DisplayName = "Address Line 2", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXMassMergableField]
        protected virtual void Address_AddressLine2_CacheAttached(PXCache cache)
        { }

        [PXDBString(32, IsUnicode = true)]
        [PXUIField(DisplayName = "City", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault()]
        [PXMassMergableField]
        protected virtual void Address_City_CacheAttached(PXCache cache)
        { }

        [PXDBString(10)]
        [PXUIField(DisplayName = "Postal Code")]
        [PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask), countryIdField: typeof(Address.countryID))]
        [PXDefault()]
        [PXDynamicMask(typeof(Search<Country.zipCodeMask, Where<Country.countryID, Equal<Current<Address.countryID>>>>))]
        [PXMassMergableField]
        protected virtual void Address_PostalCode_CacheAttached(PXCache cache) { }

        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.customer>, Or<Terms.visibleTo, Equal<TermsVisibleTo.all>>>>), DescriptionField = typeof(Terms.descr), CacheGlobal = true)]
        [PXDefault(typeof(Search<CustomerClass.termsID, Where<CustomerClass.customerClassID, Equal<Current<Customer.customerClassID>>>>))]
        [PXUIField(DisplayName = "Terms")]
        protected virtual void Customer_TermsID_CacheAttached(PXCache cache) { }

        [PXDBString(5, IsUnicode = true)]
        [PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
        [PXUIField(DisplayName = "Currency ID")]
        [PXDefault(typeof(Search<CustomerClass.curyID, Where<CustomerClass.customerClassID, Equal<Current<Customer.customerClassID>>>>))]
        protected virtual void Customer_CuryID_CacheAttached(PXCache cache) { }

        [PXDBString(6, IsUnicode = true)]
        [PXSelector(typeof(CurrencyRateType.curyRateTypeID))]
        [PXDefault(typeof(Search<CustomerClass.curyRateTypeID, Where<CustomerClass.customerClassID, Equal<Current<Customer.customerClassID>>>>))]
        [PXUIField(DisplayName = "Curr. Rate Type ")]
        protected virtual void Customer_CuryRateTypeID_CacheAttached(PXCache cache) { }



        #endregion

        #region Event Handlers

        protected void Customer_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            Customer row = (Customer)e.Row;
            var str = row.AcctCD.ToString().Trim();
            int value = str.Length;

            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^[a-zA-Z0-9]+$"))
            {
                if (value != 6)
                {
                    sender.RaiseExceptionHandling<Customer.acctCD>(e.Row, row.AcctCD, new PXSetPropertyException("Cannot Be Less Or More Than 6 Characters"));
                }
            }
            else
            {
                sender.RaiseExceptionHandling<Customer.acctCD>(e.Row, row.AcctCD, new PXSetPropertyException("is NOT an alphanumeric string. Please enter only letters or numbers."));
            }
            
        }
        
        #endregion
    }
}