using System;
using SeriousGame.Content;

namespace SeriousGame.Trace
{
    public class TraceService
    {
        private readonly ITraceStore _store;

        public TraceService(ITraceStore store)
        {
            _store = store;
        }

        public void RecordChoice(
            string sessionId,
            EpisodeSO episode,
            BeatSO beat,
            InteractionSO interaction,
            ChoiceSO choice)
        {
            if (choice == null) return;

            // Only meaningful actions should be logged
            if (string.IsNullOrWhiteSpace(choice.traceTypeId))
                return;

            var e = new TraceEvent
            {
                sessionId = sessionId,
                episodeId = episode != null ? episode.episodeId : "",
                beatId = beat != null ? beat.beatId : "",
                interactionId = interaction != null ? interaction.interactionId : "",
                choiceId = choice.choiceId,
                traceTypeId = choice.traceTypeId,
                unixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            _store.Add(e);
        }

        public System.Collections.Generic.List<TraceEvent> GetSession(string sessionId)
            => _store.GetBySession(sessionId);
    }
}
