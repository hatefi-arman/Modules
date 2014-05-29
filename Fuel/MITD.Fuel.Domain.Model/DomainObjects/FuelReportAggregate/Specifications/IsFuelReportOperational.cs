using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.Specifications
{
    public class IsFuelReportOperational : SpecificationBase<FuelReport>
    {
        public IsFuelReportOperational()
            : base(
                fr =>
                    (fr.Voyage == null || fr.Voyage.IsActive) &&
                    (fr.State != States.Cancelled && fr.State != States.Closed))
        {

        }
    }
}
