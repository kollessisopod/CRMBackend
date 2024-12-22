using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMBackend.Entities;

[Table("employee")]
public class Employee
{
    [Key]
    [Column("employee_id")]
    public int Id { get; set; }
    [Column("e_name")]
    public string Username { get; set; }
    [Column("e_password")]
    public string Password { get; set; }
    [Column("e_email")]
    public string Email { get; set; }
    [Column("e_type")]
    public bool UserType { get; set; }
}
