#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using MITD.Fuel.Domain.Model.Enums;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    /// <summary>
    /// There are no comments for MITD.Fuel.Domain.Model.DomainObjects.Vessel in the schema.
    /// </summary>
    public class Vessel
    {
        #region Properties

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CompanyId { get; set; }

        public bool IsActive { get; set; }
        public VesselStates VesselState { get; set; }

        public virtual Company Company { get; set; }

        public virtual List<Tank> Tanks { get; set; }

        #endregion

        public Vessel()
        {
        }

        public Vessel(long id, string code, string name, string description, long companyId, VesselStates vesselState, bool isActive)
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
            CompanyId = companyId;
            VesselState = vesselState;
            IsActive = isActive;
        }
    }
}