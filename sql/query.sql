
---- VERIONS 1.0 ----
-- UPDATED ON: ?
-- IMPLEMENT ALL QUERY
---------------------

---- VERSION 2.0 ---- 
-- UPDATED ON: 18/12/2021
-- QUERY FOR ALL CLASSES INSTEAD JUST QUERY THE CLASSES HAS BEEN USED, QUERY ONLY USED REGULATION/STUDENT/...
-- GetDcpRanking
---------------------

---- VERSION 3.0 ---- 
-- UPDATED ON: 03/01/2022
-- ONLY APPROVED REPORT IS COUNTED
-- ADD OVERALL RANKING QUERY
-- GetDcpRanking
---------------------

-- Get DCP RANKING
SELECT RANK() OVER (ORDER BY R.TotalPoints DESC, R.Faults ASC) Ranking,
R.* FROM
(SELECT 
F.Id ClassId,
COALESCE(X.Faults, 0) Faults,
COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
COALESCE(X.TotalPoints, 100) TotalPoints,
F.Name ClassName, 
J.Name FormTeacherName 
FROM  (
SELECT A.Id ClassId,
COUNT(D.Id) Faults,
COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints,
100 - COALESCE(SUM(B.PenaltyTotal), 0) AS TotalPoints
FROM [AppClass] A
LEFT JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
LEFT JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
LEFT JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
LEFT JOIN [AppRegulation] E ON D.RegulationId = E.Id
WHERE (C.Status = 'Approved') AND ((DATEDIFF(DAY, '06/1/2021', C.CreationTime) >= 0 
AND DATEDIFF(DAY, C.CreationTime, '06/25/2021') >= 0))
GROUP BY A.ID
) X RIGHT JOIN [AppClass] F ON F.ID = X.ClassId
JOIN [AppTeacher] J ON J.Id =  F.FormTeacherId) R

SELECT 
F.Id ClassId, 
COALESCE(X.LRPoint, 0) LRPoint,
COALESCE(X.TotalAbsence, 0) TotalAbsence
FROM (
SELECT 
LR.ClassId ClassId,
SUM(LR.TotalPoint) LRPoint,
SUM(LR.AbsenceNo) TotalAbsence
FROM [AppLessonsRegister] LR
WHERE (LR.Status = 'Approved') AND ((DATEDIFF(DAY, '06/1/2021', LR.CreationTime) >= 0 
AND DATEDIFF(DAY, LR.CreationTime, '1/1/2022') >= 0))
GROUP BY LR.ClassId
) X RIGHT JOIN [AppClass] F ON F.Id = X.ClassId

-- Overall ranking original version
SELECT RANK() OVER (
	ORDER BY R.RankingPoints DESC, 
	R.LrPoints DESC, 
	R.TotalAbsence ASC, 
	R.Faults ASC) Ranking,
	R.*
FROM (
SELECT
CL.Id,
CL.Name ClassName,
TC.Name FormTeacherName,
LR.TotalAbsence,
DCP.Faults,
DCP.PenaltyPoints,
LR.LRPoints,
DCP.DcpPoints,
CONVERT(int, ROUND((LR.LrPoints * 2 + DCP.DcpPoints) / 3, 0)) RankingPoints
FROM
(
SELECT 
F.Id ClassId,
COALESCE(X.Faults, 0) Faults,
COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
COALESCE(X.TotalPoints, 100) DcpPoints
FROM  (
SELECT A.Id ClassId,
COUNT(D.Id) Faults,
COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints,
100 - COALESCE(SUM(B.PenaltyTotal), 0) AS TotalPoints
FROM [AppClass] A
LEFT JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
LEFT JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
LEFT JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
LEFT JOIN [AppRegulation] E ON D.RegulationId = E.Id
WHERE (C.Status = 'Approved') AND ((DATEDIFF(DAY, '06/1/2021', C.CreationTime) >= 0 
AND DATEDIFF(DAY, C.CreationTime, '06/25/2021') >= 0))
GROUP BY A.ID
) X RIGHT JOIN [AppClass] F ON F.Id = X.ClassId
) DCP 

JOIN (
SELECT 
B.Id ClassId, 
COALESCE(X.LRPoints, 0) LRPoints,
COALESCE(X.TotalAbsence, 0) TotalAbsence
FROM (
SELECT 
A.ClassId ClassId,
SUM(A.TotalPoint) LRPoints,
SUM(A.AbsenceNo) TotalAbsence
FROM [AppLessonsRegister] A
WHERE (A.Status = 'Approved') AND ((DATEDIFF(DAY, '06/1/2021', A.CreationTime) >= 0 
AND DATEDIFF(DAY, A.CreationTime, '1/1/2022') >= 0))
GROUP BY A.ClassId
) X RIGHT JOIN [AppClass] B ON B.Id = X.ClassId
) LR ON DCP.ClassId = LR.ClassId 
JOIN [AppClass] CL ON CL.Id = DCP.ClassId 
JOIN [AppTeacher] TC ON TC.Id =  CL.FormTeacherId
) R

-- Overall ranking original with CTE
WITH LR AS
(
	SELECT 
	B.Id ClassId, 
	COALESCE(X.LrPoints, 0) LRPoints,
	COALESCE(X.TotalAbsence, 0) TotalAbsence
	FROM (
	SELECT 
	A.ClassId ClassId,
	SUM(A.TotalPoint) LRPoints,
	SUM(A.AbsenceNo) TotalAbsence
	FROM [AppLessonsRegister] A
	WHERE (A.Status = 'Approved') AND ((DATEDIFF(DAY, '06/1/2021', A.CreationTime) >= 0 
	AND DATEDIFF(DAY, A.CreationTime, '1/1/2022') >= 0))
	GROUP BY A.ClassId
	) X RIGHT JOIN [AppClass] B ON B.Id = X.ClassId
)
SELECT RANK() OVER (
	ORDER BY R.RankingPoints DESC, 
	R.LrPoints DESC, 
	R.TotalAbsence ASC, 
	R.Faults ASC) Ranking,
	R.*
FROM (
	SELECT
	CL.Id ClassId,
	CL.Name ClassName,
	TC.Name FormTeacherName,
	LR.TotalAbsence,
	DCP.Faults,
	DCP.PenaltyPoints,
	LR.LrPoints,
	DCP.DcpPoints,
	CONVERT(int, ROUND((LR.LRPoints * 2 + DCP.DcpPoints) / 3, 0)) RankingPoints
	FROM
	(
	SELECT 
		F.Id ClassId,
		COALESCE(X.Faults, 0) Faults,
		COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
		COALESCE(X.TotalPoints, 100) DcpPoints
	FROM  (
		SELECT A.Id ClassId,
		COUNT(D.Id) Faults,
		COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints,
		100 - COALESCE(SUM(B.PenaltyTotal), 0) AS TotalPoints
		FROM [AppClass] A
		LEFT JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
		LEFT JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
		LEFT JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
		LEFT JOIN [AppRegulation] E ON D.RegulationId = E.Id
		WHERE (C.Status = 'Approved') AND ((DATEDIFF(DAY, '06/1/2021', C.CreationTime) >= 0 
		AND DATEDIFF(DAY, C.CreationTime, '06/25/2021') >= 0))
		GROUP BY A.ID
	) X RIGHT JOIN [AppClass] F ON F.Id = X.ClassId
	) DCP 
	JOIN LR ON DCP.ClassId = LR.ClassId 
	JOIN [AppClass] CL ON CL.Id = DCP.ClassId 
	JOIN [AppTeacher] TC ON TC.Id =  CL.FormTeacherId
) R

-- GetClassesFaults
SELECT 
F.Id ClassId,
COALESCE(X.Faults, 0) Faults,
COALESCE(X.PenaltyPoints, 0) PenaltyPoints,
F.[Name] ClassName,
J.Name FormTeacherName 
FROM (
SELECT A.Id ClassId,
COUNT(D.Id) Faults,
COALESCE(SUM(B.PenaltyTotal), 0) PenaltyPoints
FROM [AppClass] A
LEFT JOIN [AppDcpClassReport] B ON A.Id = B.ClassId
LEFT JOIN [AppDcpReport] C ON B.DcpReportId = C.Id
LEFT JOIN [AppDcpClassReportItem] D ON B.Id = D.DcpClassReportId
LEFT JOIN [AppRegulation] E ON D.RegulationId = E.Id
WHERE (C.Status = 'Approved') AND ((DATEDIFF(DAY, '06/1/2021', C.CreationTime) >= 0 
AND DATEDIFF(DAY, C.CreationTime, '06/25/2021') >= 0) OR D.Id IS NULL)
GROUP BY A.Id
) X RIGHT JOIN [AppClass] F ON F.ID = X.ClassId
JOIN [AppTeacher] J ON J.Id =  F.FormTeacherId
ORDER BY Faults DESC, PenaltyPoints DESC, ClassName ASC


-- GetCommonFaults
SELECT 
Y.Id, 
Y.DisplayName Name, 
Z.DisplayName CriteriaName, 
X.Faults
FROM (
SELECT A.Id, COUNT(B.Id) Faults FROM [AppRegulation] A
LEFT JOIN [AppDcpClassReportItem] B ON A.Id = B.RegulationId
LEFT JOIN [AppDcpClassReport] C ON B.DcpClassReportId = C.Id
LEFT JOIN [AppDcpReport] D ON C.DcpReportId = D.Id
LEFT JOIN [AppClass] E ON C.ClassId = E.Id
WHERE (D.Status = 'Approved') AND ((DATEDIFF(DAY, '06/01/2021', D.CreationTime) >= 0 
AND DATEDIFF(DAY, D.CreationTime, '06/25/2021') >= 0) OR B.Id IS NULL)
GROUP BY A.Id
) X JOIN [AppRegulation] Y ON X.Id = Y.Id
JOIN [AppCriteria] Z ON Y.CriteriaId = Z.Id
WHERE Faults > 0
ORDER BY X.Faults DESC, Z.Name ASC

-- GetStudentsWithMostFaults
SELECT
Y.Id Id,
COALESCE(X.Faults, 0) Faults,
Y.Name StudentName, 
Z.Name ClassName FROM
(
SELECT A.ID, COUNT(C.Id) Faults 
FROM [AppStudent] A
LEFT JOIN [AppDcpStudentReport] B ON A.Id = B.StudentId
LEFT JOIN [AppDcpClassReportItem] C ON B.DcpClassReportItemId = C.Id
LEFT JOIN [AppDcpClassReport] D ON C.DcpClassReportId = D.Id
LEFT JOIN [AppDcpReport] E ON D.DcpReportId = E.Id
WHERE (E.Status = 'Approved') AND ((DATEDIFF(DAY, '06/01/2021', E.CreationTime) >= 0 
AND DATEDIFF(DAY, E.CreationTime, '06/25/2021') >= 0) OR B.Id IS NULL)
GROUP BY A.Id
) X RIGHT JOIN [AppStudent] Y ON X.Id = Y.Id
JOIN [AppClass] Z ON Y.ClassId = Z.Id
WHERE Faults > 0
ORDER BY Faults DESC, StudentName ASC