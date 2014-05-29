using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.ScrapStates;
using ClosedState = MITD.Fuel.Domain.Model.DomainObjects.ScrapStates.ClosedState;
using SubmitRejectedState = MITD.Fuel.Domain.Model.DomainObjects.ScrapStates.SubmitRejectedState;
using SubmittedState = MITD.Fuel.Domain.Model.DomainObjects.ScrapStates.SubmittedState;

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface IApprovableScrapDomainService : IApprovableDomainService
    {
        void ValidateMiddleApprove(Scrap scrap);
        void Submit(Scrap scrap, SubmittedState submittedState);
        void Cancel(Scrap scrap, CancelledState cancelledState);
        void RejectSubmittedState(Scrap scrap, SubmitRejectedState submitRejectedState);
        void Close(Scrap scrap, ClosedState closedState);
    }
}