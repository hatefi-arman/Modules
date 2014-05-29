#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.Enums;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class ApprovalResult
    {
        public long EntityId { get; set; }

        public long ActorId { get; set; }

        public string Remark { get; set; }

        public WorkflowActions WorkflowAction { get; set; }

        public WorkflowEntities Entity { get; set; }

        public DecisionTypes DecisionType { get; set; }
    }
}