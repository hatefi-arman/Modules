using System;
using MITD.Core;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class ScrapWorkflowLog : WorkflowLog
    {
        public ScrapWorkflowLog()
        {

        }

        public ScrapWorkflowLog(
            Scrap scrap,
            WorkflowEntities actionEntity,
            DateTime actionDate,
            WorkflowActions? workflowAction,
            long actorUserId,
            string remark, long currentWorkflowStepId, bool active)
            : base(actionEntity, workflowAction, actorUserId, actionDate, remark, currentWorkflowStepId, active)
        {
            Scrap = scrap;
        }

        public long ScrapId { get; set; }

        public virtual Scrap Scrap { get; set; }

        public override WorkflowLog CreateNextStep(long actorUserId, long stepId, States state, WorkflowStages currentWorkflowStage)
        {
            var scrapWorkflowLog = new ScrapWorkflowLog(
                Scrap,
                WorkflowEntity,
                ActionDate,
                null,
                actorUserId,
                Remark,
                stepId, true);

            return scrapWorkflowLog;
        }

        public override void UpdatedState(WorkflowStep step)
        {
            if (step.State != step.NextWorkflowStep.State)
            {
                //Manage Change State:
                switch (step.WithWorkflowAction)
                {
                    case WorkflowActions.Approve:

                        this.Scrap.EntityState.Approve(this.Scrap);

                        break;

                    case WorkflowActions.Reject:

                        this.Scrap.EntityState.Reject(this.Scrap);

                        break;

                    case WorkflowActions.Cancel:

                        this.Scrap.EntityState.Cancel(this.Scrap);

                        break;
                }
            }
            else
            {
                var approvableDomainService = ServiceLocator.Current.GetInstance<IApprovableScrapDomainService>();

                if ((this.CurrentWorkflowStep.CurrentWorkflowStage == WorkflowStages.Initial ||
                    this.CurrentWorkflowStep.CurrentWorkflowStage == WorkflowStages.Approved) &&
                    step.WithWorkflowAction == WorkflowActions.Approve)
                {
                    //Manage Middle Approve:
                    approvableDomainService.ValidateMiddleApprove(this.Scrap);
                }
            }
        }

    }
}