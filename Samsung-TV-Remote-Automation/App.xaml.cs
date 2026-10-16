using Microsoft.Extensions.DependencyInjection;
using Samsung_TV_Remote_Automation.Views;

namespace Samsung_TV_Remote_Automation
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}