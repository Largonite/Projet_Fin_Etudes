SET NOCOUNT ON
GO
USE PAIMEN
GO

set quoted_identifier on
GO

CREATE TABLE "Sections"(
  "Id" "int" IDENTITY (1,1) NOT NULL,
  "Code" nvarchar(5) NOT NULL,
  "Name" nvarchar(50) NOT NULL,
  CONSTRAINT "PK_Section" PRIMARY KEY CLUSTERED (
    "Id"
  )
)

GO

CREATE TABLE "Profiles"(
  "Id" "int" IDENTITY (1,1) NOT NULL,
  "Name" nvarchar(15) NOT NULL,
  CONSTRAINT "PK_Profile" PRIMARY KEY CLUSTERED (
    "Id"
  )
)

GO

CREATE TABLE "Users"(
  "Id" "int" IDENTITY (1,1) NOT NULL,
  "FirstName" nvarchar(50) NOT NULL,
  "LastName" nvarchar(50) NOT NULL,
  "Password" nvarchar(50) NOT NULL,
  "Email" nvarchar(30),
  "Login" nvarchar(7) NOT NULL,
  "Year" "int",
  "Type" nvarchar(15) NOT NULL CHECK ("Type" IN ('Teacher','Student','Guest','Admin')),
  "RegNumber" "int",
  "Section" "int",
  "Profile" "int" NOT NULL,
  CONSTRAINT "PK_User" PRIMARY KEY CLUSTERED (
    "Id"
  ),
  CONSTRAINT "FK_User_Section" FOREIGN KEY(
    "Section"
  ) REFERENCES "Sections" (
    "Id"
  ),
  CONSTRAINT "FK_User_Profile" FOREIGN KEY(
    "Profile"
  )REFERENCES "Profiles"(
    "Id"
  )
)

GO

CREATE INDEX "UserLogin" ON "Users"("Login")

GO

CREATE TABLE "Softwares"(
  "Id" "int" IDENTITY(1,1) NOT NULL,
  "Name" nvarchar(50) NOT NULL,
  CONSTRAINT "PK_Software" PRIMARY KEY CLUSTERED(
    "Id"
  )
)

GO

CREATE TABLE "Profiles_Softwares"(
  "IdProfile" "int" NOT NULL,
  "IdSoftware" "int" NOT NULL,
  CONSTRAINT "PK_Profile_Software" PRIMARY KEY CLUSTERED(
    "IdProfile", "IdSoftware"
  ),
  CONSTRAINT "FK_Profile" FOREIGN KEY(
    "IdProfile"
  )REFERENCES "Profiles"(
    "Id"
  ),
  CONSTRAINT "FK_Software" FOREIGN KEY(
    "IdSoftware"
  )REFERENCES "Softwares"(
    "Id"
  )
)

GO
