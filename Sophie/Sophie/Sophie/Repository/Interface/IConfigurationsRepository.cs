using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IConfigurationsRepository
    {
        Configurations CreateConfigurations(Configurations item);
        Configurations RestoreConfigurations(Configurations item);
        Configurations UpdateConfigurations(Configurations item);
        List<Configurations> ListConfigurations(int pageIndex = 0, int pageSize = 99);
        Configurations DeleteConfigurations(string configurationsId);
    }
}
