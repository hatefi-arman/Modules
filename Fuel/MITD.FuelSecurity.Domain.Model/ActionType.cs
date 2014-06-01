using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{


    public class ActionType
    {
        private enum ActionTypes
        {
            AddCharterIn,
            EditCharterIn,
            DeleteCharterIn,
            AddCharterInItem,
            EditCharterInItem,
            DeleteCharterInItem,
            AddCharterOut,
            EditCharterOut,
            DeleteCharterOut,
            AddCharterOutItem,
            EditCharterOutItem,
            DeleteCharterOutItem,
            ViewFuelReports,
            EditFuelReport,
            ImportFuelReports,
            ManageFuelReportApprovement,
            ManageFuelReportSubmittion,
            ViewScraps,
            CreateScrap,
            EditScrap,
            RemoveScrap,
            ManageScrapApprovement,
            ManageScrapSubmittion,
            ViewOrders,
            CreateOrder,
            EditOrder,
            RemoveOrder,
            ManageOrderApprovement,
            ManageOrderSubmittion,
            CancelOrder,
            ViewInvoices,
            RegisterInvoice,
            EditInvoice,
            RemoveInvoice,
            ManageInvoiceApprovement,
            ManageInvoiceSubmittion,
            ManageEffectiveFactors,
            ViewOffhires,
            ImportOffhire,
            EditOffhire,
            RemoveOffhire,
            ManageOffhireApprovement,
            ManageOffhireSubmittion,
        }

     
        #region Charter In Actions
        public static readonly ActionType AddCharterIn = new ActionType((int)ActionTypes.AddCharterIn, "AddCharterIn", "افزودن چارتر In");
        public static readonly ActionType EditCharterIn = new ActionType((int)ActionTypes.EditCharterIn, "EditCharterIn", "ویرایش چارتر In");
        public static readonly ActionType DeleteCharterIn = new ActionType((int)ActionTypes.DeleteCharterIn, "DeleteCharterIn", "حذف چارتر In");
        public static readonly ActionType AddCharterInItem = new ActionType((int)ActionTypes.AddCharterInItem, "AddCharterInItem", "افزودن ردیف چارتر In");
        public static readonly ActionType EditCharterInItem = new ActionType((int)ActionTypes.EditCharterInItem, "EditCharterInItem", "ویرایش ردیف چارتر In");
        public static readonly ActionType DeleteCharterInItem = new ActionType((int)ActionTypes.DeleteCharterInItem, "DeleteCharterInItem", "حذف ردیف چارتر In");
        public static readonly ActionType AddCharterOut = new ActionType((int)ActionTypes.AddCharterOut, "AddCharterOut", "افزودن چارتر Out");
        public static readonly ActionType EditCharterOut = new ActionType((int)ActionTypes.EditCharterOut, "EditCharterOut", "ویرایش چارتر Out");
        public static readonly ActionType DeleteCharterOut = new ActionType((int)ActionTypes.DeleteCharterOut, "DeleteCharterOut", "حذف چارتر Out");
        public static readonly ActionType AddCharterOutItem = new ActionType((int)ActionTypes.AddCharterOutItem, "AddCharterOutItem", "افزودن ردیف چارتر Out");
        public static readonly ActionType EditCharterOutItem = new ActionType((int)ActionTypes.EditCharterOutItem, "EditCharterOutItem", "ویرایش ردیف چارتر Out");
        public static readonly ActionType DeleteCharterOutItem = new ActionType((int)ActionTypes.DeleteCharterOutItem, "DeleteCharterOutItem", "حذف ردیف چارتر Out");
        #endregion

        #region FuelReport Actions

        public static readonly ActionType ViewFuelReports = new ActionType((int)ActionTypes.ViewFuelReports, "ViewFuelReports", "مشاهده لیست گزارشات سوخت");
        public static readonly ActionType EditFuelReport = new ActionType((int)ActionTypes.EditFuelReport, "EditFuelReport", "ویرایش گزارش سوخت");
        public static readonly ActionType ImportFuelReports = new ActionType((int)ActionTypes.ImportFuelReports, "ImportFuelReports", "ثبت دستی گزارشات سوخت");
        public static readonly ActionType ManageFuelReportApprovement = new ActionType((int)ActionTypes.ManageFuelReportApprovement, "ManageFuelReportApprovement", "انجام تأییدات میانی گزارش سوخت");
        public static readonly ActionType ManageFuelReportSubmittion = new ActionType((int)ActionTypes.ManageFuelReportSubmittion, "ManageFuelReportSubmittion", "انجام تأیید نهایی گزارش سوخت");

        #endregion

        #region Scrap Actions

        public static readonly ActionType ViewScraps = new ActionType((int)ActionTypes.ViewScraps, "ViewScraps", "مشاهده لیست Scrap");
        public static readonly ActionType CreateScrap = new ActionType((int)ActionTypes.CreateScrap, "CreateScrap", "ایجاد Scrap");
        public static readonly ActionType EditScrap = new ActionType((int)ActionTypes.EditScrap, "EditScrap", "ویرایش Scrap");
        public static readonly ActionType RemoveScrap = new ActionType((int)ActionTypes.RemoveScrap, "RemoveScrap", "حذف Scrap");
        public static readonly ActionType ManageScrapApprovement = new ActionType((int)ActionTypes.ManageScrapApprovement, "ManageScrapApprovement", "انجام تأییدات میانی Scrap");
        public static readonly ActionType ManageScrapSubmittion = new ActionType((int)ActionTypes.ManageScrapSubmittion, "ManageScrapSubmittion", "انجام تأیید نهایی Scrap");

        #endregion

        #region Order Actions

        public static readonly ActionType ViewOrders = new ActionType((int)ActionTypes.ViewOrders, "ViewOrders", "مشاهده لیست سفارشات");
        public static readonly ActionType CreateOrder = new ActionType((int)ActionTypes.CreateOrder, "CreateOrder", "ایجاد سفارش");
        public static readonly ActionType EditOrder = new ActionType((int)ActionTypes.EditOrder, "EditOrder", "ویرایش سفارش");
        public static readonly ActionType RemoveOrder = new ActionType((int)ActionTypes.RemoveOrder, "RemoveOrder", "حذف سفارش");
        public static readonly ActionType ManageOrderApprovement = new ActionType((int)ActionTypes.ManageOrderApprovement, "ManageOrderApprovement", "انجام تأییدات میانی سفارش");
        public static readonly ActionType ManageOrderSubmittion = new ActionType((int)ActionTypes.ManageOrderSubmittion, "ManageOrderSubmittion", "انجام تأییدات نهایی سفارش");
        public static readonly ActionType CancelOrder = new ActionType((int)ActionTypes.CancelOrder, "CancelOrder", "کنسل نمودن سفارش");


        #endregion

        #region Offhire Actions

        public static readonly ActionType ViewOffhires = new ActionType((int)ActionTypes.ViewOffhires, "ViewOffhires", "نمایش آف هایر");
        public static readonly ActionType ImportOffhire= new ActionType((int)ActionTypes.ImportOffhire, "ImportOffhire", "ثبت آف هایر");
        public static readonly ActionType EditOffhire= new ActionType((int)ActionTypes.EditOffhire, "EditOffhire", "اصلاح آف هایر");
        public static readonly ActionType RemoveOffhire= new ActionType((int)ActionTypes.RemoveOffhire, "RemoveOffhire", "حذف آف هایر");
        public static readonly ActionType ManageOffhireApprovement = new ActionType((int)ActionTypes.ManageOffhireApprovement, "ManageOffhireApprovement", "مدیریت تایید آف هایر");
        public static readonly ActionType ManageOffhireSubmittion = new ActionType((int)ActionTypes.ManageOffhireSubmittion, "ManageOffhireSubmittion", "مدیریت ارسال آف هایر");

        #endregion

        #region Invoice & Effective Factors Actions

        public static readonly ActionType ViewInvoices = new ActionType((int)ActionTypes.ViewInvoices, "ViewInvoices", "نمایش صورتحساب");
        public static readonly ActionType RegisterInvoice = new ActionType((int)ActionTypes.RegisterInvoice, "RegisterInvoice", "ثبت صورتحساب");
        public static readonly ActionType EditInvoice = new ActionType((int)ActionTypes.EditInvoice, "EditInvoice", "اصلاح صورتحساب");
        public static readonly ActionType RemoveInvoice = new ActionType((int)ActionTypes.RemoveInvoice, "RemoveInvoice", "حذف صورتحساب");
        public static readonly ActionType ManageInvoiceApprovement = new ActionType((int)ActionTypes.ManageInvoiceApprovement, "ManageInvoiceApprovement", "مدیریت تایید صورتحساب");
        public static readonly ActionType ManageInvoiceSubmittion = new ActionType((int)ActionTypes.ManageInvoiceSubmittion, "ManageInvoiceSubmittion", "مدیریت ارسال صورتحساب");
        public static readonly ActionType ManageEffectiveFactors = new ActionType((int)ActionTypes.ManageEffectiveFactors, "ManageEffectiveFactors", "مدیریت عوامل تاثیر گزار");

        #endregion



       

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ActionType()
        {

        }

        public ActionType(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;

        }

        public static IEnumerable<ActionType> GetAllActions()
        {
            var fields =
                (typeof(ActionType)).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                ActionType actionType = fieldInfo.GetValue((object)null) as ActionType;
                if (actionType != null)
                    yield return actionType;
            }
        }

        public static ActionType FromValue(int actionTypeId)
        {
            var actionType = GetAllActions().FirstOrDefault(item => item.Id == actionTypeId);

            if (actionType == null)
                throw new Exception();

            return actionType;

        }

       

    }
}





