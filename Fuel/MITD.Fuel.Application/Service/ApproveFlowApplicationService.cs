#region

using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Core;
using MITD.Domain.Repository;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.ApproveFlow;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.Factories;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

#endregion

namespace MITD.Fuel.Application.Service
{
    public class ApproveFlowApplicationService : IApproveFlowApplicationService
    {
        #region prop

        private readonly IWorkflowLogRepository _workflowLogRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IEntityConfigurator<Order> orderConfigurator;
        private readonly IEntityConfigurator<Invoice> invoiceConfigurator;
        private readonly IEntityConfigurator<FuelReport> fuelReportConfigurator;
        private readonly IEntityConfigurator<Scrap> scrapConfigurator;
        private readonly IEntityConfigurator<Charter> _charterConfigurator;
        private readonly IEntityConfigurator<Offhire> offhireConfigurator;
        private readonly IUnitOfWorkScope _unitOfWorkScope;
        //    private readonly IApproveWorkFlowFactory _approveWorkFlowFactory;
        //  private readonly IOrderApplicationService _orderApplicationService;

        #endregion

        #region ctor

        public ApproveFlowApplicationService(IUnitOfWorkScope unitOfWorkScope, IWorkflowLogRepository workflowLogRepository,
                                             IWorkflowRepository workflowRepository,
            IEntityConfigurator<Order> orderConfigurator,
            IEntityConfigurator<FuelReport> fuelReportConfigurator,
            IEntityConfigurator<Scrap> scrapConfigurator,
            IEntityConfigurator<Charter> charterConfigurator,
            IEntityConfigurator<Invoice> invoiceConfigurator, IEntityConfigurator<Offhire> offhireConfigurator)
        {
            _unitOfWorkScope = unitOfWorkScope;
            _workflowLogRepository = workflowLogRepository;
            _workflowRepository = workflowRepository;
            this.orderConfigurator = orderConfigurator;
            this.fuelReportConfigurator = fuelReportConfigurator;
            this.scrapConfigurator = scrapConfigurator;
            this.invoiceConfigurator = invoiceConfigurator;
            this.offhireConfigurator = offhireConfigurator;
            this._charterConfigurator = charterConfigurator;
            //          _approveWorkFlowFactory = approveWorkFlowFactory;
            //           _orderApplicationService = orderApplicationService;
        }

        #endregion

        #region Prob

        #endregion

        #region Method

        #endregion

        #region Private Methodes

        protected virtual void CreateWorkFlowStep(long entityId, WorkflowEntities actionEntity, WorkflowActions workflowAction, long actorUserId, string remark, long approveWorkFlowConfigId)
        {

            //            var ent = _approveWorkFlowFactory.CreateApproveFlowLogObject(
            //                entityId,
            //                actionEntity,
            //                DateTime.Now,
            //                workflowAction,
            //                actorUserId,
            //                remark, approveWorkFlowConfigId);
            //
            //            _workflowLogRepository.Add(ent);
            //
            //            _unitOfWorkScope.Commit();
        }


        #endregion

        public ApprovalResult ActApprovalFlow(long entityId, WorkflowEntities workflowEntity, long approverId, string remark, WorkflowActions action)
        {
            // _orderApplicationService;
            //TODO: Fake User Id
            approverId = 1101;  //Defined user for SAPID
            var lastWorkflowLog = GetLastWorkflowLog(entityId, workflowEntity);

            lastWorkflowLog.ValidateUserAccess(approverId, action);

            //next Step
            var step = GetWorkflowStep(lastWorkflowLog.CurrentWorkflowStep, action, approverId);

            if (step == null)
                throw new WorkFlowException("Invalid Access");

            if (step.NextWorkflowStep != null)
            {
                //var approvableDomainService = ServiceLocator.Current.GetInstance(lastWorkflowLog.GetDomainServiceType()) as IApprovableDomainService;

                lastWorkflowLog.UpdatedState(step);

                var newApprovalWorkFlow = lastWorkflowLog.CreateNextStep(step.ActorUserId,
                    step.NextWorkflowStepId.Value, step.NextWorkflowStep.State, step.NextWorkflowStep.CurrentWorkflowStage);

                _workflowLogRepository.Add(newApprovalWorkFlow);
            }
            else
            {
                throw new WorkFlowException("Invalid Action");
            }

            var result = UpdateApproveState(entityId, workflowEntity, approverId, remark, step.WithWorkflowAction, lastWorkflowLog);


            return result;
        }

        private WorkflowStep GetWorkflowStep(WorkflowStep workflowStep, WorkflowActions action, long approverId)
        {
            var result =
                _workflowRepository.First(
                    c =>
                    c.ActorUserId == approverId &&
                    c.CurrentWorkflowStage == workflowStep.CurrentWorkflowStage &&
                    c.WorkflowEntity == workflowStep.WorkflowEntity &&
                    c.State == workflowStep.State &&
                    c.WithWorkflowAction == action
                    );

            return result;
        }

        private ApprovalResult UpdateApproveState(long entityId, WorkflowEntities actionEntity, long approverId, string remark,
                                              WorkflowActions currentAction, WorkflowLog currentApprovalWorkFlowLog)
        {
            currentApprovalWorkFlowLog.UpdateInfo(currentAction, approverId, remark);

            _unitOfWorkScope.Commit();

            var result = new ApprovalResult
                             {
                                 WorkflowAction = currentAction,
                                 ActorId = approverId,
                                 DecisionType = DecisionTypes.Approved,
                                 EntityId = entityId,
                                 Entity = actionEntity,
                                 Remark = remark
                             };

            return result;
        }

        private WorkflowLog GetLastWorkflowLog(long entityId, WorkflowEntities actionEntity)
        {
            WorkflowLog currentApprovalWorkFlow;
            switch (actionEntity)
            {
                case WorkflowEntities.Order:
                    var orderWorkflow = _workflowLogRepository.GetQuery().OfType<OrderWorkflowLog>().FirstOrDefault(
                            c => c.WorkflowEntity == actionEntity && c.OrderId == entityId && c.Active);
                    if (orderWorkflow == null)
                        throw new ObjectNotFound("OrderWorkflow", entityId);

                    orderConfigurator.Configure(orderWorkflow.Order);
                    currentApprovalWorkFlow = orderWorkflow;

                    break;
                case WorkflowEntities.Invoice:
                    var invoiceWorkflow = _workflowLogRepository.GetQuery().OfType<InvoiceWorkflowLog>().
                        FirstOrDefault(
                            c => c.WorkflowEntity == actionEntity && c.InvoiceId == entityId && c.Active);

                    if (invoiceWorkflow == null)
                        throw new ObjectNotFound("InvoiceWorkflow", entityId);

                    invoiceConfigurator.Configure(invoiceWorkflow.Invoice);
                    currentApprovalWorkFlow = invoiceWorkflow;

                    break;
                case WorkflowEntities.FuelReport:
                    var fuelReportWorkflow =
                        _workflowLogRepository.GetQuery().OfType<FuelReportWorkflowLog>().FirstOrDefault(
                            c => c.WorkflowEntity == actionEntity && c.FuelReportId == entityId && c.Active);

                    if (fuelReportWorkflow == null)
                        throw new ObjectNotFound("FuelReportWorkflow", entityId);

                    fuelReportConfigurator.Configure(fuelReportWorkflow.FuelReport);

                    currentApprovalWorkFlow = fuelReportWorkflow;

                    break;

                case WorkflowEntities.CharterIn:
                    var charterWorkflow =
                        _workflowLogRepository.GetQuery().OfType<CharterWorkflowLog>().FirstOrDefault(
                            c => c.WorkflowEntity == actionEntity && c.CharterId == entityId && c.Active);

                    if (charterWorkflow == null)
                        throw new ObjectNotFound("CharterWorkflow", entityId);

                    _charterConfigurator.Configure(charterWorkflow.Charter);

                    currentApprovalWorkFlow = charterWorkflow;

                    break;
                case WorkflowEntities.CharterOut:
                    var charterOutWorkflow =
                        _workflowLogRepository.GetQuery().OfType<CharterWorkflowLog>().FirstOrDefault(
                            c => c.WorkflowEntity == actionEntity && c.CharterId == entityId && c.Active);

                    if (charterOutWorkflow == null)
                        throw new ObjectNotFound("CharterWorkflow", entityId);

                    _charterConfigurator.Configure(charterOutWorkflow.Charter);

                    currentApprovalWorkFlow = charterOutWorkflow;

                    break;
                case WorkflowEntities.Scrap:
                    var scrapWorkflow =
                        _workflowLogRepository.GetQuery().OfType<ScrapWorkflowLog>().FirstOrDefault(
                            c => c.WorkflowEntity == actionEntity && c.ScrapId == entityId && c.Active);

                    if (scrapWorkflow == null)
                        throw new ObjectNotFound("ScrapWorkflow", entityId);

                    scrapConfigurator.Configure(scrapWorkflow.Scrap);

                    currentApprovalWorkFlow = scrapWorkflow;

                    break;
                case WorkflowEntities.Offhire:
                    var offhireWorkflow =
                        _workflowLogRepository.GetQuery().OfType<OffhireWorkflowLog>().FirstOrDefault(
                            c => c.WorkflowEntity == actionEntity && c.OffhireId == entityId && c.Active);

                    if (offhireWorkflow == null)
                        throw new ObjectNotFound("OffhireWorkflow", entityId);

                    offhireConfigurator.Configure(offhireWorkflow.Offhire);

                    currentApprovalWorkFlow = offhireWorkflow;

                    break;
                default:
                    throw new ArgumentOutOfRangeException("actionEntity");
            }

            if (currentApprovalWorkFlow == null)
                throw new WorkFlowException("Record Not Have WorkFlow Object");
            return currentApprovalWorkFlow;
        }
    }
}