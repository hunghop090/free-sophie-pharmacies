using System;
using App.Core.Entities;
using AutoMapper;
using Sophie.Resource.Dtos;
using Sophie.Resource.Dtos.Health;
using Sophie.Resource.Entities;
using Sophie.Resource.Entities.MedicalAppointment;
using Sophie.Resource.Entities.Health;
using Sophie.Resource.Dtos.MedicalAppointment;
using Sophie.Resource.Entities.Shop;
using Sophie.Resource.Dtos.Shop;
using Sophie.Resource.Model;

namespace App.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDto>();
            CreateMap<AccountDto, Account>();
            CreateMap<Account, AccountRegistryDto>();
            CreateMap<AccountRegistryDto, Account>();

            CreateMap<ApplicationDevice, DeviceDto>();
            CreateMap<DeviceDto, ApplicationDevice>();

            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>();

            CreateMap<Hospital, HospitalDto>();
            CreateMap<HospitalDto, Hospital>();

            CreateMap<Doctor, DoctorDto>();
            CreateMap<DoctorDto, Doctor>();

            CreateMap<MedicalAppointment, MedicalAppointmentDto>();
            CreateMap<MedicalAppointmentDto, MedicalAppointment>();

            CreateMap<MedicalAppointmentByPhoneDto, MedicalAppointment>();
            CreateMap<MedicalAppointment, MedicalAppointmentByPhoneDto>();
            CreateMap<MedicalAppointmentByHomeDto, MedicalAppointment>();
            CreateMap<MedicalAppointment, MedicalAppointmentByHomeDto>();
            CreateMap<MedicalAppointmentByHospitalDto, MedicalAppointment>();
            CreateMap<MedicalAppointment, MedicalAppointmentByHospitalDto>();
            CreateMap<MedicalAppointmentByFastPhoneDto, MedicalAppointment>();
            CreateMap<MedicalAppointment, MedicalAppointmentByFastPhoneDto>();

            CreateMap<HomeServiceBabyBathDto, MedicalAppointment>();
            CreateMap<HomeServiceBabyBathDto, MedicalAppointmentDto>();
            CreateMap<HomeServiceCareSickDto, MedicalAppointment>();
            CreateMap<HomeServiceCareSickDto, MedicalAppointmentDto>();
            CreateMap<HomeServiceBloodCollectionDto, MedicalAppointment>();
            CreateMap<HomeServiceBloodCollectionDto, MedicalAppointmentDto>();
            CreateMap<HomeServicePhysiotherapyDto, MedicalAppointment>();
            CreateMap<HomeServicePhysiotherapyDto, MedicalAppointmentDto>();

            CreateMap<News, NewsDto>();
            CreateMap<NewsDto, News>();

            CreateMap<Notification, NotificationDto>();
            CreateMap<NotificationDto, Notification>();

            CreateMap<HealthBloodPressure, HealthBloodPressureDto>();
            CreateMap<HealthBloodPressureDto, HealthBloodPressure>();
            CreateMap<HealthBloodSugar, HealthBloodSugarDto>();
            CreateMap<HealthBloodSugarDto, HealthBloodSugar>();
            CreateMap<HealthSpO2, HealthSpO2Dto>();
            CreateMap<HealthSpO2Dto, HealthSpO2>();
            CreateMap<HealthWeight, HealthWeightDto>();
            CreateMap<HealthWeightDto, HealthWeight>();
            CreateMap<HealthMenstrualCycle, HealthMenstrualCycleDto>();
            CreateMap<HealthMenstrualCycleDto, HealthMenstrualCycle>();
            CreateMap<HealthHBA1C, HealthHBA1CDto>();
            CreateMap<HealthHBA1CDto, HealthHBA1C>();

            CreateMap<Info, InfoDto>();
            CreateMap<InfoDto, Info>();

            CreateMap<Pharmacist, PharmacistDto>();
            CreateMap<PharmacistDto, Pharmacist>();
            CreateMap<Analysis, AnalysisDto>();
            CreateMap<AnalysisDto, Analysis>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<Product, ProductInListDto>();

            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();
            CreateMap<Order, DraftOrder>();
            CreateMap<DraftOrder, Order>();
            CreateMap<Order, OrderInListDto>();
            CreateMap<DraftOrderCreateOrUpdateDto, DraftOrder>();
            CreateMap<OrderGroupBy, OrderGroupByDto>();
            CreateMap<DraftOrder, DraftOrderDto>();
            CreateMap<DraftOrderSaveDto, DraftOrderDto>();

            CreateMap<Promotion, PromotionDto>();
            CreateMap<PromotionDto, Promotion>();
            CreateMap<Promotion, PromotionInListDto>();
            CreateMap<ProductOrder, ProductOrderDto>();

            CreateMap<TransportPromotion, TransportPromotionDto>();
            CreateMap<TransportPromotionDto, TransportPromotion>();
            CreateMap<TransportPromotion, TransportPromotionInListDto>();
        }
    }
}
