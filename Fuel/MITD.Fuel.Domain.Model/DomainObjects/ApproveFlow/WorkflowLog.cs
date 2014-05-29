#region

using System;
using System.Collections.Generic;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class WorkflowLog
    {


        #region Properties

        public long Id { get; private set; }

        public WorkflowEntities WorkflowEntity { get; private set; }

        public DateTime ActionDate { get; private set; }

        public WorkflowActions? WorkflowAction { get; private set; }

        public long? ActorUserId { get; private set; }

        public string Remark { get; private set; }

        public bool Active { get; set; }

        public virtual User ActorUser { get; private set; }

        public virtual WorkflowStep CurrentWorkflowStep { get; set; }
        public long CurrentWorkflowStepId { get; set; }

        #endregion

        public WorkflowLog()
        {
            //To be used as parameter 'TEntity' in the generic type or method 'MITD.DataAccess.EF.EntityTypeConfigurationBase<TEntity>'	
        }

        public WorkflowLog(WorkflowEntities actionEntity, WorkflowActions? workflowAction, long actorUserId, DateTime actionDate, string remark, long currentWorkflowStepId, bool active)
        {
            WorkflowEntity = actionEntity;

            WorkflowAction = workflowAction;
            ActorUserId = actorUserId;
            ActionDate = actionDate;
            Remark = remark;
            CurrentWorkflowStepId = currentWorkflowStepId;
            Active = active;
        }

        public void UpdateInfo(WorkflowActions workflowAction, long actorUserId, string remark)
        {
            WorkflowAction = workflowAction;
            ActorUserId = actorUserId;
            Remark = remark;
            ActionDate = DateTime.Now;
            Active = false;
        }
        public virtual Type GetDomainServiceType()
        {
            throw new NotImplementedException();
        }

        public virtual WorkflowLog CreateNextStep(long actorUserId, long stepId, States state, WorkflowStages currentWorkflowStage)
        {
            throw new NotImplementedException();
        }

        public void UpdateNextStep(long actorUserId, long workflowStepId)
        {
            Active = true;
            CurrentWorkflowStepId = workflowStepId;
            ActorUserId = actorUserId;
        }

        protected object Clone()
        {
            return new
                       {
                           Id,
                           ActionEntityType = WorkflowEntity,
                           ActionDate,
                           ActionType = WorkflowAction,
                           ActorUserId,
                           Remark,
                           ApproveWorkFlowConfigId = CurrentWorkflowStepId
                       };
        }

        public void ValidateUserAccess(long approverId, WorkflowActions action)
        {
            if (ActorUserId != approverId)
                throw new WorkFlowException("InvalidAccess");
        }

        public virtual void UpdatedState(WorkflowStep step)
        {
            throw new NotImplementedException();
        }

    }
}