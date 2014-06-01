#region

using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.Enums;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class WorkflowStep
    {
        #region Properties

        public long Id { get; set; }
        public long ActorUserId { get; private set; }
        public long? NextWorkflowStepId { get; private set; }
        public WorkflowActions WithWorkflowAction { get; private set; }
        public States State { get; private set; }
        public WorkflowEntities WorkflowEntity { get; private set; }
        public WorkflowStages CurrentWorkflowStage { get; private set; }

        public virtual WorkflowStep NextWorkflowStep { get; set; }
        public virtual FuelUser ActorUser { get; private set; }


        #endregion

        public WorkflowStep()
        {
            //To be used as parameter 'TEntity' in the generic type or method 'MITD.DataAccess.EF.EntityTypeConfigurationBase<TEntity>'	
        }

        public WorkflowStep(
            long id,
            long actorUserId,
            long? nextWorkflowStepId,
            WorkflowActions withWorkflowAction,
            States state,
            WorkflowEntities workflowEntity,
            WorkflowStages currentWorkflowStage
            )
        {
            Id = id;
            ActorUserId = actorUserId;
            NextWorkflowStepId = nextWorkflowStepId;
            WithWorkflowAction = withWorkflowAction;
            State = state;
            WorkflowEntity = workflowEntity;
            CurrentWorkflowStage = currentWorkflowStage;
        }

        public WorkflowStep(
              WorkflowEntities workflowEntity,
             long actorUserId,
             States state,
             WorkflowStages currentWorkflowStage,
             WorkflowActions withWorkflowAction,
            long? nextWorkflowStepId

   )
        {
            ActorUserId = actorUserId;
            NextWorkflowStepId = nextWorkflowStepId;
            WithWorkflowAction = withWorkflowAction;
            State = state;
            WorkflowEntity = workflowEntity;
            CurrentWorkflowStage = currentWorkflowStage;
        }
    }
}