using MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.Factories;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.FuelReportStates
{
    public class SubmitRejectedState : FuelReportState
    {
        public SubmitRejectedState(IFuelReportStateFactory fuelReportStateFactory)
            : base(fuelReportStateFactory, States.SubmitRejected)
        {

        }

        public override void Approve(FuelReport fuelReport, IApprovableFuelReportDomainService approveService)
        {
            approveService.Submit(fuelReport, this.FuelReportStateFactory.CreateSubmitState());
        }

        public override void Cancel(FuelReport fuelReport)
        {
            fuelReport.Invalidate(this.FuelReportStateFactory.CreateInvalidState());
        }
    }
}