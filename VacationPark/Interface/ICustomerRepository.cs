using VacationPark.Models;

namespace VacationPark.Interface
{
    // Customer interface 
    public interface ICustomerRepository
    {
        //in interface we just define function and implementaion in class 
        IEnumerable<Customer> GetAllCustomers();

        void AddCustomer(Customer customer);

    }
}
