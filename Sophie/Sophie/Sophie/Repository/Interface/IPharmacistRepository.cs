using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IPharmacistRepository
    {
        Pharmacist CreatePharmacist(Pharmacist item);
        Pharmacist DeletePharmacist(string pharmacistId);
        Pharmacist FindByIdPharmacist(string pharmacistId);
        Pharmacist FindByVideoCallIdPharmacist(string videoCallId);
        List<Pharmacist> ListPharmacist(int pageIndex = 0, int pageSize = 99);
        Pharmacist RestorePharmacist(Pharmacist item);
        long TotalPharmacist();
        Pharmacist UpdatePharmacist(Pharmacist item);
        Pharmacist FindByEmailPharmacist(string email);
    }
}
