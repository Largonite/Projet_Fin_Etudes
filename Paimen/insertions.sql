SET NOCOUNT ON
GO
USE PAIMEN
GO

INSERT INTO Sections(Code, Name)
	VALUES 
	('BIN', 'Bachelier Informatique de Gestion'),
	('BDI', 'Bachelier Diététique'),
	('BBMI', 'Bachelier Biochimie');

GO

INSERT INTO Softwares(Name)
	VALUES
	('Windows'),
	('Claroline'),
	('Nutrilog');
GO

INSERT INTO Profiles(Name)
	VALUES ('Admin'),
		('1BIN'),
		('1BDI'),
		('1BBM'),
		('2BIN'),
		('2BDI'),
		('2BBM'),
		('3BIN'),
		('3BDI'),
		('3BBM');
GO


INSERT INTO Profiles_Softwares(IdProfile, IdSoftware)
	VALUES
	(1, 1),
	(1, 2),
	(1, 3),

	(2, 1),
	(2, 2),

	(3, 1),
	(3, 2),
	(3, 3),

	(4, 1),
	(4, 2);

GO

INSERT INTO Users(FirstName, LastName, Password, Login, Year, Type, RegNumber, Section, Profile, AddedDate)
	VALUES 
	('AdminFirstName', 'AdminLastName', '123456', 'admin', 0, 'Admin', 1, NULL, 1, GETDATE()),
	
	('Info1FirstName', 'Info1LastName', '123456', 'info1', 1, 'Student', 10, 1, 2, GETDATE()),
	('Info2FirstName', 'Info2LastName', '123456','info2', 2, 'Student', 11, 1, 2, GETDATE()),
	('Info3FirstName', 'Info3LastName', '123456','info3', 3, 'Student', 12, 1, 2, GETDATE()),

	('Diet1FirstName', 'Diet1LastName', '123456','diet1', 1, 'Student', 20, 2, 3, GETDATE()),
	('Diet2FirstName', 'Diet2LastName', '123456','diet2', 2, 'Student', 21, 2, 3, GETDATE()),
	('Diet3FirstName', 'Diet3LastName', '123456','diet3', 3, 'Student', 22, 2, 3, GETDATE()),


	('Bio1FirstName', 'Bio1LastName', '123456','bio1', 1, 'Student', 30, 3, 4, GETDATE()),
	('Bio2FirstName', 'Bio2LastName', '123456','bio2', 2, 'Student', 31, 3, 4, GETDATE()),
	('Bio3FirstName', 'Bio3LastName', '123456','bio3', 3, 'Student', 32, 3, 4, GETDATE());

GO