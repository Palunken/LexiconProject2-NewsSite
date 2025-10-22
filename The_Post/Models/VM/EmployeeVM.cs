namespace The_Post.Models.VM
{
    public class EmployeeVM
    {
        public IEnumerable<AllEmployeesVM> Employees { get; set; }
        public AddEmployeeVM NewEmployee { get; set; }
    }
}
