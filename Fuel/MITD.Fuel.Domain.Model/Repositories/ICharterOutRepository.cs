using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;

namespace MITD.Fuel.Domain.Model.Repositories
{
    public interface ICharterOutRepository : ICharterRepository
    {
        PageResult<CharterOut> GetByFilter(long companyId, int pageSize, int pageIndex);
        CharterOut GetCharterStartById(long id);
        CharterOut GetCharterEnd(long startId);
        CharterOut GetById(long id);
        CharterOut GetCharterStart(long vesselInCompanyId, long ownerId);
        IQueryable<CharterOut> GetQueryable();
        IQueryable<CharterOut> GetQueryInclude();
    }
}
