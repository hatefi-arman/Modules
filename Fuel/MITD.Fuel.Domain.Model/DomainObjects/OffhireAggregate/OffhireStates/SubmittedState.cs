using MITD.Fuel.Domain.Model.DomainObjects.Factories;
using MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.OffhireStates
{
    public class SubmittedState : OffhireState
    {
        public SubmittedState(
            IOffhireStateFactory offhireStateFactory,
            IApprovableOffhireDomainService approvableDomainService)
            : base(offhireStateFactory, States.Submitted, approvableDomainService)
        {
        }

        public override void Approve(Offhire offhire)
        {
            ApprovableDomainService.Close(offhire, (ClosedState)this.OffhireStateFactory.CreateClosedState());
        }

        public override void Reject(Offhire offhire)
        {
            ApprovableDomainService.RejectSubmittedState(offhire, (SubmitRejectedState)this.OffhireStateFactory.CreateSubmitRejectedState());
        }

        public override void Cancel(Offhire offhire)
        {
            ApprovableDomainService.Cancel(offhire, (CancelledState)this.OffhireStateFactory.CreateCancelledState());
        }
    }
}