using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuinCalc.Core.Enums;
using QuinCalcData.Models;

namespace QuinCalc.Core.Services
{
  /// <summary>
  /// Service that allows you to manupulate Expense objects into the database
  /// this class implements the IDisposable Interface to allow for 
  /// </summary>
  public class ExpenseService : IBasicService<Expense>, IDisposable
  {
    private readonly QuincalcContext _context;

    public ExpenseService(QuincalcContext context = null)
    {
      _context = context ?? new QuincalcContext();
    }

    /// <summary>
    /// Saves into the database the expense item passed to it
    /// it must be a <see cref="Expense"/> object that is not present in the database
    /// </summary>
    /// <param name="item">The expense object you want to create</param>
    /// <returns>A task that contains a boolean value that signals if the operation was successful</returns>
    public Task<bool> CreateAsync(Expense item)
    {
      try
      {
        _context.Expenses.Add(item);
        return _context.SaveChangesAsync().ContinueWith(_ => true);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Create");
        return Task.FromResult(false);
      }
    }

    /// <summary>
    /// Takes an existing <see cref="Expense"/> item and removes it from the database
    /// </summary>
    /// <param name="item">Expense object to be removed from the database</param>
    /// <returns>A task that contains a boolean value that signals if the operation was successful</returns>
    public Task<bool> DestroyAsync(Expense item)
    {
      try
      {
        _context.Expenses.Remove(item);
        return _context.SaveChangesAsync().ContinueWith(_ => true);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Destroy");
        return Task.FromResult(false);
      }
    }

    /// <summary>
    /// Finds <see cref="Expense"/> objects in the database
    /// </summary>
    /// <param name="skip">number of items to skip</param>
    /// <param name="limit">number of items to bring</param>
    /// <returns>
    /// Returns a <see cref="Tuple{int, IEnumerable{Expense}}"/> Tuple
    /// </returns>
    public (int, IEnumerable<Expense>) Find(int skip, int limit)
    {
      var count = _context.Expenses.Count();
      var expenses = _context.Expenses
        .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
        .Skip(skip)
        .Take(limit)
        .AsEnumerable();
      return (count, expenses);

    }

    /// <summary>
    /// In the same way of <see cref="Find(int, int)"/>
    /// looks for <see cref="Expense"/> objects into the database
    /// taking into account <see cref="Expense.IsDone"/> as a parameter
    /// </summary>
    /// <param name="isDone">Type of the Expense Objects to bring</param>
    /// <param name="skip">number of items to skip</param>
    /// <param name="limit">number of items to bring</param>
    /// <returns>
    /// Returns a <see cref="Tuple{int, IEnumerable{Expense}}"/> Tuple
    /// </returns>
    public (int, IEnumerable<Expense>) FindByIsDone(bool isDone, int skip, int limit)
    {
      var count = _context.Expenses.Where(e => e.IsDone == isDone).Count();
      var expenses = _context.Expenses
        .Where(e => e.IsDone == isDone)
        .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
        .Skip(skip)
        .Take(limit)
        .AsEnumerable();
      return (count, expenses);
    }

    /// <summary>
    /// A <see cref="Expense"/> object to be updated in the database
    /// </summary>
    /// <param name="item">Item to be updated</param>
    /// <returns>A task that contains a boolean value that signals if the operation was successful</returns>
    public Task<bool> UpdateAsync(Expense item)
    {
      try
      {
        _context.Expenses.Update(item);
        return _context.SaveChangesAsync().ContinueWith(_ => true);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Update");
        return Task.FromResult(false);
      }
    }

    /// <summary>
    /// Tries to find in terms of due to time, the closest <see cref="Expense"/> object to the current time
    /// </summary>
    /// <returns><see cref="Expense"/>The closest expense due to today/now</returns>
    public Task<Expense> FindClosest()
    {
      var expense = _context.Expenses
        .Where(e => !e.IsDone)
        .OrderBy(e => Math.Abs((e.DueDate - DateTimeOffset.Now).Ticks))
        .FirstOrDefaultAsync();
      return expense;
    }

    /// <summary>
    /// Does a sumatory depending on the <see cref="Expense"/> type to get the total amount
    /// in <see cref="Expense.Amount"/> objects in the database
    /// </summary>
    /// <param name="expenseType">Type of the Expense to check and get the Total Amount</param>
    /// <returns></returns>
    public Task<decimal> GetTotalAmount(LoadExpenseType expenseType)
    {
      switch (expenseType)
      {
        case LoadExpenseType.Done:
          return _context.Expenses
          .Where(e =>
            (e.DueDate.Day >= DateTimeOffset.Now.Day) &&
            (e.DueDate.Day <= DateService.GetNextQuin(DateService.GetDayToCheck()).Day) &&
            (e.IsDone == true)
           )
          .SumAsync(e => e.Amount);
        case LoadExpenseType.NotDone:
          return _context.Expenses
            .Where(e =>
              (e.DueDate.Day >= DateTimeOffset.Now.Day) &&
              (e.DueDate.Day <= DateService.GetNextQuin(DateService.GetDayToCheck()).Day) &&
              (e.IsDone == false)
             )
            .SumAsync(e => e.Amount);
        default:
          return _context.Expenses
          .Where(e =>
            (e.DueDate.Day >= DateTimeOffset.Now.Day) &&
            (e.DueDate.Day <= DateService.GetNextQuin(DateService.GetDayToCheck()).Day)
           )
          .SumAsync(e => e.Amount);
      }
    }

    /// <summary>
    /// Get the Total Amount to pay on the NextQuin (day 15 or last day of the current month)
    /// </summary>
    /// <param name="dayToCheck"></param>
    /// <returns></returns>
    public Task<decimal> GetBiweekAmount(int dayToCheck)
    {
      return _context.Expenses
        .Where(e =>
          (e.DueDate.Day >= DateTimeOffset.Now.Day) &&
          (e.DueDate.Day <= DateService.GetNextQuin(dayToCheck).Day)
         )
        .SumAsync(e => e.Amount);
    }

    /// <summary>
    /// Get the Total Amount to pay on the rest of the month (last day of the current month)
    /// </summary>
    /// <returns></returns>
    public Task<decimal> GetMonthlyAmount()
    {
      return _context.Expenses
        .Where(e =>
          (e.DueDate.Day >= DateTimeOffset.Now.Day) &&
          (e.DueDate.Day <= DateTime.DaysInMonth(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month))
         )
        .SumAsync(e => e.Amount);
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
      // free native resources if there are any.
    }
  }
}
