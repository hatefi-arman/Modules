using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.FuelSecurity.Domain;
using MITD.FuelSecurity.Domain.Model;

namespace MITD.Fuel.Application.Facade
{
    public class MethodMapper
    {
        public class MethodAction
        {
            public string ClassName { get; private set; }
            public string MethodName { get; private set; }
            public List<ActionType> Actions { get; private set; }
            public MethodAction(string className, string methodName, List<ActionType> actions)
            {
                ClassName = className;
                MethodName = methodName;
                Actions = actions;
            }
        }

        private readonly List<MethodAction> mapTable = new List<MethodAction>
        {
             #region chrter in
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("GetAll").Name,new List<ActionType>(){ActionType.QueryCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("GetById").Name,new List<ActionType>(){ActionType.QueryCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("GetCharterEnd").Name,new List<ActionType>(){ActionType.QueryCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("GetAllItem").Name,new List<ActionType>(){ActionType.QueryCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("GetItemById").Name,new List<ActionType>(){ActionType.QueryCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("Add").Name,new List<ActionType>(){ActionType.AddCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("Update").Name,new 
               List<ActionType>(){ActionType.EditCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("Delete").Name,new 
               List<ActionType>(){ActionType.DeleteCharterIn}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("AddItem").Name,new 
               List<ActionType>(){ActionType.AddCharterInItem}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("UpdateItem").Name,new 
               List<ActionType>(){ActionType.EditCharterInItem}),
             new MethodAction(typeof(ICharterInFacadeService).Name,typeof(ICharterInFacadeService).GetMethod("DeleteItem").Name,new 
               List<ActionType>(){ActionType.DeleteCharterInItem}),

            #endregion
      
            #region chater out
                new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("GetAll").Name,new List<ActionType>(){ActionType.QueryCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("GetById").Name,new List<ActionType>(){ActionType.QueryCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("GetCharterEnd").Name,new List<ActionType>(){ActionType.QueryCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("GetAllItem").Name,new List<ActionType>(){ActionType.QueryCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("GetItemById").Name,new List<ActionType>(){ActionType.QueryCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("Add").Name,new List<ActionType>(){ActionType.AddCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("Update").Name,new 
               List<ActionType>(){ActionType.EditCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("Delete").Name,new 
               List<ActionType>(){ActionType.DeleteCharterOut}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("AddItem").Name,new 
               List<ActionType>(){ActionType.AddCharterOutItem}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("UpdateItem").Name,new 
               List<ActionType>(){ActionType.EditCharterOutItem}),
             new MethodAction(typeof(ICharterOutFacadeService).Name,typeof(ICharterOutFacadeService).GetMethod("DeleteItem").Name,new 
               List<ActionType>(){ActionType.DeleteCharterOutItem}),
            #endregion
      
       

        };

        private readonly List<MethodAction> MapTableWorkFlow = new List<MethodAction>
        {
             #region chrter in
             new MethodAction(ActionEntityTypeEnum.CharterIn.ToString(),DecisionTypeEnum.Approved.ToString(),new List<ActionType>(){ActionType.ApproveCharterIn}),
             new MethodAction(ActionEntityTypeEnum.CharterIn.ToString(),DecisionTypeEnum.Canceled.ToString(),new List<ActionType>(){ActionType.FinalApproveCharterIn}),
             new MethodAction(ActionEntityTypeEnum.CharterIn.ToString(),DecisionTypeEnum.Rejected.ToString(),new List<ActionType>(){ActionType.RejectCharterIn}),
             
            #endregion
      
           #region chrter Out
             new MethodAction(ActionEntityTypeEnum.CharterOut.ToString(),DecisionTypeEnum.Approved.ToString(),new List<ActionType>(){ActionType.ApproveCharterOut}),
             new MethodAction(ActionEntityTypeEnum.CharterOut.ToString(),DecisionTypeEnum.Canceled.ToString(),new List<ActionType>(){ActionType.FinalApproveCharterOut}),
             new MethodAction(ActionEntityTypeEnum.CharterOut.ToString(),DecisionTypeEnum.Rejected.ToString(),new List<ActionType>(){ActionType.RejectCharterOut}),
             
            #endregion
      
       

        };



        public List<ActionType> MapWF(ActionEntityTypeEnum workflowEntitie, DecisionTypeEnum decisionTypeEnum)
        {

            var mapRow = MapTableWorkFlow.Where(m => m.ClassName == workflowEntitie.ToString() && m.MethodName == decisionTypeEnum.ToString());
            if (!mapRow.Any())
                return new List<ActionType>();
            return mapRow.FirstOrDefault().Actions;
            
        }

        public List<ActionType> Map(string className, string methodName)
        {

            var mapRow = mapTable.Where(m => m.ClassName == className && m.MethodName == methodName);
            if (!mapRow.Any())
                return new List<ActionType>();
            return mapRow.FirstOrDefault().Actions;

        }


    }

}
