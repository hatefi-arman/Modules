using MITD.Fuel.Domain.Model.DomainObjects.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.OffhireStates
{
    public class OpenState : OffhireState
    {
        public OpenState(
            IOffhireStateFactory offhireStateFactory,
            IApprovableOffhireDomainService approvableDomainService)
            : base(offhireStateFactory, States.Open, approvableDomainService)
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