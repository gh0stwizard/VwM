using VwM.Database.Models;

namespace VwM.Database.Collections
{
    public sealed class DeviceCollection : CollectionBase<Device>
    {
        public DeviceCollection(IDatabaseSettings dbSettings) : base(dbSettings) { }
    }
}
