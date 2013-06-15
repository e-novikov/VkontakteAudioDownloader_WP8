using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace VkSoundDownloader
{
    public class AudioEventArgs : EventArgs
    {
        public List<Audio> AudioClips { get; set; }
    }

    class VKApiWrapper
    {
        WebBrowser browser;
        int appId;

        public string AccessToken { get; set; }
        public string UserId { get; set; }
        public string Response { get; set; }

        private string oAuthString = "https://oauth.vk.com/authorize?client_id={0}&scope=friends,video,audio&redirect_uri=https://oauth.vk.com/blank.html&display=touch&response_type=token";

        // Declare the delegate (if using non-generic pattern).
        public delegate void AudioListHandler(object sender, AudioEventArgs e);

        // Declare the event.
        public event AudioListHandler OnAudioList;

        public VKApiWrapper(WebBrowser browser, int appId)
        {
            this.browser = browser;
            this.appId = appId;

            //this.browser.ClearCookiesAsync();
            //this.browser.ClearInternetCacheAsync();
        }

        private void onBrowserNavigated(object sender, NavigationEventArgs args)
        {
            GetAccessTokenFromUri(args.Uri.OriginalString, browser.Source.AbsoluteUri);
        }

        private void GetAccessTokenFromUri(string uri, string baseUrl)
        {
            if (uri.IndexOf("access_token") == -1)
                return;
            int firs = uri.IndexOf(baseUrl);
            string parameters = uri.Substring(firs + baseUrl.Length + 1);
            string[] pairs = parameters.Split('&');
            Dictionary<string, string> urlParameters = new Dictionary<string, string>();
            foreach (var s in pairs)
            {
                string[] kv = s.Split('=');
                urlParameters.Add(kv[0], kv[1]);
            }

            if (urlParameters.ContainsKey("access_token"))
            {
                AccessToken = urlParameters["access_token"];
                UserId = urlParameters["user_id"];
                System.Diagnostics.Debug.WriteLine("sssssssssssssssssssssssssssssssssssss");
                Uri httpuri = new Uri(string.Format("https://api.vk.com/method/{0}?{1}&access_token={2}", "audio.get", "uid=" + UserId, AccessToken));
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(httpuri);
                httpWebRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), httpWebRequest);
            }
        }

        void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
            var reader = new System.IO.StreamReader(response.GetResponseStream());
            var data = reader.ReadToEnd();
            Response = data;

            var parsedJson = JObject.Parse(data);

            List<Audio> AudioClips = JsonConvert.DeserializeObject<List<Audio>>(parsedJson["response"].ToString());
            OnAudioList(this, new AudioEventArgs() { AudioClips = AudioClips });
        }

        private Uri getAuthUri(int appId)
        {
            return new Uri(string.Format(oAuthString, appId));
        }

        public void GetAudio()
        {
            browser.Navigated += new EventHandler<NavigationEventArgs>(onBrowserNavigated);
            browser.Navigate(getAuthUri(appId));
        }
    }
}
