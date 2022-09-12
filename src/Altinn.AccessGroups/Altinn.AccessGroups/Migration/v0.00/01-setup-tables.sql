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
    AccessGroupCode text PRIMARY KEY,
    AccessGroupType accessgroup.AccessGroupType NOT NULL,
    Hidden boolean,
    Created timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Modified timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
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
    ExternalId text NOT NULL,
    AccessGroupCode text NOT NULL,
    UnitTypeFilter text DEFAULT NULL,
    PRIMARY KEY (ExternalSource, ExternalId, AccessGroupCode),
    CONSTRAINT externalrelationship_accessgroup_fkey FOREIGN KEY (AccessGroupCode) REFERENCES accessgroup.AccessGroup(AccessGroupCode)
)
TABLESPACE pg_default;

-- Enum: AccessGroup.CategoryType
DO $$ BEGIN
    CREATE TYPE accessgroup.CategoryType AS ENUM ('organization', 'person');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Table: accessgroup.Category
CREATE TABLE IF NOT EXISTS accessgroup.Category
(
    CategoryCode text PRIMARY KEY,
    CategoryType accessgroup.CategoryType
)
TABLESPACE pg_default;

-- Table: accessgroup.AccessGroupCategory
CREATE TABLE IF NOT EXISTS accessgroup.AccessGroupCategory
(
    AccessGroupCode text,
    CategoryCode text,
    PRIMARY KEY (AccessGroupCode, CategoryCode),
    CONSTRAINT accessgroupcategory_accessgroup_fkey FOREIGN KEY (AccessGroupCode) REFERENCES accessgroup.AccessGroup(AccessGroupCode),
    CONSTRAINT accessgroupcategory_category_fkey FOREIGN KEY (CategoryCode) REFERENCES accessgroup.Category(CategoryCode)
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

-- Table: accessgroup.AccessGroupMembership
CREATE TABLE IF NOT EXISTS accessgroup.AccessGroupMembership
(
    MembershipId bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    OfferedByParty bigint,
    UserId bigint,
    PartyId bigint,
    DelegationId bigint,
    AccessGroupCode character varying,
    ValidTo timestamp with time zone,
    CONSTRAINT delegationId_fkey FOREIGN KEY (delegationid)
    REFERENCES accessgroup.membershipdelegation (delegationid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
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

-- Enum: AccessGroup.TextResourceType
DO $$ BEGIN
    CREATE TYPE accessgroup.TextResourceType AS ENUM ('accessgroup', 'category');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Table: accessgroup.TextResources
CREATE TABLE IF NOT EXISTS accessgroup.TextResource
(
    TextResourceId bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    TextType accessgroup.TextResourceType NOT NULL,
    Key text NOT NULL,
    Content text NOT NULL,
    Language text NOT NULL,
    AccessGroupCode text,
    CategoryCode text,
    CONSTRAINT textresource_accessgroup_fkey FOREIGN KEY (AccessGroupCode) REFERENCES accessgroup.AccessGroup(AccessGroupCode),
    CONSTRAINT textresource_category_fkey FOREIGN KEY (CategoryCode) REFERENCES accessgroup.Category(CategoryCode)
)
TABLESPACE pg_default;

