BEGIN TRAN;

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