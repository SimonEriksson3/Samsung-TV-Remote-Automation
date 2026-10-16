using Samsung_TV_Remote_Automation.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace Samsung_TV_Remote_Automation.Services
{
    public class SamsungTvClient
    {
        public ClientWebSocket webSocket;
        private string token;
        public bool hasToken => !string.IsNullOrEmpty(token);

        private TaskCompletionSource<bool> _authCompletedSource;

        public SamsungTvClient()
        {
            webSocket = new ClientWebSocket();
        }

        public async Task Connect(string url)
        {
            try
            {
                //Create new TaskCompletionSource for this connection
                _authCompletedSource = new TaskCompletionSource<bool>();

                // Accept all SSL certificates (for development purposes only)
                webSocket.Options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                await webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
                Debug.WriteLine("Connected to Samsung TV WebSocket server.");

                _ = RecieveAsync();

                await _authCompletedSource.Task;
                Debug.WriteLine("TV connection and authorization completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error connecting to Samsung TV WebSocket server: {ex.Message}");
                return;
            }
        }

        private async Task RecieveAsync()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        Debug.WriteLine("WebSocket connection closed.");
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Debug.WriteLine($"Received message: {message}");

                        if (token == null && message.Contains("token"))
                        {
                            // Extract the token from the message
                            var tokenStartIndex = message.IndexOf("\"token\":\"") + 9;
                            var tokenEndIndex = message.IndexOf("\"", tokenStartIndex);
                            token = message.Substring(tokenStartIndex, tokenEndIndex - tokenStartIndex);

                            Debug.WriteLine($"Token received: {token}");
                            await SecureStorage.Default.SetAsync("samsung_tv_token", token);
                        }

                        _authCompletedSource?.TrySetResult(true);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error receiving message from Samsung TV WebSocket server: {ex.Message}");
                _authCompletedSource.TrySetException(ex);
            }
        }



        public async Task SendCommandAsync(string jsonPayload)
        {
            if (_authCompletedSource != null)
            {
                await _authCompletedSource.Task;
            }

            if (webSocket.State == WebSocketState.Open && hasToken)
            {
                var bytes = Encoding.UTF8.GetBytes(jsonPayload);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Debug.WriteLine($"Sent command: {jsonPayload}");
            }
            else
            {
                Debug.WriteLine("WebSocket is not connected. Cannot send command.");
            }
        }
    }
}
