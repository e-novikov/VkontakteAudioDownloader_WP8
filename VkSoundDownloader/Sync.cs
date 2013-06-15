using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkSoundDownloader
{
    public class Sync
    {
        public List<Audio> AudioClips { get; set; }

        public Sync(List<Audio> AudioClips)
        {
            this.AudioClips = AudioClips;
        }

        public IEnumerable<Audio> GetClipsToDownload()
        {
            var ml = new MediaLibrary();
            SongCollection songs = ml.Songs;

            List<Audio> chachedClips = new List<Audio>(songs.Where(item => item.Genre.Name == "vkontakte").Select(song => new Audio() {title=song.Name}));

            Dictionary<string, Audio> clipMap = new Dictionary<string, Audio>();
            foreach (var item in chachedClips)
            {
                clipMap.Add(item.title, item);
            }

            System.Diagnostics.Debug.WriteLine(clipMap.Count);
            var result = AudioClips.Where(kvp => !clipMap.ContainsKey(kvp.title));

            return result;
        }
    }
}
