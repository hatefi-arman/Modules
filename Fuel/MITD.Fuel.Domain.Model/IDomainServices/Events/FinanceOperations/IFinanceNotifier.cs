using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;

namespace MITD.Fuel.Domain.Model.IDomainServices.Events.FinanceOperations
{
    public interface IFinanceNotifier : IEventNotifier
    {
        void NotifySubmittingOffhire(Offhire offhire);

        void NotifyOffhireCancelled(Offhire offhire);
    }
}