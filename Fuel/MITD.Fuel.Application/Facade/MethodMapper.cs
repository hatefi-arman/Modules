using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Fuel.Presentation.Contracts;

using MITD.FuelSecurity.Domain;
using MITD.FuelSecurity.Domain.Model;

namespace MITD.Fuel.Application.Facade
{
    public class MethodMapper
    {
        public class MethodAction
        {
            public string ClassName { get; private set; }
            public string MethodName { get; private set; }
            public List<ActionType> Actions { get; private set; }
            public MethodAction(string className, string methodName, List<ActionType> actions)
            {
                ClassName = className;
                MethodName = methodName;
                Actions = actions;
            }
        }

        private readonly List<MethodAction> mapTable = new List<MethodAction>
        {

            //new MethodAction(typeof (IPeriodServiceFacade).Name,
            //    typeof (IPeriodServiceFacade).GetMethod("AddPeriod").Name, new List<ActionType> {ActionType.AddPeriod}),
            //new MethodAction(typeof (IPeriodServiceFacade).Name,
            //    typeof (IPeriodServiceFacade).GetMethod("UpdatePeriod").Name,
            //    new List<ActionType> {ActionType.ModifyPeriod}),
            //new MethodAction(typeof (IPeriodServiceFacade).Name,
            //    typeof (IPeriodServiceFacade).GetMethod("DeletePeriod").Name,
            //    new List<ActionType> {ActionType.DeletePeriod}),
            //new MethodAction(typeof (IPeriodServiceFacade).Name,
            //    typeof (IPeriodServiceFacade).GetMethod("ChangePeriodState").Name,
            //    new List<ActionType>
            //    {
            //        ActionType.ActivatePeriod,
            //        ActionType.InitializePeriodForInquiry,
            //        ActionType.StartInquiry,
            //        ActionType.CompleteInquiry,
            //        ActionType.ClosePeriod
            //    }),

            //    //RollBackPeriodState

            //new MethodAction(typeof (IPeriodUnitServiceFacade).Name,
            //    typeof (IPeriodUnitServiceFacade).GetMethod("GetUnitsWithActions").Name,
            //    new List<ActionType> {ActionType.ManageUnits}),
            //new MethodAction(typeof (IPeriodJobPositionServiceFacade).Name,
            //    typeof (IPeriodJobPositionServiceFacade).GetMethod("GetJobPositionsWithActions").Name,
            //    new List<ActionType> {ActionType.ManageJobPositions}),
            //new MethodAction(typeof (IPeriodJobIndexServiceFacade).Name,
            //    typeof (IPeriodJobIndexServiceFacade).GetMethod("GetAllAbstractJobIndices").Name,
            //    new List<ActionType> {ActionType.ManageJobIndices}),
            //new MethodAction(typeof (IPeriodJobServiceFacade).Name,
            //    typeof (IPeriodJobServiceFacade).GetMethod("GetAllJobs").Name,
            //    new List<ActionType> {ActionType.ManageJobs}),
            //new MethodAction(typeof (ICalculationServiceFacade).Name,
            //    typeof (ICalculationServiceFacade).GetMethod("GetAllCalculations").Name,
            //    new List<ActionType> {ActionType.ManageCalculations}),
            //new MethodAction(typeof (IEmployeeServiceFacade).Name,
            //    typeof (IEmployeeServiceFacade).GetMethod("GetAllEmployees", new[] {typeof (long)}).Name,
            //    new List<ActionType> {ActionType.ManageEmployees}),
            //new MethodAction(typeof (IEmployeeServiceFacade).Name,
            //    typeof (IEmployeeServiceFacade).GetMethod("GetAllEmployees",
            //        new[] {typeof (long), typeof (int), typeof (int)}).Name,
            //    new List<ActionType> {ActionType.ManageEmployees}),

            //new MethodAction(typeof (IPeriodJobServiceFacade).Name,
            //    typeof (IPeriodJobServiceFacade).GetMethod("AssignJob").Name,
            //    new List<ActionType> {ActionType.AddJobInPeriod}),
            //new MethodAction(typeof (IPeriodJobServiceFacade).Name,
            //    typeof (IPeriodJobServiceFacade).GetMethod("UpdateJob").Name,
            //    new List<ActionType> {ActionType.ModifyJobInPeriod, ActionType.ModifyJobInPeriod}),
            //new MethodAction(typeof (IPeriodJobServiceFacade).Name,
            //    typeof (IPeriodJobServiceFacade).GetMethod("RemoveJob").Name,
            //    new List<ActionType> {ActionType.DeleteJobInPeriod}),

            //new MethodAction(typeof (IPeriodUnitServiceFacade).Name,
            //    typeof (IPeriodUnitServiceFacade).GetMethod("AssignUnit").Name,
            //    new List<ActionType> {ActionType.AddUnitInPeriod}),
            //new MethodAction(typeof (IPeriodUnitServiceFacade).Name,
            //    typeof (IPeriodUnitServiceFacade).GetMethod("RemoveUnit").Name,
            //    new List<ActionType> {ActionType.DeleteUnitInPeriod}),

            //new MethodAction(typeof (IPeriodJobPositionServiceFacade).Name,
            //    typeof (IPeriodJobPositionServiceFacade).GetMethod("AssignJobPosition").Name,
            //    new List<ActionType> {ActionType.AddJobPositionInPeriod}),
            //new MethodAction(typeof (IPeriodJobPositionServiceFacade).Name,
            //    typeof (IPeriodJobPositionServiceFacade).GetMethod("RemoveJobPosition").Name,
            //    new List<ActionType> {ActionType.DeleteJobPositionInPeriod}),
            //new MethodAction(typeof (IPeriodJobPositionServiceFacade).Name,
            //    typeof (IPeriodJobPositionServiceFacade).GetMethod("GetInquirySubjectsWithInquirers").Name,
            //    new List<ActionType> {ActionType.ManageJobPositionInPeriodInquiry}),
            //new MethodAction(typeof (IPeriodJobPositionServiceFacade).Name,
            //    typeof (IPeriodJobPositionServiceFacade).GetMethod("UpdateInquirySubjectInquirers").Name,
            //    new List<ActionType> {ActionType.ManageJobPositionInPeriodInquiry}),

            //new MethodAction(typeof (IPeriodJobIndexServiceFacade).Name,
            //    typeof (IPeriodJobIndexServiceFacade).GetMethod("AddJobIndex").Name,
            //    new List<ActionType> {ActionType.AddJobIndexInPeriod}),
            //new MethodAction(typeof (IPeriodJobIndexServiceFacade).Name,
            //    typeof (IPeriodJobIndexServiceFacade).GetMethod("UpdateJobIndex").Name,
            //    new List<ActionType> {ActionType.ModifyJobIndexInPeriod}),
            //new MethodAction(typeof (IPeriodJobIndexServiceFacade).Name,
            //    typeof (IPeriodJobIndexServiceFacade).GetMethod("DeleteAbstractJobIndex").Name,
            //    new List<ActionType> {ActionType.DeleteJobIndexInPeriod, ActionType.DeleteJobIndexGroupInPeriod}),
            //new MethodAction(typeof (IPeriodJobIndexServiceFacade).Name,
            //    typeof (IPeriodJobIndexServiceFacade).GetMethod("AddJobIndexGroup").Name,
            //    new List<ActionType> {ActionType.AddJobIndexGroupInPeriod}),
            //new MethodAction(typeof (IPeriodJobIndexServiceFacade).Name,
            //    typeof (IPeriodJobIndexServiceFacade).GetMethod("UpdateJobIndexGroup").Name,
            //    new List<ActionType> {ActionType.ModifyJobIndexGroupInPeriod}),

            //new MethodAction(typeof (IEmployeeServiceFacade).Name,
            //    typeof (IEmployeeServiceFacade).GetMethod("AddEmployee").Name,
            //    new List<ActionType> {ActionType.AddEmployee}),
            //new MethodAction(typeof (IEmployeeServiceFacade).Name,
            //    typeof (IEmployeeServiceFacade).GetMethod("UpdateEmployee").Name,
            //    new List<ActionType> {ActionType.ModifyEmployee}),
            //new MethodAction(typeof (IEmployeeServiceFacade).Name,
            //    typeof (IEmployeeServiceFacade).GetMethod("DeleteEmployee").Name,
            //    new List<ActionType> {ActionType.DeleteEmployee}),
            ////ActionType AddEmployeeJobPositions = new ActionType("1"}), "AddPeriod");
            ////new MethodAction(typeof(IEmployeeServiceFacade).Name ,typeof(IPeriodServiceFacade).GetMethod("GetEmployeeJobPositions").Name, new List<ActionType> {ActionType.ManageEmployeeJobPositions,ActionType.GetEmployeeJobPositions}),
            //new MethodAction(typeof (IEmployeeServiceFacade).Name,
            //    typeof (IEmployeeServiceFacade).GetMethod("AssignJobPositionsToEmployee").Name,
            //    new List<ActionType> {ActionType.ManageEmployeeJobPositions}),
            ////ActionType DeleteEmployeeJobPositions = new ActionType("3"}), "Employee"); 
            ////new MethodAction(typeof(EmployeeServiceFacade).Name ,typeof(IPeriodServiceFacade).GetMethod("").Name, new List<ActionType> {ActionType.AddEmployeeJobCustomFields}),
            ////new MethodAction(typeof(EmployeeServiceFacade).Name ,typeof(IPeriodServiceFacade).GetMethod("").Name, new List<ActionType> {ActionType.ModifyEmployeeJobCustomFields}),
            ////ActionType DeleteEmployeeJobCustomFields = new ActionType("3"}), "Employee");

            //new MethodAction(typeof (IJobIndexFacadeService).Name,
            //    typeof (IJobIndexFacadeService).GetMethod("AddJobIndex").Name,
            //    new List<ActionType> {ActionType.AddJobIndex}),
            //new MethodAction(typeof (IJobIndexFacadeService).Name,
            //    typeof (IJobIndexFacadeService).GetMethod("UpdateJobIndex").Name,
            //    new List<ActionType> {ActionType.ModifyJobIndex, ActionType.ManageJobIndexCustomFields}),
            //new MethodAction(typeof (IJobIndexFacadeService).Name,
            //    typeof (IJobIndexFacadeService).GetMethod("DeleteAbstractJobIndex").Name,
            //    new List<ActionType> {ActionType.DeleteJobIndex, ActionType.DeleteJobIndexCategory}),
            ////ActionType AddJobIndexCustomFields = new ActionType("3"}), "Employee");
            ////ActionType ModifyJobIndexCustomFields = new ActionType("3"}), "Employee");
            //new MethodAction(typeof (IJobIndexFacadeService).Name,
            //    typeof (IJobIndexFacadeService).GetMethod("AddJobIndexCategory").Name,
            //    new List<ActionType> {ActionType.AddJobIndexCategory}),
            //new MethodAction(typeof (IJobIndexFacadeService).Name,
            //    typeof (IJobIndexFacadeService).GetMethod("UpdateJobIndexCategory").Name,
            //    new List<ActionType> {ActionType.ModifyJobIndexCategory}),

            //new MethodAction(typeof (IJobPositionFacadeService).Name,
            //    typeof (IJobPositionFacadeService).GetMethod("AddJobPosition").Name,
            //    new List<ActionType> {ActionType.AddJobPosition}),
            //new MethodAction(typeof (IJobPositionFacadeService).Name,
            //    typeof (IJobPositionFacadeService).GetMethod("DeleteJob").Name,
            //    new List<ActionType> {ActionType.DeleteJobPosition}),
            //new MethodAction(typeof (IJobPositionFacadeService).Name,
            //    typeof (IJobPositionFacadeService).GetMethod("UpdateJobPosition").Name,
            //    new List<ActionType> {ActionType.ModifyJobPosition}),

            //new MethodAction(typeof (IFunctionFacadeService).Name,
            //    typeof (IFunctionFacadeService).GetMethod("AddFunction").Name,
            //    new List<ActionType> {ActionType.AddFunction}),
            //new MethodAction(typeof (IFunctionFacadeService).Name,
            //    typeof (IFunctionFacadeService).GetMethod("DeleteFunction").Name,
            //    new List<ActionType> {ActionType.DeleteFunction}),
            //new MethodAction(typeof (IFunctionFacadeService).Name,
            //    typeof (IFunctionFacadeService).GetMethod("UpdateFunction").Name,
            //    new List<ActionType> {ActionType.ModifyFunction}),

            //new MethodAction(typeof (ICustomFieldFacadeService).Name,
            //    typeof (ICustomFieldFacadeService).GetMethod("AddCustomField").Name,
            //    new List<ActionType> {ActionType.AddCustomField}),
            //new MethodAction(typeof (ICustomFieldFacadeService).Name,
            //    typeof (ICustomFieldFacadeService).GetMethod("DeleteCustomeField").Name,
            //    new List<ActionType> {ActionType.DeleteCustomField}),
            //new MethodAction(typeof (ICustomFieldFacadeService).Name,
            //    typeof (ICustomFieldFacadeService).GetMethod("UpdateCustomField").Name,
            //    new List<ActionType> {ActionType.ModifyCustomField}),

            //new MethodAction(typeof (IJobFacadeService).Name, typeof (IJobFacadeService).GetMethod("AddJob").Name,
            //    new List<ActionType> {ActionType.AddJob}),
            //new MethodAction(typeof (IJobFacadeService).Name, typeof (IJobFacadeService).GetMethod("DeleteJob").Name,
            //    new List<ActionType> {ActionType.DeleteJob}),
            //new MethodAction(typeof (IJobFacadeService).Name, typeof (IJobFacadeService).GetMethod("UpdateJob").Name,
            //    new List<ActionType> {ActionType.ModifyJob, ActionType.ManageJobCustomFields}),

            //new MethodAction(typeof (IUnitFacadeService).Name, typeof (IUnitFacadeService).GetMethod("AddUnit").Name,
            //    new List<ActionType> {ActionType.AddUnit}),
            //new MethodAction(typeof (IUnitFacadeService).Name, typeof (IUnitFacadeService).GetMethod("DeleteUnit").Name,
            //    new List<ActionType> {ActionType.DeleteUnit}),
            //new MethodAction(typeof (IUnitFacadeService).Name, typeof (IUnitFacadeService).GetMethod("UpdateUnit").Name,
            //    new List<ActionType> {ActionType.ModifyUnit}),


            //new MethodAction(typeof (IPolicyFacadeService).Name,
            //    typeof (IPolicyFacadeService).GetMethod("AddPolicy").Name, new List<ActionType> {ActionType.AddPolicy}),
            //new MethodAction(typeof (IPolicyFacadeService).Name,
            //    typeof (IPolicyFacadeService).GetMethod("DeletePolicy").Name,
            //    new List<ActionType> {ActionType.DeletePolicy}),
            //new MethodAction(typeof (IPolicyFacadeService).Name,
            //    typeof (IPolicyFacadeService).GetMethod("UpdatePolicy").Name,
            //    new List<ActionType> {ActionType.ModifyPolicy}),
            //new MethodAction(typeof (IRuleFacadeService).Name,
            //    typeof (IRuleFacadeService).GetMethod("GetPolicyRulesWithPagination").Name,
            //    new List<ActionType> {ActionType.ManageRules}),
            //new MethodAction(typeof (IFunctionFacadeService).Name,
            //    typeof (IFunctionFacadeService).GetMethod("GetPolicyFunctionsWithPagination").Name,
            //    new List<ActionType> {ActionType.ManageFunctions}),

            //new MethodAction(typeof (IRuleFacadeService).Name, typeof (IRuleFacadeService).GetMethod("AddRule").Name,
            //    new List<ActionType> {ActionType.AddRule}),
            //new MethodAction(typeof (IRuleFacadeService).Name, typeof (IRuleFacadeService).GetMethod("DeleteRule").Name,
            //    new List<ActionType> {ActionType.DeleteRule}),
            //new MethodAction(typeof (IRuleFacadeService).Name, typeof (IRuleFacadeService).GetMethod("UpdateRule").Name,
            //    new List<ActionType> {ActionType.ModifyRule}),

            //new MethodAction(typeof (ICalculationServiceFacade).Name,
            //    typeof (ICalculationServiceFacade).GetMethod("AddCalculation").Name,
            //    new List<ActionType> {ActionType.AddCalculation}),
            //new MethodAction(typeof (ICalculationServiceFacade).Name,
            //    typeof (ICalculationServiceFacade).GetMethod("DeleteCalculation").Name,
            //    new List<ActionType> {ActionType.DeleteCalculation}),
            //new MethodAction(typeof (ICalculationServiceFacade).Name,
            //    typeof (ICalculationServiceFacade).GetMethod("ChangeCalculationState").Name,
            //    new List<ActionType>
            //    {
            //        ActionType.ModifyCalculation,
            //        ActionType.RunCalculation,
            //        ActionType.StopCalculation,
            //        ActionType.SetDeterministicCalculation
            //    }),
            ////new MethodAction(typeof(CalculationServiceFacade).Name ,typeof(IPeriodServiceFacade).GetMethod("").Name, new List<ActionType> {ActionType.ShowCalculationState}),
            //new MethodAction(typeof (IJobIndexPointFacadeService).Name,
            //    typeof (IJobIndexPointFacadeService).GetMethod("GetAllJobIndexPoints").Name,
            //    new List<ActionType> {ActionType.ShowCalculationResult}),
            ////new MethodAction(typeof(IJobIndexPointFacadeService).Name ,typeof(IPeriodServiceFacade).GetMethod("GetEmployeeSummeryCalculationResult").Name, new List<ActionType> {ActionType.ShowCalculationResultDetail}),
            //// new MethodAction(typeof(IJobIndexPointFacadeService).Name ,typeof(IPeriodServiceFacade).GetMethod("GetEmployeeJobPositionsCalculationResult").Name, new List<ActionType> {ActionType.ShowCalculationResultDetail}),
            
            //new MethodAction(typeof (IInquiryServiceFacade).Name,
            //    typeof (IInquiryServiceFacade).GetMethod("GetInquiryForm").Name,
            //    new List<ActionType> {ActionType.FillInquiryForm}),
            //new MethodAction(typeof (IInquiryServiceFacade).Name,
            //    typeof (IInquiryServiceFacade).GetMethod("UpdateInquirySubjectForm").Name,
            //    new List<ActionType> {ActionType.FillInquiryForm}),
            ////new MethodAction(typeof().Name ,typeof(IPeriodServiceFacade).GetMethod("").Name, new List<ActionType> {ActionType.DeleteCustomInquirer}),
           
            //new MethodAction(typeof (IPeriodClaimServiceFacade).Name,
            //    typeof (IPeriodClaimServiceFacade).GetMethod("AddClaim").Name,
            //    new List<ActionType> {ActionType.AddClaim}),
            //new MethodAction(typeof (IPeriodClaimServiceFacade).Name,
            //    typeof (IPeriodClaimServiceFacade).GetMethod("GetClaim").Name,
            //    new List<ActionType> {ActionType.ShowClaim}),
            //new MethodAction(typeof (IPeriodClaimServiceFacade).Name,
            //    typeof (IPeriodClaimServiceFacade).GetMethod("ChangeClaimState").Name,
            //    new List<ActionType> {ActionType.ReplyToClaim}),
            //new MethodAction(typeof (IPeriodClaimServiceFacade).Name,
            //    typeof (IPeriodClaimServiceFacade).GetMethod("DeleteClaim").Name,
            //    new List<ActionType> {ActionType.DeleteClaim}),

            // new MethodAction(typeof (IPeriodClaimServiceFacade).Name,
            //    typeof (IPeriodClaimServiceFacade).GetMethod("GetAllClaimsForAdminWithActions").Name,
            //    new List<ActionType> {ActionType.ShowAdminClaimList}),   
           
            ////new MethodAction(typeof().Name ,typeof(IPeriodServiceFacade).GetMethod("").Name, new List<ActionType> {ActionType.AddPermittedUserToMyTasks}),
            ////new MethodAction(typeof().Name ,typeof(IPeriodServiceFacade).GetMethod("").Name, new List<ActionType> {ActionType.RemovePermittedUserFromMyTasks}),
            ////new MethodAction(typeof().Name ,typeof(IPeriodServiceFacade).GetMethod("").Name, new List<ActionType> {ActionType.SettingPermittedUserToMyTasks}),

        };

        public List<ActionType> Map(string className, string methodName)
        {
             //var mapRow = mapTable.SingleOrDefault(m => m.ClassName == className && m.MethodName == methodName);
            //if (mapRow == null)
            //    return new List<ActionType>();
            //return mapRow.Actions;

            var mapRow = mapTable.Where(m => m.ClassName == className && m.MethodName == methodName);
            if (!mapRow.Any())
                return new List<ActionType>();
            return mapRow.FirstOrDefault().Actions;

        }

    }

}
