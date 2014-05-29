#region

using System;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;

#endregion

namespace MITD.Fuel.Application.Facade
{
    public class ApprovalFlowFasadeService : IApprovalFlowFacadeService
    {
        private IApproveFlowApplicationService approvalFlowAppService { get; set; }

        public ApprovalFlowFasadeService(IApproveFlowApplicationService _approvalFlowAppService)
        {
            approvalFlowAppService = _approvalFlowAppService;
        }

        #region IApprovalFlowFacadeService Members


       

        public ApprovmentDto ActApprovalFlow(ApprovmentDto entity)
        {
            var result = approvalFlowAppService.ActApprovalFlow(entity.EntityId,
                (WorkflowEntities)(int)entity.ActionEntityType, 
                entity.ActorId,
                entity.Remark,
                MapDesicionToAction(entity)
                );

            var retVal = new ApprovmentDto
                {
                    EntityId = result.EntityId,
                    ActorId = result.ActorId,
                    ActionType = MapEntityActionTypeToDtoActionType(result.WorkflowAction),
                    ActionEntityType = (ActionEntityTypeEnum)(int)result.Entity,
                    DecisionType = (DecisionTypeEnum)(int)result.DecisionType,
                    Remark = entity.Remark
                };
            return retVal;
        }

        private static WorkflowActions MapDesicionToAction(ApprovmentDto entity)
        {
            WorkflowActions action;
            switch (entity.DecisionType)
            {
                case DecisionTypeEnum.Approved:
                    action = WorkflowActions.Approve;
                    break;
                case DecisionTypeEnum.Rejected:
                    action = WorkflowActions.Reject;
                    break;
                case DecisionTypeEnum.Canceled:
                    action = WorkflowActions.Cancel;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return action;
        }

        public ActionTypeEnum MapEntityActionTypeToDtoActionType(WorkflowActions workflowAction)
        {
            switch (workflowAction)
            {
//                case WorkflowActions.None:
//
//                    return ActionTypeEnum.None;

                case WorkflowActions.Approve:

                    return ActionTypeEnum.Approved;

                case WorkflowActions.Reject:

                    return ActionTypeEnum.Rejected;

                case WorkflowActions.FinalApprove:

                    return ActionTypeEnum.FinalApproved;

                default:
                    return ActionTypeEnum.Undefined;
            }
        }
        #endregion
    }
}