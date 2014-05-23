using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.DisplayItems
{
    [Serializable]
	public class ItemWorkflowEvent : WorkflowEvent
	{
		public ItemWorkflowEvent(string oldState, string newState, string text, string user, DateTime date)
			: base(oldState, newState, text, user, date) { }

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
	}
}
