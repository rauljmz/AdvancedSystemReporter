using System;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    [Serializable]
    public class FilterItem:ReferenceItem
    {
        public FilterItem(Item innerItem) : base(innerItem)
        {
        }
    }
}
