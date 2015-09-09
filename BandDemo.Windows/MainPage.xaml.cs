using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BandDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;

            await vm.DownloadDataAsync();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;

            await vm.GetFilteredHeartRatesDataAsync();
        }
    }
}
