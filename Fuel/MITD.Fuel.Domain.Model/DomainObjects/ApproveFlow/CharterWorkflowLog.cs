using System;
using MITD.Core;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class CharterWorkflowLog : WorkflowLog
    {
        public CharterWorkflowLog()
        {

        }

        public CharterWorkflowLog(
            Charter charter,
            WorkflowEntities actionEntity,
            DateTime actionDate,
            WorkflowActions? workflowAction,
            long actorUserId,
            string remark, long currentWorkflowStepId, bool active)
            : base(actionEntity, workflowAction, actorUserId, actionDate, remark, currentWorkflowStepId, active)
        {
            Charter = charter;
        }

        public long CharterId { get; set; }

        public virtual Charter Charter { get; set; }

        public override WorkflowLog CreateNextStep(long actorUserId, long stepId, States state, WorkflowStages currentWorkflowStage)
        {
            var charterWorkflowLog = new CharterWorkflowLog(
                Charter,
                WorkflowEntity,
                ActionDate,
                null,
                actorUserId,
                Remark,
                stepId, true);

            return charterWorkflowLog;
        }

        public override void UpdatedState(WorkflowStep step)
        {
            //var approvableDomainService = ServiceLocator.Current.GetInstance<IApprovableScrapDomainService>();

            if (step.State != step.NextWorkflowStep.State)
            {
                //Manage Change State:
                switch (step.WithWorkflowAction)
                {
                    case WorkflowActions.Approve:


                        this.Charter.CharterState.Approve(this.Charter);

                        break;

                    case WorkflowActions.Reject:

                        this.Charter.CharterState.Reject(this.Charter);

                        break;
                }
            }
            else
            {
                if ((this.CurrentWorkflowStep.CurrentWorkflowStage == WorkflowStages.Initial ||
                    this.CurrentWorkflowStep.CurrentWorkflowStage == WorkflowStages.Approved) &&
                    step.WithWorkflowAction == WorkflowActions.Approve)
                {

                    Charter.Approve();
                    //Manage Middle Approve:
                    //approvableDomainService.ValidateMiddleApprove(this.CharterIn);
                }
            }
        }

    }
}