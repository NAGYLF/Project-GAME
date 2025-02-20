-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Feb 20. 08:36
-- Kiszolgáló verziója: 10.4.20-MariaDB
-- PHP verzió: 7.3.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `ephemeral_courage`
--
CREATE DATABASE IF NOT EXISTS `ephemeral_courage` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `ephemeral_courage`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `achievement`
--

CREATE TABLE `achievement` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `FirstBlood` tinyint(1) DEFAULT 0,
  `RookieWork` tinyint(1) DEFAULT 0,
  `YouAreOnYourOwnNow` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `achievement`
--

INSERT INTO `achievement` (`Id`, `PlayerId`, `FirstBlood`, `RookieWork`, `YouAreOnYourOwnNow`) VALUES
(1, 1, 1, 0, 0),
(2, 2, 1, 1, 0),
(3, 3, 0, 1, 1);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `admin`
--

CREATE TABLE `admin` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `DevConsole` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `admin`
--

INSERT INTO `admin` (`Id`, `PlayerId`, `DevConsole`) VALUES
(1, 1, 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `player`
--

CREATE TABLE `player` (
  `Id` int(11) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `IsAdmin` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `player`
--

INSERT INTO `player` (`Id`, `Name`, `Password`, `Email`, `IsAdmin`) VALUES
(1, 'AdminUser', 'adminpass', 'admin@example.com', 1),
(2, 'RegularUser', 'userpass', 'user@example.com', 0),
(3, 'ZSAAAAA', 'mewmeow', '@supernut@gmail.com', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `statistic`
--

CREATE TABLE `statistic` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `DeathCount` int(11) DEFAULT 0,
  `Score` int(11) DEFAULT 0,
  `EnemiesKilled` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `statistic`
--

INSERT INTO `statistic` (`Id`, `PlayerId`, `DeathCount`, `Score`, `EnemiesKilled`) VALUES
(1, 1, 5, 1200, 20),
(2, 2, 2, 800, 10),
(3, 3, 7, 1500, 30);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `achievement`
--
ALTER TABLE `achievement`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PlayerId` (`PlayerId`);

--
-- A tábla indexei `admin`
--
ALTER TABLE `admin`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `PlayerId` (`PlayerId`);

--
-- A tábla indexei `player`
--
ALTER TABLE `player`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- A tábla indexei `statistic`
--
ALTER TABLE `statistic`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PlayerId` (`PlayerId`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `achievement`
--
ALTER TABLE `achievement`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `admin`
--
ALTER TABLE `admin`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT a táblához `player`
--
ALTER TABLE `player`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `statistic`
--
ALTER TABLE `statistic`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `achievement`
--
ALTER TABLE `achievement`
  ADD CONSTRAINT `achievement_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `player` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `admin`
--
ALTER TABLE `admin`
  ADD CONSTRAINT `admin_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `player` (`Id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `statistic`
--
ALTER TABLE `statistic`
  ADD CONSTRAINT `statistic_ibfk_1` FOREIGN KEY (`PlayerId`) REFERENCES `player` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
