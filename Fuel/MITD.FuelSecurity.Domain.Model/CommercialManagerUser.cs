using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
    public class CommercialManagerUser:User
    {
        public override List<ActionType> Actions
        {
            get
            {
                return new List<ActionType>()
                       {
                           ActionType.QueryCharterIn,
                           ActionType.AddCharterIn,
                           ActionType.EditCharterIn,
                           ActionType.DeleteCharterIn,
                           ActionType.AddCharterInItem,
                           ActionType.EditCharterInItem,
                           ActionType.DeleteCharterInItem,
                           ActionType.QueryCharterOut,
                           ActionType.AddCharterOut,
                           ActionType.EditCharterOut,
                           ActionType.DeleteCharterOut,
                           ActionType.AddCharterOutItem,
                           ActionType.EditCharterOutItem,
                           ActionType.DeleteCharterOutItem,
                           ActionType.ViewFuelReports,
                           ActionType.EditFuelReport,
                           ActionType.ImportFuelReports,
                           ActionType.ViewOffhires,
                           ActionType.ImportOffhire,
                           ActionType.RemoveOffhire,
                           ActionType.EditOffhire,
                           ActionType.EditInvoice,
                           ActionType.RegisterInvoice,
                           ActionType.RemoveInvoice,
                           ActionType.ViewInvoices,
                           ActionType.CreateScrap,
                           ActionType.EditScrap,
                           ActionType.RemoveScrap,
                           ActionType.ViewScraps,
                           ActionType.CreateOrder,
                           ActionType.EditOrder,
                           ActionType.RemoveOrder,
                           ActionType.ViewOrders

                          
                       };
            }
        }

        public CommercialManagerUser(long id, string firstName, string lastName, string email, string userName)
            : base(id, "CommercialManagerUser", firstName, lastName, email, userName)
        {

        }
    }
}
