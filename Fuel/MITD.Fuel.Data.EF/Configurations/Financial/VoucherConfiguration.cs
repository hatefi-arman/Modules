﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;

namespace MITD.Fuel.Data.EF.Configurations.Financial
{
    public class VoucherConfiguration : EntityTypeConfiguration<Voucher>
    {
        public VoucherConfiguration()
        {
            ToTable("Vouchers", "Fuel").
            HasKey(c => c.Id);

            Property(c => c.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(c => c.Currency).WithMany().HasForeignKey(c => c.CurrencyId).WillCascadeOnDelete(false);

            Ignore(c => c.ReferenceType);
            Property(c => c.TimeStamp).IsRowVersion();


        }
    }
}
