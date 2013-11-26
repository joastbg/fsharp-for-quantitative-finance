CREATE TABLE LOG
(
	log_id int IDENTITY PRIMARY KEY,
	log_datetime datetime DEFAULT CURRENT_TIMESTAMP,
	log_level nvarchar(12) DEFAULT 'info',
	log_msg ntext
)