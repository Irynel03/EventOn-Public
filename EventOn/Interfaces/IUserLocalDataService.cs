using EventOn.API.Models;

namespace EventOn.Interfaces;

public interface IUserLocalDataService
{
    void SaveOrders(List<Order> orders);
    List<Order> GetOrders();
    void ClearOrders();
}