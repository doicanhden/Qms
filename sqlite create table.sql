CREATE TABLE Card
(
	CardId TEXT PRIMARY KEY,
	Data TEXT,
	Type TEXT,
	State Integer /* 0 = Use, 1 = Error, 2 = Lock, 4 = Lost */
);

CREATE TABLE Employee
(
	EmployeeId TEXT PRIMARY KEY,
	CardId Text,
	Username Text,
	Password Text,
	
	FOREIGN KEY (CardId) REFERENCES Card(CardId)
);

CREATE TABLE InOutRecord
(
	RecordId INTEGER PRIMARY KEY AUTOINCREMENT,

	InEmployeeId TEXT,
	InLaneCode TEXT,
	InTime TEXT,
	InBackImg TEXT,
	InFrontImg TEXT,

	OutEmployeeId TEXT,
	OutLaneCode TEXT,
	OutTime TEXT,
	OutBackImg TEXT,
	OutFrontImg TEXT,

	CardId TEXT,
	UserData TEXT,
	Comment TEXT,
	FeeValue NUMERIC,
	
	FOREIGN KEY (CardId) REFERENCES Card(CardId),
	FOREIGN KEY (InEmployeeId) REFERENCES Employee(EmployeeId),
	FOREIGN KEY (OutEmployeeId) REFERENCES Employee(EmployeeId)
);

