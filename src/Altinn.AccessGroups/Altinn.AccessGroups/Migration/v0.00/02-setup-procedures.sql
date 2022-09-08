-- FUNCTION: accessgroup.insert_textresource
CREATE OR REPLACE FUNCTION accessgroup.insert_textresource(
  _textType accessgroup.TextResourceType,
  _key character varying,
  _content character varying,
  _language character varying,
  _accessgroupcode character varying,
  _categorycode character varying
)
RETURNS SETOF accessgroup.TextResource
LANGUAGE 'sql'
VOLATILE
ROWS 1
AS $BODY$
  INSERT INTO accessgroup.TextResource(
    TextType, 
    Key,
    Content,
    Language,
    AccessGroupCode,
    CategoryCode
  )
  VALUES (
    _textType,
    _key,
    _content,
    _language,
    _accessgroupcode,
    _categorycode
  ) RETURNING *;
$BODY$;

-- FUNCTION: accessgroup.insert_category
CREATE OR REPLACE FUNCTION accessgroup.insert_category(
  _categoryCode character varying
)
RETURNS SETOF accessgroup.Category
LANGUAGE 'sql'
VOLATILE
ROWS 1
AS $BODY$
  INSERT INTO accessgroup.Category(
    CategoryCode
  )
  VALUES (
    _categoryCode
  ) RETURNING *;
$BODY$;

-- FUNCTION: accessgroup.insert_accessgroup
CREATE OR REPLACE FUNCTION accessgroup.insert_accessgroup(
  _accessGroupCode character varying,
  _accessGroupType accessgroup.AccessGroupType,
  _hidden bool
)
RETURNS SETOF accessgroup.accessgroup
LANGUAGE 'sql'
VOLATILE
ROWS 1
AS $BODY$
  INSERT INTO accessgroup.AccessGroup(
    AccessGroupCode,
    AccessGroupType, 
    Hidden,
    Created,
    Modified
  )
  VALUES (
    _accessGroupCode,
    _accessGroupType,
    _hidden,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
  ) RETURNING *;
$BODY$;

-- FUNCTION: accessgroup.insert_externalrelationship
CREATE OR REPLACE FUNCTION accessgroup.insert_externalrelationship(
	_externalsource accessgroup.externalsource,
	_externalid character varying,
    _accessgroupcode character varying,
    _unittypefilter character varying
	)
    RETURNS SETOF accessgroup.externalrelationship 
    LANGUAGE 'sql'
    VOLATILE
    ROWS 1
AS $BODY$
    INSERT INTO accessgroup.externalrelationship(
        externalsource,
        externalid,
        accessgroupcode,
        unittypefilter        
    )
    VALUES (
        _externalsource,
        _externalid,
        _accessgroupcode,
        _unittypefilter        
    ) RETURNING *;
$BODY$;

-- FUNCTION: accessgroup.insert_accessgroupcategory
CREATE OR REPLACE FUNCTION accessgroup.insert_accessgroupcategory(
    _accessgroupcode character varying,
    _categorycode character varying
	)
    RETURNS SETOF accessgroup.AccessGroupCategory 
    LANGUAGE 'sql'
    VOLATILE
    ROWS 1
AS $BODY$
    INSERT INTO accessgroup.AccessGroupCategory(
        accessgroupcode,
        categorycode     
    )
    VALUES (
        _accessgroupcode,
        _categorycode      
    ) RETURNING *;
$BODY$;

--FUNCTION: accessgroup.insert_accessgroupmembership
CREATE OR REPLACE FUNCTION accessgroup.insert_accessgroupmembership(
    _offeredbyparty bigint,
    _userid bigint,
    _partyid bigint,
    _delegationid bigint
    )
    RETURNS SETOF accessgroup.accessgroupmembership
    LANGUAGE 'sql'
    VOLATILE
    ROWS 1
AS $BODY$
    INSERT INTO accessgroup.accessgroupmembership(
    offeredbyparty,
    userid,
    partyid,
    delegationid,
    validto
    )
    VALUES (
    _offeredbyparty,
    _userid,
    _partyid,
    _delegationid,
    CURRENT_TIMESTAMP + interval '1 year'
    ) RETURNING *;
$BODY$;

--FUNCTION: accessgroup.select_accessgroupmembership
    CREATE OR REPLACE FUNCTION accessgroup.select_accessgroupmembership()
    RETURNS SETOF accessgroup.accessgroupmembership
    LANGUAGE 'sql'
    VOLATILE
    ROWS 1
AS $BODY$
    SELECT membershipid
    ,offeredbyparty
    ,userid
    ,partyid
    ,accessgroupid
    ,validto
    FROM accessgroup.accessgroupmembership;
$BODY$;