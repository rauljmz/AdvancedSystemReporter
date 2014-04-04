using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
	
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
