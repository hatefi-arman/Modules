using System.Collections.Generic;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Integration.Inventory.Data.ReversePOCO;
using System.Linq;
using MITD.Fuel.Integration.Inventory;

namespace MITD.Fuel.ACL.StorageSpace.DomainServices
{
    public class InventoryManagementDomainService : IInventoryManagementDomainService
    {
        private readonly IGoodDomainService goodDomainService;
        private readonly ICurrencyDomainService currencyDomainService;
        private IInventoryOperationManager inventoryOperationManager;
        public InventoryManagementDomainService(IGoodDomainService goodDomainService, ICurrencyDomainService currencyDomainService)
        {
            this.goodDomainService = goodDomainService;
            this.currencyDomainService = currencyDomainService;
            this.inventoryOperationManager = new InventoryOperationManager();
        }

        public InventoryResult GetPricedIssueResult(long companyId, long operationId)
        {
            var issueTransaction = inventoryOperationManager.GetTransaction(operationId, InventoryOperationType.Issue);
            var mainCurrency = this.currencyDomainService.GetMainCurrency();
            return new InventoryResult()
                   {
                       Id = operationId,
                       Number = issueTransaction.Code.ToString(),
                       ActionType = InventoryActionType.Issue,
                       InventoryResultItems = issueTransaction.TransactionItems.Select(
                                    ti => new InventoryResultItem
                                            {
                                                Id = ti.Id,
                                                Good = this.goodDomainService.FindGood(companyId, ti.GoodId),
                                                Currency = mainCurrency, //Base Currency;
                                                Price = inventoryOperationManager.GetAveragePrice(ti.TransactionId, InventoryOperationManager.TransactionActionType.Issue, ti.GoodId, mainCurrency.Id)
                                            }).ToList()
                   };
        }

        public bool CanIssuance(long vesselInCompanyId)
        {
            //TODO : Fake implementation

            
            return true;
        }

        public bool CanRecipt(long VesselInCompanyId)
        {
            //TODO : Fake implementation

            return true;
        }

        public List<Reference> GetVesselPurchaseReceiptNumbers(long companyId, long vesselInCompanyId)
        {
            //TODO : Fake Receipt Numbers

            return new List<Reference>()
                   {
                       new Reference(){Code = "aaa111111", ReferenceId = 1, ReferenceType = ReferenceType.Inventory}, 
                       new Reference(){Code = "bbb222222", ReferenceId = 2, ReferenceType = ReferenceType.Inventory},
                   };
        }
    }
}