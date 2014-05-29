using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.IDomainServices
{
   public interface ICharterOutDomainService:IDomainService
    {
        bool ExistCharterOutHeader(long charterId);
        bool HasCharterItem(long charterId);
        bool HasCharterEnd(long charterId);
        CharterOut GetCharterStartDate(long vesselInCompanyId, long ownerId);
        CharterOut GetCharterEnd(long charterstartId);
       bool CheckNextCharterStartDate(long vesselInCompanyId, long ownerId, DateTime actionDate);
       States GetCharterState(long id);
       bool HasVesselCharterOutStart(long vesselInCompanyId);
       bool HasVesselCharterOutStart(long vesselInCompanyId, long ownerId);
       bool HasVesselCharterOutEnd(long vesselInCompanyId);
       bool IsLastCharter(long vesselInCompanyId, long id);
       CharterOut GetDateEndLast(long vesselInCompanyId);
       bool IsCharterStartDateGreaterThanPrevCharterEndDate(long vesselInCompanyId, DateTime dateTime);
       CharterOut GetCharterOutStart(long vesselInCompanyId, long ownerId, DateTime charterInStartDateTime);
       CharterOut GetCharterInStart(long Id);
       States GetCharterStartState(long vesselInCompanyId, long ownerId);
       void DeleteCharterItem(long id);
    }
}
