using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VkSoundDownloader
{
    // Declare the delegate (if using non-generic pattern).
    public delegate void DownloaderTotalProgressChanged(object sender, ProgressEventArgs e);
    public delegate void DownloaderTotalCurrentChanged(object sender, ProgressEventArgs e);
    public delegate void DownloaderDownloadEnd(object sender, EventArgs e);

    public class ProgressEventArgs : EventArgs
    {
        public int Percentage { get; set; }
        public string Item { get; set; }
    }

    class Downloader
    {
        public List<Audio> AudioClips { get; set; }

        int currDownloadIndex = -1;
        Audio currClip;

        

        // Declare the event.
        public event DownloaderTotalProgressChanged OnTotalProgressChanged;
        public event DownloaderTotalCurrentChanged OnCurrentProgressChanged;
        public event DownloaderDownloadEnd OnDownloadEnd;

        public Downloader(List<Audio> clips)
        {
            this.AudioClips = clips;
        }

        private void DownloadAudio()
        {
            Uri file = new Uri(currClip.url, UriKind.Absolute);

            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            webClient.OpenReadAsync(file);
        }

        private void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Result == null)
                return;
            var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
            using (isolatedStorageFile)
            {
                var isolatedStorageFileStream = isolatedStorageFile.CreateFile(currClip.aid.ToString());

                int chunkSize = 4096;
                byte[] bytes = new byte[chunkSize];
                int byteCount;
                while ((byteCount = e.Result.Read(bytes, 0, chunkSize)) > 0)
                {
                    isolatedStorageFileStream.Write(bytes, 0, byteCount);
                }
                isolatedStorageFileStream.Close();

                Microsoft.Xna.Framework.Media.PhoneExtensions.SongMetadata metaData = new Microsoft.Xna.Framework.Media.PhoneExtensions.SongMetadata();
                metaData.ArtistName = currClip.artist;
                metaData.Name = currClip.title;
                metaData.GenreName = "vkontakte";

                var ml = new MediaLibrary();
                Uri songUri = new Uri(currClip.aid.ToString(), UriKind.RelativeOrAbsolute);

                var song = Microsoft.Xna.Framework.Media.PhoneExtensions.MediaLibraryExtensions.SaveSong
                           (ml,
                            songUri,
                            metaData, Microsoft.Xna.Framework.Media.PhoneExtensions.SaveSongOperation.CopyToLibrary
                            );

                isolatedStorageFile.DeleteFile(currClip.aid.ToString());
            }

            DownloadNext();
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if(OnCurrentProgressChanged!=null)
                OnCurrentProgressChanged(this, new ProgressEventArgs() {Percentage = e.ProgressPercentage, Item=currClip.title });
        }

        private void DownloadNext()
        {
            currDownloadIndex++;
            if (currDownloadIndex < AudioClips.Count)
            {
                currClip = AudioClips[currDownloadIndex];
                DownloadAudio();
                if (OnTotalProgressChanged != null)
                    OnTotalProgressChanged(this, new ProgressEventArgs() { Percentage = (int)((float)currDownloadIndex / (float)AudioClips.Count * 100) });
            }
            else
            {
                if (OnDownloadEnd != null)
                    OnDownloadEnd(this, new EventArgs());
            }
        }

        public void DownloadAll()
        {
            DownloadNext();
        }
    }
}
