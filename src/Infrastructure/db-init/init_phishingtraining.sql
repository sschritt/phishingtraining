IF EXISTS (SELECT name FROM master.sys.databases WHERE name = 'phishingtraining')
BEGIN
	PRINT 'phishingtraining already exits. Skipping initialization..';
	set noexec on;
END

CREATE DATABASE phishingtraining;
GO

USE phishingtraining;
CREATE LOGIN phishingtraining_admin WITH PASSWORD='dockerPhishingTrainingAdminDevelopment_1010';
CREATE USER phishingtraining_admin;
GO

EXEC sp_addrolemember 'db_owner', 'phishingtraining_admin';

PRINT 'Setup of phishingtraining database done..';
