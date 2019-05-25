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
    private readonly TestHelper _helper = TestHelper.GetInstance();

    [TestMethod]
    [Priority(1)]
    public async Task Create_Todo_Async_Test()
    {
      var todo10 = new Todo { Id = 11, Name = "Todo10", Description = "Created10", IsDone = true, DueDate = DateTimeOffset.UtcNow.AddDays(1) };
      using (var todos = new TodoService(dbstring: _helper.DbString))
      {
        var r1 = await todos.CreateAsync(todo10);
        Assert.IsTrue(r1);
      }
      using (var context = new QuincalcContext(_helper.DbString))
      {
        var exists = context.Todos.Any(t => t.Id == 11);
        Assert.IsTrue(exists);
      }
    }

    [TestMethod]
    [Priority(1)]
    public async Task Update_Todo_Async_Test()
    {
      var todo9 = new Todo { Id = 10, Name = "TodoUpdated9", Description = "Updated9", IsDone = true, DueDate = DateTimeOffset.UtcNow.AddDays(15) };
      using (var todos = new TodoService(dbstring: _helper.DbString))
      {
        var r1 = await todos.UpdateAsync(todo9);
        Assert.IsTrue(r1);
      }
      using (var context = new QuincalcContext(_helper.DbString))
      {
        var todo = context.Todos.First(t => t.Id == 10);
        Assert.AreEqual("Updated9", todo.Description);
        Assert.AreEqual("TodoUpdated9", todo.Name);
      }
    }

    [TestMethod]
    [Priority(1)]
    public async Task Destroy_Todo_Async_Test()
    {
      var todo6 = new Todo { Id = 7, Name = "Todo6", Description = "Description6", IsDone = false, DueDate = DateTimeOffset.UtcNow.AddDays(1) };
      using (var todos = new TodoService(dbstring: _helper.DbString))
      {
        var r1 = await todos.DestroyAsync(todo6);
        Assert.IsTrue(r1);
      }
      using (var context = new QuincalcContext(_helper.DbString))
      {
        var exists = context.Todos.Any(t => t.Id == 7);
        Assert.IsFalse(exists);
      }
    }

    [TestMethod]
    [Priority(2)]
    public async Task Find_Closest_Async_Todo__Test()
    {
      using (var todos = new TodoService(dbstring: _helper.DbString))
      {
        var todo = await todos.FindClosestAsync();
        Assert.AreEqual(1, todo.Id);
      }
    }

    [TestMethod]
    [Priority(2)]
    public void Find_Todo_Test()
    {
      using (var todos = new TodoService(dbstring: _helper.DbString))
      {
        var (count, unordered) = todos.Find(0, 3);
        var list = unordered.OrderBy(t => t.Id);
        Assert.AreEqual(10, count);
        Assert.AreEqual(1, list.ElementAt(0).Id);
        Assert.AreEqual(2, list.ElementAt(1).Id);
        Assert.AreEqual(3, list.ElementAt(2).Id);
      }
    }

    [TestMethod]
    [Priority(2)]
    public void Find_By_IsDone_Todo__Test()
    {
      using (var todos = new TodoService(dbstring: _helper.DbString))
      {
        var (count1, list1) = todos.FindByIsDone(true, 0, 5);
        Assert.AreEqual(4, count1);
        var ordered1 = list1.OrderBy(t => t.Id);
        Assert.AreEqual(8, ordered1.ElementAt(0).Id);
        Assert.AreEqual(9, ordered1.ElementAt(1).Id);
        Assert.AreEqual(10, ordered1.ElementAt(2).Id);

        var (count2, list2) = todos.FindByIsDone(false, 0, 5);
        Assert.AreEqual(6, count2);
        var ordered2 = list2.OrderBy(t => t.Id);
        Assert.AreEqual(1, ordered2.ElementAt(0).Id);
        Assert.AreEqual(2, ordered2.ElementAt(1).Id);
        Assert.AreEqual(3, ordered2.ElementAt(2).Id);
        Assert.AreEqual(4, ordered2.ElementAt(3).Id);
        Assert.AreEqual(5, ordered2.ElementAt(4).Id);
      }
    }
  }
}
