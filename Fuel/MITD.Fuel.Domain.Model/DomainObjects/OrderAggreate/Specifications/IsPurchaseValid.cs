#region

using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;

#endregion

namespace MITD.Fuel.Domain.Model.Specifications
{
    //BR_PO3
    public class IsPurchaseValid : SpecificationBase<Order>
    {
        public IsPurchaseValid()
            : base(
                order => order.OrderType == OrderTypes.Purchase &&
                         !order.TransporterId.HasValue &&
                         !order.ReceiverId.HasValue &&
                         order.OwnerId > 0 &&
                         order.SupplierId.HasValue && order.SupplierId > 0 &&
                         !order.FromVesselInCompanyId.HasValue &&
                         !order.ToVesselInCompanyId.HasValue)
        {
        }
    }
}