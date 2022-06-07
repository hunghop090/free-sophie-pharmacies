using System;

namespace Sophie.Resource.Model
{
    public class ConfigurationsModel
    {
        public HealthSettingModel HealthSetting { get; set; }
        public PaymentSettingModel PaymentSetting { get; set; }
        public MomoSettingModel MomoSetting { get; set; }
        public ZaloPaySettingModel ZaloPaySetting { get; set; }
        public ShopeePaySettingModel ShopeePaySetting { get; set; }
    }

    public class HealthSettingModel
    {
        // BloodPressure (Huyết áp) Systolic(tâm thu)/Diastole(tâm trương)
        public double MinBasicBloodPressureSystolic { get; set; }
        public double MaxBasicBloodPressureSystolic { get; set; }
        public double MinBasicBloodPressureDiastole { get; set; }
        public double MaxBasicBloodPressureDiastole { get; set; }

        // BloodSugar (Đường huyết)
        public double MinBasicBloodSugarNormal { get; set; }
        public double MaxBasicBloodSugarNormal { get; set; }
        public double MinBasicBloodSugarHungry { get; set; }
        public double MaxBasicBloodSugarHungry { get; set; }
        public double MinBasicBloodSugarAfterMeal { get; set; }
        public double MaxBasicBloodSugarAfterMeal { get; set; }
        public double MinBasicBloodSugarAfterTestHBA1C { get; set; }
        public double MaxBasicBloodSugarAfterTestHBA1C { get; set; }

        // HBA1C (Chỉ số đái tháo đường)
        public double MinBasicHBA1C { get; set; }
        public double MaxBasicHBA1C { get; set; }

        // MenstrualCycle (Chu kỳ kinh nguyệt)
        public double MinBasicMenstrualCycle { get; set; }
        public double MaxBasicMenstrualCycle { get; set; }
        public double MinBasicMenstrualCycleDelay { get; set; }
        public double MaxBasicMenstrualCycleDelay { get; set; }

        // SpO2 (Nồng độ O2 trong máu)
        public double MinBasicSpO2 { get; set; }
        public double MaxBasicSpO2 { get; set; }

        // Weight (Cân nặng)
        public double MinBasicWeight { get; set; }
        public double MaxBasicWeight { get; set; }
    }

    public class PaymentSettingModel
    {
        public string AmountMedicalAppointment { get; set; }
        public string NotificationExpriedRead { get; set; }
        public string NotificationExpriedUnRead { get; set; }
    }

    public class MomoSettingModel
    {
        public bool IsProduction { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantNameLabel { get; set; }
        public string Fee { get; set; }
        public string Description { get; set; }

        public string IOSSchemeId { get; set; }
        public string PartnerCode { get; set; }

        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
    }

    public class ZaloPaySettingModel
    {
        public bool IsProduction { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Appid { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Fee { get; set; }
    }

    public class ShopeePaySettingModel
    {
        public bool IsProduction { get; set; }
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public string Fee { get; set; }
    }
}
