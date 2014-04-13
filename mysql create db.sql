create database vidoparking;

CREATE TABLE card_type
(
	card_type_id INTEGER PRIMARY KEY,
	name nvarchar(30),
	description nvarchar(100)
);

CREATE TABLE card
(
	card_id nvarchar(100) PRIMARY KEY,
	card_type integer,
	is_use bit,

	FOREIGN KEY(card_type) REFERENCES card_type(card_type_id)
);

CREATE TABLE entry_exit
(
	entry_exit_id int primary key,
	card nvarchar(100),

	entry_plate_number nvarchar(15),
	entry_time datetime,
	entry_plate_image nvarchar(255),
	entry_face_image nvarchar(255),

	exit_plate_number nvarchar(15),
	exit_time datetime,
	exit_plate_image nvarchar(255),
	exit_face_image nvarchar(255),

	foreign key(card) references card(card_id)
);



/* */
create table customer_type
(
	customer_type_id int primary key,
	name nvarchar(30),
	discount double,
	description nvarchar(100)
);

create table customer
(
	customer_id int primary key,
	customer_type int,
	customer_code char(8),
	first_name nvarchar(15),
	last_name nvarchar(30),
	email nvarchar(100),
	address nvarchar(250),
	phone nchar(20),

	foreign key(customer_type) references customer_type(customer_type_id)
);

create table month_card
(
	month_card_id int primary key,
	customer int,
	plate_number nvarchar(15),
	active bit,
	is_expired bit,
	from_date datetime,
	to_date datetime,
	last_update datetime,
	price numeric(20, 2),

	foreign key(customer) references customer(customer_id)
);

INSERT INTO card_type(card_type_id, Name, Description)
VALUES(0, 'THEXE', 'The xe');

INSERT INTO card(card_id, card_type, is_use) VALUES('0228282404', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0082408356', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0082408340', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0228282932', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0228373332', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0086260500', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0228282868', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0228282916', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0086260804', 0, 1);
INSERT INTO card(card_id, card_type, is_use) VALUES('0082408404', 0, 1);





















