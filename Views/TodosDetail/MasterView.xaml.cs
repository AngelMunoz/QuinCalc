using QuinCalc.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuinCalc.Views.TodosDetail
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MasterView
  {
    public MasterView()
    {
      InitializeComponent();
    }

    public TodosDetailViewModel ViewModel => DataContext as TodosDetailViewModel;
  }
}
