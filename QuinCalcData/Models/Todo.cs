﻿using System;

namespace QuinCalcData.Models
{
  public class Todo
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public bool IsDone { get; set; }
  }
}
