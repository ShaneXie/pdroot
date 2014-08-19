Welcome to the Enzo Shard Library. This library is provided as-is, and is meant for educational purposes only. 
If you intend to use this library in a production environment, please contact info@bluesyntax.net for support options.

1) OVERVIEW
The Enzo Shard Library supports multiple sharding configurations. This project (CompressedSampleUI) shows you how it can be used 
in a linear sharding model, where each customer has its own copy of tables and database objects, or a compressed sharding model
in which some of the customers reside on the same database stored in different schema containers. 

This sharding model is called compressed because it supports multiple customers/tenants inside a given database, separated by
schema. This allows each customer to be securely isolated within the same database. Note that you can still configure the 
environment to have some of the tenants/customers on other databases (they do not have to be all in the same database). The 
distribution of tenants across databases and schemas is entirely up to you. Note however, that even if you decide to use store
each customer/tenant in separate databases, you should use a different schema name for each tenant. This will make it easier
to move tenants into the same database later if the need ever arises.

2) SETUP ENVIRONMENT
To setup the sample environment you will need to perform the following steps the first time you start the application
(note: 2a and 2b need to be performed manually):

 a) Create a root database
 b) Create customer/tenant databases 
 c) Create the system tables in the root database
 d) Add tenant databases in the shard definition
 e) Create a sample table in each customer database

2a)
To create the root database, you will need to execute the CREATE DATABASE T-SQL statement. Like tenant databases, the root database can be created on 
SQL Server or SQL Database. 

2b) 
Once the root database is created, create two or more tenant databases using the CREATE DATABASE T-SQL statement
as well. You do not need to create any tables at this time. 

2c)
Once the databases have been created manually, you can start the application. A Shard Configuration window will appear. Make sure the root database
credentials are correct, then click on Connect. Once connected to the root database, you can create the system tables for the root database
by clicking on the "Create Empty Config Tables" button. You can also change the connection settings of the App.Config file to specify the
connection string of the root database (make sure to use the InitialCatalog property in the connection string to specify the name of the
root database). When you use the App.Config file, the FormConfigure.cs screen will automatically use the configuration file to
pre-fill the root database connection settings.

2d)
Once the root and tenants databases have been created, and the system tables in the root database have been created, 
you can add the tenant databases. To add a tenant database, click on the New Tenant link, then enter the server name, 
database name, UID and PWD, Customer Key, and make sure Enable Tenant is checked, then click on "Save". 
This will add the tenant to the configuration tables. 

2e) 
Then click on "Create Test History Records" to create a sample table with one record in it. Repeat the steps above 
for each tenant database. Click on Refresh to list each tenant database. Once your Tenant databases have been added, you
can close the FormConfigure form. 

3) USE THE APPLICATION
Now that your root database and tenant databases are created you can use the application. Once you close the Shard Configuration screen
a window will show the list of available tenants in a dropdown box, next to Refresh Tenants. Simply pick the first tenant; 
it will return the records from the test table (called History) found in the selected tenant. [NOTE: The History table was 
created in step 2e before].

Selecting the second tab allows you to fetch records from all tenants. Running queries across tenants can be important for reporting and 
administrative purposes. This tab also allows you to use a feature of this library called Distributed TSQL, or D-SQL. D-SQL allows you
to execute statements across multiple tenants without setting the API parameters; they are automatically inferred based on the values
provided in the SQL statement. 

The third tab allows you to enter additional test records in the History table. 

3) CONCLUSION
This sample application shows you how to use the Compressed Shard API by defining a root database, tenant databases, and by
using the API to manage records in the shard. 

