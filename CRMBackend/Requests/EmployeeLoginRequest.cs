namespace CRMBackend.Requests;

public class EmployeeLoginRequest
{
    public required int Id { get; set; }
    public required string Password { get; set; }
}
