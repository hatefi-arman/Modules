using System.Web.Http;

using MITD.Services;
using System;

namespace MITD.Main.Services.Host.Controllers
{
    public class DeploymentController : ApiController
    {

        public string GetXapVersion(string fileName)
        {
            return DeploymentServiceHelper.GetXapFileVersion(fileName);
        }

        public string[] PostXapVersions(string[] fileNames)
        {
            return DeploymentServiceHelper.GetXapFileVersion(fileNames);
        }

         
    }
}
