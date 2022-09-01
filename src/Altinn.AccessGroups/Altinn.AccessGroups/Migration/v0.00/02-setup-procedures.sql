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
    _unittypefilter character varying,
	_accessgroupid bigint)
    RETURNS SETOF accessgroup.externalrelationship 
    LANGUAGE 'sql'
    VOLATILE
    ROWS 1
AS $BODY$
    INSERT INTO accessgroup.externalrelationship(
        externalsource,
        externalid,
        unittypefilter,
        accessgroupid
    )
    VALUES (
        _externalsource,
        _externalid,
        _unittypefilter,
        _accessgroupid
    ) RETURNING *;
$BODY$;
