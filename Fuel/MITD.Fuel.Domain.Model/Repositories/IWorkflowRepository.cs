#region

using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace MITD.Fuel.Domain.Model.Repositories
{
    public interface IWorkflowRepository : IRepository<WorkflowStep>
    {
    }
}