using System;
using MITD.Core;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class FuelReportWorkflowLog : WorkflowLog
    {
        public FuelReportWorkflowLog()
        {

        }

        public FuelReportWorkflowLog(
            long entityId,
            WorkflowEntities actionEntity,
            DateTime actionDate,
            WorkflowActions? workflowAction,
            long actorUserId,
            string remark, long currentWorkflowStepId, bool active)
            : base(actionEntity, workflowAction, actorUserId, actionDate, remark, currentWorkflowStepId, active)
        {
            FuelReportId = entityId;
        }
        public long FuelReportId { get; set; }

        public virtual FuelReport FuelReport { get; set; }

        //public override Type GetDomainServiceType()
        //{
        //    return typeof(IApprovableFuelReportDomainService);
        //}



        public override WorkflowLog CreateNextStep(long actorUserId, long stepId, States state, WorkflowStages currentWorkflowStage)
        {
            var fuelReportApproveWorkFlow = new FuelReportWorkflowLog(
                FuelReportId,
                WorkflowEntity,
                ActionDate,
                null,
                actorUserId,
                Remark,
                stepId, true);

            return fuelReportApproveWorkFlow;
        }

        public override void UpdatedState(WorkflowStep step)
        {
            IApprovableFuelReportDomainService approveService = ServiceLocator.Current.GetInstance<IApprovableFuelReportDomainService>();

            if (step.State != step.NextWorkflowStep.State)
                switch (step.WithWorkflowAction)
                {
                    case WorkflowActions.Approve:

                        FuelReport.EntityState.Approve(FuelReport, approveService);

                        break;

                    case WorkflowActions.Reject:

                        FuelReport.EntityState.Reject(FuelReport);

                        break;

                    case WorkflowActions.Cancel:

                        FuelReport.EntityState.Cancel(FuelReport);

                        break;
                }
        }
    }
}