using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IAddressRepository
    {
        Address CreateAddress(Address item);
        Address RestoreAddress(Address item);
        Address DeleteAddress(string addressId);
        List<Address> ListAddress(int pageIndex = 0, int pageSize = 99);
        Address UpdateAddress(Address item);
        long TotalAddress();

        Address FindByIdAddress(string addressId);
        List<Address> FindByIdAccount(string accountId);
    }
}