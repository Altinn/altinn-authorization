-- Function: insert_accessgroup
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