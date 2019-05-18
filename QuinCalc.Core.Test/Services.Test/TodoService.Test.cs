using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using QuinCalcData.Models;
using QuinCalc.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuinCalc.Core.Test.Services.Test
{
  [TestClass]
  public class TodoServiceTest
  {
    private string dir;
    private string dbstring;

    [TestInitialize]
    public async Task Startup()
    {
      dir = Path.GetTempPath();
      dbstring = $"Data Source={dir}\\TodoServiceTest.db";
      using (var context = new QuincalcContext(dbstring))
      {
        await context.Database.MigrateAsync();
      }
    }

    [TestCleanup]
    public void Cleanup()
    {
      File.Delete($"{dir}\\TodoServiceTest.db");
    }

    [TestMethod]
    public async Task Create_Todo_Test()
    {
      int count;
      int expetctedCount = 1;
      Todo todo;
      Todo expectedTodo = new Todo { Name = "Test Todo", Description = "This is a Test Todo", IsDone = false, DueDate = DateTimeOffset.Now.AddDays(1) };
      using (var service = new TodoService(dbstring: dbstring))
      {
        await service.CreateAsync(expectedTodo);
        var result = service.Find(0, 10);
        count = result.Item1;
        todo = result.Item2.FirstOrDefault();
      }
      using (var context = new QuincalcContext(dbstring))
      {
        count = context.Todos.Count();
        expectedTodo = context.Todos.FirstOrDefault();
      }
      Assert.AreEqual(expetctedCount, count);
      Assert.IsInstanceOfType(todo, typeof(Todo));
      Assert.AreEqual(expectedTodo.Name, todo.Name);
      Assert.AreEqual(expectedTodo.Description, todo.Description);
      Assert.AreEqual(expectedTodo.IsDone, todo.IsDone);
      Assert.AreEqual(expectedTodo.DueDate, todo.DueDate);
    }
  }
}
