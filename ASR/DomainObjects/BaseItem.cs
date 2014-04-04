using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.DomainObjects
{
    [Serializable]
    public class BaseItem
    {       
        protected BaseItem(Item innerItem) //: base(innerItem)
        {            
            Name = innerItem.Name;
            ID = innerItem.ID;
            Icon = innerItem.Appearance.Icon;
            Database = innerItem.Database;
            Path = innerItem.Paths.FullPath;
            Uri = innerItem.Uri;
        }

        public string Name { get; set; }

        public Sitecore.Data.ID ID { get; set; }

        public string Icon { get; set; }

        public Sitecore.Data.Database Database { get; set; }

        public string Path
        {
            get;
            set;
        }

        public Sitecore.Data.ItemUri Uri { get; set; }

        protected IEnumerable<Item> GetMultilistField(string name)
        {
            var item = GetItem();   
            MultilistField field = item.Fields[name];
            return field.GetItems() ?? new Item[] {};
        }

        public Item GetItem()
        {
           return Sitecore.Data.Database.GetItem(Uri);
        } 
    }
}
