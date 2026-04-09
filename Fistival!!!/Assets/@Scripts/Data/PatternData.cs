using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class PatternData
    {
        public PatternData(PatternData original)
        {
            Key = original.Key;
            IsLooping = original.IsLooping;
            BPM = original.BPM;
            SongName = original.SongName;
            PerfactRange = original.PerfactRange;
            GoodRange = original.GoodRange;
            SongLength = original.SongLength;
            Notes = original.Notes;
        }

        public PatternData() { }

        public string Key;
        public bool IsLooping;
        public float BPM;
        public string SongName;
        public double PerfactRange;
        public double GoodRange;
        public float SongLength;
        public List<NoteData> Notes;

    }
}
