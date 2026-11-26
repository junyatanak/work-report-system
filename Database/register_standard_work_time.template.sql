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


-- Step 1: add master rows that do not exist yet
INSERT INTO WorkClasses (Name)
SELECT DISTINCT s.WorkClassName
FROM #Src s
WHERE NOT EXISTS (SELECT 1 FROM WorkClasses m WHERE m.Name = s.WorkClassName);

INSERT INTO Processes (Name)
SELECT DISTINCT s.ProcessName
FROM #Src s
WHERE NOT EXISTS (SELECT 1 FROM Processes m WHERE m.Name = s.ProcessName);

INSERT INTO WorkPatterns (Name)
SELECT DISTINCT s.WorkPatternName
FROM #Src s
WHERE NOT EXISTS (SELECT 1 FROM WorkPatterns m WHERE m.Name = s.WorkPatternName);

-- Step 2: convert names to Ids and insert StandardWorkTimes (skip existing combinations)
INSERT INTO StandardWorkTimes (WorkClassId, ProcessId, WorkPatternId, StandardCycleSeconds)
SELECT wc.Id, p.Id, wp.Id, s.StandardCycleSeconds
FROM #Src s
JOIN WorkClasses wc ON wc.Name = s.WorkClassName
JOIN Processes p ON p.Name = s.ProcessName
JOIN WorkPatterns wp ON wp.Name = s.WorkPatternName
WHERE NOT EXISTS (
    SELECT 1
    FROM StandardWorkTimes t
    WHERE t.WorkClassId = wc.Id
      AND t.ProcessId = p.Id
      AND t.WorkPatternId = wp.Id
)
