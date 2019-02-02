using QuinCalc.Helpers;

namespace QuinCalc.ViewModels
{
  public class ExpensePage
  {
    public int PageNum { get; set; }
    public int TotalCount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool ShowNextBtn { get; set; }
    public bool ShowBackBtn { get; set; }
  }

  public class ExpensePageVm : NotificationBase<ExpensePage>
  {
    public ExpensePageVm(ExpensePage expage = null) : base(expage) { }

    public int PageNum
    {
      get { return This.PageNum; }
      set { SetProperty(This.PageNum, value, () => This.PageNum = value); }
    }

    public int TotalCount
    {

      get { return This.TotalCount; }
      set { SetProperty(This.TotalCount, value, () => This.TotalCount = value); }
    }

    public decimal TotalAmount
    {

      get { return This.TotalAmount; }
      set { SetProperty(This.TotalAmount, value, () => This.TotalAmount = value); }
    }

    public bool ShowNextBtn
    {

      get { return This.ShowNextBtn; }
      set { SetProperty(This.ShowNextBtn, value, () => This.ShowNextBtn = value); }
    }

    public bool ShowBackBtn
    {
      get { return This.ShowBackBtn; }
      set { SetProperty(This.ShowBackBtn, value, () => This.ShowBackBtn = value); }
    }
  }
}

