﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuinCalc.Models;

namespace QuinCalc.Services
{
  public class ExpenseService : IBasicService<Expense>
  {
    private QuincalcContext _context;

    public ExpenseService(QuincalcContext context = null)
    {
      _context = context ?? new QuincalcContext();
    }

    public async Task<bool> Create(Expense item)
    {
      try
      {
        await _context.Expenses.AddAsync(item);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Create");
        return false;
      }
    }

    public async Task<bool> Destroy(Expense item)
    {
      try
      {
        _context.Expenses.Remove(item);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Destroy");
        return false;
      }
    }

    public async Task<(int, List<Expense>)> Find(int skip, int limit)
    {
      var count = _context.Expenses.Count();
      var expenses = await _context.Expenses
                .OrderBy(e => e.DueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
      return (count, expenses);

    }

    public async Task<bool> Update(Expense item)
    {
      try
      {
        await _context.Expenses.AddAsync(item);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.StackTrace, "Service:Error:Update");
        return false;
      }
    }
  }
}