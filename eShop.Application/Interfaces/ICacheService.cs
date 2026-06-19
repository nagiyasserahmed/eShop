using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ReserveStockAsync(string key, int quantity);
        Task IncrementStockAsync(string key, int quantity);
    }
}
