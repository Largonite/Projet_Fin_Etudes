SET NOCOUNT ON
GO
USE PAIMEN
GO

INSERT INTO Sections(Code, Name)
	VALUES ('BIN', 'Bachelier Informatique de Gestion'),
	('BDI', 'Bachelier Diététique'),
	('BIN', 'Bachelier Biochimie');

GO


INSERT INTO Profiles(Name)
	VALUES ('Informatique de gestion'),
		('Diététique'),
		('Biochimie');

GO

INSERT INTO Users(FirstName, LastName, Password, Login, Year, Type, RegNumber, Section, Profile)
	VALUES ('Info1FirstName', 'Info1LastName', '123456', 'info1', 1, 'Student', 10, 1, 1),
	('Info2FirstName', 'Info2LastName', '123456','info2', 2, 'Student', 11, 1, 1),
	('Info3FirstName', 'Info3LastName', '123456','info3', 3, 'Student', 12, 1, 1),

	('Diet1FirstName', 'Diet1LastName', '123456','diet1', 1, 'Student', 20, 2, 2),
	('Diet2FirstName', 'Diet2LastName', '123456','diet2', 2, 'Student', 21, 2, 2),
	('Diet3FirstName', 'Diet3LastName', '123456','diet3', 3, 'Student', 22, 2, 2),


	('Bio1FirstName', 'Bio1LastName', '123456','bio1', 1, 'Student', 10, 3, 3),
	('Bio2FirstName', 'Bio2LastName', '123456','bio2', 2, 'Student', 10, 3, 3),
	('Bio3FirstName', 'Bio3LastName', '123456','bio3', 3, 'Student', 10, 3, 3);

GO