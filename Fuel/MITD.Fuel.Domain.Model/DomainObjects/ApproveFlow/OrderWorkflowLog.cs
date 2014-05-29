#region

using System;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class OrderWorkflowLog : WorkflowLog
    {
        public OrderWorkflowLog()
        {
        }

        public OrderWorkflowLog(long entityId,
                                WorkflowEntities actionEntity,
                                DateTime actionDate,
                                WorkflowActions? workflowAction,
                                long actorUserId,
                                string remark,
                                long currentWorkflowStepId,
                                bool active)
            : base(actionEntity, workflowAction, actorUserId, actionDate, remark, currentWorkflowStepId, active)
        {
            OrderId = entityId;
        }


        public virtual Order Order { get; set; }
        public long OrderId { get; set; }

        public override WorkflowLog CreateNextStep(long actorUserId, long stepId, States state, WorkflowStages currentWorkflowStage)
        {
            var ordeWorkflowLog = new OrderWorkflowLog(OrderId, WorkflowEntity, ActionDate, null, actorUserId, Remark, stepId, true);

            return ordeWorkflowLog;
        }

        public override void UpdatedState(WorkflowStep step)
        {
            if (step.State != step.NextWorkflowStep.State)


                switch (step.WithWorkflowAction)
                {
                    case WorkflowActions.Approve:
                        Order.OrderState.ApproveOrder(Order);

                        break;
                    case WorkflowActions.Reject:
                        Order.OrderState.RejectOrder(Order);
                        break;

                    case WorkflowActions.Cancel:
                        Order.OrderState.CancelOrder(Order);
                        break;
                }
        }
    }
}