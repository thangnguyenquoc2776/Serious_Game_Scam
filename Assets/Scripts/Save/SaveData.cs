using System;
using System.Collections.Generic;
using SeriousGame.State;

namespace SeriousGame.Save
{
    [Serializable]
    public class SaveFlagEntry
    {
        public string key;
        public bool value;
    }

    [Serializable]
    public class SaveData
    {
        public string sessionId;
        public string participantId;
        public string currentEpisodeId;
        public string currentUnityScene;
        public string currentYarnNode;
        public PlayerStateSnapshot playerState;
        public List<SaveFlagEntry> flags = new List<SaveFlagEntry>();
    }
}
