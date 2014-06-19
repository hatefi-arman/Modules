using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class InventoryOperation
    {
        public long Id { get; private set; }

        public long InventoryOperationId { get; private set; }

        public string ActionNumber { get; private set; }

        public DateTime ActionDate { get; private set; }

        public InventoryActionType ActionType { get; private set; }

        public byte[] TimeStamp { get; set; }

        public long? FuelReportDetailId { get; set; }

        public long? CharterId { get; set; }

        public virtual Charter Charter { get; private set; }

        public virtual FuelReportDetail FuelReportDetail { get; set; }

        public InventoryOperation()
        {


        }

        public InventoryOperation(
            long inventoryOperationId,
            string actionNumber,
            DateTime actionDate,
            InventoryActionType actionType,
            long? fuelReportDetailId,
            long? charterId)
        {
            InventoryOperationId = inventoryOperationId;
            ActionNumber = actionNumber;
            ActionDate = actionDate;
            ActionType = actionType;
            FuelReportDetailId = fuelReportDetailId;
            this.CharterId = charterId;
        }
    }
}
