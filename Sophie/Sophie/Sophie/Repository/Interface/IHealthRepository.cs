using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.Health;

namespace Sophie.Repository.Interface
{
    public interface IHealthRepository
    {
        // BloodPressure (Huyết áp)
        HealthBloodPressure CreateHealthBloodPressure(HealthBloodPressure item);
        HealthBloodPressure RestoreHealthBloodPressure(HealthBloodPressure item);
        HealthBloodPressure DeleteHealthBloodPressure(string healthId);
        HealthBloodPressure UpdateHealthBloodPressure(HealthBloodPressure item);
        long TotalHealthBloodPressure();
        HealthBloodPressure FindByIdHealthBloodPressure(string healthId);
        List<HealthBloodPressure> ListHealthBloodPressure(int? year, int pageIndex = 0, int pageSize = 99);
        List<HealthBloodPressure> FindByIdAccountForHealthBloodPressure(int? year, string accountId);

        // BloodSugar (Đường huyết)
        HealthBloodSugar CreateHealthBloodSugar(HealthBloodSugar item);
        HealthBloodSugar RestoreHealthBloodSugar(HealthBloodSugar item);
        HealthBloodSugar DeleteHealthBloodSugar(string healthId);
        HealthBloodSugar UpdateHealthBloodSugar(HealthBloodSugar item);
        long TotalHealthBloodSugar();
        HealthBloodSugar FindByIdHealthBloodSugar(string healthId);
        List<HealthBloodSugar> ListHealthBloodSugar(int? year, int pageIndex = 0, int pageSize = 99);
        List<HealthBloodSugar> FindByIdAccountForHealthBloodSugar(int? year, string accountId);

        // SpO2 (Nồng độ O2 trong máu)
        HealthSpO2 CreateHealthSpO2(HealthSpO2 item);
        HealthSpO2 RestoreHealthSpO2(HealthSpO2 item);
        HealthSpO2 DeleteHealthSpO2(string healthId);
        HealthSpO2 UpdateHealthSpO2(HealthSpO2 item);
        long TotalHealthSpO2();
        HealthSpO2 FindByIdHealthSpO2(string healthId);
        List<HealthSpO2> ListHealthSpO2(int? year, int pageIndex = 0, int pageSize = 99);
        List<HealthSpO2> FindByIdAccountForHealthSpO2(int? year, string accountId);

        // Weight (Cân nặng)
        HealthWeight CreateHealthWeight(HealthWeight item);
        HealthWeight RestoreHealthWeight(HealthWeight item);
        HealthWeight DeleteHealthWeight(string healthId);
        HealthWeight UpdateHealthWeight(HealthWeight item);
        long TotalHealthWeight();
        HealthWeight FindByIdHealthWeight(string healthId);
        List<HealthWeight> ListHealthWeight(int? year, int pageIndex = 0, int pageSize = 99);
        List<HealthWeight> FindByIdAccountForHealthWeight(int? year, string accountId);

        // MenstrualCycle (Chu kỳ kinh nguyệt)
        HealthMenstrualCycle CreateHealthMenstrualCycle(HealthMenstrualCycle item);
        HealthMenstrualCycle RestoreHealthMenstrualCycle(HealthMenstrualCycle item);
        HealthMenstrualCycle DeleteHealthMenstrualCycle(string healthId);
        HealthMenstrualCycle UpdateHealthMenstrualCycle(HealthMenstrualCycle item);
        long TotalHealthMenstrualCycle();
        HealthMenstrualCycle FindByIdHealthMenstrualCycle(string healthId);
        //List<HealthMenstrualCycle> ListHealthMenstrualCycle(int? year, int pageIndex = 0, int pageSize = 99);
        HealthMenstrualCycle ListHealthMenstrualCycle(int? year, int pageIndex = 0, int pageSize = 99);
        //List<HealthMenstrualCycle> FindByIdAccountForHealthMenstrualCycle(int? year, string accountId);
        HealthMenstrualCycle FindByIdAccountForHealthMenstrualCycle(int? year, string accountId);

        // HBA1C (Chỉ số đái tháo đường)
        HealthHBA1C CreateHealthHBA1C(HealthHBA1C item);
        HealthHBA1C RestoreHealthHBA1C(HealthHBA1C item);
        HealthHBA1C DeleteHealthHBA1C(string healthId);
        HealthHBA1C UpdateHealthHBA1C(HealthHBA1C item);
        long TotalHealthHBA1C();
        HealthHBA1C FindByIdHealthHBA1C(string healthId);
        List<HealthHBA1C> ListHealthHBA1C(int? year, int pageIndex = 0, int pageSize = 99);
        List<HealthHBA1C> FindByIdAccountForHealthHBA1C(int? year, string accountId);

        // ...

    }
}
