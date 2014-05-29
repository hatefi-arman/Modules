
using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Fuel.Application.Facade.Mappers;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade
{
    public class UserFacadeService : IUserFacadeService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFacadeMapper<User, UserDto> _mapper;
        private readonly IFacadeMapper<Company, CompanyDto> _companyMapper;

        #region props

        #endregion

        #region ctor

        public UserFacadeService()
        {

        }
        public UserFacadeService(IUserRepository userRepository, IFacadeMapper<User, UserDto> mapper, IFacadeMapper<Company, CompanyDto> companyMapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _companyMapper = companyMapper;
        }

        #endregion

        #region methods

        public void Add(UserDto data)
        {
            throw new NotImplementedException();

        }

        public void Update(UserDto data)
        {
            throw new NotImplementedException();

        }

        public void Delete(UserDto data)
        {
            throw new NotImplementedException();

        }

        public UserDto GetUserWithCompany(int id)
        {
            var fetch = new SingleResultFetchStrategy<User>().Include(c => c.Company);

            var ent = _userRepository.Single(c => c.Id == id, fetch);

            var ou = _mapper.MapToModel(ent);
            ou.CompanyDto = _companyMapper.MapToModel(ent.Company);
            return ou;
        }

        public UserDto GetById(int id)
        {

            var ent = _userRepository.FindByKey(id);

            var ou = _mapper.MapToModel(ent);

            return ou;
        }

        public List<UserDto> GetAll(int pageSize, int pageIndex)
        {
            var fetch = new ListFetchStrategy<User>().WithPaging(pageSize, pageIndex );

            _userRepository.GetAll(fetch);

            var finalResult = new PageResultDto<UserDto>
                                  {
                                      CurrentPage = pageIndex,
                                      PageSize = pageSize,
                                      Result = _mapper.MapToModel(fetch.PageCriteria.PageResult.Result).ToList(),
                                      TotalCount = fetch.PageCriteria.PageResult.TotalCount,
                                      TotalPages = fetch.PageCriteria.PageResult.TotalPages
                                  };

            foreach (var user in finalResult.Result)
                user.Code = user.FirstName;

            return finalResult.Result.ToList();

        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();

        }



        #endregion


        public UserDto GetUserByUserName(string username)
        {
            throw new NotImplementedException();
        }

        public UserGroupDto GetUserGroupByName(string name)
        {
            throw new NotImplementedException();
        }

        public UserGroupDto AddUserGroup(UserGroupDto userGroupDto)
        {
            throw new NotImplementedException();
        }

        public UserGroupDto UpdateUserGroup(UserGroupDto userGroupDto)
        {
            throw new NotImplementedException();
        }

        public string DeleteUserGroup(string name)
        {
            throw new NotImplementedException();
        }

        public List<ActionTypeDto> GetAllActionTypes()
        {
            throw new NotImplementedException();
        }
    }
}
