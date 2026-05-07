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


        //constructor with dependency injection for session and state services, and optional file name for save data.
        public SaveService(SessionService session, PlayerStateService state, string fileName = "save.json")
        {
            _session = session;
            _state = state;
            _savePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public string SavePath => _savePath;

        //!SAVE

        // Builds a SaveData object representing the current game state, including session info, player state, and optional narrative context.
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
        // Saves the current game state to a file by building a SaveData object and serializing it to JSON. Returns true if the save was successful.
        public bool SaveCurrent(string currentEpisodeId = "", string currentYarnNode = "")
        {
            return Save(BuildCurrent(currentEpisodeId, currentYarnNode));
        }
        // Saves the provided SaveData to a file. Returns true if the save was successful.
        public bool Save(SaveData data)
        {
            if (data == null) return false;
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_savePath, json);
            Debug.Log($"[SaveService] Saved game state to {_savePath}");
            return true;
        }

        // Loads the game state from a file and returns it as a SaveData object. Returns null if the file does not exist or if loading fails.


        //! LOAD
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

            if (_state != null)
            {
                ApplyLegacyFlags(data.flags);
            }
        }

        // Builds a list of SaveFlagEntry objects representing the current state of all flags in the PlayerStateService. This is used for saving legacy flag data.
        private List<SaveFlagEntry> BuildFlagsSnapshot()
        {
            var list = new List<SaveFlagEntry>();
            if (_state == null) return list;

            var snapshot = _state.GetFlagsSnapshot();
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

        // Applies legacy flag data to the PlayerStateService.
        private void ApplyLegacyFlags(List<SaveFlagEntry> flags)
        {
            if (_state == null || flags == null) return;

            for (int i = 0; i < flags.Count; i++)
            {
                var flag = flags[i];
                if (flag == null || string.IsNullOrWhiteSpace(flag.key)) continue;
                _state.SetFlag(flag.key, flag.value);
            }
        }
    }
}
