﻿using MITD.DataAccess.EF;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Data.EF.Repositories
{
    public class WorkflowLogRepository :EFRepository<WorkflowLog>, IWorkflowLogRepository
    {
         public WorkflowLogRepository(EFUnitOfWork efUnitOfWork)
            : base(efUnitOfWork)
        {
              
        }

         public WorkflowLogRepository(IUnitOfWorkScope iUnitOfWorkScope)
            : base(iUnitOfWorkScope)
        {

        }

        public void UpdateApprovalState(WorkflowLog entityApprovalWorkFlowLog, long user, WorkflowEntities actionEntity, string remark)
        {
            
        }
    }
}
