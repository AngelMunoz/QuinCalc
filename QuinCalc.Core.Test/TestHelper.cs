using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuinCalcData.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace QuinCalc.Core.Test
{
  [TestClass]
  public class TestHelper
  {
    private static readonly TestHelper _instance = new TestHelper();
    private readonly QuincalcContext _context;
    private readonly string _dir;
    public string DbString { get; private set; }

    private TestHelper()
    {
      _dir = Path.GetTempPath();
      DbString = $"Data Source={_dir}\\CoreTest.db";
      _context = new QuincalcContext(DbString);
    }

    public static TestHelper GetInstance()
    {
      return _instance;
    }

    [AssemblyInitialize]
    public static void SeedDatabase(TestContext tc)
    {
      var helpers = GetInstance();
      helpers.CreateDatabase();
    }

    [AssemblyCleanup]
    public static void CleanDatabase()
    {
      var helpers = GetInstance();
      helpers.DeleteDatabase();
    }

    public void CreateDatabase()
    {
      _context.Database.Migrate();
      try
      {
        AddTodos();
      }
      catch (Exception e)
      {
        Debug.WriteLine(e.Message, "[TestHelper]: CreateDatabase");
      }
      _context.Dispose();
    }

    public void DeleteDatabase()
    {
      File.Delete($"{_dir}\\CoreTest.db");
    }

    public void AddTodos()
    {
      var todo0 = new Todo { Id = 1, Name = "Todo0", Description = "Description0", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddMinutes(5) };
      var todo1 = new Todo { Id = 2, Name = "Todo1", Description = "Description1", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddMinutes(6) };
      var todo2 = new Todo { Id = 3, Name = "Todo2", Description = "Description2", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddMinutes(7) };
      var todo3 = new Todo { Id = 4, Name = "Todo3", Description = "Description3", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddDays(1) };
      var todo4 = new Todo { Id = 5, Name = "Todo4", Description = "Description4", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddDays(1) };
      var todo5 = new Todo { Id = 6, Name = "Todo5", Description = "Description5", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddDays(1) };
      var todo6 = new Todo { Id = 7, Name = "Todo6", Description = "Description6", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddDays(1) };
      var todo7 = new Todo { Id = 8, Name = "Todo7", Description = "Description7", IsDone = true, DueDate = DateTimeOffset.UtcNow.AddDays(15) };
      var todo8 = new Todo { Id = 9, Name = "Todo8", Description = "Description8", IsDone = true, DueDate = DateTimeOffset.UtcNow.AddDays(15) };
      var todo9 = new Todo { Id = 10, Name = "Todo9", Description = "Description9", IsDone = true, DueDate = DateTimeOffset.UtcNow.AddDays(15) };
      _context.Todos.AddRange(new Todo[] { todo0, todo1, todo2, todo3, todo4, todo5, todo6, todo7, todo8, todo9, });
      _context.SaveChanges();
    }
  }
}
