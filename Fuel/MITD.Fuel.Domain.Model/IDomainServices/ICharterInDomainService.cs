﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.IDomainServices
{
   public interface ICharterInDomainService:IDomainService
   {
       bool ExistCharterInHeader(long charterId);
       bool HasCharterItem(long charterId);
       bool HasCharterEnd(long charterId);
       DateTime GetCharterStartDate(long vesselInCompanyId, long chartereId);
       States GetCharterStartState(long vesselInCompanyId, long chartereId);
       CharterIn GetCharterEnd(long charterstartId);
       States GetCharterState(long id);
       bool HasVesselCharterInStart(long vesselInCompanyId);
       bool HasVesselCharterInStart(long vesselInCompanyId, long chatererId);
       bool HasVesselCharterInEnd(long vesselInCompanyId);
       bool IsLastCharter(long vesselInCompanyId, long id);
       CharterIn GetCharterInPrevCharterOut(long vesselInCompanyId, DateTime dateCharterOutStart);
       bool IsCharterStartDateGreaterThanPrevCharterEndDate(long vesselInCompanyId, DateTime dateTime);
       CharterIn GetCharterInStart(long vesselInCompanyId, long ownerId, DateTime charterOutStartDateTime);
       CharterIn GetCharterInStart(long Id);
       void DeleteCharterItem(long id);
   }
}
