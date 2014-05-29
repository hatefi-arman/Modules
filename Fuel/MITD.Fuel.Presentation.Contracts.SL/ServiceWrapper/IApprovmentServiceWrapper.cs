using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation;
using MITD.Presentation.Contracts;
using System;


namespace MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper
{
    public interface IApprovmentServiceWrapper : IServiceWrapper
    {
         void GetAll(Action<PageResultDto<ApprovmentDto>, Exception> action, string methodName, int pageSize,
                          int pageIndex);

        void GetById(Action<ApprovmentDto, Exception> action, int id);

        void Add(Action<ApprovmentDto, Exception> action, ApprovmentDto ent);

        void Update(Action<ApprovmentDto, Exception> action, ApprovmentDto ent);

        void Delete(Action<string, Exception> action, int id);
    }
    public interface IApproveServiceWrper
    {
        void LogApproveAction(ApprovmentDto logobj);
        void DoApproveAction(ApprovmentDto approvAction);
    }
}
