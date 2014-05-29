using System;
using MITD.Core;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class OffhireWorkflowLog : WorkflowLog
    {
        public OffhireWorkflowLog()
        {

        }

        public OffhireWorkflowLog(
            Offhire offhire,
            WorkflowEntities actionEntity,
            DateTime actionDate,
            WorkflowActions? workflowAction,
            long actorUserId,
            string remark, long currentWorkflowStepId, bool active)
            : base(actionEntity, workflowAction, actorUserId, actionDate, remark, currentWorkflowStepId, active)
        {
            Offhire = offhire;
        }

        public long OffhireId { get; set; }

        public virtual Offhire Offhire { get; set; }

        public override WorkflowLog CreateNextStep(long actorUserId, long stepId, States state, WorkflowStages currentWorkflowStage)
        {
            var offhireWorkflowLog = new OffhireWorkflowLog(
                Offhire,
                WorkflowEntity,
                ActionDate,
                null,
                actorUserId,
                Remark,
                stepId, true);

            return offhireWorkflowLog;
        }

        public override void UpdatedState(WorkflowStep step)
        {
            var approvableDomainService = ServiceLocator.Current.GetInstance<IApprovableOffhireDomainService>();

            if (step.State != step.NextWorkflowStep.State)
            {
                //Manage Change State:
                switch (step.WithWorkflowAction)
                {
                    case WorkflowActions.Approve:

                        this.Offhire.EntityState.Approve(this.Offhire);

                        break;

                    case WorkflowActions.Reject:

                        this.Offhire.EntityState.Reject(this.Offhire);

                        break;

                    case WorkflowActions.Cancel:

                        this.Offhire.EntityState.Cancel(this.Offhire);

                        break;
                }
            }
            else
            {
                if ((this.CurrentWorkflowStep.CurrentWorkflowStage == WorkflowStages.Initial ||
                    this.CurrentWorkflowStep.CurrentWorkflowStage == WorkflowStages.Approved) &&
                    step.WithWorkflowAction == WorkflowActions.Approve)
                {
                    //Manage Middle Approve:
                    approvableDomainService.ValidateMiddleApprove(this.Offhire);
                }
            }
        }

    }
}