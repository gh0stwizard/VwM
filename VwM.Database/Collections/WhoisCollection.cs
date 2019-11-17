using VwM.Database.Models;

namespace VwM.Database.Collections
{
    public class WhoisCollection : CollectionBase<Whois>
    {
        public WhoisCollection(IDatabaseSettings dbSettings) : base(dbSettings) { }
    }
}
