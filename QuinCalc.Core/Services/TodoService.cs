using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuinCalcData.Models;

namespace QuinCalc.Core.Services
{
  public class TodoService : IBasicService<Todo>, IDisposable
  {
    private readonly QuincalcContext _context;

    public TodoService(QuincalcContext context = null, string dbstring = null)
    {
      _context = context ?? new QuincalcContext(dbstring);
    }

    public Task<bool> CreateAsync(Todo item)
    {
      try
      {
        _context.Todos.Add(item);
        return _context.SaveChangesAsync().ContinueWith(_ => true);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Create");
        return Task.FromResult(false);
      }
    }

    public Task<bool> DestroyAsync(Todo item)
    {
      try
      {
        _context.Todos.Remove(item);
        return _context.SaveChangesAsync().ContinueWith(_ => true);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Destroy");
        return Task.FromResult(false);
      }
    }

    public (int, IEnumerable<Todo>) Find(int skip, int limit)
    {
      var count = _context.Todos.Count();
      var todos = _context.Todos
        .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
        .Skip(skip)
        .Take(limit)
        .AsEnumerable();
      return (count, todos);
    }

    public (int, IEnumerable<Todo>) FindByIsDone(bool isDone, int skip, int limit)
    {
      var count = _context.Todos.Where(t => t.IsDone == isDone).Count();
      var todos = _context.Todos
        .Where(t => t.IsDone == isDone)
        .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
        .Skip(skip)
        .Take(limit)
        .AsEnumerable();
      return (count, todos);
    }

    public Task<bool> UpdateAsync(Todo item)
    {
      try
      {
        _context.Todos.Update(item);
        return _context.SaveChangesAsync().ContinueWith(_ => true);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Update");
        return Task.FromResult(false);
      }
    }

    public Task<Todo> FindClosestAsync()
    {
      var todo = _context.Todos
       .Where(t => !t.IsDone)
       .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
       .FirstOrDefaultAsync();
      return todo;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Managed Resources Dosposal
        _context.Dispose();
      }
    }
  }
}
