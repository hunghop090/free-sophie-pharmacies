using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.MedicalAppointment;

namespace Sophie.Repository.Interface
{
    public interface IMedicalAppointmentRepository
    {
        MedicalAppointment CreateMedicalAppointment(MedicalAppointment item);
        MedicalAppointment RestoreMedicalAppointment(MedicalAppointment item);
        MedicalAppointment DeleteMedicalAppointment(string medicalAppointmentId);
        List<MedicalAppointment> ListMedicalAppointment(int pageIndex = 0, int pageSize = 99);
        MedicalAppointment UpdateMedicalAppointment(MedicalAppointment item);
        long TotalMedicalAppointment();

        MedicalAppointment FindByIdMedicalAppointment(string medicalAppointmentId);
        List<MedicalAppointment> FindByIdAccount(string accountId);
    }
}
