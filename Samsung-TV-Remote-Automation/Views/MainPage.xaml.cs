using Samsung_TV_Remote_Automation.Services;
using Samsung_TV_Remote_Automation.ViewModels;

namespace Samsung_TV_Remote_Automation.Views
{
    public partial class MainPage : ContentPage
    {

        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;

            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.InitialiseAsync();

            
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            await _viewModel.Test();
        }
    }
}
