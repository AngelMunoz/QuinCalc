using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuinCalc.Core.Services
{
  public interface IBasicService<T>
  {

    (int, IEnumerable<T>) Find(int skip, int limit);
    Task<bool> CreateAsync(T item);
    Task<bool> UpdateAsync(T item);
    Task<bool> DestroyAsync(T item);
  }
}
