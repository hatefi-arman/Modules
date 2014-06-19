#region

using System;
using System.Windows;
using MITD.Fuel.Presentation.Contracts.SL.Infrastructure;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Presentation.Contracts;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Logic.SL.Infrastructure;
using System.IO;

#endregion

namespace MITD.Fuel.Presentation.FuelApp.Logic.SL.ServiceWrapper
{
    public class ApprovalFlowServiceWrapper : IApprovalFlowServiceWrapper
    {
        #region fields

        private readonly string approvalFlowAddressStringFormat = Path.Combine(ApiConfig.HostAddress,
                                                                               "apiarea/Fuel/ApprovalFlow/{0}");

        #endregion

        #region  methodes

        public void ActApproveFlow(Action<ApprovmentDto, Exception> action, long entityId, ActionEntityTypeEnum entityTypeId)
        {
            var url = string.Format(approvalFlowAddressStringFormat, entityId);
            var ent = new ApprovmentDto
                          {
                              EntityId = entityId,
                              DecisionType = DecisionTypeEnum.Approved,
                              ActionEntityType = entityTypeId
                          };

            WebClientHelper.Put<ApprovmentDto, ApprovmentDto>(new Uri(url, UriKind.Absolute),
                                                       (res, exp) => action(res, exp), ent,
                                                       WebClientHelper.MessageFormat.Json,ApiConfig.Headers);
        }

        public void ActRejectFlow(Action<ApprovmentDto, Exception> action, long entityId, ActionEntityTypeEnum entityTypeId)
        {
            var url = string.Format(approvalFlowAddressStringFormat, entityId);
            var ent = new ApprovmentDto
                          {
                              EntityId = entityId,
                              DecisionType = DecisionTypeEnum.Rejected,
                              ActionEntityType = entityTypeId
                          };

            WebClientHelper.Put<ApprovmentDto, ApprovmentDto>(new Uri(url, UriKind.Absolute),
                                                       (res, exp) => action(res, exp), ent,
                                                       WebClientHelper.MessageFormat.Json, ApiConfig.Headers);
        }

        public void ActCancelFlow(Action<ApprovmentDto, Exception> action, long entityId, ActionEntityTypeEnum entityTypeId)
        {
            var url = string.Format(approvalFlowAddressStringFormat, entityId);
            var ent = new ApprovmentDto
                          {
                              EntityId = entityId,
                              DecisionType = DecisionTypeEnum.Canceled,
                              ActionEntityType = entityTypeId
                          };

            WebClientHelper.Put<ApprovmentDto, ApprovmentDto>(new Uri(url, UriKind.Absolute),
                                                       (res, exp) => action(res, exp), ent,
                                                       WebClientHelper.MessageFormat.Json, ApiConfig.Headers);
        }

        #endregion
    }
}