﻿using System;
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

        public static readonly ActionType ViewFuelReports;
        public static readonly ActionType EditFuelReport;
        public static readonly ActionType ImportFuelReports;
        public static readonly ActionType ManageFuelReportApprovement;
        public static readonly ActionType ManageFuelReportSubmittion;

        #endregion

        #region Scrap Actions

        public static readonly ActionType ViewScraps;
        public static readonly ActionType CreateScrap;
        public static readonly ActionType EditScrap;
        public static readonly ActionType RemoveScrap;
        public static readonly ActionType ManageScrapApprovement;
        public static readonly ActionType ManageScrapSubmittion;

        #endregion

        #region Order Actions

        public static readonly ActionType ViewOrders;
        public static readonly ActionType CreateOrder;
        public static readonly ActionType EditOrder;
        public static readonly ActionType RemoveOrder;
        public static readonly ActionType ManageOrderApprovement;
        public static readonly ActionType ManageOrderSubmittion;
        public static readonly ActionType CancelOrder;


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





