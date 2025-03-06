-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: db
-- Generation Time: Mar 05, 2025 at 06:43 PM
-- Server version: 8.4.2
-- PHP Version: 8.2.23

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `movingpapa`
--

-- --------------------------------------------------------

--
-- Table structure for table `Moves`
--

CREATE TABLE `Moves` (
  `ID` int NOT NULL,
  `Uuid` varchar(256) NOT NULL,
  `MoveDetails` json NOT NULL,
  `Email` varchar(256) NOT NULL,
  `PhoneNumber` varchar(256) NOT NULL,
  `PriceInCents` int NOT NULL,
  `MoveDate` datetime NOT NULL,
  `MoveTime` enum('Early Morning','Afternoon','Late Afternoon','Evening') NOT NULL,
  `TimeCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `QuoteId` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `QuotesAndContact`
--

CREATE TABLE `QuotesAndContact` (
  `ID` int NOT NULL,
  `TimeCreated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FullName` varchar(256) NOT NULL,
  `Email` varchar(256) NOT NULL,
  `PhoneNumber` varchar(256) NOT NULL,
  `IsMuscleOnly` tinyint(1) NOT NULL,
  `Addresses` json NOT NULL,
  `Uuid` varchar(256) NOT NULL,
  `MoveInfo` json DEFAULT NULL,
  `Packages` json DEFAULT NULL,
  `TimeUpdated` datetime DEFAULT NULL,
  `IsCallNow` tinyint(1) NOT NULL,
  `MoveDate` datetime DEFAULT NULL,
  `MoveTime` enum('Early Morning','Afternoon','Late Afternoon','Evening') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `RateCalendar`
--

CREATE TABLE `RateCalendar` (
  `Date` datetime NOT NULL,
  `RatePerMoverInCents` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `RateCalendar`
--

INSERT INTO `RateCalendar` (`Date`, `RatePerMoverInCents`) VALUES
('2024-09-09 00:00:00', 6000),
('2024-09-10 00:00:00', 7500),
('2024-09-11 00:00:00', 6000),
('2024-09-12 00:00:00', 6000),
('2024-09-13 00:00:00', 6000),
('2024-09-14 00:00:00', 6000),
('2024-09-15 00:00:00', 6000),
('2024-09-16 00:00:00', 6000),
('2024-09-17 00:00:00', 6000),
('2024-09-18 00:00:00', 6000),
('2024-09-19 00:00:00', 6000),
('2024-09-20 00:00:00', 6000),
('2024-09-21 00:00:00', 6000),
('2024-09-22 00:00:00', 6000),
('2024-09-23 00:00:00', 6000),
('2024-09-24 00:00:00', 6000),
('2024-09-25 00:00:00', 6000),
('2024-09-26 00:00:00', 6000),
('2024-09-27 00:00:00', 8000),
('2024-09-28 00:00:00', 8000),
('2024-09-29 00:00:00', 8000),
('2024-09-30 00:00:00', 8000),
('2024-10-01 00:00:00', 8000),
('2024-10-02 00:00:00', 8000),
('2024-10-03 00:00:00', 8000),
('2024-10-04 00:00:00', 6000),
('2024-10-05 00:00:00', 6000),
('2024-10-06 00:00:00', 6000),
('2024-10-07 00:00:00', 6000),
('2024-10-08 00:00:00', 6000),
('2024-10-09 00:00:00', 6000),
('2024-10-10 00:00:00', 6000),
('2025-02-05 09:45:29', 12900);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Moves`
--
ALTER TABLE `Moves`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `QuoteId` (`QuoteId`);

--
-- Indexes for table `QuotesAndContact`
--
ALTER TABLE `QuotesAndContact`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `RateCalendar`
--
ALTER TABLE `RateCalendar`
  ADD PRIMARY KEY (`Date`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Moves`
--
ALTER TABLE `Moves`
  MODIFY `ID` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `QuotesAndContact`
--
ALTER TABLE `QuotesAndContact`
  MODIFY `ID` int NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `Moves`
--
ALTER TABLE `Moves`
  ADD CONSTRAINT `Moves_ibfk_1` FOREIGN KEY (`QuoteId`) REFERENCES `QuotesAndContact` (`ID`) ON DELETE RESTRICT ON UPDATE RESTRICT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
