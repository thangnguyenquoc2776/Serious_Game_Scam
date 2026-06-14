using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SeriousGame.Trace
{
    public class FileTraceStore : ITraceStore
    {
        [Serializable]
        private class TraceRecord
        {
            public string sessionId;
            public GameTrace trace;
        }

        private readonly string _filePath;
        private readonly object _lock = new object();

        public FileTraceStore(string fileName = "trace.jsonl")
        {
            _filePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public string FilePath => _filePath;

        public void Add(GameTrace trace)
        {
            if (trace == null) return;
            var record = new TraceRecord
            {
                sessionId = trace.sessionId,
                trace = trace
            };
            var json = JsonConvert.SerializeObject(record);

            lock (_lock)
            {
                File.AppendAllText(_filePath, json + Environment.NewLine);
            }
            Debug.Log($"[FileTraceStore] Added trace to file: {json}");
        }

        public List<GameTrace> GetBySession(string sessionId)
        {
            var res = new List<GameTrace>();
            if (string.IsNullOrWhiteSpace(sessionId)) return res;
            if (!File.Exists(_filePath)) return res;

            var lines = File.ReadAllLines(_filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                TraceRecord record = null;
                try
                {
                    record = JsonConvert.DeserializeObject<TraceRecord>(line);
                }
                catch
                {
                    continue;
                }

                if (record != null && record.sessionId == sessionId && record.trace != null)
                    res.Add(record.trace);
            }

            return res;
        }

        public void ClearSession(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return;
            if (!File.Exists(_filePath)) return;

            var lines = File.ReadAllLines(_filePath);
            var kept = new List<string>(lines.Length);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                TraceRecord record = null;
                try
                {
                    record = JsonConvert.DeserializeObject<TraceRecord>(line);
                }
                catch
                {
                    kept.Add(line);
                    continue;
                }

                if (record == null || record.sessionId != sessionId)
                    kept.Add(line);
            }

            File.WriteAllLines(_filePath, kept.ToArray());
        }
    }
}
