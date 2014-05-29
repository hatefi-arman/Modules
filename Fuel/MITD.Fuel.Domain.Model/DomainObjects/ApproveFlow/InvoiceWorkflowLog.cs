using System;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.DomainObjects.ApproveFlow
{
    public class InvoiceWorkflowLog : WorkflowLog
    {
        public InvoiceWorkflowLog()
        {

        }
        public InvoiceWorkflowLog
            (long entityId, WorkflowEntities actionEntity, DateTime actionDate, WorkflowActions? workflowAction, long actorUserId, string remark, long currentWorkflowStepId, bool active)
            : base(actionEntity, workflowAction, actorUserId, actionDate, remark, currentWorkflowStepId, active)
        {
            InvoiceId = entityId;
        }


        public virtual Invoice Invoice { get; set; }
        public long InvoiceId { get; set; }


        public override WorkflowLog CreateNextStep(long actorUserId, long stepId, States state, WorkflowStages currentWorkflowStage)
        {
            var ordeWorkflowLog = new InvoiceWorkflowLog(InvoiceId, WorkflowEntity, ActionDate, null, actorUserId,
                                                       Remark, stepId, true);

            return ordeWorkflowLog;
        }

        public override void UpdatedState(WorkflowStep step)
        {
            if (step.State != step.NextWorkflowStep.State)

                switch (step.WithWorkflowAction)
                {
                    case WorkflowActions.Approve:
                        Invoice.InvoiceState.ApproveInvoice(Invoice);

                        break;
                    case WorkflowActions.Reject:
                        Invoice.InvoiceState.RejectInvoice(Invoice);
                        break;

                    case WorkflowActions.Cancel:
                        Invoice.InvoiceState.CancelInvoice(Invoice);
                        break;
                }
        }
    }
}