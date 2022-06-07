using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using App.Core;
using App.Core.Constants;
using App.Core.Middleware.IPAddressTools;
using App.Core.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sophie.Repository.Interface;
using Sophie.Units;

namespace Sophie.Areas.Admin.Pages.Dashboard
{
    [Authorize(Roles = RolePrefix.AdminSys + "," + RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Manager)]
    //[Authorize(Roles = RolePrefix.Admin + "," + RolePrefix.Developer + "," + RolePrefix.Moderator)]
    //[Authorize(Policy = "RequireAdministratorRoleForCMS")]
    public class IndexModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMedicalAppointmentRepository _medicalAppointmentRepository;
        private readonly ApplicationSettings _applicationSettings;// Inject Data

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public string MachineName { get; set; }

        [BindProperty]
        public string MachineIP { get; set; } = "Not found";

        [BindProperty]
        public string MacAddress { get; set; }

        [BindProperty]
        public long TotalAccount { get; set; }

        [BindProperty]
        public long TotalHospital { get; set; }

        [BindProperty]
        public long TotalDoctor { get; set; }

        [BindProperty]
        public long TotalMedicalAppointment { get; set; }

        public IndexModel(IMapper mapper, IAccountRepository accountRepository, IHospitalRepository hospitalRepository, IDoctorRepository doctorRepository, IMedicalAppointmentRepository medicalAppointmentRepository,
            IOptions<ApplicationSettings> applicationSettingsAccessor)
        {
            _mapper = mapper;
            _accountRepository = accountRepository;
            _hospitalRepository = hospitalRepository;
            _doctorRepository = doctorRepository;
            _medicalAppointmentRepository = medicalAppointmentRepository;
            _applicationSettings = applicationSettingsAccessor.Value;
        }

        public Task OnGet()
        {
            TotalAccount = _accountRepository.TotalAccount();
            TotalHospital = _hospitalRepository.TotalHospital();
            TotalDoctor = _doctorRepository.TotalDoctor();
            TotalMedicalAppointment = _medicalAppointmentRepository.TotalMedicalAppointment();

            MachineName = SystemExtensions.GetMachineName();
            //IPAddress remoteIpAddress = IPAddressFinder.Find(this.HttpContext, _applicationSettings, true);
            //if (remoteIpAddress != null)
            //{
            //    // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
            //    // This usually only happens when the browser is on the same machine as the server.
            //    if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            //    {
            //        remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            //    }
            //    MachineIP = remoteIpAddress.ToString();
            //}
            MachineIP = SystemExtensions.GetLocalIPv4();
            MacAddress = SystemExtensions.GetMacAddress();
            return Task.CompletedTask;
        }

        public async Task<JsonResult> OnPostCpuUsageForProcess()
        {
            var startTime = DateTimes.Now();
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500);

            var endTime = DateTimes.Now();
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            var cpuPercent = cpuUsageTotal * 100;
            //Logs.debug($"CPU { cpuPercent }");
            cpuPercent = (cpuPercent * 10) >= 100 ? 100 : (cpuPercent * 10);
            getCurrentCpuUsage();
            return new JsonResult(new { message = "Success", data = cpuPercent }) { StatusCode = 200 };
        }

        private void getCurrentCpuUsage()
        {
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            var cpu = proc.TotalProcessorTime;
            //Logs.debug($"My process used working set {mem / 1024.0} K of working set and CPU {cpu.TotalMilliseconds} msec. {(mem / 1024.0) / cpu.TotalMilliseconds}");
        }


    }
}
