using System;
using System.Collections.Generic;
using MITD.Domain.Repository;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.Fuel.Application.Service
{
    public class VoyageApplicationService: IVoyageApplicationService
    {
        private readonly IVoyageRepository _repository;

        public VoyageApplicationService(IVoyageRepository repository,IUnitOfWorkScope uows)
        {
            _repository = repository;
        }


        public List<Voyage> GetByFilter(long companyId, long vesselId)
        {
            var result = _repository.GetByFilter(companyId, vesselId);
            return result;
        }

        public List<Voyage> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Voyage> Get(List<long> IDs)
        {
            throw new NotImplementedException();
        }
    }
}
