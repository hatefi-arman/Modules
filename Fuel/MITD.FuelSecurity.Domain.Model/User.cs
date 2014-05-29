using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
    public class User:Party
    {
        #region Prop

        public virtual List<ActionType> Actions { get; set; }
        
        IList<Group> _groups=new List<Group>(); 
        public IReadOnlyList<Group> Groups { get { return _groups.ToList().AsReadOnly(); } }


        private string fristName ;
        public virtual string FirstName
        {
            get { return fristName; }
        }

        private string lastName;
        public virtual string LasttName
        {
            get { return lastName; }
        }

        private string email    ;
        public virtual string Email 
        {
            get { return email; }
        }

        private bool active ;
        public virtual bool Active
        {
            get { return active; }
        }
       
        #endregion

        #region ctor

        public User()
        {
                
        }
        public User(long id, string firstName,string lastName,string email,bool active):base(id)
        {
            this.fristName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.active = active;
           
        }

        public User(long id, string firstName, string lastName, string email)
            : base(id)
        {
            this.fristName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.active = true;

        }
        #endregion

        #region Method

        public virtual void Update(string firstName, string lastName, string email, bool active,
            Dictionary<int,bool> acions,List<long> groups)
        {
            this.fristName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.active = active;
         //   UpdateGroup(groups);
        }

        public virtual void UpdateGroup(List<Group> groups)
        {
            if (groups==null)
                return;

            foreach (var group in groups)
            {
                if (!_groups.Contains(group))
                    AssignGroup(group);
            }

            for (int i = 0; i < _groups.Count; i++)
            {
                if(!groups.Contains(_groups[i]))
                    RemoveGroup(_groups[i]);
            }
            
        }

        public virtual void AssignGroup(Group group)
        {
            if (group==null)
                throw  new NullReferenceException();
            _groups.Add(group);


        }

        public virtual void RemoveGroup(Group group)
        {
            _groups.Remove(group);
        }


        #endregion
    }
}
