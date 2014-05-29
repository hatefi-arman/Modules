using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;

namespace MITD.Fuel.Integration.VesselReportManagementSystem.Mapper
{
    public class ReportMapper
    {
        public static FuelReportCommandDto Map(Data.RPMInfo rpmInfo)
        {
            FuelReportCommandDto res = new FuelReportCommandDto();
            res.FuelReportDetails = new List<FuelReportCommandDetailDto>();
            res.VesselReportReference = rpmInfo.ID.ToString();

            res.VesselCode = rpmInfo.Ship.ShipID;

            res.FuelReportType = GetFuelReportTypeEnum(rpmInfo.FuelRepotType.Value);

            res.ReportDate = new DateTime(rpmInfo.Year.Value, rpmInfo.Month.Value, rpmInfo.Day.Value);

            res.VoyageNumber = rpmInfo.VoyageNo;

            res.IsActive = true;


            if (rpmInfo.ROBHO != null && rpmInfo.ROBHO.Value > 0)
            {
                //HFO
                var hFoFuelReportCommandDetailDto = new FuelReportCommandDetailDto();
                hFoFuelReportCommandDetailDto.Consumption = Convert.ToDecimal(rpmInfo.ConsInPortHO) + Convert.ToDecimal(rpmInfo.ConsAtSeaHO);
                hFoFuelReportCommandDetailDto.Transfer = rpmInfo.TransferHo;

                hFoFuelReportCommandDetailDto.ROB = Convert.ToDecimal(rpmInfo.ROBHO);

                hFoFuelReportCommandDetailDto.Recieve = Convert.ToDecimal(rpmInfo.ReceivedHO);

                hFoFuelReportCommandDetailDto.Correction = Convert.ToDecimal(rpmInfo.CorrectionHo);
                hFoFuelReportCommandDetailDto.CorrectionTypeEnum =
                    (rpmInfo.CorrectionTypeHo != null && rpmInfo.CorrectionTypeHo.Value) ? CorrectionTypeEnum.Plus : CorrectionTypeEnum.Minus;

                hFoFuelReportCommandDetailDto.Unit = "TON";
                hFoFuelReportCommandDetailDto.FuelType = "HFO";

                res.FuelReportDetails.Add(hFoFuelReportCommandDetailDto);

            }


            if (rpmInfo.ROBDO != null && rpmInfo.ROBDO.Value > 0)
            {
                //MDO
                var hFoFuelReportCommandDetailDto = new FuelReportCommandDetailDto();
                hFoFuelReportCommandDetailDto.Consumption = Convert.ToDecimal(rpmInfo.ConsInPortDO) + Convert.ToDecimal(rpmInfo.ConsAtSeaDO);
                hFoFuelReportCommandDetailDto.Transfer = rpmInfo.TransferDo;

                hFoFuelReportCommandDetailDto.ROB = Convert.ToDecimal(rpmInfo.ROBDO);

                hFoFuelReportCommandDetailDto.Recieve = Convert.ToDecimal(rpmInfo.ReceivedDO);

                hFoFuelReportCommandDetailDto.Correction = Convert.ToDecimal(rpmInfo.CorrectionDo);
                hFoFuelReportCommandDetailDto.CorrectionTypeEnum =
                    (rpmInfo.CorrectionTypeHo != null && rpmInfo.CorrectionTypeDo.Value) ? CorrectionTypeEnum.Plus : CorrectionTypeEnum.Minus;

                hFoFuelReportCommandDetailDto.Unit = "TON";
                hFoFuelReportCommandDetailDto.FuelType = "MDO";

                res.FuelReportDetails.Add(hFoFuelReportCommandDetailDto);

            }

            if (rpmInfo.ROBMGO != null && rpmInfo.ROBMGO.Value > 0)
            {
                //MGO
                var hFoFuelReportCommandDetailDto = new FuelReportCommandDetailDto();
                hFoFuelReportCommandDetailDto.Consumption = Convert.ToDecimal(rpmInfo.ConsInPortMGO) + Convert.ToDecimal(rpmInfo.ConsAtSeaMGO);
                hFoFuelReportCommandDetailDto.Transfer = rpmInfo.TransferMGOLS;

                hFoFuelReportCommandDetailDto.ROB = Convert.ToDecimal(rpmInfo.ROBMGO);

                hFoFuelReportCommandDetailDto.Recieve = Convert.ToDecimal(rpmInfo.ReceivedMGO);

                hFoFuelReportCommandDetailDto.Correction = Convert.ToDecimal(rpmInfo.CorrectionMGOLS);
                hFoFuelReportCommandDetailDto.CorrectionTypeEnum =
                    (rpmInfo.CorrectionTypeHo != null && rpmInfo.CorrectionTypeMGOLS.Value) ? CorrectionTypeEnum.Plus : CorrectionTypeEnum.Minus;

                hFoFuelReportCommandDetailDto.Unit = "TON";
                hFoFuelReportCommandDetailDto.FuelType = "MGO";

                res.FuelReportDetails.Add(hFoFuelReportCommandDetailDto);

            }


            return res;

        }

        public static FuelReportTypeEnum GetFuelReportTypeEnum(int i)
        {
            switch (i)
            {
                case 1:
                    return FuelReportTypeEnum.NoonReport;
                case 2:
                    return FuelReportTypeEnum.EndofVoyage;
                case 3:
                    return FuelReportTypeEnum.CharterOutStart;
                case 4:
                    return FuelReportTypeEnum.CharterInEnd;
                case 5:
                    return FuelReportTypeEnum.EndOfYear;
                case 6:
                    return FuelReportTypeEnum.EndOfMonth;
                case 7:
                    return FuelReportTypeEnum.ArrivalReport;
                case 8:
                    return FuelReportTypeEnum.DepartureReport;
                default:
                    return FuelReportTypeEnum.None;
            }

        }
    }
}