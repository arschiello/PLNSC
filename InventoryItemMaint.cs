using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PX.Api.Models;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.GL;
using PX.Objects.PO;
using PX.Objects.RUTROT;
using PX.Objects.SO;
using PX.SM;
using PX.Web.UI;
using CRLocation = PX.Objects.CR.Standalone.Location;
using ItemStats = PX.Objects.IN.Overrides.INDocumentRelease.ItemStats;
using PX.Objects;
using PX.Objects.IN;

namespace PX.Objects.IN
{
    public class InventoryItemMaint_Extension : PXGraphExtension<InventoryItemMaint>
    {
        #region Event Handlers

        [PXOverride]
        public void Persist(System.Action del)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                InventoryItem item = Base.Item.Current;
                if (item != null && Base.itemxrefrecords.Cache.IsDirty)
                {
                    string alternateIDs = string.Empty;
                    foreach (INItemXRef crossRef in Base.itemxrefrecords.Select())
                    {
                        alternateIDs = string.IsNullOrEmpty(alternateIDs) ?
                            crossRef.Descr : alternateIDs + "; " + crossRef.Descr;
                    }
                    item.GetExtension<InventoryItemExt>().UsrAlternateIDs = alternateIDs;
                    Base.Item.Update(item);
                }

                del();
                ts.Complete();
            }
        }


        public PXAction<PX.Objects.IN.InventoryItem> RecalcAlternateIDs;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Generate Part No")]
        protected void recalcAlternateIDs()
        {
            PXLongOperation.StartOperation(Base, () =>
            {
                InventoryItemMaint itemMaint = PXGraph.CreateInstance<InventoryItemMaint>();
                var items = PXSelect<InventoryItem, Where<InventoryItem.stkItem, Equal<boolTrue>>>.Select(itemMaint);
                foreach (InventoryItem item in items)
                {
                    itemMaint.Clear();
                    itemMaint.Item.Current = item;
                    itemMaint.itemxrefrecords.Cache.IsDirty = true;
                    itemMaint.Actions.PressSave();
                }
            });
        }

        #endregion
    }

}