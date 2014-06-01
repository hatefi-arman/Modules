using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
    public class User : Party
    {
        #region Prop
        public virtual List<ActionType> Actions
        {
            get
            {
                return new List<ActionType>();
            }
        }


        public virtual List<Group> Groups { get; set; }

        public virtual string FirstName { get; private set; }

        public virtual string LastName { get; private set; }

        public virtual string Email { get; private set; }

        public virtual bool Active { get; private set; }

        #endregion

        #region ctor

        public User()
            : base()
        {
            this.Groups = new List<Group>();
        }

        public User(long id, string partyName, string firstName, string lastName, string email, bool active)
            : base(id, partyName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Active = active;
            this.Groups = new List<Group>();

        }

        public User(long id, string partyName, string firstName, string lastName, string email)
            : base(id, partyName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Active = true;
            this.Groups = new List<Group>();
        }
        #endregion

        #region Method

        public virtual void Update(string firstName, string lastName, string email, bool active,
            Dictionary<int, bool> acions, List<Group> groups)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Active = active;
            UpdateGroup(groups);
        }

        public virtual void UpdateGroup(List<Group> groups)
        {
            if (groups == null)
                return;

            foreach (var group in groups)
            {
                AssignGroup(group);
            }


            this.Groups.RemoveAll(localg => groups.Count(gparam => gparam.Id == localg.Id) == 0);
        }

        public virtual void AssignGroup(Group group)
        {
            if (group == null)
                throw new NullReferenceException();


            if (this.Groups.Count(g => g.Id == group.Id) == 0)
                this.Groups.Add(group);
        }

        public virtual void RemoveGroup(Group group)
        {
            this.Groups.RemoveAll(g => g.Id == group.Id);
        }


        #endregion
    }
}
