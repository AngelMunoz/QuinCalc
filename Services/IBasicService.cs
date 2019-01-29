using System.Collections.Generic;
using System.Threading.Tasks;
using QuinCalc.Models;

namespace QuinCalc.Services
{
  public interface IBasicService<T>
  {
    
    Task<(int, List<T>)> Find(int skip, int limit);
    Task<bool> Create(T item);
    Task<bool> Update(T item);
    Task<bool> Destroy(T item);
  }
}
