using MITD.Fuel.Domain.Model.DomainObjects.Factories;
using MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.OffhireStates
{
    public class SubmitRejectedState : OffhireState
    {
        public SubmitRejectedState(
            IOffhireStateFactory offhireStateFactory,
            IApprovableOffhireDomainService approvableDomainService)
            : base(offhireStateFactory, States.SubmitRejected, approvableDomainService)
        {
        }

        public override void Approve(Offhire offhire)
        {
            ApprovableDomainService.Submit(offhire, (SubmittedState)this.OffhireStateFactory.CreateSubmittedState());
        }

        public override void Cancel(Offhire offhire)
        {
            ApprovableDomainService.Cancel(offhire, (CancelledState)this.OffhireStateFactory.CreateCancelledState());
        }
    }
}