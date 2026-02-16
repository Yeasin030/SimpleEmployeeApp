using System.Collections.ObjectModel;
using System.Windows.Input;
using SimpleEmployeeApp.Models;
using SimpleEmployeeApp.Services;

namespace SimpleEmployeeApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly EmployeeService _employeeService;
        private ObservableCollection<Employee> _employees;
        private Employee _selectedEmployee;
        private string _searchText;

        public MainViewModel()
        {
            _employeeService = new EmployeeService();
            _employees = _employeeService.GetAllEmployees();

            AddCommand = new Command(OnAdd);
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(OnDelete);
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
        }

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
                if (value != null)
                {
                    IsEditing = true;
                    CurrentEmployee = new Employee
                    {
                        Id = value.Id,
                        FirstName = value.FirstName,
                        LastName = value.LastName,
                        Email = value.Email,
                        Department = value.Department,
                        Position = value.Position,
                        Salary = value.Salary,
                        IsActive = value.IsActive
                    };
                }
            }
        }

        private Employee _currentEmployee;
        public Employee CurrentEmployee
        {
            get => _currentEmployee ??= new Employee();
            set
            {
                _currentEmployee = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterEmployees();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void FilterEmployees()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Employees = new ObservableCollection<Employee>(_employeeService.GetAllEmployees());
            }
            else
            {
                var filtered = _employeeService.GetAllEmployees()
                    .Where(e => e.FullName.ToLower().Contains(SearchText.ToLower()) ||
                               e.Department.ToLower().Contains(SearchText.ToLower()))
                    .ToList();
                Employees = new ObservableCollection<Employee>(filtered);
            }
        }

        private void OnAdd()
        {
            CurrentEmployee = new Employee { IsActive = true };
            IsEditing = true;
        }

        private void OnEdit()
        {
            // SelectedEmployee already sets up CurrentEmployee
        }

        private async void OnDelete()
        {
            if (SelectedEmployee != null)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Delete",
                    $"Delete {SelectedEmployee.FullName}?",
                    "Yes", "No");

                if (answer)
                {
                    await _employeeService.DeleteEmployee(SelectedEmployee.Id);
                    Employees.Remove(SelectedEmployee);
                    SelectedEmployee = null;
                    IsEditing = false;
                }
            }
        }

        private async void OnSave()
        {
            if (string.IsNullOrWhiteSpace(CurrentEmployee.FirstName) ||
                string.IsNullOrWhiteSpace(CurrentEmployee.LastName))
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "First Name and Last Name are required", "OK");
                return;
            }

            if (CurrentEmployee.Id == 0)
            {
                await _employeeService.AddEmployee(CurrentEmployee);
                Employees.Add(CurrentEmployee);
            }
            else
            {
                await _employeeService.UpdateEmployee(CurrentEmployee);
                var index = Employees.IndexOf(Employees.First(e => e.Id == CurrentEmployee.Id));
                Employees[index] = CurrentEmployee;
            }

            SelectedEmployee = null;
            IsEditing = false;
            CurrentEmployee = new Employee();
        }

        private void OnCancel()
        {
            SelectedEmployee = null;
            IsEditing = false;
            CurrentEmployee = new Employee();
        }
    }
}