using System.Collections.Generic;
using MITD.Fuel.Presentation.Contracts.DTOs;

namespace MITD.Fuel.Presentation.Contracts.SL.Controllers
{
    public interface IOrderController
    {
        void Add(List<CompanyDto> dtos, List<VesselDto> vesselDtos);
        void Edit(OrderDto dto, List<CompanyDto> dtos, List<VesselDto> vesselDtos);
        void ShowList();
    }
}
