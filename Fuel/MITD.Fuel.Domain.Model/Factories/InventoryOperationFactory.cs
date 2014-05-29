using System;
using MITD.Fuel.Domain.Model.DomainObjects;

namespace MITD.Fuel.Domain.Model.Factories
{
    public class InventoryOperationFactory : IInventoryOperationFactory
    {
        public DomainObjects.InventoryOperation Create(FuelReportDetail fuelReportDetail, string actionNumber, Enums.InventoryActionType actionType, DateTime actionDate)
        {
            InventoryOperation result = new InventoryOperation(
                actionNumber,
                actionDate,
                actionType,
                fuelReportDetail.Id,
                null);

            return result;
        }
    }
}
