using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IHospitalRepository
    {
        Hospital CreateHospital(Hospital item);
        Hospital RestoreHospital(Hospital item);
        Hospital DeleteHospital(string hospitalId);
        List<Hospital> ListHospital(int pageIndex = 0, int pageSize = 99);
        Hospital UpdateHospital(Hospital item);
        long TotalHospital();

        Hospital FindByIdHospital(string hospitalId);
    }
}
