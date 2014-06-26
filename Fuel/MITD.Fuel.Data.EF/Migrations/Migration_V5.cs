using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;

namespace MITD.Fuel.Data.EF.Migrations
{
    [Migration(5)]
    public class Migration_V5 : Migration
    {
        public override void Down()
        {
            Delete.Table("Accounts").InSchema("Fuel");
            Delete.Table("VoucherSetings").InSchema("Fuel");
            Delete.Table("AsgnVoucherAconts").InSchema("Fuel");
            Delete.Table("AsgnVoucherSegments").InSchema("Fuel");

        }

        public override void Up()
        {


            Create.Table("Accounts").InSchema("Fuel")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                .WithColumn("Name").AsString(50)
                .WithColumn("TimeStamp").AsCustom("RowVersion")
                .WithColumn("Code").AsString(50);

            Create.Table("VoucherSetings").InSchema("Fuel")
                .WithColumn("Id").AsInt64().Identity().NotNullable().PrimaryKey()
                .WithColumn("VoucherCeditRefDescription").AsString(250)
                .WithColumn("VoucherMainRefDescription").AsString(250)
                .WithColumn("VoucherDebitDescription").AsString(250)
                .WithColumn("VoucherDebitRefDescription").AsString(250)
                .WithColumn("VoucherCreditDescription").AsString(250)
                .WithColumn("VoucherMainDescription").AsString(250)
                .WithColumn("GoodId").AsInt64().NotNullable()
                .WithColumn("CompanyId").AsInt64().NotNullable()
                .WithColumn("VoucherDetailTypeId").AsInt32()
                .WithColumn("TimeStamp").AsCustom("RowVersion")
                .WithColumn("VoucherTypeId").AsInt32();



            Create.Table("AsgnVoucherAconts").InSchema("Fuel")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("VoucherSetingId").AsInt64()
                  .ForeignKey("FK_VoucherSeting", "Fuel", "VoucherSetings", "Id")
                .WithColumn("AccountId").AsInt32()
                  .ForeignKey("FK_Account_AsgnVoucherAcont", "Fuel", "Accounts", "Id")
                .WithColumn("Type").AsInt32()
                 .WithColumn("TimeStamp").AsCustom("RowVersion");

            Create.Table("AsgnVoucherSegments").InSchema("Fuel")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey()
                .WithColumn("Type").AsInt32()
                .WithColumn("SegmentTypeId").AsInt32()
                .WithColumn("TimeStamp").AsCustom("RowVersion")
                .WithColumn("VoucherSetingId").AsInt64()
                 .ForeignKey("FK_VoucherSeting_AsgnVoucherSegment", "Fuel", "VoucherSetings", "Id");


            Create.Table("Segments").InSchema("Fuel")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("Code").AsString(50)
                .WithColumn("TimeStamp").AsCustom("RowVersion")
                .WithColumn("SegmentTypeId").AsInt32();

            Create.Table("Vouchers").InSchema("Fuel")
                .WithColumn("Id").AsInt64().Identity().PrimaryKey()
                .WithColumn("CurrencyId").AsInt64().NotNullable()
                .WithColumn("Description").AsString(250)
                .WithColumn("FinancialVoucherDate").AsDateTime()
                .WithColumn("LocalVoucherDate").AsDateTime()
                .WithColumn("LocalVoucherNo").AsString(50)
                .WithColumn("ReferenceNo").AsString(50)
                .WithColumn("VoucherRef").AsInt64()
                .WithColumn("ReferenceTypeId").AsInt32()
                .WithColumn("TimeStamp").AsCustom("RowVersion");


            Create.Table("JournalEntries").InSchema("Fuel")
                .WithColumn("Id").AsInt64().Identity().NotNullable().PrimaryKey()
                .WithColumn("VoucherId").AsInt64().NotNullable()
                    .ForeignKey("FK_Voucher_JournalEntries_", "Fuel", "Vouchers", "Id")
                .WithColumn("AccountNo").AsString(250)
                .WithColumn("Description").AsString(250)
                .WithColumn("TimeStamp").AsCustom("RowVersion")
                .WithColumn("VoucherRef").AsString(250)
                .WithColumn("ForeignAmount").AsDecimal()
                .WithColumn("IrrAmount").AsDecimal()
                .WithColumn("SegmentId").AsInt64().NotNullable()
                    .ForeignKey("FK_Segment_JournalEntries", "Fuel", "Segments", "Id")
                .WithColumn("Typ").AsInt32();




        }
    }
}
