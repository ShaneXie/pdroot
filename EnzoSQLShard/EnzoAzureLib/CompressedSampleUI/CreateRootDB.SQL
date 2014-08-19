-- You must first create a root database. 
-- To create a database you can run this statement:  CREATE DATABASE rootdb
-- Then, make sure to change the context of your database to rootdb, as such:  USE rootdb
--   NOTE: SQL Azure does not support the USE command - you will need to reconnect to rootdb directly before running the statements below
-- Finaly you can create the tables below that this library requires. 

CREATE TABLE serversdef
( id int IDENTITY(1,1) primary key,
  servername nvarchar(255) NOT NULL,
  port int,
  uid nvarchar(50),
  pwd varbinary(50),
  enabled bit)

CREATE TABLE tenantsdef
( id int IDENTITY(1,1) primary key,
  serverId int NOT NULL,
  databasename nvarchar(50) NOT NULL,
  tenantKey nvarchar(50) NOT NULL,
  uid nvarchar(100),
  pwd varbinary(50),
  enabled bit)

CREATE UNIQUE INDEX IDX1 ON TenantsDef (tenantKey)
CREATE UNIQUE INDEX IDX2 ON TenantsDef (serverId, databasename, uid)