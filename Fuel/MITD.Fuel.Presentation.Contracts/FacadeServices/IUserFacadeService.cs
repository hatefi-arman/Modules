using System.Collections.Generic;
using System.Globalization;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Services.Facade;

namespace MITD.Fuel.Presentation.Contracts.FacadeServices
{
    public interface IUserFacadeService : IFacadeService
    {

        UserDto GetUserByUserName(string username);
      //  UserGroupDescriptionDto GetUserGroupByName(string groupName);
        UserGroupDto GetUserGroupByName(string name);

        UserGroupDto AddUserGroup(UserGroupDto userGroupDto);
        UserGroupDto UpdateUserGroup(UserGroupDto userGroupDto);
        string DeleteUserGroup(string name);

        List<ActionTypeDto> GetAllActionTypes();

        
        void Add(UserDto data);
        void Update(UserDto data);
        void Delete(UserDto data);
        UserDto GetById(int id);
        List<UserDto> GetAll(int pageSize, int pageIndex);
        void DeleteById(int id);
        UserDto GetUserWithCompany(int id);
    }
}
