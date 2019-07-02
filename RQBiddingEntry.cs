using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using System.Collections;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects;
using PX.Objects.RQ;

namespace PX.Objects.RQ
{
    public class RQBiddingEntry_Extension : PXGraphExtension<RQBiddingEntry>
    {

        public const string LeadTimeCannotBeZero = "Lead Time Must Be Greater Than Zero";

        public PXSelect<RQRequisitionLineBidding,
            Where<RQRequisitionLineBidding.reqNbr, Equal<Current<RQBiddingVendor.reqNbr>>>> Lines;


        protected virtual IEnumerable lines()
        {
            if (Base.Vendor.Current == null || Base.Vendor.Current.VendorLocationID == null)
                yield break;

            using (ReadOnlyScope scope = new ReadOnlyScope(this.Lines.Cache))
            {
                bool reset = !Base.Bidding.Cache.IsDirty;

                PXResultset<RQRequisitionLineBidding> list =
                    PXSelectJoin<RQRequisitionLineBidding,
                        LeftJoin<RQBidding,
                                    On<RQBidding.reqNbr, Equal<RQRequisitionLineBidding.reqNbr>,
                                 And<RQBidding.lineNbr, Equal<RQRequisitionLineBidding.lineNbr>,
                                And<RQBidding.vendorID, Equal<Current<RQBiddingVendor.vendorID>>,
                                And<RQBidding.vendorLocationID, Equal<Current<RQBiddingVendor.vendorLocationID>>>>>>>,
                        Where<RQRequisitionLineBidding.reqNbr, Equal<Current<RQBiddingVendor.reqNbr>>>>
                        .Select(Base);

                if (reset)
                {
                    this.Lines.Cache.Clear();
                }

                foreach (PXResult<RQRequisitionLineBidding, RQBidding> item in list)
                {
                    RQRequisitionLineBidding result = PrepareRQRequisitionLineBiddingInViewDelegate(item);
                    yield return result;
                }
            }
        }

        protected virtual RQRequisitionLineBidding PrepareRQRequisitionLineBiddingInViewDelegate(PXResult<RQRequisitionLineBidding, RQBidding> item)
        {
            RQRequisitionLineBidding rqLineBidding = item;
            RQBidding bidding = item;
            bidding = Base.Bidding.Locate(bidding) ?? item;
            RQBiddingExt biddingExt = Base.Bidding.Cache.GetExtension<RQBiddingExt>(bidding);

            rqLineBidding = (RQRequisitionLineBidding)this.Lines.Cache.CreateCopy(rqLineBidding);
            RQRequisitionLineBiddingExt rqLineBiddingExt = (RQRequisitionLineBiddingExt)this.Lines.Cache.GetExtension<RQRequisitionLineBiddingExt>(rqLineBidding);

            FillRequisitionLineBiddingPropertiesInViewDelegate(rqLineBidding, bidding, rqLineBiddingExt, biddingExt);

            rqLineBidding = this.Lines.Insert(rqLineBidding) ?? (RQRequisitionLineBidding)this.Lines.Cache.Locate(rqLineBidding);
            return rqLineBidding;
        }

        protected virtual void FillRequisitionLineBiddingPropertiesInViewDelegate(RQRequisitionLineBidding rqLineBidding, RQBidding bidding, RQRequisitionLineBiddingExt rqLineBiddingExt, RQBiddingExt biddingExt)
        {
            rqLineBidding.QuoteNumber = bidding.QuoteNumber;
            rqLineBidding.QuoteQty = bidding.QuoteQty ?? 0m;
            rqLineBidding.CuryInfoID = Base.Vendor.Current.CuryInfoID;
            rqLineBidding.CuryQuoteUnitCost = bidding.CuryQuoteUnitCost ?? 0m;
            rqLineBidding.QuoteUnitCost = bidding.QuoteUnitCost ?? 0m;
            rqLineBidding.CuryQuoteExtCost = bidding.CuryQuoteExtCost ?? 0m;
            rqLineBidding.QuoteExtCost = bidding.QuoteExtCost ?? 0m;
            rqLineBidding.MinQty = bidding.MinQty ?? 0m;

            if(biddingExt.UsrInitialCost == 0)
            {
                rqLineBiddingExt.UsrInitialCost = bidding.QuoteUnitCost ?? 0m;
                rqLineBiddingExt.UsrInitialExtCost = (bidding.QuoteUnitCost ?? 0m) * (bidding.QuoteQty ?? 0m);
            }
            else
            {
                rqLineBiddingExt.UsrInitialCost = biddingExt.UsrInitialCost ?? 0m;
                rqLineBiddingExt.UsrInitialExtCost = biddingExt.UsrInitialExtCost ?? 0m;
            }
            
            if(biddingExt.UsrCuryInitialCost == 0)
            {
                rqLineBiddingExt.UsrCuryInitialCost = bidding.CuryQuoteUnitCost ?? 0m;
                rqLineBiddingExt.UsrCuryInitialExtCost = (bidding.CuryQuoteUnitCost ?? 0m) * (bidding.QuoteQty ??0m);
            }
            else
            {
                rqLineBiddingExt.UsrCuryInitialCost = biddingExt.UsrCuryInitialCost ?? 0m;
                rqLineBiddingExt.UsrCuryInitialExtCost = biddingExt.UsrCuryInitialExtCost ?? 0m;
            }
            

            if (bidding.CuryQuoteUnitCost == null && rqLineBidding.InventoryID != null)
            {
                string bidVendorCuryID = (string)Base.Vendor.GetValueExt<RQBiddingVendor.curyID>(Base.Vendor.Current);

                POItemCostManager.ItemCost cost = POItemCostManager.Fetch(Base, Base.Vendor.Current.VendorID, Base.Vendor.Current.VendorLocationID,
                                                                          docDate: null,
                                                                          curyID: bidVendorCuryID,
                                                                          inventoryID: rqLineBidding.InventoryID,
                                                                          subItemID: rqLineBidding.SubItemID,
                                                                          siteID: null,
                                                                          uom: rqLineBidding.UOM);
                rqLineBidding.CuryQuoteUnitCost =
                    cost.Convert<RQRequisitionLineBidding.inventoryID, RQRequisitionLineBidding.curyInfoID>(Base, rqLineBidding, rqLineBidding.UOM);
            }

            if (rqLineBidding.CuryQuoteUnitCost == null)
            {
                rqLineBidding.CuryQuoteUnitCost = 0m;
            }
        }
        
        #region Event Handlers

        protected virtual void RQRequisitionLineBidding_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            RQRequisitionLineBidding newRow = (RQRequisitionLineBidding)e.Row;
            RQRequisitionLineBidding oldRow = (RQRequisitionLineBidding)e.OldRow;

            RQRequisitionLineBiddingExt newExt = (RQRequisitionLineBiddingExt)cache.GetExtension<RQRequisitionLineBiddingExt>(e.Row);
            RQRequisitionLineBiddingExt oldExt = (RQRequisitionLineBiddingExt)cache.GetExtension<RQRequisitionLineBiddingExt>(e.OldRow);

            if (newRow.MinQty != oldRow.MinQty || newRow.QuoteUnitCost != oldRow.QuoteUnitCost ||
                newRow.QuoteQty != oldRow.QuoteQty || newRow.QuoteNumber != oldRow.QuoteNumber)
            {
                RQBidding bidding = PXSelect<RQBidding,
                    Where<RQBidding.reqNbr, Equal<Required<RQBidding.reqNbr>>,
                    And<RQBidding.lineNbr, Equal<Required<RQBidding.lineNbr>>,
                    And<RQBidding.vendorID, Equal<Required<RQBidding.vendorID>>,
                    And<RQBidding.vendorLocationID, Equal<Required<RQBidding.vendorLocationID>>>>>>>
                    .SelectWindowed(Base, 0, 1,
                    Base.Vendor.Current.ReqNbr,
                    newRow.LineNbr,
                    Base.Vendor.Current.VendorID,
                    Base.Vendor.Current.VendorLocationID);

                if(bidding == null)
                {
                    bidding = new RQBidding
                    {
                        VendorID = Base.Vendor.Current.VendorID,
                        VendorLocationID = Base.Vendor.Current.VendorLocationID,
                        ReqNbr = Base.Vendor.Current.ReqNbr,
                        CuryInfoID = Base.Vendor.Current.CuryInfoID,
                        LineNbr = newRow.LineNbr
                    };
                }
                else
                    bidding = (RQBidding)Base.Bidding.Cache.CreateCopy(bidding);

                RQBiddingExt biddingExt = Base.Bidding.Cache.GetExtension<RQBiddingExt>(bidding);

                biddingExt.UsrInitialCost = newExt.UsrInitialCost;
                biddingExt.UsrCuryInitialCost = newExt.UsrCuryInitialCost;
                biddingExt.UsrCuryInitialExtCost = newExt.UsrCuryInitialExtCost;
                biddingExt.UsrInitialExtCost = newExt.UsrInitialExtCost;

                Base.Bidding.Update(bidding);
            }
        }

        protected virtual void RQBiddingVendor_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            RQBiddingVendor row = (RQBiddingVendor)e.Row;
            if (row == null) return;

            RQBiddingVendorExt rowExt = row.GetExtension<RQBiddingVendorExt>();

            bool bidExists = row.CuryTotalQuoteExtCost > Decimal.Zero == true;

            RQRequisition req = PXSelect<RQRequisition,
                Where<RQRequisition.reqNbr, Equal<Required<RQRequisition.reqNbr>>>>.Select(Base, row.ReqNbr);
            RQRequisitionExt reqExt = req.GetExtension<RQRequisitionExt>();


            if (bidExists)
            {
                if (req.CustomerID != null || reqExt.UsrPurchMethod != 2)
                {
                    if (req.CustomerID != null)
                        PXDefaultAttribute.SetPersistingCheck<RQBiddingVendorExt.usrIncoterm>(sender, row, bidExists ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

                    PXDefaultAttribute.SetPersistingCheck<RQBiddingVendorExt.usrLeadFrom>(sender, row, bidExists ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                    PXDefaultAttribute.SetPersistingCheck<RQBiddingVendorExt.usrLeadTime>(sender, row, bidExists ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                    PXDefaultAttribute.SetPersistingCheck<RQBiddingVendorExt.usrRefDueDate>(sender, row, bidExists ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                }
            }

            //ValidateLeadTime(sender, e);
        }

        protected virtual void RQBiddingVendor_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            RQBiddingVendor row = (RQBiddingVendor)e.Row;
            if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete) return;
            RQBiddingVendorExt rowExt = row.GetExtension<RQBiddingVendorExt>();
            
            RQRequisition req = PXSelect<RQRequisition,
                Where<RQRequisition.reqNbr, Equal<Required<RQRequisition.reqNbr>>>>.Select(Base, row.ReqNbr);
            RQRequisitionExt reqExt = req.GetExtension<RQRequisitionExt>();

            if (reqExt.UsrPurchMethod == 2) return;

            if (row.CuryTotalQuoteExtCost > Decimal.Zero)
            {
                if (rowExt.UsrLeadTime == Decimal.Zero)
                {
                    if (sender.RaiseExceptionHandling<RQBiddingVendorExt.usrLeadTime>(e.Row, rowExt.UsrLeadTime, new PXSetPropertyException(LeadTimeCannotBeZero, typeof(RQBiddingVendorExt.usrLeadTime).Name)))
                    {
                        throw new PXRowPersistingException(typeof(RQBiddingVendorExt.usrLeadTime).Name, null, LeadTimeCannotBeZero, typeof(RQBiddingVendorExt.usrLeadTime).Name);
                    }
                }

                if (string.IsNullOrEmpty(rowExt.UsrIncoterm))
                {
                    if (sender.RaiseExceptionHandling<RQBiddingVendorExt.usrIncoterm>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(RQBiddingVendorExt.usrIncoterm).Name)))
                    {
                        throw new PXRowPersistingException(typeof(RQBiddingVendorExt.usrIncoterm).Name, null, ErrorMessages.FieldIsEmpty, typeof(RQBiddingVendorExt.usrIncoterm).Name);
                    }
                }                

                if (rowExt.UsrRefDueDate == null)
                {
                    if (sender.RaiseExceptionHandling<RQBiddingVendorExt.usrRefDueDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(RQBiddingVendorExt.usrRefDueDate).Name)))
                    {
                        throw new PXRowPersistingException(typeof(RQBiddingVendorExt.usrRefDueDate).Name, null, ErrorMessages.FieldIsEmpty, typeof(RQBiddingVendorExt.usrRefDueDate).Name);
                    }
                }

                if (rowExt.UsrLeadFrom == null)
                {
                    if (sender.RaiseExceptionHandling<RQBiddingVendorExt.usrLeadFrom>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(RQBiddingVendorExt.usrLeadFrom).Name)))
                    {
                        throw new PXRowPersistingException(typeof(RQBiddingVendorExt.usrLeadFrom).Name, null, ErrorMessages.FieldIsEmpty, typeof(RQBiddingVendorExt.usrLeadFrom).Name);
                    }
                }
            }
        }

        public virtual void ValidateLeadTime(PXCache sender, PXRowSelectedEventArgs e)
        {
            RQBiddingVendor row = (RQBiddingVendor)e.Row;
            if (row == null) return;

            RQBiddingVendorExt rowExt = row.GetExtension<RQBiddingVendorExt>();

            bool isErr = false;

            if (row.CuryTotalQuoteExtCost > Decimal.Zero)
            {
                if (rowExt.UsrLeadTime != null && rowExt.UsrLeadTime == 0)
                {
                    sender.RaiseExceptionHandling<RQBiddingVendorExt.usrLeadTime>(e.Row, rowExt.UsrLeadTime, new PXSetPropertyException("Lead Time Must Be Greater Than Zero"));
                    isErr = true;
                }

                if (!isErr)
                {
                    sender.RaiseExceptionHandling<RQBiddingVendorExt.usrLeadTime>(e.Row, null, null);
                }
            }
        }

        #endregion
    }
}
  
