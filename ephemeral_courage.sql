-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Ápr 14. 18:02
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `achievement`
--

INSERT INTO `achievement` (`Id`, `PlayerId`, `FirstBlood`, `RookieWork`, `YouAreOnYourOwnNow`) VALUES
(12, 14, 0, 0, 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `admin`
--

CREATE TABLE `admin` (
  `Id` int(11) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `DevConsole` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `admin`
--

INSERT INTO `admin` (`Id`, `PlayerId`, `DevConsole`) VALUES
(14, 14, 1),
(15, 15, 1),
(16, 16, 0),
(21, 21, 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `player`
--

CREATE TABLE `player` (
  `Id` int(11) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `IsAdmin` tinyint(1) NOT NULL DEFAULT 0,
  `IsBanned` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `player`
--

INSERT INTO `player` (`Id`, `Name`, `Password`, `Email`, `IsAdmin`, `IsBanned`) VALUES
(14, 'Admin', '$2a$11$JV7zDIPuH0/EZQf2559ev.XFL.528Ic0K8xvW67DfYnfAgBx45f92', 'Admin@.Admin', 1, 0),
(15, 'TEST', '$2a$11$iULFl9CbVAuK/BeM5t16k.tYRgo8jUXr/dFrZ9WxGGJSY.VNMW97q', 'TEST@.TEST', 1, 0),
(16, 'TEST2', '$2a$11$YXoWMIou5yMgdk3INeeOO.amRcRqNGsTgJh.hwyELYfsLs7248.SW', 'TEST@.TEST2', 1, 0),
(17, 'TestPlayer', '$2a$11$03ES3mZHAng2j7mi2O2M9.6sMh1.kTdCF7AWyTTbEX/vUys3fzlFG', 'TestPlayer@gmail.com', 0, 0),
(21, 'TestAdmin', '$2a$11$cNtmeiKYotigIoNlZQfKAuwohG.FJO4cyQ9Ye80hyQZ.dnXbHC3Sm', 'TestAdmin@gmail.com', 1, 0),
(23, 'TestValami', '$2a$11$Q4C.oYumlO3qQudH5cmJ8uu0JDsWim4d/fsvXZxoYFssufOt/nN4O', 'valamiemail@gmail.com', 0, 0);

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `statistic`
--

INSERT INTO `statistic` (`Id`, `PlayerId`, `DeathCount`, `Score`, `EnemiesKilled`) VALUES
(12, 14, 0, 0, 0);

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
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT a táblához `admin`
--
ALTER TABLE `admin`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT a táblához `player`
--
ALTER TABLE `player`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT a táblához `statistic`
--
ALTER TABLE `statistic`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

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
