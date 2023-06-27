CREATE TABLE `test`.`users` (
    `id` INT PRIMARY KEY AUTO_INCREMENT,
    `username` VARCHAR(20),
    `email` VARCHAR(320),
    `pass` VARCHAR(128)
);

INSERT INTO `test`.users(id, username, email, pass)
VALUES (NULL, 'admin00', 'admin@email.com', 'admin');

SELECT * FROM test.users;
