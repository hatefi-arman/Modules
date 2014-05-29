﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Model;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface IInventoryManagementDomainService : IDomainService
    {
        InventoryResult GetPricedIssueResult(string issueNumber);
        bool CanIssuance(long VesselInCompanyId);
        bool CanRecipt(long VesselInCompanyId);
        List<Reference> GetVesselPurchaseReceiptNumbers(long companyId, long vesselInCompanyId);
    }
}
