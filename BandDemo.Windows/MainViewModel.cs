using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BandDemo.Models;
using Newtonsoft.Json;

namespace BandDemo
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<HeartRateData> _heartRateDataList;

        public List<HeartRateData> HeartRateDataList
        {
            get { return _heartRateDataList; }
            set
            {
                _heartRateDataList = value; 
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            DownloadDataAsync();
        }

        public async Task DownloadDataAsync()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetStringAsync("http://banddemo.azurewebsites.net/api/HeartRates/");

            HeartRateDataList  = JsonConvert.DeserializeObject<List<HeartRateData>>(response);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task GetFilteredHeartRatesDataAsync()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetStringAsync("http://banddemo.azurewebsites.net/api/FilteredHeartRates/");

            var heartRateDataList = JsonConvert.DeserializeObject<List<HeartRateData>>(response);

            foreach (var heartRateData in heartRateDataList)
            {
                heartRateData.CreatedAtStr = heartRateData.CreatedAt.Hour + ":"
                                             + heartRateData.CreatedAt.Minute;
            }

            HeartRateDataList = heartRateDataList;

            //HeartRateDataList = heartRateDataList.Select(data => new HeartRateData
            //{
            //    Value = data.Value,
            //    CreatedAtStr = data.CreatedAt.Hour + ":" + data.CreatedAt.Minute
            //}).ToList();
        }
    }
}
