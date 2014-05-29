using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.FuelSecurity.Domain.Model.ErorrException;

namespace MITD.FuelSecurity.Domain.Model
{
    public class Party
    {
        #region Prop

        private readonly byte[] rowVersion;
        protected IDictionary<int, bool> customActions = new Dictionary<int, bool>();
        public virtual IDictionary<int, bool> CustomActions
        {
            get { return customActions.ToDictionary(c => c.Key, c => c.Value); }
        }

        private long id;
        public long Id { get; private set; }


        #endregion

        #region ctor

        public Party()
        {

        }

        public Party(long? id)
        {
            if (id == null)
                throw new ArgumentNullException();
            this.Id = id.Value;
        }

        #endregion

        #region Method

        public virtual void AssignCustomActions(ActionType actionType, bool value)
        {
            if (actionType == null)
                throw new FuelSecurityException(0, "عدم دسترسی مناسب");
            int actid = int.Parse(actionType.Value);
            if (CustomActions.Keys.Contains(actid))
                customActions.Remove(actid);
            customActions.Add(int.Parse(actionType.Value), value);

        }

        public virtual void UpdateCustomActions(Dictionary<ActionType, bool> customActionsParam)
        {
            var actions = customActionsParam.Keys;
            foreach (var action in actions)
            {
                if (customActions.Keys.Contains(int.Parse(action.Value)))
                    customActions[int.Parse(action.Value)] = customActionsParam[action];
                
                else
                {
                    customActions.Add(int.Parse(action.Value), customActionsParam[action]);
                }

                var actids = new List<int>(customActions.Keys);
                actids.ForEach(c =>
                               {
                                   if (!customActionsParam.Keys.Select(a => a.Value).Contains(c.ToString()))
                                       customActions.Remove(c);
                               });

            }


        }

        #endregion

    }
}
