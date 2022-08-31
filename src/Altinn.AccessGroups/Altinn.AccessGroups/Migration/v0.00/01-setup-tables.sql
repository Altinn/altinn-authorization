-- Enum: AccessGroup.AccessGroupType
DO $$ BEGIN
    CREATE TYPE accessgroup.AccessGroupType AS ENUM ('altinn');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

CREATE TABLE IF NOT EXISTS accessgroup."AccessGroup"
(
    "AccessGroupId" bigint NOT NULL DEFAULT nextval('accessgroup."AccessGroup_AccessGroupId_seq"'::regclass),
    "AccessGroupCode" "char"[],
    "AccessGroupType" accessgroup.AccessGroupType NOT NULL,
    "Hidden" boolean,
    CONSTRAINT "AccessGroup_pkey" PRIMARY KEY ("AccessGroupId")
)

TABLESPACE pg_default;

ALTER TABLE accessgroup."AccessGroup"
    OWNER to postgres;

-- Table: accessgroup.AccessGroupCategory


-- DROP TABLE accessgroup."AccessGroupCategory";

CREATE TABLE IF NOT EXISTS accessgroup."AccessGroupCategory"
(
    "CategoryId" bigint NOT NULL DEFAULT nextval('accessgroup."AccessGroupCategory_CategoryId_seq"'::regclass),
    "ParentCategoryId" integer,
    CONSTRAINT "AccessGroupCategory_pkey" PRIMARY KEY ("CategoryId")
)

TABLESPACE pg_default;

ALTER TABLE accessgroup."AccessGroupCategory"
    OWNER to postgres;

-- Table: accessgroup.AccessGroupMembership

-- DROP TABLE accessgroup."AccessGroupMembership";

CREATE TABLE IF NOT EXISTS accessgroup."AccessGroupMembership"
(
    "MembershipID" bigint NOT NULL,
    "OfferedByParty" bigint,
    "UserId" bigint,
    "PartyId" bigint,
    "DelegationId" bigint,
    "ValidTo" date,
    CONSTRAINT "AccessGroupMembership_pkey" PRIMARY KEY ("MembershipID")
)

TABLESPACE pg_default;

ALTER TABLE accessgroup."AccessGroupMembership"
    OWNER to postgres;

    -- Table: accessgroup.ExternalRelationship

-- DROP TABLE accessgroup."ExternalRelationship";

-- Enum: AccessGroup.ExternalSource
DO $$ BEGIN
    CREATE TYPE accessgroup.ExternalSource AS ENUM ('ER');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;


CREATE TABLE IF NOT EXISTS accessgroup."ExternalRelationship"
(
    "ExternalSource" accessgroup.ExternalSource NOT NULL,
    "ExternalID" char[] NOT NULL,
    "PartyTypeFilter" char[] NOT NULL,
    "AccessGroupID" bigint
)

TABLESPACE pg_default;

ALTER TABLE accessgroup."ExternalRelationship"
    OWNER to postgres;

-- Table: accessgroup.MemberShipDelegation

-- DROP TABLE accessgroup."MemberShipDelegation";

--Brukerdelegering, Klientdelegering, Tjenestedelegering
-- Enum: AccessGroup.DelegationType
DO $$ BEGIN
    CREATE TYPE accessgroup.DelegationType AS ENUM ('Brukerdelegering', 'Klientdelegering', 'Tjenestedelegering');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

CREATE TABLE IF NOT EXISTS accessgroup."MemberShipDelegation"
(
    "DelegationId" bigint NOT NULL DEFAULT nextval('accessgroup."MemberShipDelegation_DelegationId_seq"'::regclass),
    "DelegatedByUserId" bigint,
    "DelegatedByPartyId" bigint,
    "DelegationTime" date,
    "DelegationType" text COLLATE pg_catalog."default",
    CONSTRAINT "MemberShipDelegation_pkey" PRIMARY KEY ("DelegationId")
)

TABLESPACE pg_default;

ALTER TABLE accessgroup."MemberShipDelegation"
    OWNER to postgres;

-- Table: accessgroup.MembershipHistory

-- DROP TABLE accessgroup."MembershipHistory";

CREATE TABLE IF NOT EXISTS accessgroup."MembershipHistory"
(
    "HistoryId" bigint NOT NULL DEFAULT nextval('accessgroup."MembershipHistory_HistoryId_seq"'::regclass),
    "MembershipId" bigint,
    "OfferedByParty" bigint,
    "UserId" bigint,
    "PartyId" bigint,
    "DelegationId" bigint,
    "ValidTo" date,
    CONSTRAINT "MembershipHistory_pkey" PRIMARY KEY ("HistoryId")
)

TABLESPACE pg_default;

ALTER TABLE accessgroup."MembershipHistory"
    OWNER to postgres;

-- Table: accessgroup.ResourceRight

-- DROP TABLE accessgroup."ResourceRight";

CREATE TABLE IF NOT EXISTS accessgroup."ResourceRight"
(
    "RightId" bigint,
    "GroupId" bigint,
    "ResourceId" bigint
)

TABLESPACE pg_default;

ALTER TABLE accessgroup."ResourceRight"
    OWNER to postgres;
