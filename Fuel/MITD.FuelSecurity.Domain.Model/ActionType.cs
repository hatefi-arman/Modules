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
        public static IEnumerable<ActionType> GetAllActions()
        {
            var fields =
                (typeof (ActionType)).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                ActionType actionType = fieldInfo.GetValue((object)null) as ActionType;
                if (actionType != null)
                    yield return actionType;
            }
        }

        public static ActionType FromValue(string value)
        {
            return parse(value, "value", (Func<ActionType, bool>) (item => item.Value == value));
        }

        private static ActionType parse(string value, string description, Func<ActionType, bool> func)
        {
            ActionType actionType = GetAllActions().FirstOrDefault(func);
            if (actionType == null)
                throw new Exception();
            else
                return actionType;
        }

        #region Charter In Action
        public static readonly ActionType AddCharterIn = new ActionType("100", "AddCharterIn", "افزودن چارتر In");
        public static readonly ActionType EditCharterIn = new ActionType("101", "EditCharterIn", "ویرایش چارتر In");
        public static readonly ActionType DeleteCharterIn = new ActionType("102", "DeleteCharterIn", "حذف چارتر In");
        public static readonly ActionType AddCharterInItem = new ActionType("103", "AddCharterInItem", "افزودن چارتر In");
        public static readonly ActionType EditCharterInItem = new ActionType("104", "EditCharterInItem", "ویرایش چارتر In");
        public static readonly ActionType DeleteCharterInItem = new ActionType("105", "DeleteCharterInItem", "حذف چارتر In");
        public static readonly ActionType AddCharterOut = new ActionType("106", "AddCharterOut", "افزودن چارتر Out");
        public static readonly ActionType EditCharterOut = new ActionType("107", "EditCharterOut", "ویرایش چارتر Out");
        public static readonly ActionType DeleteCharterOut = new ActionType("108", "DeleteCharterOut", "حذف چارتر Out");
        public static readonly ActionType AddCharterOutItem = new ActionType("109", "AddCharterOutItem", "افزودن چارتر Out");
        public static readonly ActionType EditCharterOutItem = new ActionType("110", "EditCharterOutItem", "ویرایش چارتر Out");
        public static readonly ActionType DeleteCharterOutItem = new ActionType("111", "DeleteCharterOutItem", "حذف چارتر Out");
        #endregion
        
        
        public ActionType(string value,string displayName,string discription)
        {
            Value = value;
            DisplayName = displayName;
            Discription = discription;
           
        }

        public string Value { get; set; }
        public string DisplayName { get; set; }
        public string Discription { get; set; }


    }

    

}
