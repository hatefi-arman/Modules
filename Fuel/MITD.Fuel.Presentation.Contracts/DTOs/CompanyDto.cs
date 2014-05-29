using System.Collections.Generic;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Contracts.DTOs
{
    public partial class CompanyDto
    {
        long id;
        public long Id
        {
            get { return id; }
            set { this.SetField(p => p.Id, ref id, value); }
        }

        string code;
        public string Code
        {
            get { return code; }
            set { this.SetField(p => p.Code, ref code, value); }
        }

        string name;
        public string Name
        {
            get { return name; }
            set { this.SetField(p => p.Name, ref name, value); }
        }

        List<VesselDto> _vessels;
        public List<VesselDto> Vessels
        {
            get { return _vessels; }
            set { this.SetField(p => p.Vessels, ref _vessels, value); }
        }

        public CompanyDto()
        {
            _vessels = new List<VesselDto>();
        }
    }
}