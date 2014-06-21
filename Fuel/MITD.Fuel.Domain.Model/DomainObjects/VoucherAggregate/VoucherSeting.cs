using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class VoucherSeting
    {
        #region Prop

        public long Id { get; private set; }
        public long GoodId { get; private set; }
        public long CompanyId { get; private set; }
        public virtual Company Company { get; private set; }

        public virtual VoucherDetailType VoucherDetailType { get; private set; }
        public virtual List<SegmentType> SegmentTypes { get; private set; }
        public virtual List<Account> Accounts { get; private set; }

        public string VoucherCeditRefDescription { get; set; }

        public string VoucherMainRefDescription { get; set; }
        public string VoucherDebitDescription { get; set; }
        public string VoucherDebitRefDescription { get; set; }
        public string VoucherCreditDescription { get; set; }
        public string VoucherMainDescription { get; set; }
        //public virtual List<CharterWorkflowLog> ApproveWorkflows { get; private set; }
        public byte[] TimeStamp { get; set; }

        #endregion

        #region ctor

        public VoucherSeting(
            long id, long goodId,
            long companyId,
            List<Account>accounts
            , VoucherDetailType voucherDetailType,
            List<SegmentType> segmentTypes,
            string voucherMainRefDescription,
            string voucherDebitDescription,
            string voucherDebitRefDescription,
            string voucherCreditDescription,
            string voucherMainDescription,
            string voucherCeditRefDescription)
        {
            
            Id=id;
            GoodId = goodId;
            CompanyId = companyId;
            VoucherDetailType = voucherDetailType;
            Accounts = accounts;
            SegmentTypes = segmentTypes;
            VoucherMainRefDescription=voucherMainRefDescription;
            VoucherDebitDescription = voucherDebitDescription;
            VoucherDebitRefDescription = voucherDebitRefDescription;
            VoucherCreditDescription = voucherCreditDescription;
            VoucherMainDescription = voucherMainDescription;
            VoucherCeditRefDescription = voucherCeditRefDescription;
        }

        #endregion
    }
}


