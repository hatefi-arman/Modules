using MITD.Fuel.Domain.Model.DomainObjects.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
namespace MITD.Fuel.Domain.Model.DomainObjects.ScrapStates
{
    public class ClosedState : ScrapState
    {
        public ClosedState(
            IScrapStateFactory scrapStateFactory,
            IApprovableScrapDomainService approvableDomainService)
            : base(scrapStateFactory, States.Closed, approvableDomainService)
        {

        }
    }
}