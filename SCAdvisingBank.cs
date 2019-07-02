using System;
using PX.Data;

namespace PLNSC
{
  [Serializable]
  public class SCAdvisingBank : IBqlTable
  {
    #region AdvisingBankID
    [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Advising Bank ID")]
    public virtual string AdvisingBankID { get; set; }
    public abstract class advisingBankID : IBqlField { }
    #endregion

    #region AdvisingBankName
    [PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Advising Bank Name")]
    public virtual string AdvisingBankName { get; set; }
    public abstract class advisingBankName : IBqlField { }
    #endregion

    #region AdvisingBankBranch
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Advising Bank Branch")]
    public virtual string AdvisingBankBranch { get; set; }
    public abstract class advisingBankBranch : IBqlField { }
    #endregion

    #region BankAcctNbr
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Bank Acct Nbr")]
    public virtual string BankAcctNbr { get; set; }
    public abstract class bankAcctNbr : IBqlField { }
    #endregion

    #region AddressLine1
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Address Line1")]
    public virtual string AddressLine1 { get; set; }
    public abstract class addressLine1 : IBqlField { }
    #endregion

    #region AddressLine2
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Address Line2")]
    public virtual string AddressLine2 { get; set; }
    public abstract class addressLine2 : IBqlField { }
    #endregion

    #region AddressLine3
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Address Line3")]
    public virtual string AddressLine3 { get; set; }
    public abstract class addressLine3 : IBqlField { }
    #endregion

    #region City
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "City")]
    public virtual string City { get; set; }
    public abstract class city : IBqlField { }
    #endregion

    #region State
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "State")]
    public virtual string State { get; set; }
    public abstract class state : IBqlField { }
    #endregion

    #region CountryID
    [PXDBString(2, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Country ID")]
    public virtual string CountryID { get; set; }
    public abstract class countryID : IBqlField { }
    #endregion

    #region PostalCode
    [PXDBString(20, InputMask = "")]
    [PXUIField(DisplayName = "Postal Code")]
    public virtual string PostalCode { get; set; }
    public abstract class postalCode : IBqlField { }
    #endregion

    #region SwiftCode
    [PXDBString(11, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Swift Code")]
    public virtual string SwiftCode { get; set; }
    public abstract class swiftCode : IBqlField { }
    #endregion

    #region CreatedByID
    [PXDBCreatedByID()]
    public virtual Guid? CreatedByID { get; set; }
    public abstract class createdByID : IBqlField { }
    #endregion

    #region CreatedByScreenID
    [PXDBCreatedByScreenID()]
    public virtual string CreatedByScreenID { get; set; }
    public abstract class createdByScreenID : IBqlField { }
    #endregion

    #region CreatedDateTime
    [PXDBDate()]
    [PXUIField(DisplayName = "Created Date Time")]
    public virtual DateTime? CreatedDateTime { get; set; }
    public abstract class createdDateTime : IBqlField { }
    #endregion

    #region LastModifiedByID
    [PXDBLastModifiedByID()]
    public virtual Guid? LastModifiedByID { get; set; }
    public abstract class lastModifiedByID : IBqlField { }
    #endregion

    #region LastModifiedByScreenID
    [PXDBLastModifiedByScreenID()]
    public virtual string LastModifiedByScreenID { get; set; }
    public abstract class lastModifiedByScreenID : IBqlField { }
    #endregion

    #region LastModifiedDateTime
    [PXDBDate()]
    [PXUIField(DisplayName = "Last Modified Date Time")]
    public virtual DateTime? LastModifiedDateTime { get; set; }
    public abstract class lastModifiedDateTime : IBqlField { }
    #endregion

    #region Tstamp
    [PXDBTimestamp()]
    [PXUIField(DisplayName = "Tstamp")]
    public virtual byte[] Tstamp { get; set; }
    public abstract class tstamp : IBqlField { }
    #endregion
  }
}