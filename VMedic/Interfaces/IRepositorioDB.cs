using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.Data;

namespace VMedic.Interfaces
{
    public interface IRepositorioDB<T> : IDisposable where T : TableData, new()
    {
        void InsertItem(T item);
        void UpdateITEM(T item);
        T GetItem(T item);
        List<T>? GetItems();
        void DeleteItem(T id);
    }
}
