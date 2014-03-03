using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMAPI
{
    public class BeatmapInfo
    {
        public int? Format = null;
        public string AudioFilename;
        public int? AudioLeadIn = null;
        public int? PreviewTime = null;
        public int? Countdown = null;
        public string SampleSet;
        public double? StackLeniency = null;
        public GameMode? Mode = null;
        public int? LetterboxInBreaks = null;
        public int? SpecialStyle = null;
        public int? CountdownOffset = null;
        public OverlayOptions? OverlayPosition = null;
        public string SkinPreference;
        public int? WidescreenStoryboard = null;
        public int? UseSkinSprites = null;
        public int? StoryFireInFront = null;
        public int? EpilepsyWarning = null;
        public int? CustomSamples = null;
        public List<int> EditorBookmarks = new List<int>();
        public double? EditorDistanceSpacing = null;
        public string AudioHash;
        public bool? AlwaysShowPlayfield = null;

        //Editor (Other Editor tag stuff (v12))
        public int? GridSize = null;
        public List<int> Bookmarks = new List<int>();
        public int? BeatDivisor = null;
        public double? DistanceSpacing = null;
        public int? CurrentTime = null;

        //Metadata
        public string Title;
        public string TitleUnicode;
        public string Artist;
        public string ArtistUnicode;
        public string Creator;
        public string Version;
        public string Source;
        public List<string> Tags = new List<string>();
        public int? BeatmapID = null;
        public int? BeatmapSetID = null;

        //Difficulty
        public int? HPDrainRate = null;
        public int? CircleSize = null;
        public int? OverallDifficulty = null;
        public int? ApproachRate = null;
        public double? SliderMultiplier = null;
        public double? SliderTickRate = null;

        //Events
        public List<dynamic> Events = new List<dynamic>();

        //Timingpoints
        public List<TimingPointInfo> TimingPoints = new List<TimingPointInfo>();

        //Colours
        public List<BMAPI.ComboInfo> ComboColours = new List<BMAPI.ComboInfo>();
        public BMAPI.ColourInfo SliderBorder = new ColourInfo();

        //Hitobjects
        public List<dynamic> HitObjects = new List<dynamic>();
    }
}
