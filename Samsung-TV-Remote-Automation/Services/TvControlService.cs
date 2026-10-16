using Samsung_TV_Remote_Automation.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Samsung_TV_Remote_Automation.Services
{
    /**
     * Used to send a series of commands to the TV with a set delay between each of them to assure each command is executed sequentially.
     */
    public class TvControlService
    {
        private readonly SamsungTvClient _tvClient;

        public TvControlService(SamsungTvClient tvClient)
        {
            _tvClient = tvClient;
        }

        public async Task SetPictureModeAsync(int modeIndex, int delay)
        {
            List<string> commands = new List<string> {SamsungKeys.Menu, SamsungKeys.Enter, SamsungKeys.Enter };

            for(int i = 0; i < modeIndex; i++)
            {
                commands.Add(SamsungKeys.Down);
            }

            commands.Add(SamsungKeys.Enter);
            commands.Add(SamsungKeys.Left);
            commands.Add(SamsungKeys.Enter);
            commands.Add(SamsungKeys.Return);
            commands.Add(SamsungKeys.Return);

            await SendWithDelay(commands, 500);
        }

        public async Task SendWithDelay(List<string> commands, int delay)
        {
            foreach(string s in commands)
            {
                await SendKeyAsync(s);
                await Task.Delay(delay);
            }
        }

         

        private async Task SendKeyAsync(string key)
        {
            string json = $"{{\"method\":\"ms.remote.control\",\"params\":{{\"Cmd\":\"Click\",\"DataOfCmd\":\"{key}\",\"Option\":\"false\",\"TypeOfRemote\":\"SendRemoteKey\"}}}}";
            await _tvClient.SendCommandAsync(json);
        }
    }
}
