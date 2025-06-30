using EventOn.API.Models;
using EventOn.Interfaces;
using Newtonsoft.Json;

namespace EventOn.BusinessLogic.Services;

public class UserLocalDataService : IUserLocalDataService
{
    private const string LocalOrdersKey = "local_orders";

    public List<Order> GetOrders()
    {
        try
        {
            var ordersJson = Preferences.Get(LocalOrdersKey, null);
            if (!string.IsNullOrEmpty(ordersJson))
                return JsonConvert.DeserializeObject<List<Order>>(ordersJson);
        }
        catch { }

        return [];
    }

    public void SaveOrders(List<Order> orders)
    {
        try
        {
            string ordersJson = JsonConvert.SerializeObject(orders);
            Preferences.Set(LocalOrdersKey, ordersJson);
        }
        catch { }
    }

    public void ClearOrders()
    {
        try
        {
            Preferences.Remove(LocalOrdersKey);
        }
        catch { }
    }
}