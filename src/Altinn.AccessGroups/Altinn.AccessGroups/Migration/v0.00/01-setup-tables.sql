-- Schema: accessgroup
CREATE SCHEMA IF NOT EXISTS accessgroup
    AUTHORIZATION postgres;

-- Enum: AccessGroup.AccessGroupType
DO $$ BEGIN
    CREATE TYPE accessgroup.AccessGroupType AS ENUM ('altinn');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Table: accessgroup.AccessGroup
CREATE TABLE IF NOT EXISTS accessgroup.AccessGroup
(
    AccessGroupId bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    AccessGroupCode character varying,
    AccessGroupType accessgroup.AccessGroupType NOT NULL,
    Hidden boolean,
    Created timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Modified timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
)
TABLESPACE pg_default;

-- Table: accessgroup.AccessGroupCategory
CREATE TABLE IF NOT EXISTS accessgroup.AccessGroupCategory
(
    CategoryId bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ParentCategoryId bigint
)
TABLESPACE pg_default;

-- Table: accessgroup.AccessGroupMembership
CREATE TABLE IF NOT EXISTS accessgroup.AccessGroupMembership
(
    MembershipId bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    OfferedByParty bigint,
    UserId bigint,
    PartyId bigint,
    AccessGroupId bigint,
    ValidTo timestamp with time zone
)
TABLESPACE pg_default;

-- Enum: AccessGroup.ExternalSource
DO $$ BEGIN
    CREATE TYPE accessgroup.ExternalSource AS ENUM ('enhetsregisteret');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Table: accessgroup.ExternalRelationship
CREATE TABLE IF NOT EXISTS accessgroup.ExternalRelationship
(
    ExternalSource accessgroup.ExternalSource NOT NULL,
    ExternalID character varying NOT NULL,
    AccessGroupID bigint NOT NULL,
    UnitTypeFilter character varying DEFAULT NULL
)
TABLESPACE pg_default;

-- Enum: AccessGroup.DelegationType
DO $$ BEGIN
    CREATE TYPE accessgroup.DelegationType AS ENUM ('brukerdelegering', 'klientdelegering', 'tjenestedelegering');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Table: accessgroup.MemberShipDelegation
CREATE TABLE IF NOT EXISTS accessgroup.MemberShipDelegation
(
    DelegationId bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    DelegatedByUserId bigint,
    DelegatedByPartyId bigint,
    DelegationTime date,
    DelegationType accessgroup.DelegationType NOT NULL
)
TABLESPACE pg_default;

-- Table: accessgroup.MembershipHistory
CREATE TABLE IF NOT EXISTS accessgroup.MembershipHistory
(
    HistoryId bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    MembershipId bigint,
    OfferedByParty bigint,
    UserId bigint,
    PartyId bigint,
    DelegationId bigint,
    ValidTo timestamp with time zone
)
TABLESPACE pg_default;

-- Table: accessgroup.ResourceRight
CREATE TABLE IF NOT EXISTS accessgroup.ResourceRight
(
    RightId bigint,
    GroupId bigint,
    ResourceId bigint
)
TABLESPACE pg_default;

