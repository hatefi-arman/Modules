using System.Collections.Generic;

namespace MITD.FuelSecurity.Domain.Model
{
    public class AdminUser : User
    {
        public AdminUser(long id, string firstName, string lastName, string email)
            : base(id, "AdminUser", firstName, lastName, email)
        {

        }

        public override List<ActionType> Actions
        {
            get
            {
                return new List<ActionType>()
                {
                    ActionType.AddCharterIn,
                    ActionType.EditCharterIn,
                    ActionType.DeleteCharterIn,
                    ActionType.AddCharterInItem,
                    ActionType.EditCharterInItem,
                    ActionType.DeleteCharterInItem,
                    ActionType.AddCharterOut,
                    ActionType.EditCharterOut,
                    ActionType.DeleteCharterOut,
                    ActionType.AddCharterOutItem,
                    ActionType.EditCharterOutItem,
                    ActionType.DeleteCharterOutItem,
                    ActionType.ViewFuelReports,
                    ActionType.EditFuelReport,
                    ActionType.ImportFuelReports,
                    ActionType.ManageFuelReportApprovement,
                    ActionType.ManageFuelReportSubmittion,
                    ActionType.ViewScraps,
                    ActionType.CreateScrap,
                    ActionType.EditScrap,
                    ActionType.RemoveScrap,
                    ActionType.ManageScrapApprovement,
                    ActionType.ManageScrapSubmittion,
                    ActionType.ViewOrders,
                    ActionType.CreateOrder,
                    ActionType.EditOrder,
                    ActionType.RemoveOrder,
                    ActionType.ManageOrderApprovement,
                    ActionType.ManageOrderSubmittion,
                    ActionType.CancelOrder,
                    ActionType.ViewInvoices,
                    ActionType.RegisterInvoice,
                    ActionType.EditInvoice,
                    ActionType.RemoveInvoice,
                    ActionType.ManageInvoiceApprovement,
                    ActionType.ManageInvoiceSubmittion,
                    ActionType.ManageEffectiveFactors,
                    ActionType.ViewOffhires,
                    ActionType.ImportOffhire,
                    ActionType.EditOffhire,
                    ActionType.RemoveOffhire,
                    ActionType.ManageOffhireApprovement,
                    ActionType.ManageOffhireSubmittion,
                };
            }
        }
    }
}
