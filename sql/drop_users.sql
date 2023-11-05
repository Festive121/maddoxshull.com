DELETE FROM users;
DELETE FROM user_cards;
ALTER TABLE users AUTO_INCREMENT = 1;
SELECT * FROM users JOIN user_cards ON users.id = user_cards.user_id;