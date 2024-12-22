DROP TABLE IF EXISTS player_game CASCADE;
DROP TABLE IF EXISTS player CASCADE;
DROP TABLE IF EXISTS game CASCADE;
DROP TABLE IF EXISTS employee CASCADE;
DROP TABLE IF EXISTS feedback CASCADE;
DROP TABLE IF EXISTS campaign CASCADE;
DROP TABLE IF EXISTS notification CASCADE;
DROP FUNCTION IF EXISTS get_all_players();
DROP FUNCTION IF EXISTS get_player_avg_score_by_genre(p_player_id INT);
DROP TRIGGER IF EXISTS trg_update_feedback_id ON feedback;
DROP FUNCTION IF EXISTS update_feedback_id();
DROP SEQUENCE IF EXISTS fseq;

CREATE SEQUENCE f_seq
MINVALUE 1
MAXVALUE 100
INCREMENT BY 1

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
	feedback_info varchar(20) not null,
	
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
	reciever_id int not null,
	not_info varchar(50) not null,
	isRead boolean,
	
	CONSTRAINT fk_not FOREIGN KEY (reciever_id)
	REFERENCES player(player_id)	
);

INSERT INTO player VALUES (21011050, 'Ali Eren Arık', '12346789', 'eren.arik@std.yildiz.edu.tr', '5352528503', '2024-12-22');

INSERT INTO employee VALUES (21011104, 'Aziz Çifçibaşı', '12346789', 'azizcifcibasi7@gmail.com', '1'); 


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

CREATE OR REPLACE FUNCTION update_feedback_id()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.feedback_id = 0 THEN
        NEW.feedback_id := NEXTVAL('f_seq');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_feedback_id
BEFORE INSERT ON feedback
FOR EACH ROW
EXECUTE FUNCTION update_feedback_id();

INSERT INTO feedback (feedback_id, sender_id, sender_name, feedback_type, feedback_info)
VALUES (0, 21011050, 'Ali Eren Arık', 'Suggestion', 'Great Game');

SELECT * FROM feedback