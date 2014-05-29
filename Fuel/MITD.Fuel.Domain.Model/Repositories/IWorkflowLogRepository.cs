﻿#region

using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.Enums;

#endregion

namespace MITD.Fuel.Domain.Model.Repositories
{
    public interface IWorkflowLogRepository : IRepository<WorkflowLog>
    {
        void UpdateApprovalState(WorkflowLog entityApprovalWorkFlowLog, long user, WorkflowEntities actionEntity, string remark);
    }
}