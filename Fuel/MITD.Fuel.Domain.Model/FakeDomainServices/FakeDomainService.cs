using System;
using System.Collections.Generic;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.FakeDomainServices
{
    public static class FakeDomainService
    {
        public static List<VesselInCompany> GetVesselsInCompanies()
        {
            return new List<VesselInCompany>
                       {
                           
                           new VesselInCompany("0123", "ABBA", "ABBA In SAPID", 11,  2,VesselStates.Inactive, false),           //1, 
                           //new VesselInCompany("0093", "AGEAN", "AGEAN In SAPID", 11, 3, VesselStates.Inactive, false),             //2, 
                           //new VesselInCompany("AREZOO", "AREZOO", "AREZOO In HAFIZ", 11, 4,VesselStates.Inactive, false),     //3, 
                           //new VesselInCompany("NESHAT", "NESHAT", "NESHAT In HAFIZ", 11, 5,VesselStates.Inactive, false),         //4, 
                           //new VesselInCompany("0123", "ABBA", "ABBA In IRISL", 10, 13, VesselStates.Owned, true),         //5, 
                           //new VesselInCompany("0093", "AGEAN", "AGEAN In IRISL", 10, 3, VesselStates.Owned, true),             //6, 
                           //new VesselInCompany("AREZOO", "AREZOO", "AREZOO In IRISL", 10, 4,VesselStates.Owned, true),   //7, 
                           //new VesselInCompany("NESHAT", "NESHAT", "NESHAT In IRISL", 10, 5,VesselStates.Owned, true),          //8, 
                           //new VesselInCompany("5002", "TABUK", "TABUK In IMSENGCO", 10, 7, VesselStates.Owned, true),    //9, 
                           //new VesselInCompany( "5001", "PAGAS", "PAGAS In IMSENGCO", 10, 7, VesselStates.Owned, true),         //10,
                           //new VesselInCompany( "1093", "TINA", "TINA In SAPID", 11, 1, VesselStates.Owned, true),              //11,
                       };
        }


        public static List<Vessel> GetVessels()
        {
            return new List<Vessel>
                       {
                           new Vessel("1093", 1),  //SAPID      1
                           new Vessel("0123", 4),  //IRISL      2
                           new Vessel("0093", 4),  //IRISL      3
                           new Vessel("AREZOO", 4),//IRISL      4
                           new Vessel("NESHAT", 4),//IRISL      5
                           new Vessel("5002", 3),  //IMSENGCO   6
                           new Vessel("5001", 3)   //IMSENGCO   7
                       };
        }


        //public static List<Company> GetCompanies()
        //{
        //    return new List<Company> { new Company(1, "SAPID", "SAPID"), new Company(2, "HAFIZ", "HAFIZ"), new Company(3, "IMSENGCO", "IMSENGCO"), new Company(4, "IRISL", "IRISL") };
        //}

        public static List<FuelUser> GetUsers()
        {
            return new List<FuelUser> { new FuelUser(1101, "Admin", 1), }; // User In SAPID
        }

        public static List<Voyage> GetVoyages()
        {
            return new List<Voyage>
                       {
                           new Voyage(0, "S4-1089", "S4-1089", 1, 11, new DateTime(2012, 1, 1, 0, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0)),
                           new Voyage(0, "S4-1092", "S4-1092", 1, 11, new DateTime(2014, 1, 2, 17, 0, 1), new DateTime(2014, 12, 12, 8, 0, 0)),
                           //new Voyage(3, "S3-0893", "S3-0893", 2, 1, new DateTime(2014, 3, 1, 0, 0, 0), new DateTime(2014, 4, 1, 0, 0, 0)),
                           //new Voyage(4, "S3-0905", "S3-0905", 2, 1, new DateTime(2014, 4, 1, 0, 0, 0), new DateTime(2014, 5, 1, 0, 0, 0)),
                           //new Voyage(5, "HDM0220N", "HDM0220N", 3, 2, new DateTime(2014, 5, 1, 0, 0, 0), new DateTime(2014, 6, 1, 0, 0, 0)),
                           //new Voyage(6, "HDM0220S", "HDM0220S", 3, 2, new DateTime(2014, 6, 1, 0, 0, 0), new DateTime(2014, 7, 1, 0, 0, 0)),
                           //new Voyage(7, "HDM0221N", "HDM0221N", 4, 2, new DateTime(2014, 7, 1, 0, 0, 0), new DateTime(2014, 8, 1, 0, 0, 0)),
                           //new Voyage(8, "HDM0221S", "HDM0221S", 4, 2, new DateTime(2014, 8, 1, 0, 0, 0), new DateTime(2014, 9, 1, 0, 0, 0))
                       };
        }

        public static List<VoyageLog> CreateVoyagesLog(Voyage voyage)
        {
            return new List<VoyageLog>();
            //{
            //    new VoyageLog
            //        (
            //        0, voyage.Id, DateTime.Now.AddDays(-5), voyage.VoyageNumber , voyage.Description, voyage.VesselInCompany, voyage.Company,
            //        voyage.StartDate.AddHours(1), voyage.EndDate, true),
            //    new VoyageLog
            //        (
            //        0, voyage.Id, DateTime.Now.AddDays(-3), voyage.VoyageNumber, voyage.Description, voyage.VesselInCompany, voyage.Company,
            //        voyage.StartDate, voyage.EndDate.AddHours(1), false),
            //    new VoyageLog
            //        (
            //        0, voyage.Id, DateTime.Now.AddHours(-17), voyage.VoyageNumber, voyage.Description, voyage.VesselInCompany,
            //        voyage.Company, voyage.StartDate, voyage.EndDate.AddHours(3), false),
            //    new VoyageLog
            //        (
            //        0, voyage.Id, DateTime.Now.AddMinutes(-45), voyage.VoyageNumber, voyage.Description, voyage.VesselInCompany,
            //        voyage.Company, voyage.StartDate, voyage.EndDate.AddHours(17), true),
            //};
        }


        //public static List<Tank> GetTanks()
        //{
        //    return new List<Tank> { 
        //        new Tank(1, "Tank1", 1), 
        //        new Tank(2, "Tank2", 1), 
        //        new Tank(3, "Tank3", 2), 
        //        new Tank(4, "Tank4", 2), 
        //        new Tank(5, "Tank5", 3), 
        //        new Tank(6, "Tank6", 11), 
        //        new Tank(7, "Tank7", 11) };
        //}

        //public static List<FuelReport> GetFuelReports()
        //{
        //    return new List<FuelReport>
        //               {
        //                   //new FuelReport(0, "NoonReport1", "NoonReport1", new DateTime(2013, 1, 1, 12, 0, 0) , new DateTime(2013, 1, 1, 12, 0, 0) , 1, 1,
        //                   //               FuelReportTypes.NoonReport),
        //                   //new FuelReport(0, "NoonReport2", "NoonReport2", new DateTime(2013, 1, 2, 12, 0, 0) ,new DateTime(2013, 1, 2, 12, 0, 0)  , 1, null,
        //                   //               FuelReportTypes.NoonReport),
        //                   new FuelReport
        //                       (
        //                       0, "EndOfVoyage1", "EndOfVoyage1", new DateTime(2013, 1, 2, 17, 0, 0), new DateTime(2013, 1, 2, 17, 0, 0), 1, 1,
        //                       FuelReportTypes.EndOfVoyage, States.Open),
        //                   //new FuelReport(0, "NoonReport3", "NoonReport3", new DateTime(2013, 1, 3, 12, 0, 0) ,new DateTime(2013, 1, 3, 12, 0, 0)  , 1, 2,
        //                   //               FuelReportTypes.NoonReport),
        //                   //new FuelReport(0, "NoonReport4", "NoonReport4", new DateTime(2013, 1, 4, 12, 0, 0) ,new DateTime(2013, 1, 4, 12, 0, 0) , 1, 2,
        //                   //               FuelReportTypes.NoonReport),                     
        //                   //new FuelReport(0, "NoonReport5", "NoonReport5", new DateTime(2013, 1, 5, 12, 0, 0) ,new DateTime(2013, 1, 5, 12, 0, 0) , 1, 1,
        //                   //               FuelReportTypes.NoonReport),
        //                   //new FuelReport(0, "EndOfVoyage2", "EndOfVoyage2", new DateTime(2013, 1, 6, 8, 0, 0) ,new DateTime(2013, 1, 6, 8, 0, 0) , 1, 2,
        //                   //               FuelReportTypes.EndOfVoyage),
        //                   //new FuelReport(0, "EndOfYear", "EndOfYear", new DateTime(2013, 1, 6, 12, 0, 0) ,new DateTime(2013, 1, 6, 12, 0, 0) , 1, null,
        //                   //               FuelReportTypes.EndOfYear)
        //               };
        //}

        //public static List<FuelReportDetail> GetFuelReportDetails()
        //{
        //    return new List<FuelReportDetail>
        //               {
        //                   new FuelReportDetail(0, 1, 410, 20, null, null, null, null, null, null, null, null, 1, 1, 1),
        //                   new FuelReportDetail(0, 1, 70, 10, null, null, null, null, null, null, null, null, 2, 1, 2),

        //                   //new FuelReportDetail(0, 1, 470, 15, 0, null, 0, null, 12.5, CorrectionTypes.Minus, 700, 1, 1, 1, 1),
        //                   //new FuelReportDetail(0, 1, 120, 2, 50, ReceiveTypes.InternalTransfer, 20,TransferTypes.TransferSale, null,null,null,null, 2, 1, 2),

        //                   //new FuelReportDetail(0, 2, 430, 13, 13, ReceiveTypes.TransferPurchase, 0,null,  null,null,null,null, 1, 1, 1),
        //                   //new FuelReportDetail(0, 2, 100, 2.5, 0, null, 50,TransferTypes.TransferSale, 1, CorrectionTypes.Minus, 1900, 1, 2, 1, 2),

        //                   //new FuelReportDetail(0, 3, 410, 20, null,null,null,null,null,null,null,null,1, 1, 1),
        //                   //new FuelReportDetail(0, 3, 70, 10, 111, ReceiveTypes.TransferPurchase, 50,TransferTypes.TransferSale, 1, CorrectionTypes.Minus, 1600, 1, 2, 1, 2),

        //                   //new FuelReportDetail(0, 4, 380, 12, 0, null, 65,TransferTypes.InternalTransfer, null,null,null,null, 1, 1, 1),
        //                   //new FuelReportDetail(0, 4, 140, 5, 50, ReceiveTypes.TransferPurchase, 10,TransferTypes.TransferSale, 30, CorrectionTypes.Plus, 1800, 1, 2, 1,2),

        //                   //new FuelReportDetail(0, 5, 350, 20, 13, ReceiveTypes.TransferPurchase, 54,TransferTypes.TransferSale, 1, CorrectionTypes.Minus, 900, 1, 1, 1, 1),
        //                   //new FuelReportDetail(0, 5, 130, 10, 111, ReceiveTypes.TransferPurchase, 50,TransferTypes.TransferSale, 1, CorrectionTypes.Plus, 1850, 1, 2, 1, 2),

        //                   //new FuelReportDetail(0, 6, 380, 11, 0, null,null,null,null,null,null,null,1, 1, 1),
        //                   //new FuelReportDetail(0, 6, 100, 10, 0, null, 0,null, 0, null, 200, 1, 2, 1,2),

        //                   //new FuelReportDetail(0, 7, 320, 15, 0, null,null,null,5,null,null,null,1, 1, 1),
        //                   //new FuelReportDetail(0, 7, 100, 10, 111, ReceiveTypes.TransferPurchase, 50,TransferTypes.TransferSale, 0, null, 200, 1, 2, 1,2),

        //                   //new FuelReportDetail(0, 8, 310, 8, 0, null,null,null,null,null,null,null,1, 1, 1),
        //                   //new FuelReportDetail(0, 8, 100, 10, 0, null, 0,null, 0, null, 200, 1, 2, 1,2)
        //               };
        //}


        public static List<FuelReport> GetFuelReports()
        {
            return new List<FuelReport>
                       {
                           new FuelReport(0, "NoonReport1", "NoonReport1", new DateTime(2014, 1, 1, 12, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0), 1, 1,
                                          FuelReportTypes.NoonReport, States.Open),
                           new FuelReport(0, "NoonReport2", "NoonReport2", new DateTime(2014, 1, 2, 12, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0), 1, null,
                                          FuelReportTypes.NoonReport, States.Open),
                           new FuelReport(0, "EndOfVoyage1", "EndOfVoyage1", new DateTime(2014, 1, 2, 17, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0),1, 1,
                                          FuelReportTypes.EndOfVoyage, States.Open),
                           new FuelReport(0, "NoonReport1", "NoonReport1", new DateTime(2014, 1, 3, 12, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0),1, 2,
                                          FuelReportTypes.NoonReport, States.Open),
                           new FuelReport(0, "NoonReport2", "NoonReport2", new DateTime(2014, 1, 4, 12, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0),1, 1,
                                          FuelReportTypes.NoonReport, States.Open),                     
                           new FuelReport(0, "NoonReport3", "NoonReport3", new DateTime(2014, 1, 5, 12, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0),1, 2,
                                          FuelReportTypes.NoonReport, States.Open),
                           new FuelReport(0, "EndOfVoyage2", "EndOfVoyage2", new DateTime(2014, 1, 6, 8, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0),1,2,
                                          FuelReportTypes.EndOfVoyage, States.Open),
                           new FuelReport(0, "EndOfYear", "EndOfYear", new DateTime(2014, 1, 6, 12, 0, 0), new DateTime(2014, 1, 2, 17, 0, 0),1, null,
                                          FuelReportTypes.EndOfYear, States.Open)
                       };
        }

        public static List<FuelReportDetail> GetFuelReportDetails(List<FuelReport> fuelReports)
        {
            return new List<FuelReportDetail>
                       {
                           new FuelReportDetail(0, fuelReports[0].Id, 470, "Ton", 15, 0, null, 0, null, 12.5, CorrectionTypes.Minus, 700,"USD", 1, 11010, 110101, 12001),
                           new FuelReportDetail(0, fuelReports[0].Id, 120, "Ton", 2, 50, ReceiveTypes.InternalTransfer, 20,TransferTypes.TransferSale, null,null,null,"USD",null, 11011, 110112, 12001),
                                                                           
                           new FuelReportDetail(0, fuelReports[1].Id, 430, "Ton", 13, 13, ReceiveTypes.TransferPurchase, 0,null,  null,null,null,"USD",null, 11010, 110101, 12001),
                           new FuelReportDetail(0, fuelReports[1].Id, 100, "Ton", 2.5, 0, null, 50,TransferTypes.TransferSale, 1, CorrectionTypes.Minus, 1900, "USD",1, 11011, 110112, 12001),
                                                                          
                           new FuelReportDetail(0, fuelReports[2].Id, 410, "Ton", 20, null,null,null,null,null,null,null,"USD",null,11010, 110101, 12001),
                           new FuelReportDetail(0, fuelReports[2].Id, 70, "Ton", 0, 111, ReceiveTypes.TransferPurchase, 50,TransferTypes.TransferSale, 1, CorrectionTypes.Minus, 1600, "USD",1, 11011, 110112, 12001),
                                                                          
                           new FuelReportDetail(0, fuelReports[3].Id, 380, "Ton", 12, 0, null, 65,TransferTypes.InternalTransfer, null,null,null,"USD",null, 11010, 110101, 12001),
                           new FuelReportDetail(0, fuelReports[3].Id, 140, "Ton", 5, 50, ReceiveTypes.TransferPurchase, 10,TransferTypes.TransferSale, 30, CorrectionTypes.Plus, 1800, "USD",1, 11011, 110112, 12001),
                                                                         
                           new FuelReportDetail(0, fuelReports[4].Id, 350, "Ton", 20, 13, ReceiveTypes.TransferPurchase, 54,TransferTypes.TransferSale, 1, CorrectionTypes.Minus, 900,"USD", 1, 11010, 110101, 12001),
                           new FuelReportDetail(0, fuelReports[4].Id, 130, "Ton", 10, 111, ReceiveTypes.TransferPurchase, 50,TransferTypes.TransferSale, 1, CorrectionTypes.Plus, 1850, "USD",1, 11011, 110112, 12001),
                                                                       
                           new FuelReportDetail(0, fuelReports[5].Id, 380, "Ton", 11, 0, null,null,null,null,null,null,"USD",null,11010, 110101, 12001),
                           new FuelReportDetail(0, fuelReports[5].Id, 100, "Ton", 10, 0, null, 0,null, 0, null, 200, "USD",1, 11011, 110112, 12001),

                           //new FuelReportDetail(0, 7, 320, 15, 0, null,null,null,5,null,null,null,1, 1, 1),
                           //new FuelReportDetail(0, 7, 100, 10, 111, ReceiveTypes.TransferPurchase, 50,TransferTypes.TransferSale, 0, null, 200, 1, 2, 1,2),

                           //new FuelReportDetail(0, 8, 310, 8, 0, null,null,null,null,null,null,null,1, 1, 1),
                           //new FuelReportDetail(0, 8, 100, 10, 0, null, 0,null, 0, null, 200, 1, 2, 1,2)
                       };
        }

        public static List<Unit> GetUnits()
        {
            return new List<Unit>
                       {
                           new Unit(1, "Metric Ton"),
                           new Unit(2, "Liter"),
                           new Unit(3, "Long Ton")
                       };
        }

        public static List<GoodUnit> GetCompanyGoodUnits()
        {
            return new List<GoodUnit>
                       {
                           new GoodUnit(1, 1, null, "Ton", 1, "Ton"),
                           new GoodUnit(2, 1, 1, "Liter", (decimal)0.990, "Liter"),
                           new GoodUnit(3, 1, 2, "Long Ton", (decimal)1.2*1000, "Long Ton"),
                           new GoodUnit(4, 2, null, "Ton", 1, "Ton"),
                           new GoodUnit(5, 2, 4, "Liter", (decimal)0.990, "Liter"),
                           new GoodUnit(6, 2, 5, "Long Ton", (decimal)1.2*1000, "Long Ton"),
                       };
        }

        public static List<Good> GetGoods()
        {
            var res1 = new Good(1, "Heavy Fuel Oil", "HFO", 1, 1);
            //res1.GoodUnit = new GoodUnit(1, 1, 1, "", 1, "");
            var res2 = new Good(2, "Marine Diesel Oil", "MDO", 2, 1);
            //res2.GoodUnit = new GoodUnit(1, 2, 1, "", 1, "");
            return new List<Good>
                   {
                      res1,res2
                   };
        }

        public static List<Currency> GetCurrencies()
        {
            return new List<Currency> { new Currency(1, "USD", "USD"), new Currency(2, "EUR", "EUR"), new Currency(3, "IR Rials", "IRR") };
        }

        //public static List<GoodPartyAssignment> GetGoodPartyAssignments()
        //{
        //    return new List<GoodPartyAssignment> { new GoodPartyAssignment(1, "GoodPartyAssignment 1", 1, 1), new GoodPartyAssignment(2, "GoodPartyAssignment 2", 1, 2) };
        //}

        public static List<SharedGood> GetShareGoods()
        {
            return new List<SharedGood> { new SharedGood(1, "Shared Good 1", "SharedGood1", 1), new SharedGood(2, "Shared Good 2", "SharedGood2", 2), };
        }


        //
        //        public static List<Scrap> GetScraps(List<Vessel> vessels, Company secondParty)
        //        {
        //
        //            return new List<Scrap>
        //                   {
        //                       new Scrap(vessels[0],secondParty, DateTime.Now, null),
        //                       new Scrap(vessels[1],secondParty, DateTime.Now, null)
        //                   };
        //        }
        //
        //        public static List<ScrapDetail> GetScrapDetails(List<Scrap> scraps, List<Currency> currencies, List<Good> goods, List<GoodUnit> units, List<Tank> tanks)
        //        {
        //            return new List<ScrapDetail>
        //                   {
        //                       new ScrapDetail(234, 560, currencies[0], goods[1], units[0] ,tanks[0], scraps[0]),
        //                       new ScrapDetail(125, 895, currencies[0], goods[0], units[0] ,tanks[0], scraps[0]),
        //                   };
        //        }
        public static List<EffectiveFactor> GetEffectiveFactors()
        {
            return new List<EffectiveFactor>
                       {
                           new EffectiveFactor("مالیات ", EffectiveFactorTypes.Increase),
                           new EffectiveFactor("تخفیف", EffectiveFactorTypes.Increase),
                           new EffectiveFactor("حمل و نقل", EffectiveFactorTypes.Decrease),
                       };
        }
    }
}