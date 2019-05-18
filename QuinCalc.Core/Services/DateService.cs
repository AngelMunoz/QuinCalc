using System;

namespace QuinCalc.Core.Services
{
  public static class DateService
  {

    /// <summary>
    /// Get the closest day to pay the expenses (either day 15 or last day of the month)
    /// </summary>
    /// <param name="dayToCheck"></param>
    /// <returns></returns>
    public static DateTimeOffset GetNextQuin(int? dayToCheck = null)
    {
      dayToCheck = dayToCheck ?? GetDayToCheck();
      return new DateTimeOffset(new DateTime(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, dayToCheck.Value));
    }

    /// <summary>
    /// Gets the Next Day to Check (either 15 or last day of the month)
    /// </summary>
    /// <returns></returns>
    public static int GetDayToCheck()
    {
      return DateTimeOffset.Now.Day < 15 ? 15 : DateTime.DaysInMonth(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month);
    }
  }
}
