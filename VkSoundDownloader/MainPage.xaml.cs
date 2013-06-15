using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VkSoundDownloader.Resources;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;

namespace VkSoundDownloader
{
    public partial class MainPage : PhoneApplicationPage
    {
        int appID = 3497419;
        VKApiWrapper vk;

        public List<Audio> clips { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            vk = new VKApiWrapper(browser, appID);
            vk.OnAudioList += onAudioList;

            vk.GetAudio();
        }

        private void onAudioList(object sender, AudioEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Sync synker = new Sync(e.AudioClips);
                clips = new List<Audio>(synker.GetClipsToDownload());
                NavigationService.Navigate(new Uri("/SelectClips.xaml", UriKind.Relative));
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // NavigationEventArgs returns destination page
            SelectClips destinationPage = e.Content as SelectClips;
            if (destinationPage != null)
            {
                // Change property of destination page
                destinationPage.AudioClips = clips;
            }
        }

        public void TryReenableApplicationIdleDetection()
        {
            try
            {
                Microsoft.Phone.Shell.PhoneApplicationService.Current.ApplicationIdleDetectionMode =
                    Microsoft.Phone.Shell.IdleDetectionMode.Disabled;
            }
            catch (InvalidOperationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}