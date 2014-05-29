using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate.DomainServices
{
    public class CharterInDomainService : ICharterInDomainService
    {

        #region Prop

        private ICharterInRepository _charterInRepository;
        private IRepository<CharterItem> _charterItemRepository;

        #endregion

        #region ctor

        public CharterInDomainService(ICharterInRepository charterInRepository, IRepository<CharterItem> charterItemRepository)
        {
            this._charterInRepository = charterInRepository;
            this._charterItemRepository = charterItemRepository;
        }

        #endregion


        #region Method
        public bool ExistCharterInHeader(long charterId)
        {
            
            return this._charterInRepository.GetById(charterId)!=null;
        }

        public bool HasCharterEnd(long charterId)
        {
            
            return this._charterInRepository.GetCharterEnd(charterId)!=null;
        }
        #endregion



        public DateTime GetCharterStartDate(long vesselInCompanyId,long chartereId)
        {
            return this._charterInRepository.GetCharterStart(vesselInCompanyId, chartereId).ActionDate;
        }

        public CharterIn GetCharterEnd(long charterstartId)
        {
            return  this._charterInRepository.GetCharterEnd(charterstartId);
            
        }

        public States GetCharterState(long id)
        {
            return _charterInRepository.GetById(id).CurrentState;
        }

        public bool HasCharterItem(long charterId)
        {
            return this._charterInRepository.GetById(charterId).CharterItems.Count > 0;
        }


        public bool HasVesselCharterInStart(long vesselInCompanyId)
        {
            var res = _charterInRepository.GetAll().OfType<CharterIn>().Where(c => c.VesselInCompanyId == vesselInCompanyId
                                                            && c.CharterType == CharterType.Start
                                                            && c.CurrentState != States.Submitted);
            return (res.Count() >=1);
        }

        public bool HasVesselCharterInStart(long vesselInCompanyId,long chatererId)
        {
            var res = this._charterInRepository.GetCharterStart(vesselInCompanyId, chatererId);
            return (!(res == null || res.CurrentState != States.Submitted));
        }


        public bool HasVesselCharterInEnd(long vesselInCompanyId)
        {
            var res = _charterInRepository.GetAll().OfType<CharterIn>().Where(c => c.VesselInCompanyId == vesselInCompanyId
                                                            && c.CharterType == CharterType.End
                                                            && c.CurrentState != States.Submitted
                                                          ).SingleOrDefault();
         
            return (res != null);
        }



        public bool IsLastCharter(long vesselInCompanyId,long id)
        {
            var res = _charterInRepository.GetAll().OfType<CharterIn>().Where(c => c.CharterType == CharterType.End && c.VesselInCompanyId == vesselInCompanyId)
                .OrderBy(c => c.ActionDate).Last();
            return (id == res.Id);
        }

        public CharterIn GetCharterInPrevCharterOut(long vesselInCompanyId,DateTime dateCharterOutStart)
        {
            var res = _charterInRepository.GetAll().OfType<CharterIn>()
                .Where(c => c.VesselInCompanyId == vesselInCompanyId
                         && c.ActionDate < dateCharterOutStart
                         && c.CurrentState == States.Submitted)
                             .OrderByDescending(c => c.ActionDate).FirstOrDefault();
            return res;
        }

       public bool IsCharterStartDateGreaterThanPrevCharterEndDate(long vesselInCompanyId,DateTime dateTime)
       {
           var res = _charterInRepository.GetAll().OfType<CharterIn>().Where(c => c.VesselInCompanyId == vesselInCompanyId
                                                                         && c.CurrentState == States.Submitted
                                                                         && c.CharterType==CharterType.End)
                .OrderByDescending(c => c.ActionDate).FirstOrDefault();
           return (res == null) ? true : (res.ActionDate < dateTime);
           ;
       }



       public CharterIn GetCharterInStart(long vesselInCompanyId, long ownerId, DateTime charterOutStartDateTime)
       {
           var res = _charterInRepository.GetAll().OfType<CharterIn>().SingleOrDefault(c => c.VesselInCompanyId == vesselInCompanyId
                                                                                              && c.OwnerId == ownerId
                                                                                              && c.CurrentState == States.Submitted
                                                                                              && c.CharterType == CharterType.Start
                                                                                              && c.ActionDate >= charterOutStartDateTime);
           return res;
       }


       public CharterIn GetCharterInStart(long Id)
       {
           var res = _charterInRepository.GetAll().OfType<CharterIn>().SingleOrDefault(c => c.Id == Id);
           return res;
       }


       public States GetCharterStartState(long vesselInCompanyId, long chartereId)
       {
           return this._charterInRepository.GetCharterStart(vesselInCompanyId, chartereId).CurrentState;
       }


       public void DeleteCharterItem(long id)
       {
           var res = this._charterItemRepository.Find(c => c.Id == id).SingleOrDefault();
           if (res!=null)
           {
               this._charterItemRepository.Delete(res);
           }
       }
    }
}
