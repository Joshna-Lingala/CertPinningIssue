using System.Net.Security;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using Microsoft.Maui.Storage;

namespace MauiAppNet9
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            string result = string.Empty;
            var sslOptions = new SslClientAuthenticationOptions();
            var shHandler = new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 100,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                PooledConnectionLifetime = TimeSpan.FromMinutes(1),
                ConnectTimeout = TimeSpan.FromSeconds(10),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(10),
                ResponseDrainTimeout = TimeSpan.FromSeconds(10),
                PreAuthenticate = true
            };

            shHandler.SslOptions = new SslClientAuthenticationOptions()
            {
                ClientCertificates = new X509CertificateCollection(),
                EnabledSslProtocols= System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13 
            };

            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream2 = assembly.GetManifestResourceStream("MauiAppNet9.Resources.RQMobile.pfx");

            var bytes2 = default(byte[]);

            using (MemoryStream ms2 = new MemoryStream())
            {
                stream2.CopyTo(ms2);
                bytes2 = ms2.ToArray();
            }

            string pfxPassword = "Revomobile";

            X509Certificate2 clientCertificate = new X509Certificate2(bytes2, pfxPassword);
            shHandler.SslOptions.ClientCertificates.Add(clientCertificate);

            try
            {
                using (var client = new HttpClient(shHandler))
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://web.revoquest.com/RESTClient50/Region/RequestRouter");

                    requestMessage.Content = new StringContent(
                            "{\"Data\": \"cgp3uykqrUkOoQlOeeLglauCJDhi8a5zgvGXgbCvmQVgc93URWvgae9dr0ywwiGEf1vpABVsEU3k1u7jYBFipxlyAe3ds69Gii+4ogbtZ6KUHM8NVXGNKQw5m+Wkm8YRWRE97S5KyhcBq53v0Ss3DK+1S8CpfNVz8XELBtYc8yhMN9bzXDwD3cTCODtiqCbZPVpcbK8uQ79iJiPo6ZxWYQ==\"," +
                            "\"Route\": \"lxfmvRZVnmok7AjlKOX1V8SkZxlT2eq7wlSWqtJW1Ms=\"}",
                            Encoding.UTF8,
                            "application/json");

                    client.Timeout = TimeSpan.FromMilliseconds(120000);
                    var response = client.Send(requestMessage);

                    using (StreamReader reader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        string responseText = reader.ReadToEnd();
                        result = responseText.Replace("{\"d\":null}", "");
                        Console.WriteLine("Response received successfully:");
                        Console.WriteLine(result);
                        ResponseTextBox.Text = $"Success\n\n{result}";
                    }
                };
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = $"Response receiving failed. Exception occurred: {ex.Message}";
            }

        }
    }

}
