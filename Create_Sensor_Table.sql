Create TABLE Sensors (
	SensorId int IDENTITY(1,1) PRIMARY KEY NOT NULL ,
	UserId int references Users(UserId),
	Latitude float NOT NULL,
	Longitude float NOT NULL,
	SensorType varchar
);
