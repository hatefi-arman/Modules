#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace MITD.Fuel.Domain.Model.Enums
{
    public enum FuelReportTypes
    {
        NoonReport = 1,
        EndOfVoyage = 2,
        ArrivalReport = 3,
        DepartureReport = 4,
        EndOfYear = 5,
        EndOfMonth = 6,
        CharterInEnd = 7,
        CharterOutStart = 8,
        DryDock = 9,
        OffHire = 10,
        LayUp = 11,
    }
}