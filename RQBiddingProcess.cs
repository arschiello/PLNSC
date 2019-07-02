using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using System.Collections;
using PX.Objects.PO;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.TM;
using PX.Objects.AR;
using PX.Objects;
using PX.Objects.RQ;

namespace PX.Objects.RQ
{
    public class RQBiddingProcess_Extension : PXGraphExtension<RQBiddingProcess>
    {
        #region Event Handlers
        protected virtual void RQRequisition_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            RQRequisition upd = e.Row as RQRequisition;

            EnsurePattern(e.Row as RQRequisition);
        }
        protected virtual void RQRequisition_CuryRateTypeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EnsurePattern(e.Row as RQRequisition);
        }
        protected virtual void RQRequisition_CuryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EnsurePattern(e.Row as RQRequisition);
        }

        protected virtual void RQRequisition_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            RQRequisition row = (RQRequisition)e.Row;
            RQRequisitionExt rowExt = row.GetExtension<RQRequisitionExt>();

            if (row == null)
                return;

            bool splitable = row.Splittable == true;
            PXUIFieldAttribute.SetVisible<RQRequisition.curyID>(sender, null, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
            PXUIFieldAttribute.SetVisible<RQRequisitionLine.biddingQty>(Base.Lines.Cache, null, splitable);
            PXUIFieldAttribute.SetVisible<RQBidding.orderQty>(Base.Bidding.Cache, null, splitable);

            PXUIFieldAttribute.SetEnabled<RQBidding.selected>(Base.Bidding.Cache, null, splitable);
            PXUIFieldAttribute.SetEnabled<RQRequisition.reqNbr>(sender, null, !(Base.State.Current.SingleMode == true));
            PXUIFieldAttribute.SetEnabled<RQRequisition.vendorID>(sender, row, false);
            PXUIFieldAttribute.SetEnabled<RQRequisition.vendorLocationID>(sender, row, false);

            if (row.Status == RQRequisitionStatus.Bidding)
            {
                PXUIFieldAttribute.SetEnabled<RQRequisition.vendorRefNbr>(sender, row, true);
                PXUIFieldAttribute.SetEnabled<RQRequisition.splittable>(sender, row, true);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled(sender, row, false);
                PXUIFieldAttribute.SetEnabled<RQRequisition.reqNbr>(sender, row, true);
            }

            Base.Process.SetEnabled(row.Status == RQRequisitionStatus.Bidding);
            Base.ChooseVendor.SetEnabled(row.Status == RQRequisitionStatus.Bidding);
            Base.UpdateResult.SetEnabled(row.Status == RQRequisitionStatus.Bidding);
            Base.ClearResult.SetEnabled(row.Status == RQRequisitionStatus.Bidding);

            Base.Document.Cache.AllowInsert =
            Base.Document.Cache.AllowDelete = false;
            Base.Document.Cache.AllowUpdate = true;

            Base.Bidding.Cache.AllowInsert =
            Base.Bidding.Cache.AllowUpdate =
            Base.Bidding.Cache.AllowDelete =

            Base.Vendors.Cache.AllowInsert =
            Base.Vendors.Cache.AllowUpdate =
            Base.Vendors.Cache.AllowDelete =
            row.Status == RQRequisitionStatus.Bidding;

            bool rHold = row.Hold == true;
            bool custOrder = row.CustomerID != null;
            bool gaRequest = (row.CustomerID == null && rowExt.UsrPurchMethod == 2) || row.CustomerID != null;

        }

        protected virtual void RQRequisitionLine_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            RQRequisitionLine row = e.Row as RQRequisitionLine;
            RQRequisitionLineExt rowext = row.GetExtension<RQRequisitionLineExt>();

            if (rowext.PreventUpdate == true)
            {
                e.Cancel = true;
                rowext.PreventUpdate = false;
            }
        }

        protected virtual void EnsurePattern(RQRequisition row)
        {
            foreach (RQRequisitionLine line in Base.Lines.Select())
            {
                RQRequisitionLineExt lineext = Base.Lines.Cache.GetExtension<RQRequisitionLineExt>(line);

                EnsureLine(row, line, lineext);
                Base.Lines.Cache.Update(line);

                lineext.PreventUpdate = true;
            }
        }
        protected virtual void EnsureLine(RQRequisition row, RQRequisitionLine line, RQRequisitionLineExt lineext)
        {
            if (row.VendorID == null || row.VendorLocationID == null)
            {
                lineext.UsrPatternCost = 0;
                lineext.UsrCuryPatternCost = 0;
                lineext.UsrInitialCost = 0;
                lineext.UsrCuryInitialCost = 0;
                lineext.UsrCuryInitialExtCost = 0;
                lineext.UsrInitialExtCost = 0;
            }
            else
            {
                PXCache cache = Base.Bidding.Cache;
                RQBidding bidding = PXSelect<RQBidding,
                  Where<RQBidding.reqNbr, Equal<Required<RQBidding.reqNbr>>,
                    And<RQBidding.lineNbr, Equal<Required<RQBidding.lineNbr>>,
                    And<RQBidding.vendorID, Equal<Required<RQBidding.vendorID>>,
                    And<RQBidding.vendorLocationID, Equal<Required<RQBidding.vendorLocationID>>>>>>>.Select(Base, line.ReqNbr, line.LineNbr, row.VendorID, row.VendorLocationID);
                if (bidding != null)
                {
                    RQBiddingExt biddingext = bidding.GetExtension<RQBiddingExt>();

                    //Pattern
                    lineext.UsrCuryPatternCost = Tools.ConvertCurrency<RQRequisitionLine.curyInfoID>(Base.Lines.Cache, line, biddingext.UsrPatternCost ?? 0);
                    //Unit Cost
                    line.CuryEstUnitCost = Tools.ConvertCurrency<RQRequisitionLine.curyInfoID>(Base.Lines.Cache, line, bidding.QuoteUnitCost ?? 0);
                    //Ext Cost
                    line.CuryEstExtCost = Tools.ConvertCurrency<RQRequisitionLine.curyInfoID>(Base.Lines.Cache, line, bidding.QuoteExtCost ?? 0);
                    //Initial Cost
                    lineext.UsrCuryInitialCost = Tools.ConvertCurrency<RQRequisitionLine.curyInfoID>(Base.Lines.Cache, line, biddingext.UsrInitialCost ?? 0);
                    //Initial Ext Cost
                    lineext.UsrCuryInitialCost = Tools.ConvertCurrency<RQRequisitionLine.curyInfoID>(Base.Lines.Cache, line, biddingext.UsrInitialExtCost ?? 0);
                }
            }
        }
        #endregion
    }
}