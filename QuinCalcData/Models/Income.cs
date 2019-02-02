using System;
using System.Collections.Generic;
using System.Text;

namespace QuinCalcData.Models
{
  public class Income
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public bool HasArrived { get; set; }
    public DateTimeOffset DueTo { get; set; }

    public Budget Budget { get; set; }
  }
}
