using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Media;
using System.Reflection;

namespace VkSoundDownloader
{
    public partial class DownloadPage : PhoneApplicationPage
    {
        public List<Audio> AudioClips { get; set; }

        Downloader audioDownloader;

        public DownloadPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AudioClips != null)
            {
                audioDownloader = new Downloader(AudioClips);
                audioDownloader.OnCurrentProgressChanged += audioDownloader_OnCurrentProgressChanged;
                audioDownloader.OnDownloadEnd += audioDownloader_OnDownloadEnd;
                audioDownloader.OnTotalProgressChanged += audioDownloader_OnTotalProgressChanged;

                audioDownloader.DownloadAll();

            }

            base.OnNavigatedTo(e);
        }

        void audioDownloader_OnTotalProgressChanged(object sender, ProgressEventArgs e)
        {
            totalBar.Value = e.Percentage;
        }

        void audioDownloader_OnDownloadEnd(object sender, EventArgs e)
        {
            totalBar.Value = 100;
            MessageBox.Show("Download complete");
        }

        void audioDownloader_OnCurrentProgressChanged(object sender, ProgressEventArgs e)
        {
            currentBar.Value = e.Percentage;
            currentItem.Text = e.Item;
        }
    }
}