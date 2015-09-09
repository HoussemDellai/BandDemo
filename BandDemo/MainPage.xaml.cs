using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BandDemo.Models;
using Microsoft.Band;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BandDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ReadHeartRateFromBandButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessage.Text = "Running ...";
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                var pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    StatusMessage.Text = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                    return;
                }

                // Connect to Microsoft Band.
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    int samplesReceived = 0; // the number of Accelerometer samples received

                    // Subscribe to Accelerometer data.
                    bandClient.SensorManager.HeartRate.ReadingChanged += async (s, args) =>
                    {
                        samplesReceived++;

                        var heartreading = args.SensorReading;
                        await StatusSensor.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                        {
                            StatusSensor.Text += heartreading.HeartRate + ",   ";

                            await SendDataToWebAsync(heartreading.HeartRate);
                        });
                    };

                    if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() != UserConsent.Granted)
                    {
                        await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                    }

                    bandClient.SensorManager.HeartRate.ReportingInterval = bandClient.SensorManager.HeartRate.SupportedReportingIntervals.First();

                    await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                    // Receive Accelerometer data for a while, then stop the subscription.
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    await bandClient.SensorManager.HeartRate.StopReadingsAsync();

                    StatusMessage.Text = string.Format("Done. {0} HeartRate samples were received.", samplesReceived);
                }
            }
            catch (Exception ex)
            {
                StatusMessage.Text = ex.ToString();
            }
        }

        private async Task SendDataToWebAsync(int heartRate)
        {
            var httpClient = new HttpClient();

            var heartRateData = new HeartRateData
            {
                Value = heartRate,
                CreatedAt = DateTime.Now
            };

            var jsonCustomer = JsonConvert.SerializeObject(heartRateData);

            HttpContent httpContent = new StringContent(jsonCustomer);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            httpClient.DefaultRequestHeaders.Accept
                          .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await httpClient.PostAsync("http://banddemo.azurewebsites.net/api/HeartRates", httpContent);
        }
    }
}
