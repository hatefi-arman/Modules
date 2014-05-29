using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace MITD.Fuel.Service.Host.Infrastructure
{
    public class GlobalExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private readonly IFuelApplicationExceptionAdapter _fuelApplicationExceptionAdapter;

        public GlobalExceptionHandlingAttribute(IFuelApplicationExceptionAdapter fuelApplicationExceptionAdapter)
            : base()
        {
            _fuelApplicationExceptionAdapter = fuelApplicationExceptionAdapter;
        }



        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null) return;

            var exceptionMessage = _fuelApplicationExceptionAdapter.Get(actionExecutedContext.Exception);
            actionExecutedContext.Response = new HttpResponseMessage
                                                 {
                                                     StatusCode = HttpStatusCode.InternalServerError,
                                                     Content = new StringContent("ERROR")
                                                 };
            actionExecutedContext.Response.Headers.Add("exception", exceptionMessage.GetJson());
        }
    }
}