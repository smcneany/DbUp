create or replace table Alert (
Id int not null primary key AUTOINCREMENT,
ScriptName string(255) not null,
Applied  TIMESTAMP_LTZ(9) not null DEFAULT(CURRENT_TIMESTAMP));