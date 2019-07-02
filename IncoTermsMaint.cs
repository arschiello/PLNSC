using System;
using PX.Data;

namespace PLNSC
{
  public class IncoTermsMaint : PXGraph<IncoTermsMaint>
  {
        public PXSavePerRow<SCIncoterm> Save;
        public PXCancel<SCIncoterm> Cancel;
        [PXImport(typeof(SCIncoterm))]

        public PXSelect<SCIncoterm> Incoterm;

        protected virtual void SCIncoterm_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
        {
            SCIncoterm term = PXSelect<SCIncoterm, Where<SCIncoterm.incotermID, Equal<Required<SCIncoterm.incotermID>>>>.SelectWindowed(this, 0, 1, ((SCIncoterm)e.Row).IncotermID);
            if (term != null)
            {
                cache.RaiseExceptionHandling<SCIncoterm.incotermID>(e.Row, ((SCIncoterm)e.Row).IncotermID, new PXException("Record Already Exist"));
                e.Cancel = true;
            }
        }
    }
}