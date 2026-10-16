using Samsung_TV_Remote_Automation.Models;
using Samsung_TV_Remote_Automation.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Samsung_TV_Remote_Automation.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly SamsungTvClient _tvClient;
        private TvControlService _tvMacro;
        private string _statusText = "Disconnected";

        //Change later to let user choose IP in the app
        private string tvIp = "192.168.10.151";
        //The appname displayed on the Samsung TV
        string appName = Convert.ToBase64String(Encoding.UTF8.GetBytes("SamsungTest"));

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        //Commands that buttons in XAML binds to
        public ICommand SendPowerCommand { get;  }

        public MainViewModel(SamsungTvClient tvClient)
        {
            _tvClient = tvClient;
            _tvMacro = new TvControlService(_tvClient);

            SendPowerCommand = new Command(async () => await SendPowerAsync());
        }

        public async Task Test()
        {
            await _tvMacro.SetPictureModeAsync(SamsungKeys.FilmmakerMode, 500);
        }

        public async Task InitialiseAsync()
        {
            StatusText = "Connecting...";

            string? savedToken = await SecureStorage.Default.GetAsync("samsung_tv_token");

            string tokenParam = "";
            if (!string.IsNullOrEmpty(savedToken))
            {
                tokenParam = "&token=" + savedToken;
                System.Diagnostics.Debug.WriteLine("Token found in SecureStorage" + savedToken);
            }

            //Build the URL for the Samsung TV WebSocket connection, including the app name and token if available
            string samsungCommand = $"wss://{tvIp}:8002/api/v2/channels/samsung.remote.control?name={appName}{tokenParam}";

            try
            {
                await _tvClient.Connect(samsungCommand);

                StatusText = "Connected!";
                
            } catch (Exception ex)
            {
                StatusText = $"Error when connecting: {ex.Message}";
            }
        }

        private async Task SendPowerAsync()
        {

        }
    }
}
