using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SeriousGame.Trace
{
    public class FileTraceStore : ITraceStore
    {
        private readonly string _filePath;
        private readonly object _lock = new object();

        public FileTraceStore(string fileName = "trace.jsonl")
        {
            _filePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public string FilePath => _filePath;

        public void Add(TraceEvent e)
        {
            if (e == null) return;
            var json = JsonUtility.ToJson(e);

            lock (_lock)
            {
                File.AppendAllText(_filePath, json + Environment.NewLine);
            }
        }

        public List<TraceEvent> GetBySession(string sessionId)
        {
            var res = new List<TraceEvent>();
            if (string.IsNullOrWhiteSpace(sessionId)) return res;
            if (!File.Exists(_filePath)) return res;

            var lines = File.ReadAllLines(_filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                TraceEvent e = null;
                try
                {
                    e = JsonUtility.FromJson<TraceEvent>(line);
                }
                catch
                {
                    continue;
                }

                if (e != null && e.sessionId == sessionId)
                    res.Add(e);
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

                TraceEvent e = null;
                try
                {
                    e = JsonUtility.FromJson<TraceEvent>(line);
                }
                catch
                {
                    kept.Add(line);
                    continue;
                }

                if (e == null || e.sessionId != sessionId)
                    kept.Add(line);
            }

            File.WriteAllLines(_filePath, kept.ToArray());
        }
    }
}
