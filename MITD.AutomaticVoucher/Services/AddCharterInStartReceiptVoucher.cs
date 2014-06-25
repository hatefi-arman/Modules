using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Repository;
using MITD.Fuel.ACL.Contracts.AutomaticVoucher;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.AutomaticVoucher.Services
{
    public class AddCharterInStartReceiptVoucher : IAddCharterInStartReceiptVoucher
    {

        private readonly IVoucherRepository _voucherRepository;
        private IUnitOfWorkScope _unitOfWorkScope;

        public AddCharterInStartReceiptVoucher(IVoucherRepository voucherRepository,IUnitOfWorkScope unitOfWorkScope)
        {
            _voucherRepository = voucherRepository;
            _unitOfWorkScope = unitOfWorkScope;
        }
        
        public void Execute(
            CharterIn charterIn, List<Receipt> receipts, string receiptNumber, string receiptWarehouseCode
            )
        {

            VoucherSeting voucherSeting=new VoucherSeting();

            var voucher = new Voucher();
            voucher.LocalVoucherDate()
                .FinancialVoucherDate(new DateTime())
                .Description(voucherSeting.VoucherMainDescription)
                .ReferenceNo(receiptNumber)
                .LocalVoucherNo("01")
                .VoucherRef(Int64.Parse(voucherSeting.VoucherMainRefDescription))
                .SetReferenceType(ReferenceType.CharterIn)
                .SetCurrency(charterIn.Currency);


            receipts.ForEach(c =>
                             {
                                 var debiJournalEntry = new JournalEntry();
                                 debiJournalEntry.IrrAmount(c.ReceiptQuantity * c.ReceiptFee * c.Coefficient)
                                     .VoucherRef(voucherSeting.VoucherDebitRefDescription)
                                     .Description(voucherSeting.VoucherDebitDescription)
                                     .AccountNo(voucherSeting.DebitAccounts[0].Code)
                                     .ForeignAmount(c.ReceiptFee * c.ReceiptQuantity)
                                     .SetSegmentType(SegmentType.Vessel)
                                     .SetSegment(segment =>
                                     {
                                         if (segment.SegmentType.Id == 1)
                                         {
                                             segment.Code = receiptWarehouseCode;
                                         }
                                         else if (segment.SegmentType.Id == 4)
                                         {
                                             segment.Code = charterIn.Owner.Code;
                                         }
                                     }, debiJournalEntry.Segment);

                                 voucher.JournalEntrieses.Add(debiJournalEntry);


                                var creditJournalEntry = new JournalEntry();
                                 creditJournalEntry.IrrAmount(c.ReceiptQuantity * c.ReceiptFee * c.Coefficient)
                                     .VoucherRef(voucherSeting.VoucherCeditRefDescription)
                                     .Description(voucherSeting.VoucherCreditDescription)
                                     .AccountNo( voucherSeting.CreditAccounts[0].Code)
                                     .ForeignAmount(c.ReceiptFee * c.ReceiptQuantity)
                                     .SetSegmentType( SegmentType.Company)
                                     .SetSegment(segment =>
                                     {
                                         if (segment.SegmentType.Id == 1)
                                         {
                                             segment.Code = receiptWarehouseCode;
                                         }
                                         else if (segment.SegmentType.Id == 4)
                                         {
                                             segment.Code = charterIn.Owner.Code;
                                         }
                                     }, creditJournalEntry.Segment);

                                 voucher.JournalEntrieses.Add(creditJournalEntry);


                             });

            
            
            _voucherRepository.Add(voucher);
            _unitOfWorkScope.Commit();



        }




    }
}
