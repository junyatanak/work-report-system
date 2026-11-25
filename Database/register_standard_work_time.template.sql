-- ============================================================
-- Register StandardWorkTimes (and any missing master rows) from Excel data
--
-- Usage:
--   1. Generate VALUES rows in Excel (see the formula below).
--   2. Paste them into the INSERT INTO #Src section, replacing the sample row.
--   3. Run with ROLLBACK first and check the result sets.
--   4. If everything looks right, change ROLLBACK to COMMIT and run again.
--
-- Excel formula (A=WorkClass, B=Process, C=WorkPattern, D=StandardCycleSeconds):
--   ="(N'"&SUBSTITUTE(A2,"'","''")&"',N'"&SUBSTITUTE(B2,"'","''")&"',N'"&SUBSTITUTE(C2,"'","''")&"',"&IF(D2="","NULL",D2)&"),"
-- ============================================================

USE DailyWorkReport;
SET XACT_ABORT ON;   -- roll back the whole transaction automatically if an error occurs

BEGIN TRAN;

-- Staging table (exists only for this session)
CREATE TABLE #Src (
    WorkClassName NVARCHAR(100) NOT NULL,
    ProcessName NVARCHAR(100) NOT NULL,
    WorkPatternName NVARCHAR(100) NOT NULL,
    StandardCycleSeconds DECIMAL(9,2) NULL
);

INSERT INTO #Src (WorkClassName, ProcessName, WorkPatternName, StandardCycleSeconds) VALUES
(N'SAMPLECLASS', N'SAMPLEPROCESS', N'SAMPLEPATTERN', 10.0)
-- Paste the VALUES rows generated from Excel here (replace the sample row above).
-- Do not put a comma after the last row.
;

-- Normalize: trim both ends and convert to upper case (names are stored in upper case)
UPDATE #Src SET
    WorkClassName = UPPER(LTRIM(RTRIM(WorkClassName))),
    ProcessName = UPPER(LTRIM(RTRIM(ProcessName))),
    WorkPatternName = UPPER(LTRIM(RTRIM(WorkPatternName)));

-- Check 1: duplicate combinations in the Excel data (must return no rows)
SELECT WorkClassName, ProcessName, WorkPatternName, COUNT(*) AS Cnt
FROM #Src
GROUP BY WorkClassName, ProcessName, WorkPatternName
HAVING COUNT(*) > 1;