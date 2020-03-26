Create TABLE Users (
	UserId int IDENTITY(1,1) PRIMARY KEY NOT NULL ,
	UserName varchar NOT NULL,
	UserPassword varchar NOT NULL,
	Email varchar
);
