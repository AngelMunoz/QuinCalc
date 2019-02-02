using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuinCalcData.Models;

namespace QuinCalc.Services
{
  public class TodoService : IBasicService<Todo>, IDisposable
  {
    private QuincalcContext _context;

    public TodoService(QuincalcContext context = null)
    {
      _context = context ?? new QuincalcContext();
    }

    public async Task<bool> Create(Todo item)
    {
      try
      {
        await _context.Todos.AddAsync(item);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Create");
        return false;
      }
    }

    public async Task<bool> Destroy(Todo item)
    {
      try
      {
        _context.Todos.Remove(item);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Destroy");
        return false;
      }
    }

    public async Task<(int, List<Todo>)> Find(int skip, int limit)
    {
      var count = _context.Todos.Count();
      var todos = await _context.Todos
                .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
      return (count, todos);

    }

    public async Task<bool> Update(Todo item)
    {
      try
      {
        _context.Todos.Update(item);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Update");
        return false;
      }
    }

    public async Task<Todo> FindClosest()
    {
      var todo = await _context.Todos
       .Where(t => !t.IsDone)
       .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
       .FirstOrDefaultAsync();
      return todo;
    }

    public void Dispose()
    {
      _context.Dispose();
    }
  }
}
