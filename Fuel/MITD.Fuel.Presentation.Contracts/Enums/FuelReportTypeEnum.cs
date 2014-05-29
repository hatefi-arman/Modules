using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Presentation.Contracts.Enums
{
    public enum FuelReportTypeEnum
    {
        None = 0,
        NoonReport = 1,
        EndofVoyage = 2,
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
