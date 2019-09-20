CREATE TRIGGER [dbo].[Employees_UPDATE] ON [dbo].[Employees]
	AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
                  
	IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;
                  
	IF UPDATE(Salary) AND NOT Update(LastPayRaise)
	BEGIN
		DECLARE @Id INT
		DECLARE @OldSalary INT
		DECLARE @NewSalary INT
          
		SELECT @Id = INSERTED.EmployeeId, @NewSalary = Salary        
		FROM INSERTED
          
		SELECT @OldSalary = Salary        
		FROM deleted
          
		IF @NewSalary > @OldSalary
		BEGIN
			UPDATE dbo.Employees
			SET LastPayRaise = CONVERT(date, GETDATE())
			WHERE EmployeeId = @Id
		END
	END
END