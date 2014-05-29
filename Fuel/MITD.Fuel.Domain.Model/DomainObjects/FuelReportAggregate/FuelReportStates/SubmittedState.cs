using MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.FuelReportStates
{
    public class SubmittedState : FuelReportState
    {
        public SubmittedState(IFuelReportStateFactory fuelReportStateFactory)
            : base(fuelReportStateFactory, States.Submitted)
        {
        }


        public override void Approve(FuelReport fuelReport, IApprovableFuelReportDomainService approvableFuelReportDomainService)
        {
            fuelReport.Close(this.FuelReportStateFactory.CreateClosedState());
        }

        public override void Reject(FuelReport fuelReport)
        {
            fuelReport.SubmittedReject(this.FuelReportStateFactory.CreateSubmitRejectedState());
        }
    }
}