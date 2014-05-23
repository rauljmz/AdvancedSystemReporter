using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASR.Serialization
{
    public class UserSerializator : ISerializator
    {
        public SerializedObject Serialize(object thingToSerialize)
        {
            var user = (Sitecore.Security.Accounts.User)thingToSerialize;

            return new SerializedObject(o => Sitecore.Security.Accounts.User.FromName(o.ToString(), false))
            {
                OriginalType = user.GetType(),
                SerializedContent = user.Name
            };

        }


        public Type Type
        {
            get { return typeof(Sitecore.Security.Accounts.User); }
        }
    }
}
