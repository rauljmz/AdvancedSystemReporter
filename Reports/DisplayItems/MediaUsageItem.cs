using Sitecore.Data.Items;

namespace ASR.Reports.DisplayItems
{
	public class MediaUsageItem
	{
        private Serialization.SerializedObject serializedItem;
        public Item Item
        {
            get
            {
                return serializedItem.Deserialize() as Item;
            }
            set
            {
                serializedItem = Serialization.SerializatorsFactory.Serialize(value) as Serialization.SerializedObject;
            }
        }

        private Serialization.SerializedObject serializedMedia;
        public Item Media
        {
            get
            {
                return serializedItem.Deserialize() as Item;
            }
            set
            {
                serializedItem = Serialization.SerializatorsFactory.Serialize(value) as Serialization.SerializedObject;
            }
        }

		public MediaUsageItem(Item item, Item media)
		{
			Item = item;
			Media = media;
		}
	}
}
