using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;

namespace ASR.Serialization
{
    [Serializable]
    public class ItemSerializator : ISerializator
    {
        public SerializedObject Serialize(object thingToSerialize)
        {
            var item = thingToSerialize as Sitecore.Data.Items.Item;
            if (item != null)
            {
                return new SerializedObject(Deserialize)
                {
                    OriginalType = typeof(Sitecore.Data.Items.Item),
                    SerializedContent = item.Uri.ToString()
                };
            }
            return null;
        }

        public object Deserialize(object serialized)
        {
            var itemuri = ItemUri.Parse(serialized.ToString());
           return Sitecore.Data.Database.GetItem(itemuri);
        }




        public Type Type
        {
            get { return typeof(Sitecore.Data.Items.Item); }
        }
    }
}
