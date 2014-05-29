using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Integration.VesselReportManagementSystem.Utility;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation.Contracts;

namespace MITD.Fuel.Integration.VesselReportManagementSystem.ServiceWrapper
{
    public class VesselReportServiceWrapper
    {
        private string vesselReportAddressController;
      
        public VesselReportServiceWrapper()
        {

            vesselReportAddressController = ConfigurationManager.AppSettings["FuelApi"] + "apiarea/Fuel/FuelReport";
  
        }

        public void Add(Action<ResultFuelReportDto, Exception> action, FuelReportCommandDto fuelReportCommandDto)
        {
            var uri = vesselReportAddressController;
            WebClientHelper.Post<ResultFuelReportDto, FuelReportCommandDto>(new Uri(uri, UriKind.Absolute), action, fuelReportCommandDto, WebClientHelper.MessageFormat.Json);
        }

       

    }
}
