using QuinCalc.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views.ExpensesDetail
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class DetailsView
  {
    public DetailsView()
    {
      InitializeComponent();
    }

    public ExpensesDetailViewModel ViewModel => DataContext as ExpensesDetailViewModel;
  }
}
