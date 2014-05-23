using System.Collections;
using System.Linq;
using ASR.Serialization;


namespace ASR.Reports.Users
{
    public class AllUsersScanner : ASR.Interface.BaseScanner
    {
        public string Domain { get; set; }

        public AllUsersScanner()
        {
            Serialization.SerializatorsFactory.RegisterSerializator(new UserSerializator());
        }
        public override ICollection Scan()
        {
            return Sitecore.Security.Domains.Domain.GetDomain(Domain).GetUsers().ToArray();
        }
    }
}
