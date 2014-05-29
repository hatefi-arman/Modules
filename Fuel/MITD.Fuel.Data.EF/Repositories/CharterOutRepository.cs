using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.DataAccess.EF;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.Fuel.Data.EF.Repositories
{
    public class CharterOutRepository : CharterRepository, ICharterOutRepository
    {
        public CharterOutRepository(EFUnitOfWork efUnitOfWork)
            : base(efUnitOfWork)
        {

        }

        public CharterOutRepository(IUnitOfWorkScope unitOfWorkScope)
            : base(unitOfWorkScope)
        {

        }


        public IQueryable<CharterOut> GetQueryInclude()
        {
            return this.Context.CreateObjectSet<Charter>()
                .Include("CharterItems")
                .Include("InventoryOperationItems").OfType<CharterOut>()
                .AsQueryable();
           
        }
        public IQueryable<CharterOut> GetQueryable()
        {
            return this.Context.CreateObjectSet<Charter>().OfType<CharterOut>()
                .AsQueryable();
        } 

        public PageResult<CharterOut> GetByFilter(long companyId, int pageSize, int pageIndex)
        {
            var res = new PageResult<CharterOut>();

            var strategy = new ListFetchStrategy<Charter>(Enums.FetchInUnitOfWorkOption.NoTracking);

            IQueryable<CharterOut> query = this.GetAll(strategy).OfType<CharterOut>().OrderBy(p => p.Id)
                .Where(c => c.OwnerId == companyId && c.CharterType == CharterType.Start).AsQueryable();

            res.Result = query.Skip(pageSize * (pageIndex - 1))
                .Take(pageSize).ToList();

            res.TotalCount = query.Count();

            res.TotalPages = Convert.ToInt32(Math.Ceiling(decimal.Divide(res.TotalCount, pageSize)));

            res.CurrentPage = pageIndex + 1;

            return res;

        }


        public CharterOut GetCharterStartById(long id)
        {

            var res = GetQueryInclude().Single(c => c.Id == id) as CharterOut;
            return res;
        }

        public CharterOut GetCharterEnd(long startId)
        {

            var sEntity = GetQueryInclude().Single(c => c.Id == startId);

            return GetQueryInclude().Where(c => c.VesselInCompanyId == sEntity.VesselInCompanyId &&
                                       c.CharterType == CharterType.End &&
                                       c.OwnerId == sEntity.OwnerId &&
                                       c.ActionDate > sEntity.ActionDate).OrderBy(c => c.Id).FirstOrDefault();
           
        }

        public CharterOut GetCharterStart(long vesselInCompanyId, long ownerId)
        {


            var sEntity = GetQueryInclude().Where(c => c.VesselInCompanyId == vesselInCompanyId
                                              && c.OwnerId == ownerId
                                              && c.CharterType==CharterType.Start).
                                              OrderByDescending(c => c.Id).FirstOrDefault();
            return sEntity;

        }

        public CharterOut GetById(long id)
        {
            return this.FindByKey(id) as CharterOut;
        }
    }
}
