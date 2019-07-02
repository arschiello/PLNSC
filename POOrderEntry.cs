using PX.Data.ReferentialIntegrity.Attributes;
using PX.SM;

using System.Configuration;
using PX.Objects.SM;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.TM;
using SOOrder = PX.Objects.SO.SOOrder;
using SOLine = PX.Objects.SO.SOLine;
using PX.Objects.PM;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AP.MigrationMode;
using PX.Objects.Common;
using PX.Objects;
using PX.Objects.PO;

namespace PX.Objects.PO
{
  public class POOrderEntry_Extension : PXGraphExtension<POOrderEntry>
  {
    #region Event Handlers
    
    
    [PXDBString(40, IsUnicode = true)]
    [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
    protected virtual void POOrder_OrderDesc_CacheAttached(PXCache cache)
    {
    
    }
    
    

    #endregion
  }
}