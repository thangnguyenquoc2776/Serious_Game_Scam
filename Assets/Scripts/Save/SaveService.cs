using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using SeriousGame.App;
using SeriousGame.State;

namespace SeriousGame.Save
{
    public class SaveService
    {
        private readonly SessionService _session;
        private readonly PlayerStateService _state;
        private readonly string _savePath;

        public SaveService(SessionService session, PlayerStateService state, string fileName = "save.json")
        {
            _session = session;
            _state = state;
            _savePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public string SavePath => _savePath;

        public SaveData BuildCurrent(string currentEpisodeId = "", string currentYarnNode = "")
        {
            var data = new SaveData
            {
                sessionId = _session != null ? _session.CurrentSessionId : "",
                participantId = _session != null ? _session.ParticipantId : "",
                currentEpisodeId = currentEpisodeId ?? "",
                currentUnityScene = SceneManager.GetActiveScene().name,
                currentYarnNode = currentYarnNode ?? "",
                playerState = _state != null ? _state.GetSnapshot() : new PlayerStateSnapshot()
            };

            data.flags = BuildFlagsSnapshot();
            return data;
        }

        public bool SaveCurrent(string currentEpisodeId = "", string currentYarnNode = "")
        {
            return Save(BuildCurrent(currentEpisodeId, currentYarnNode));
        }

        public bool Save(SaveData data)
        {
            if (data == null) return false;
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_savePath, json);
            return true;
        }

        public SaveData Load()
        {
            if (!File.Exists(_savePath)) return null;
            var json = File.ReadAllText(_savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public void ApplyLoaded(SaveData data)
        {
            if (data == null) return;

            if (_session != null)
                _session.Restore(data.sessionId, data.participantId);

            if (_state != null && data.playerState != null)
                _state.LoadSnapshot(data.playerState);

            var gsm = GameStateManager.Instance;
            if (gsm != null && data.flags != null)
            {
                for (int i = 0; i < data.flags.Count; i++)
                {
                    var flag = data.flags[i];
                    if (flag == null || string.IsNullOrWhiteSpace(flag.key)) continue;
                    gsm.SetFlag(flag.key, flag.value);
                }
            }
        }

        private List<SaveFlagEntry> BuildFlagsSnapshot()
        {
            var list = new List<SaveFlagEntry>();
            var gsm = GameStateManager.Instance;
            if (gsm == null) return list;

            var snapshot = gsm.GetSnapshot();
            if (snapshot == null) return list;

            foreach (var kv in snapshot)
            {
                list.Add(new SaveFlagEntry
                {
                    key = kv.Key,
                    value = kv.Value
                });
            }

            return list;
        }
    }
}
