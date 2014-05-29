using MITD.Fuel.Domain.Model.DomainObjects.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.ScrapStates
{
    public class OpenState : ScrapState
    {
        public OpenState(
            IScrapStateFactory scrapStateFactory,
            IApprovableScrapDomainService approvableDomainService)
            : base(scrapStateFactory, States.Open, approvableDomainService)
        {
        }

        public override void Approve(Scrap scrap)
        {
            ApprovableDomainService.Submit(scrap, (SubmittedState)this.ScrapStateFactory.CreateSubmittedState());
        }

        public override void Cancel(Scrap scrap)
        {
            ApprovableDomainService.Cancel(scrap, (CancelledState)this.ScrapStateFactory.CreateCancelledState());
        }
    }
}