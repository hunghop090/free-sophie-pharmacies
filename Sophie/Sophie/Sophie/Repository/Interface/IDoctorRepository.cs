using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IDoctorRepository
    {
        Doctor CreateDoctor(Doctor item);
        Doctor RestoreDoctor(Doctor item);
        Doctor DeleteDoctor(string doctorId);
        List<Doctor> ListDoctor(int pageIndex = 0, int pageSize = 99);
        Doctor UpdateDoctor(Doctor item);
        long TotalDoctor();

        Doctor FindByIdDoctor(string doctorId);
        Doctor FindByVideoCallIdDoctor(string videoCallId);
    }
}
