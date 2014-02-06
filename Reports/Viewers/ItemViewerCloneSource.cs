using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Reports.Items;

namespace ASR.Reports.Viewers
{
    public class ItemViewerCloneSource : ItemViewer
    {
        protected override Sitecore.Data.Items.Item ExtractItem(Interface.DisplayElement dElement)
        {
            var item = base.ExtractItem(dElement);
            if (item != null && item.Source != null)
            {
                return item.Source;
            }
            //if it does not have a clone source, return null so we don't display any data
            return null;
        }
    }
}
