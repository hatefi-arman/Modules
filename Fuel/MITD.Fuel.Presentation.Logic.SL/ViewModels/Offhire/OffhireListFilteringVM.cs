using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Castle.DynamicProxy.Generators.Emitters;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels
{
    public class OffhireListFilteringVM : WorkspaceViewModel
    {
        private ObservableCollection<VesselDto> vessels;
        public ObservableCollection<VesselDto> Vessels
        {
            get { return vessels; }
            set { this.SetField(p => p.Vessels, ref vessels, value); }
        }

        private VesselDto selectedVessel;
        public VesselDto SelectedVessel
        {
            get { return selectedVessel; }
            set { this.SetField(p => p.SelectedVessel, ref selectedVessel, value); }
        }

        public long? SelectedVesselId
        {
            get { return (SelectedVessel == null || SelectedVessel.Id == long.MinValue) ? null : (long?)SelectedVessel.Id; }
        }

        private DateTime? fromDate;
        public DateTime? FromDate
        {
            get { return fromDate; }
            set { this.SetField(p => p.FromDate, ref fromDate, value); }
        }

        private DateTime? toDate;
        public DateTime? ToDate
        {
            get { return toDate; }
            set { this.SetField(p => p.ToDate, ref toDate, value); }
        }

        public OffhireListFilteringVM()
        {
            this.Vessels = new ObservableCollection<VesselDto>();
        }

        public void Initialize(IEnumerable<VesselDto> vesselDtos)
        {
            this.Vessels.Clear();

            this.Vessels.Add(new VesselDto()
                             {
                                 Id = long.MinValue,
                                 Code = string.Empty,
                                 Name = string.Empty
                             });

            foreach (var vessel in vesselDtos)
            {
                this.Vessels.Add(vessel);
            }

            ResetToDefaults();
        }

        public void ResetToDefaults()
        {
            this.SelectedVessel = null;

            this.FromDate = null;

            this.ToDate = null;
        }
    }
}
