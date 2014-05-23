using System;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    [Serializable]
    public class ScannerItem:ReferenceItem
    {
        public ScannerItem(Item i) : base(i)
        {
        }
    }
}
