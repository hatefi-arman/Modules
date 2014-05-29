using MITD.Fuel.Presentation.Contracts.DTOs;

namespace MITD.Fuel.Presentation.Contracts.SL.Controllers
{
    public interface IFuelReportVesselController
    {
        void Add();
        void Edit(VesselDto dto);
        void ShowList();
    }
}
