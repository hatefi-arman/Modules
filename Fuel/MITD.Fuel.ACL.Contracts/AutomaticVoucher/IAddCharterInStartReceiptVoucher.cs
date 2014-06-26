using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;

namespace MITD.Fuel.ACL.Contracts.AutomaticVoucher
{
    public interface IAddCharterInStartReceiptVoucher : IAutomaticVoucher
    {
        void Execute( CharterIn charterIn, List<Receipt> receipts, string receiptNumber,string receiptWarehouseCode);
    }
}
