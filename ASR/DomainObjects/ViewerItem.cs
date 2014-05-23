using System;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    [Serializable]
	public class ViewerItem : ReferenceItem
	{
	    public ViewerItem(Item i) : base(i)
	    {
            ColumnsXml = i["columns"];
	    }


        public string ColumnsXml
        {
            get;
            private set;
        }

	}
}
