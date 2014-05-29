using System;
using System.Collections.Generic;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.Fuel.Domain.Model.DomainObjects.Factories
{
    public class ScrapFactory : IScrapFactory
    {
        private readonly IEntityConfigurator<Scrap> scrapConfigurator;
        private readonly IScrapDomainService scrapDomainService;
        private readonly IWorkflowRepository workflowRepository;
        private readonly IVesselInCompanyDomainService vesselDomainService;
        private readonly ICompanyDomainService companyDomainService;
        private readonly ITankDomainService tankDomainService;
        private readonly ICurrencyDomainService currencyDomainService;
        private readonly IGoodDomainService goodDomainService;
        private readonly IGoodUnitDomainService goodUnitDomainService;

        public ScrapFactory(
            IEntityConfigurator<Scrap> scrapConfigurator,
            IWorkflowRepository workflowRepository,
            IScrapDomainService scrapDomainService,
            IVesselInCompanyDomainService vesselDomainService,
            ICompanyDomainService companyDomainService,
            ITankDomainService tankDomainService,
            ICurrencyDomainService currencyDomainService,
            IGoodDomainService goodDomainService,
            IGoodUnitDomainService goodUnitDomainService)
        {
            this.scrapConfigurator = scrapConfigurator;
            this.workflowRepository = workflowRepository;
            this.vesselDomainService = vesselDomainService;
            this.companyDomainService = companyDomainService;
            this.tankDomainService = tankDomainService;
            this.currencyDomainService = currencyDomainService;
            this.goodDomainService = goodDomainService;
            this.goodUnitDomainService = goodUnitDomainService;
            this.scrapDomainService = scrapDomainService;
        }

        public Scrap CreateScrap(VesselInCompany vesselInCompany, Company secondParty, DateTime scrapDate, List<ScrapDetail> scrapDetails)
        {
            var scrap = this.CreateScrap(vesselInCompany, secondParty, scrapDate);

            foreach (var scrapDetail in scrapDetails)
            {
                scrap.AddDetail(scrapDetail);
            }

            return scrap;
        }

        public Scrap CreateScrap(VesselInCompany vesselInCompany, Company secondParty, DateTime scrapDate)
        {
            var scrap = new Scrap(vesselInCompany, secondParty, scrapDate, scrapDomainService, vesselDomainService, companyDomainService);

            var init = this.workflowRepository.Single(c => c.WorkflowEntity == WorkflowEntities.Scrap && c.CurrentWorkflowStage == WorkflowStages.Initial);
            if (init == null)
                throw new ObjectNotFound("ScrapInitialStep");

            var scrapWorkflow = new ScrapWorkflowLog(scrap, WorkflowEntities.Scrap, DateTime.Now, WorkflowActions.Init, 1, "", init.Id, true);

            scrap.ApproveWorkflows.Add(scrapWorkflow);


            scrapConfigurator.Configure(scrap);

            return scrap;
        }

        public ScrapDetail CreateScrapDetail(Scrap scrap, double rob, double price, Currency currency, Good good, GoodUnit unit, Tank tank)
        {
            var scrapDetail = new ScrapDetail(rob, price, currency, good, unit, tank, scrap,
                scrapDomainService, tankDomainService, currencyDomainService,
                goodDomainService, goodUnitDomainService);

            return scrapDetail;
        }
    }
}