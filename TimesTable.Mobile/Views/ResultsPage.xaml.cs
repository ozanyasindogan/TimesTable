namespace TimesTable.Mobile.Views;

public partial class ResultsPage : ContentPage
{
	public ResultsPage(ResultsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
