using System.Collections;
using ASR.Serialization;
using Sitecore.Web.Authentication;

namespace ASR.Reports.Sessions
{
    public class SessionsScanner : ASR.Interface.BaseScanner
    {
        public SessionsScanner()
        {
            SerializatorsFactory.RegisterSerializator(new SessionSerializator());
        }
        public override ICollection Scan()
        {
            return DomainAccessGuard.Sessions;
        }
    }

    public class SessionSerializator : ISerializator
    {
        public SerializedObject Serialize(object thingToSerialize)
        {
            var session = (Sitecore.Web.Authentication.DomainAccessGuard.Session)thingToSerialize;
            return new SerializedObject(o=> DomainAccessGuard.Sessions.Find(s=> s.SessionID == o.ToString()))
            {
                SerializedContent = session.SessionID,
                OriginalType = session.GetType()
            };
        }

        public System.Type Type
        {
            get {return  typeof(Sitecore.Web.Authentication.DomainAccessGuard.Session); }
        }
    }

}
