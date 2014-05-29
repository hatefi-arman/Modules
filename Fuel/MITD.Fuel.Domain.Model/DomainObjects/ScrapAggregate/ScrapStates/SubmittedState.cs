using MITD.Fuel.Domain.Model.DomainObjects.Factories;
using MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.ScrapStates
{
    public class SubmittedState : ScrapState
    {
        public SubmittedState(
            IScrapStateFactory scrapStateFactory,
            IApprovableScrapDomainService approvableDomainService)
            : base(scrapStateFactory, States.Submitted, approvableDomainService)
        {
        }

        public override void Approve(Scrap scrap)
        {
            ApprovableDomainService.Close(scrap, (ClosedState)this.ScrapStateFactory.CreateClosedState());
        }

        public override void Reject(Scrap scrap)
        {
            ApprovableDomainService.RejectSubmittedState(scrap, (SubmitRejectedState)this.ScrapStateFactory.CreateSubmitRejectedState());
        }

        public override void Cancel(Scrap scrap)
        {
            ApprovableDomainService.Cancel(scrap, (CancelledState)this.ScrapStateFactory.CreateCancelledState());
        }
    }
}