CREATE TABLE [dbo].[Products]
(
    [Id] VARCHAR(255) NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(50) NOT NULL, 
    [Status] VARCHAR(50) NOT NULL
);

CREATE TABLE [dbo].[Supplies]
(
	   [Id] VARCHAR(255) NOT NULL PRIMARY KEY, 
    [WorkStation] VARBINARY(50) NOT NULL
);