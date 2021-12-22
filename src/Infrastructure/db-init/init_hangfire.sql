IF EXISTS (SELECT name FROM master.sys.databases WHERE name = 'phishingtraining_hangfire')
BEGIN
	PRINT 'phishingtraining_hangfire already exits. Skipping initialization..';
	set noexec on;
END

CREATE DATABASE phishingtraining_hangfire;
GO

USE phishingtraining_hangfire;
CREATE USER phishingtraining_admin;
GO

EXEC sp_addrolemember 'db_owner', 'phishingtraining_admin';

PRINT 'Setup of phishingtraining_hangfire database done..';
