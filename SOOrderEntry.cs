using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.Common.Attributes;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using POLine = PX.Objects.PO.POLine;
using POOrder = PX.Objects.PO.POOrder;
using System.Threading.Tasks;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AR.CCPaymentProcessing;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common;
using PX.Objects.IN.Overrides.INDocumentRelease;
using PX.Objects;
using PX.Objects.SO;
using PX.Objects.RQ;

namespace PX.Objects.SO
{
  public class SOOrderEntry_Extension : PXGraphExtension<SOOrderEntry>
  {
        #region Event Handlers
      
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void SOOrder_OrderDesc_CacheAttached(PXCache cache)
        {
        }

        #endregion
    }
}