using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Services.Application;

namespace MITD.Fuel.Application.Service.Contracts
{
    public interface IApproveFlowApplicationService : IApplicationService
    {
        ApprovalResult ActApprovalFlow(long entityId, WorkflowEntities workflowEntity, long approverId, string remark, WorkflowActions action);
    }
}
