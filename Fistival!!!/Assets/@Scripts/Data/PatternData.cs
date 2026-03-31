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
            BPM = original.BPM;
            SongName = original.SongName;
            PerfactRange = original.PerfactRange;
            GoodRange = original.GoodRange;
            Notes = original.Notes;
        }

        public PatternData() { }

        public string Key;
        public float BPM;
        public string SongName;
        public float PerfactRange;
        public float GoodRange;
        public List<NoteData> Notes;

    }
}
