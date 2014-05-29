using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation.Contracts;
using System;
using MITD.Fuel.Presentation.Contracts.Enums;



namespace MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper
{
    public interface IApprovalFlowServiceWrapper : IServiceWrapper
    {
        void ActApproveFlow(Action<string, Exception> action, long entityId, ActionEntityTypeEnum entityTypeId);
        void ActRejectFlow(Action<string, Exception> action, long entityId, ActionEntityTypeEnum entityTypeId);
        void ActCancelFlow(Action<string, Exception> action, long entityId, ActionEntityTypeEnum entityTypeId);

    }
  
}
