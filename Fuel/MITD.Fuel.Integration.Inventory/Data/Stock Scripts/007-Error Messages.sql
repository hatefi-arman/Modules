USE MiniStock
GO 
DELETE FROM ErrorMessages
GO
--INSERT INTO Galaxy.ErrorMessages VALUES('chk_CodeISValid',N'کد زیر گروه از طول گروه فرعی بزرگتر است' ,'CHECK')
--GO
--------------------------------User_List--------------------------------
INSERT INTO ErrorMessages VALUES('PK_User_List_Id',N'شماره شناسايي کاربر تکراري مي باشد','PRIMARY KEY')
GO
INSERT INTO ErrorMessages VALUES('UQ_User_List_Code',N'کد کاربر تکراري مي باشد','UNIQUE KEY')
GO
INSERT INTO ErrorMessages VALUES('UQ_User_List_User_Name',N'نام کاربري تکراري مي باشد','UNIQUE KEY')
GO
INSERT INTO ErrorMessages VALUES('FK_User_List_User_Creator_ID',N'اين کاربر در اطلاعات کاربران استفاده شده است','DELETE')
GO
	raiserror(N'پایان ایجاد پیغام های مخصوص سیستم.',0,1) with nowait
GO