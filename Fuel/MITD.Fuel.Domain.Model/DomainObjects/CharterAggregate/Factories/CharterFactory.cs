using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Core;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;

namespace MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate.Factories
{
    public class CharterFactory
    {
        private readonly IWorkflowRepository _workflowRepository;
        private ICharterInDomainService _charterInDomainService;
        private ICharterOutDomainService _charterOutDomainService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IVesselInCompanyDomainService vesselInCompanyDomainService;
        private readonly IInventoryOperationNotifier inventoryOperationNotifier;

        public CharterFactory(IWorkflowRepository workflowRepository,
            ICharterInDomainService charterInDomainService,
            ICharterOutDomainService charterOutDomainService,
            IEventPublisher eventPublisher, IVesselInCompanyDomainService vesselInCompanyDomainService, IInventoryOperationNotifier inventoryOperationNotifier)
        {
            _workflowRepository = workflowRepository;
            _charterInDomainService = charterInDomainService;
            _charterOutDomainService = charterOutDomainService;
            _eventPublisher = eventPublisher;
            this.vesselInCompanyDomainService = vesselInCompanyDomainService;
            this.inventoryOperationNotifier = inventoryOperationNotifier;
        }


        public CharterIn CreateCharterIn(long id, long chartererId, long ownerId, long vesselInCompanyId, long currencyId,
                            DateTime actionDate,
                            List<CharterItem> charterItems,
                            List<InventoryOperation> inventoryOperationItems
                           , CharterType charterType, CharterEndType charterEndType, OffHirePricingType offHirePricingType)
        {


            var charterIn = new CharterIn(id, chartererId, ownerId, vesselInCompanyId,
                               currencyId, actionDate,
                               charterItems, inventoryOperationItems
                              , charterType, charterEndType, offHirePricingType,
                              _charterInDomainService, _charterOutDomainService,
                              _eventPublisher);

            var init = this._workflowRepository.Single(c => c.WorkflowEntity == WorkflowEntities.CharterIn && c.CurrentWorkflowStage == WorkflowStages.Initial);
            if (init == null)
                throw new ObjectNotFound("CharterInitialStep");

            var charterWorkflowLog = new CharterWorkflowLog(charterIn, WorkflowEntities.CharterIn, DateTime.Now, WorkflowActions.Init, 1, "", init.Id, true);

            charterIn.ApproveWorkflows.Add(charterWorkflowLog);

            return charterIn;

        }


        public CharterIn ResolveCharterIn(CharterIn charter)
        {
            charter.Resolve(_charterInDomainService, _charterOutDomainService, _eventPublisher, vesselInCompanyDomainService, inventoryOperationNotifier);
            return charter;
        }




        public CharterIn ReCreateCharterIn(CharterIn charter)
        {

            charter.Resolve(_charterInDomainService, _charterOutDomainService, _eventPublisher, vesselInCompanyDomainService, inventoryOperationNotifier);

            //var init = this._workflowRepository.Single(c => c.WorkflowEntity == WorkflowEntities.CharterIn && c.CurrentWorkflowStage == WorkflowStages.Initial);
            //if (init == null)
            //    throw new ObjectNotFound("CharterInitialStep");

            //var charterWorkflowLog = new CharterWorkflowLog(charter, WorkflowEntities.CharterIn, DateTime.Now, WorkflowActions.Init, 1, "", init.Id, true);

            //charter.ApproveWorkflows.Add(charterWorkflowLog);

            return charter;

        }


        public CharterOut ResolveCharterOut(CharterOut charter)
        {
            charter.Resolve(_charterInDomainService, _charterOutDomainService, _eventPublisher, vesselInCompanyDomainService, inventoryOperationNotifier);
            return charter;
        }

        public CharterOut CreateCharterOut(long id, long chartererId, long ownerId, long vesselInCompanyId, long currencyId,
                            DateTime actionDate,
                            List<CharterItem> charterItems,
                            List<InventoryOperation> inventoryOperationItems
                           , CharterType charterType, CharterEndType charterEndType, OffHirePricingType offHirePricingType)
        {
            var charterOut = new CharterOut(id, chartererId, ownerId, vesselInCompanyId,
                               currencyId, actionDate,
                               charterItems, inventoryOperationItems
                              , charterType, charterEndType, offHirePricingType,
                              _charterInDomainService, _charterOutDomainService, _eventPublisher);

            var init = this._workflowRepository.Single(c => c.WorkflowEntity == WorkflowEntities.CharterOut && c.CurrentWorkflowStage == WorkflowStages.Initial);
            if (init == null)
                throw new ObjectNotFound("CharterInitialStep");

            var charterWorkflowLog = new CharterWorkflowLog(charterOut, WorkflowEntities.CharterOut, DateTime.Now, WorkflowActions.Init, 1, "", init.Id, true);

            charterOut.ApproveWorkflows.Add(charterWorkflowLog);

            return charterOut;
        }


        public CharterOut ReCreateCharterOut(CharterOut charter)
        {
            charter.Resolve(_charterInDomainService, _charterOutDomainService, _eventPublisher, vesselInCompanyDomainService, inventoryOperationNotifier);

            //var init = this._workflowRepository.Single(c => c.WorkflowEntity == WorkflowEntities.CharterOut && c.CurrentWorkflowStage == WorkflowStages.Initial);
            //if (init == null)
            //    throw new ObjectNotFound("CharterOutitialStep");

            //var charterWorkflowLog = new CharterWorkflowLog(charter, WorkflowEntities.CharterOut, DateTime.Now, WorkflowActions.Init, 1, "", init.Id, true);

            //charter.ApproveWorkflows.Add(charterWorkflowLog);

            return charter;
        }


        public CharterItem CraeteCharterItem(long id, long charterId, decimal rob, decimal fee,
                                         decimal feeOffhire, long goodId, long tankId, long unitId)
        {
            return new CharterItem(id, charterId, rob, fee, feeOffhire, goodId, tankId, unitId);
        }



    }
}
