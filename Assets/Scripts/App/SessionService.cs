using System;

namespace SeriousGame.App
{
    public class SessionService
    {
        public string CurrentSessionId { get; private set; }
        public string ParticipantId { get; private set; }

        public void Begin(string participantId = null)
        {
            CurrentSessionId = Guid.NewGuid().ToString("N");
            if (!string.IsNullOrWhiteSpace(participantId))
                ParticipantId = participantId;
        }

        public void Restore(string sessionId, string participantId = null)
        {
            CurrentSessionId = sessionId;
            if (!string.IsNullOrWhiteSpace(participantId))
                ParticipantId = participantId;
        }
    }
}
