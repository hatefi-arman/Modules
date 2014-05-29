using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.IDomainServices.Events.FinanceOperations;

namespace MITD.Fuel.ACL.StorageSpace.DomainServices.Events
{
    public class FinanceNotifier : IFinanceNotifier
    {
        public void NotifySubmittingOffhire(Offhire offhire)
        {

        }

        public void NotifyOffhireCancelled(Offhire offhire)
        {

        }
    }
}
