CREATE TABLE IF NOT EXISTS "Services" (
    "Id" uuid PRIMARY KEY,
    "Title" text NOT NULL,
    "Description" text NOT NULL,
    "Price" numeric NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "DoctorId" uuid NOT NULL
);

CREATE TABLE IF NOT EXISTS "Specializations" (
    "Id" uuid PRIMARY KEY,
    "Title" text NOT NULL,
    "Description" text NOT NULL
);

CREATE TABLE IF NOT EXISTS "Offices" (
    "Id" uuid PRIMARY KEY,
    "Title" text NOT NULL
);
