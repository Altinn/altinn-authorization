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
    _accessgroupid bigint,
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
        accessgroupid,
        unittypefilter        
    )
    VALUES (
        _externalsource,
        _externalid,
        _accessgroupid,
        _unittypefilter        
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
