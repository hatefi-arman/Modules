﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MITD.Core;
using MITD.Fuel.Application.Service.Security;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.FuelSecurity.Domain.Model;

namespace MITD.Fuel.Application.Facade
{
    public class SecurityFacadeService : ISecurityFacadeService
    {
      //  private readonly IMapper<List<ActionType>, ClaimsPrincipal> _userActionMapper;
        private readonly UserActionsMapper _userActionMapper;
        private readonly UserSecurityMapper _pmsUserMapper;
        private readonly ISecurityApplicationService _securityApplicationService;

        //public SecurityFacadeService()
        //{
        //    var x = ServiceLocator.Current.GetAllInstances<IMapper<List<ActionType>, ClaimsPrincipal>>();
        //}
        
        public SecurityFacadeService(//(IMapper<List<ActionType>, ClaimsPrincipal> userActionMapper,
            ISecurityApplicationService securityApplicationService)
        {
            this._userActionMapper = new UserActionsMapper();
            this._securityApplicationService = securityApplicationService;
            this._pmsUserMapper=new UserSecurityMapper();
        }


        public bool IsAuthorize(string className, string methodName, ClaimsPrincipal userClaimsPrincipal)
        {
            var methodMapper = new MethodMapper();
            var methodRequiredActions = methodMapper.Map(className, methodName);
            List<ActionType> userActions = _userActionMapper.MapToEntity(userClaimsPrincipal);
            return _securityApplicationService.IsAuthorize(userActions, methodRequiredActions);          
        }

        public List<ActionType> GetUserAuthorizedActions(ClaimsPrincipal userClaimsPrincipal)
        {
           return this._securityApplicationService.GetAllAuthorizedActions(_pmsUserMapper.MapToEntity(userClaimsPrincipal));
        }

        public void AddUpdateUser(ClaimsPrincipal userClaimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public User GetLogonUser()
        {
            throw new NotImplementedException();
        }
    }  
}
