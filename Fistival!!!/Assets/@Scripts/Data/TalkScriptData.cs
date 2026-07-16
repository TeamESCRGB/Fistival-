using System.Collections.Generic;

namespace Data
{
    public class TalkScriptData
    {
        public string DataKey { get; set; }
        public List<(string characterImg, string typeSFX, string script)> TalkData { get; set; }
    }
}