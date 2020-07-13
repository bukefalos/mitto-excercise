CREATE USER 'mitto'@'%' IDENTIFIED BY 'mitto';
GRANT ALL ON mitto.* TO 'mitto'@'%';

CREATE TABLE `country` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `mcc` varchar(45) NOT NULL,
  `cc` varchar(45) NOT NULL,
  `price_per_sms` decimal(3,2) NOT NULL,
  PRIMARY KEY (`id`)
);
INSERT INTO country (name, mcc, cc, price_per_sms) VALUES ('Germany', '262', '49', 0.55);
INSERT INTO country (name, mcc, cc, price_per_sms) VALUES ('Austria', '232', '43', 0.53);
INSERT INTO country (name, mcc, cc, price_per_sms) VALUES ('Austria', '260', '48', 0.32);

CREATE TABLE `sms` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `from` varchar(45) NOT NULL,
  `to` varchar(45) NOT NULL,
  `text` varchar(45) NOT NULL,
  `created_at` datetime NOT NULL,
  `state` varchar(10) DEFAULT NULL,
  `price` decimal(3,2) NOT NULL,
  `country_id` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `country_id_idx` (`country_id`),
  CONSTRAINT `FK_COUNTRY` FOREIGN KEY (`country_id`) REFERENCES `country` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
);