using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace VkSoundDownloader
{
    public partial class SelectClips : PhoneApplicationPage
    {
        public List<Audio> AudioClips { get; set; }

        public SelectClips()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AudioClips != null)
            {
                clipList.ItemsSource = AudioClips;
                if(AudioClips.Count==0)
                    MessageBox.Show("Похоже что вы уже загрузили все свои аудиозаписи", "", MessageBoxButton.OK);
            }

            base.OnNavigatedTo(e);
        }

        private void DownloadSongs(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Download audio clips?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/DownloadPage.xaml", UriKind.Relative));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // NavigationEventArgs returns destination page
            DownloadPage destinationPage = e.Content as DownloadPage;
            if (destinationPage != null)
            {
                // Change property of destination page
                List<Audio> clips = new List<Audio>(AudioClips.Where(item => item.active == true));
                destinationPage.AudioClips = clips;
            }
        }
    }
}