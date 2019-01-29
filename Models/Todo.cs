using System;

namespace QuinCalc.Models
{
  public class Todo
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsDone { get; set; }
  }
}
