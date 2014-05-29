using System;
using System.Windows;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Contracts.SL.Controllers
{
    public interface IFuelController:IApplicationController
    {
        void HandleException(Exception exp);
        void GetRemoteInstance<T>(Action<T, Exception> action) where T : class;
        UserDto GetCurrentUser();

    }
}
