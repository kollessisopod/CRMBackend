DROP TABLE IF EXISTS player_game CASCADE;
DROP TABLE IF EXISTS player CASCADE;
DROP TABLE IF EXISTS game CASCADE;
DROP TABLE IF EXISTS employee CASCADE;
DROP TABLE IF EXISTS feedback CASCADE;
DROP TABLE IF EXISTS campaign CASCADE;
DROP TABLE IF EXISTS notification CASCADE;
DROP FUNCTION IF EXISTS get_all_players();
DROP FUNCTION IF EXISTS get_player_avg_score_by_genre(p_player_id INT);
-- Fonksiyonlar için DROP IF EXISTS
DROP FUNCTION IF EXISTS auto_increment_employee_id() CASCADE;
DROP FUNCTION IF EXISTS auto_increment_player_id() CASCADE;
DROP FUNCTION IF EXISTS auto_increment_game_id() CASCADE;
DROP FUNCTION IF EXISTS auto_increment_feedback_id() CASCADE;
DROP FUNCTION IF EXISTS auto_increment_campaign_id() CASCADE;
DROP FUNCTION IF EXISTS auto_increment_notification_id() CASCADE;

-- Triggerlar için DROP IF EXISTS
DROP TRIGGER IF EXISTS trigger_employee_auto_id ON employee CASCADE;
DROP TRIGGER IF EXISTS trigger_player_auto_id ON player CASCADE;
DROP TRIGGER IF EXISTS trigger_game_auto_id ON game CASCADE;
DROP TRIGGER IF EXISTS trigger_feedback_auto_id ON feedback CASCADE;
DROP TRIGGER IF EXISTS trigger_campaign_auto_id ON campaign CASCADE;
DROP TRIGGER IF EXISTS trigger_notification_auto_id ON notification CASCADE;

-- Sequenceları sil
DROP SEQUENCE IF EXISTS employee_seq;
DROP SEQUENCE IF EXISTS player_seq;
DROP SEQUENCE IF EXISTS game_seq;
DROP SEQUENCE IF EXISTS feedback_seq;
DROP SEQUENCE IF EXISTS campaign_seq;
DROP SEQUENCE IF EXISTS notification_seq;


CREATE SEQUENCE employee_seq MINVALUE 100 MAXVALUE 199 INCREMENT 1;
CREATE SEQUENCE player_seq MINVALUE 200 MAXVALUE 299 INCREMENT 1;
CREATE SEQUENCE game_seq MINVALUE 300 MAXVALUE 399 INCREMENT 1;
CREATE SEQUENCE feedback_seq MINVALUE 400 MAXVALUE 499 INCREMENT BY 1;
CREATE SEQUENCE campaign_seq MINVALUE 500 MAXVALUE 599 INCREMENT 1;
CREATE SEQUENCE notification_seq MINVALUE 600 MAXVALUE 699 INCREMENT BY 1;


CREATE TABLE employee(
	employee_id int not null primary key,
	e_name varchar(20) not null,
	e_password varchar(20) not null,
	e_email varchar(40),
	e_type boolean
);

CREATE TABLE player(
	player_id int not null primary key,
	p_name varchar(20) not null,
	p_password varchar(20) not null,
	p_email varchar(40) not null,
	p_pnumber char(10) not null,
	last_online date
);

CREATE TABLE game(
	game_id int not null primary key,
	game_name varchar(20) not null,
	game_genre varchar(20) not null
);

CREATE TABLE player_game(
	p_ID int not null,
	g_ID int not null,
	score int,
	
	CONSTRAINT fk_player FOREIGN KEY (p_ID)
	REFERENCES player(player_id),
	CONSTRAINT fk_game FOREIGN KEY (g_ID)
	REFERENCES game(game_id),
	CONSTRAINT score_ck CHECK (score<11 AND score>0)
);

CREATE TABLE feedback(
	feedback_id int not null primary key,
	sender_id int not null,
	sender_name varchar(20) not null,
	feedback_type varchar(20) not null,
	feedback_info varchar(255) not null,
	
	CONSTRAINT fk_feedback FOREIGN KEY (sender_id)
	REFERENCES player(player_id)
);

CREATE TABLE campaign(
	campaign_id int not null primary key,
	campaign_info varchar(50) not null,
	hasReward boolean not null,
	reward_info varchar(20)
);

CREATE TABLE notification(
	not_id int not null primary key,
	reciever_id int not null,
	not_info varchar(50) not null,
	is_read boolean,
	
	CONSTRAINT fk_not FOREIGN KEY (reciever_id)
	REFERENCES player(player_id)	
);

--INSERT INTO player VALUES (21011050, 'Ali Eren Arık', '12346789', 'eren.arik@std.yildiz.edu.tr', '5352528503', '2024-12-22');

--INSERT INTO employee VALUES (21011104, 'Aziz Çifçibaşı', '12346789', 'azizcifcibasi7@gmail.com', '1'); 


CREATE OR REPLACE FUNCTION get_all_players()
RETURNS TABLE (
    player_id INT,
    p_name VARCHAR(20),
    p_password VARCHAR(20),
    p_email VARCHAR(40),
    p_pnumber CHAR(10),
    last_online DATE
) AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM player;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_player_avg_score_by_genre(p_player_id INT)
RETURNS TABLE (
    game_genre VARCHAR(20),
    avg_score NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        g.game_genre,
        AVG(pg.score) AS avg_score
    FROM 
        player_game pg
    JOIN 
        game g ON pg.g_ID = g.game_id
    WHERE 
        pg.p_ID = p_player_id
    GROUP BY 
        g.game_genre;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auto_increment_employee_id()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.employee_id IS NULL OR NEW.employee_id = 1 THEN
        NEW.employee_id := NEXTVAL('employee_seq');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger tanımlaması
CREATE TRIGGER trigger_employee_auto_id
BEFORE INSERT ON employee
FOR EACH ROW
EXECUTE FUNCTION auto_increment_employee_id();

-- Trigger fonksiyonu
CREATE OR REPLACE FUNCTION auto_increment_player_id()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.player_id IS NULL OR NEW.player_id = 1 THEN
        NEW.player_id := NEXTVAL('player_seq');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger tanımlaması
CREATE TRIGGER trigger_player_auto_id
BEFORE INSERT ON player
FOR EACH ROW
EXECUTE FUNCTION auto_increment_player_id();

CREATE OR REPLACE FUNCTION auto_increment_game_id()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.game_id IS NULL OR NEW.game_id = 1 THEN
        NEW.game_id := NEXTVAL('game_seq');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger tanımlaması
CREATE TRIGGER trigger_game_auto_id
BEFORE INSERT ON game
FOR EACH ROW
EXECUTE FUNCTION auto_increment_game_id();

-- Trigger fonksiyonu
CREATE OR REPLACE FUNCTION auto_increment_feedback_id()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.feedback_id IS NULL OR NEW.feedback_id = 1 THEN
        NEW.feedback_id := NEXTVAL('feedback_seq');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger tanımlaması
CREATE TRIGGER trigger_feedback_auto_id
BEFORE INSERT ON feedback
FOR EACH ROW
EXECUTE FUNCTION auto_increment_feedback_id();

-- Trigger fonksiyonu
CREATE OR REPLACE FUNCTION auto_increment_campaign_id()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.campaign_id IS NULL OR NEW.campaign_id = 1 THEN
        NEW.campaign_id := NEXTVAL('campaign_seq');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger tanımlaması
CREATE TRIGGER trigger_campaign_auto_id
BEFORE INSERT ON campaign
FOR EACH ROW
EXECUTE FUNCTION auto_increment_campaign_id();

-- Trigger fonksiyonu
CREATE OR REPLACE FUNCTION auto_increment_notification_id()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.not_id IS NULL OR NEW.not_id = 1 THEN
        NEW.not_id := NEXTVAL('notification_seq');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger tanımlaması
CREATE TRIGGER trigger_notification_auto_id
BEFORE INSERT ON notification
FOR EACH ROW
EXECUTE FUNCTION auto_increment_notification_id();

-- Employee tablosuna 10 örnek veri
INSERT INTO employee (employee_id, e_name, e_password, e_email, e_type) 
VALUES 
(1, 'John Doe', 'secure123', 'johndoe@example.com', TRUE),
(1, 'Jane Smith', 'password456', 'janesmith@example.com', FALSE),
(1, 'Robert Brown', 'pass789', 'robertbrown@example.com', TRUE),
(1, 'Emily Davis', 'emilyd123', 'emilydavis@example.com', TRUE),
(1, 'Michael Johnson', 'mjohnson1', 'michaeljohnson@example.com', FALSE),
(1, 'Sarah Miller', 'sarahm22', 'sarahmiller@example.com', TRUE),
(1, 'David Wilson', 'dwilson99', 'davidwilson@example.com', FALSE),
(1, 'Jessica Taylor', 'jtaylor88', 'jessicataylor@example.com', TRUE),
(1, 'Daniel White', 'dwhite77', 'danielwhite@example.com', FALSE),
(1, 'Laura Moore', 'lmoore33', 'lauramoore@example.com', TRUE);

-- Player tablosuna 10 örnek veri
INSERT INTO player (player_id, p_name, p_password, p_email, p_pnumber, last_online) 
VALUES 
(1, 'Ali Veli', 'pass123', 'aliveli@example.com', '5555551234', '2024-12-22'),
(1, 'Ahmet Can', 'ahmet456', 'ahmetcan@example.com', '5555555678', '2024-12-20'),
(1, 'Ayşe Yılmaz', 'ayseyil789', 'ayseyilmaz@example.com', '5555559876', '2024-12-18'),
(1, 'Fatma Kaya', 'fatma999', 'fatmakaya@example.com', '5555554321', '2024-12-17'),
(1, 'Mehmet Ak', 'mehmet888', 'mehmetak@example.com', '5555556543', '2024-12-15'),
(1, 'Zeynep Gül', 'zeynepg', 'zeynepgul@example.com', '5555558765', '2024-12-14'),
(1, 'Emre Deniz', 'emredeniz1', 'emredeniz@example.com', '5555555432', '2024-12-12'),
(1, 'Hakan Çelik', 'hakan1234', 'hakan@example.com', '5555556543', '2024-12-10'),
(1, 'Melis Öztürk', 'melis1234', 'melis@example.com', '5555558765', '2024-12-08'),
(1, 'Ceren Tuncel', 'ceren4567', 'ceren@example.com', '5555555432', '2024-12-06');

-- Game tablosuna 10 örnek veri
INSERT INTO game (game_id, game_name, game_genre) 
VALUES 
(1, 'Chess', 'Strategy'),
(1, 'Sudoku', 'Puzzle'),
(1, 'Fortnite', 'Battle Royale'),
(1, 'Overwatch', 'Shooter'),
(1, 'FIFA', 'Sports'),
(1, 'Minecraft', 'Sandbox'),
(1, 'Among Us', 'Party'),
(1, 'The Sims', 'Simulation'),
(1, 'Candy Crush', 'Casual'),
(1, 'Call of Duty', 'Shooter');

INSERT INTO player_game (p_id, g_id, score)
VALUES
(201, 308, 7),
(203, 304, 5),
(206, 305, 6),
(208, 300, 10),
(202, 303, 4),
(205, 306, 8),
(207, 307, 9),
(204, 301, 7),
(209, 309, 5);

-- Feedback tablosuna 10 örnek veri
INSERT INTO feedback (feedback_id, sender_id, sender_name, feedback_type, feedback_info) 
VALUES 
(1, 200, 'Ali Veli', 'Positive', 'Great game, I had so much fun!'),
(1, 201, 'Ahmet Can', 'Negative', 'The game crashes frequently.'),
(1, 202, 'Ayşe Yılmaz', 'Neutral', 'The gameplay is okay, but could be better.'),
(1, 203, 'Fatma Kaya', 'Positive', 'Loved the graphics and story!'),
(1, 204, 'Mehmet Ak', 'Negative', 'The controls are very hard to use.'),
(1, 205, 'Zeynep Gül', 'Positive', 'Amazing experience, highly recommend!'),
(1, 206, 'Emre Deniz', 'Neutral', 'It’s alright, but not worth the price.'),
(1, 207, 'Hakan Çelik', 'Positive', 'I play it every day, so much fun!'),
(1, 208, 'Melis Öztürk', 'Negative', 'Poor customer support.'),
(1, 209, 'Ceren Tuncel', 'Positive', 'Best game I’ve played this year!');

-- Campaign tablosuna 10 örnek veri
INSERT INTO campaign (campaign_id, campaign_info, hasReward, reward_info) 
VALUES 
(1, 'Winter Sale', TRUE, '50% off'),
(1, 'Summer Campaign', TRUE, 'Free Skin'),
(1, 'Autumn Promo', TRUE, '20% off'),
(1, 'Spring Fest', FALSE, NULL),
(1, 'Holiday Special', TRUE, 'Double XP'),
(1, 'Black Friday', TRUE, '70% off'),
(1, 'Cyber Monday', TRUE, 'Exclusive Item'),
(1, 'Weekend Bonus', FALSE, NULL),
(1, 'Monthly Rewards', TRUE, 'Free Loot Box'),
(1, 'Anniversary Event', TRUE, 'Golden Skin');

-- Notification tablosuna 10 örnek veri
INSERT INTO notification (not_id, reciever_id, not_info, is_read) 
VALUES 
(1, 200, 'You have a new message!', FALSE),
(1, 201, 'Your friend sent a game invite.', TRUE),
(1, 202, 'Level up! Claim your reward.', FALSE),
(1, 203, 'Special offer: Get 50% off now!', FALSE),
(1, 204, 'New update available, download now.', TRUE),
(1, 205, 'Daily reward available.', FALSE),
(1, 206, 'Your XP boost has expired.', TRUE),
(1, 207, 'Weekly challenge completed!', TRUE),
(1, 208, 'Achievement unlocked: Master Player.', TRUE),
(1, 209, 'Server maintenance scheduled.', FALSE);


