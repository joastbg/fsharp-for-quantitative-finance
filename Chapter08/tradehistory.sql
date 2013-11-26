CREATE TABLE TRADEHISTORY
(
	tradehistory_id int IDENTITY PRIMARY KEY,
	tradehistory_datetime datetime DEFAULT CURRENT_TIMESTAMP,
	tradehistory_instrument nvarchar(12),
	tradehistory_qty int,
	tradehistory_type nvarchar(12),
	tradehistory_price float
)