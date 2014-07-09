using System.Collections.Generic;

namespace BMAPI
{
    public class Info_Beatmap
    {
        //Info
        public int? Format = null;
        public string Filename;
        public string BeatmapHash;

        //General
        public string AudioFilename;
        public int? AudioLeadIn = null;
        public int? PreviewTime = null;
        public int? Countdown = null;
        public string SampleSet;
        public float? StackLeniency = null;
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
        public float? EditorDistanceSpacing = null;
        public string AudioHash;
        public bool? AlwaysShowPlayfield = null;

        //Editor (Other Editor tag stuff (v12))
        public int? GridSize = null;
        public List<int> Bookmarks = new List<int>();
        public int? BeatDivisor = null;
        public float? DistanceSpacing = null;
        public int? CurrentTime = null;
        public float? TimelineZoom = null;

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
        public float HPDrainRate = 5;
        private float p_CircleSize = 5;
        public float CircleSize
        {
            get
            {
                return p_CircleSize;
            }
            set
            {
                p_CircleSize = value;
                foreach (HitObject_Circle hO in HitObjects)
                {
                    hO.Radius = 40 - 4 * (value - 2);
                }
            }
        }
        public float OverallDifficulty = 5;
        public float ApproachRate = 5;
        public float SliderMultiplier = 1.4f;
        public float SliderTickRate = 1;

        //Events
        public List<Event_Base> Events = new List<Event_Base>();

        //Timingpoints
        public List<Info_TimingPoint> TimingPoints = new List<Info_TimingPoint>();

        //Colours
        public List<Info_Combo> ComboColours = new List<Info_Combo>();
        public Helper_Colour SliderBorder = new Helper_Colour { R = 255, G = 255, B = 255 };

        //Hitobjects
        public List<HitObject_Circle> HitObjects = new List<HitObject_Circle>();
    }
}
