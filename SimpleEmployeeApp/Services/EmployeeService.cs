using System.Collections.ObjectModel;
using System.Text.Json;
using SimpleEmployeeApp.Models;

namespace SimpleEmployeeApp.Services
{
    public class EmployeeService
    {
        private ObservableCollection<Employee> _employees;
        private readonly string _filePath;

        public EmployeeService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "employees.json");
            LoadEmployees();
        }

        public ObservableCollection<Employee> GetAllEmployees() => _employees;

        private void LoadEmployees()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _employees = JsonSerializer.Deserialize<ObservableCollection<Employee>>(json);
                }
                else
                {
                    // Sample data
                    _employees = new ObservableCollection<Employee>
                    {
                        new Employee
                        {
                            Id = 1,
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john@company.com",
                            Department = "IT",
                            Position = "Developer",
                            Salary = 50000
                        },
                        new Employee
                        {
                            Id = 2,
                            FirstName = "Jane",
                            LastName = "Smith",
                            Email = "jane@company.com",
                            Department = "HR",
                            Position = "Manager",
                            Salary = 60000
                        }
                    };
                    SaveEmployees();
                }
            }
            catch
            {
                _employees = new ObservableCollection<Employee>();
            }
        }

        public async Task SaveEmployees()
        {
            var json = JsonSerializer.Serialize(_employees);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task AddEmployee(Employee employee)
        {
            employee.Id = _employees.Count > 0 ? _employees.Max(e => e.Id) + 1 : 1;
            _employees.Add(employee);
            await SaveEmployees();
        }

        public async Task UpdateEmployee(Employee employee)
        {
            var index = _employees.IndexOf(_employees.First(e => e.Id == employee.Id));
            _employees[index] = employee;
            await SaveEmployees();
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = _employees.First(e => e.Id == id);
            _employees.Remove(employee);
            await SaveEmployees();
        }
    }
}