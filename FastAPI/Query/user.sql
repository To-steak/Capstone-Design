USE user_db;

CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(20) NOT NULL,
    score INT DEFAULT 0,
    badge BIT(3) NOT NULL DEFAULT b'000'
);

# test case
INSERT INTO users (name, score, badge) VALUES ('user@0001', 20, 0);
INSERT INTO users (name, score, badge) VALUES ('user@0002', 900, 1);
INSERT INTO users (name, score, badge) VALUES ('user@0003', 2000, 2);
INSERT INTO users (name, score, badge) VALUES ('user@0004', 3000, 4);

# show all users
SELECT * FROM users;

delete from users where name = 'user@8781';

truncate table users;
