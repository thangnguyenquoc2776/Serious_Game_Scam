using System;

namespace SeriousGame.App
{
    public class SessionService
    {
        public string CurrentSessionId { get; private set; }
        public string PlayerId { get; private set; }
        public string AuthToken { get; private set; }
        public long StartTimeUnixMs { get; private set; }

        public string ParticipantId => PlayerId;

        public void Begin(string playerId = null)
        {
            CurrentSessionId = Guid.NewGuid().ToString("N");
            StartTimeUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (!string.IsNullOrWhiteSpace(playerId))
                PlayerId = playerId;
        }

        public void Restore(string sessionId, string playerId = null)
        {
            CurrentSessionId = sessionId;
            if (!string.IsNullOrWhiteSpace(playerId))
                PlayerId = playerId;
        }

        public void SetAuth(string playerId, string authToken)
        {
            if (!string.IsNullOrWhiteSpace(playerId))
                PlayerId = playerId;
            if (!string.IsNullOrWhiteSpace(authToken))
                AuthToken = authToken;
        }
    }
}
