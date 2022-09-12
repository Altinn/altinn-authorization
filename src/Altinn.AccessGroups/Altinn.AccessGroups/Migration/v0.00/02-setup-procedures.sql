-- FUNCTION: accessgroup.insert_textresource
CREATE OR REPLACE FUNCTION accessgroup.insert_textresource(
  _textType accessgroup.TextResourceType,
  _key text,
  _content text,
  _language text,
  _accessgroupcode text,
  _categorycode text
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
  _categoryCode text,
  _categoryType accessgroup.CategoryType
)
RETURNS SETOF accessgroup.Category
LANGUAGE 'sql'
VOLATILE
ROWS 1
AS $BODY$
  INSERT INTO accessgroup.Category(
    CategoryCode,
    CategoryType
  )
  VALUES (
    _categoryCode,
    _categoryType
  ) RETURNING *;
$BODY$;

-- FUNCTION: accessgroup.insert_accessgroup
CREATE OR REPLACE FUNCTION accessgroup.insert_accessgroup(
  _accessGroupCode text,
  _accessGroupType accessgroup.AccessGroupType,
  _hidden bool,
  _categoryCodes text[]
)
RETURNS SETOF accessgroup.accessgroup
LANGUAGE 'plpgsql'
VOLATILE
ROWS 1
AS $BODY$
    BEGIN
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
      );

      INSERT INTO accessgroup.AccessGroupCategory(
        AccessGroupCode,
        CategoryCode
      )
      SELECT _accessGroupCode, * FROM UNNEST($4);

      RETURN QUERY(SELECT * FROM accessgroup.AccessGroup WHERE AccessGroupCode = _accessGroupCode);
    END
$BODY$;

-- FUNCTION: accessgroup.insert_externalrelationship
CREATE OR REPLACE FUNCTION accessgroup.insert_externalrelationship(
	_externalsource accessgroup.externalsource,
	_externalid text,
    _accessgroupcode text,
    _unittypefilter text
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
    _accessgroupcode text,
    _categorycode text
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
	_accessgroupcode character varying,
	_delegationtype accessgroup.delegationtype)
    RETURNS SETOF accessgroup.accessgroupmembership 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1

AS $BODY$
DECLARE 
	inserted_delegationid bigint;
BEGIN
	INSERT INTO accessgroup.membershipdelegation(
		delegatedbyuserid,
		delegatedbypartyid,
		delegationtime,
		delegationtype
	)
	VALUES (
		_userid,
		_partyid,
		CURRENT_TIMESTAMP,
		_delegationtype
	) RETURNING delegationid into inserted_delegationid;

INSERT INTO accessgroup.accessgroupmembership(
    offeredbyparty,
    userid,
    partyid,
    delegationid,
	accessgroupcode,
    validto
    )
    VALUES (
    _offeredbyparty,
    _userid,
    _partyid,
    inserted_delegationid,
	_accessgroupcode,
    CURRENT_TIMESTAMP + interval '1 year'
    );
    RETURN QUERY(SELECT * FROM accessgroup.accessgroupmembership WHERE delegationid = inserted_delegationid);
END;
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
    ,delegationid
    ,accessgroupcode
    ,validto
    FROM accessgroup.accessgroupmembership;
$BODY$;