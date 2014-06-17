using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;


namespace MITD.Fuel.Domain.Model.DomainObjects
{
    /// <summary>
    /// There are no comments for MITD.Fuel.Domain.Model.DomainObjects.Voyage in the schema.
    /// </summary>
    public class Voyage
    {
        public Voyage()
        {
        }

        public Voyage(
            long id,
            string voyageNumber,
            string description,
            long vesselInCompanyId,
            long companyId,
            DateTime startDate,
            DateTime? endDate,
            bool isActive = true
            )
        {
            Id = id;
            this.VoyageNumber = voyageNumber;
            Description = description;
            VesselInCompanyId = vesselInCompanyId;
            CompanyId = companyId;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = isActive;
        }

        public long Id { get; set; }
        public string VoyageNumber { get; set; }
        public string Description { get; set; }
        public long VesselInCompanyId { get; set; }
        public long CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; private set; }

        public virtual VesselInCompany VesselInCompany { get; set; }
        public virtual Company Company { get; set; }

        public void CancelVoyage()
        {
            this.IsActive = false;
        }
    }
}