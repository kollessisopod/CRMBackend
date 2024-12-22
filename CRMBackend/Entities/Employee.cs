using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMBackend.Entities;

[Table("Employees")]
public class Employee
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("username")]
    public string Username { get; set; }
    [Column("password")]
    public string Password { get; set; }
    [Column("email")]
    public string Email { get; set; }
    [Column("usertype")]
    public string UserType { get; set; }
}
