#region

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MITD.Fuel.Domain.Model.DomainObjects;

#endregion

namespace MITD.Fuel.Data.EF.Configurations
{
    public class ApproveWorkFlowConfigConfiguration : EntityTypeConfiguration<WorkflowStep>
    {
        public ApproveWorkFlowConfigConfiguration()
        {
            HasKey(p => p.Id).ToTable("ApproveFlowConfig", "Fuel");

            // Properties:
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(c => c.ActorUser).WithMany().HasForeignKey(c => c.ActorUserId);


            HasOptional(c => c.NextWorkflowStep).WithMany().HasForeignKey(c => c.NextWorkflowStepId);
        }
    }
}