CREATE TABLE "User" (
    "Id" UUID PRIMARY KEY,
    "CreationDate" TIMESTAMP NOT NULL,
    "Login" VARCHAR(100) NOT NULL,
    "Password" VARCHAR(255) NOT NULL
);

CREATE TABLE "Profile" (
    "Id" UUID PRIMARY KEY,
    "CreationDate" TIMESTAMP NOT NULL,
    "Name_FirstName" VARCHAR(100),
    "Name_LastName" VARCHAR(100),
    "Email" VARCHAR(255),
    "Phone" VARCHAR(20),
    "Address_City" VARCHAR(100),
    "Address_Country" VARCHAR(100),
    "Summary" TEXT,
    "Skills" JSONB,
    "Languages" JSONB,
    "Experiences" JSONB,
    "Educations" JSONB,
    "PersonalProjects" JSONB,
    "Certifications" JSONB,
    "Publications" JSONB,
    "Awards" JSONB
);

ALTER TABLE "Profile"
ADD CONSTRAINT "FK_Profile_User_Id"
FOREIGN KEY ("Id") REFERENCES "User"("Id") ON DELETE CASCADE;
