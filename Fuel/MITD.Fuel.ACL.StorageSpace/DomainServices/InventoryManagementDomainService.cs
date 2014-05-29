using System.Collections.Generic;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.ACL.StorageSpace.DomainServices
{
    public class InventoryManagementDomainService : IInventoryManagementDomainService
    {
        private readonly IGoodDomainService goodDomainService;
        private readonly ICurrencyDomainService currencyDomainService;

        public InventoryManagementDomainService(IGoodDomainService goodDomainService, ICurrencyDomainService currencyDomainService)
        {
            this.goodDomainService = goodDomainService;
            this.currencyDomainService = currencyDomainService;
        }

        public InventoryResult GetPricedIssueResult(string issueNumber)
        {
            return new InventoryResult()
                   {
                       Id = 1,
                       Number = issueNumber,
                       InventoryResultItems = new List<InventoryResultItem>(
                           new[]
                           {
                               new InventoryResultItem()
                               {
                                   Id = 1, 
                                   Good = this.goodDomainService.Get(1), 
                                   Currency = this.currencyDomainService.Get(1), 
                                   Price = new decimal(324.46)
                               },
                               new InventoryResultItem()
                               {
                                   Id = 2, 
                                   Good = this.goodDomainService.Get(2), 
                                   Currency = this.currencyDomainService.Get(1), 
                                   Price = new decimal(745.26)
                               }
                           }
                        )
                   };
        }

        public bool CanIssuance(long VesselInCompanyId)
        {
            return true;
        }

        public bool CanRecipt(long VesselInCompanyId)
        {
            return true;
        }

        public List<Reference> GetVesselPurchaseReceiptNumbers(long companyId, long vesselInCompanyId)
        {
            return new List<Reference>()
                   {
                       new Reference(){Code = "aaa111111", ReferenceId = 1, ReferenceType = ReferenceType.Inventory}, 
                       new Reference(){Code = "bbb222222", ReferenceId = 2, ReferenceType = ReferenceType.Inventory},
                   };
        }
    }
}