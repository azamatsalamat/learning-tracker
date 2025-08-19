CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    CreationDate TIMESTAMP NOT NULL,
    Login VARCHAR(100) NOT NULL,
    Password VARCHAR(255) NOT NULL
);

CREATE TABLE Profiles (
    Id UUID PRIMARY KEY,
    CreationDate TIMESTAMP NOT NULL,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    Email VARCHAR(255),
    Phone VARCHAR(20),
    City VARCHAR(100),
    Country VARCHAR(100),
    Summary TEXT,
    Skills TEXT[],
    Languages TEXT[],
    Experiences JSONB,
    Educations JSONB,
    PersonalProjects JSONB,
    Certifications JSONB,
    Publications JSONB,
    Awards JSONB
);

ALTER TABLE Profiles 
ADD CONSTRAINT fk_profiles_user_id 
FOREIGN KEY (Id) REFERENCES Users(Id) ON DELETE CASCADE;
