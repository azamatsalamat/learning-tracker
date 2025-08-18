CREATE TABLE users (
    id UUID PRIMARY KEY,
    creation_date TIMESTAMP NOT NULL,
    login VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL
);

CREATE TABLE profiles (
    id UUID PRIMARY KEY,
    creation_date TIMESTAMP NOT NULL
);

ALTER TABLE profiles 
ADD CONSTRAINT fk_profiles_user_id 
FOREIGN KEY (id) REFERENCES users(id) ON DELETE CASCADE;
