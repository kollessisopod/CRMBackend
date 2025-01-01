using CRMBackend.Entities;

namespace CRMBackend.Services;

public class EmployeeServices
{
    private readonly AppDbContext _context;
    public EmployeeServices(AppDbContext context)
    {
        _context = context;
    }
    public List<Employee> GetEmployees()
    {
        return _context.Employees.ToList();
    }
    public Employee? GetEmployeeByUsername(string username)
    {
        return _context.Employees.FirstOrDefault(e => e.Username == username);
    }
    public Employee? GetEmployeeById(int id)
    {
        return _context.Employees.FirstOrDefault(e => e.Id == id);
    }
    public Employee CreateEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        _context.SaveChanges();
        return employee;
    }

    public void DeleteEmployee(string username)
    {
        var employee = _context.Employees.FirstOrDefault(e => e.Username == username);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
        }
    }
}
