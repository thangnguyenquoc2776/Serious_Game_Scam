using System;

namespace SeriousGame.App
{
    public class SessionService
    {
        public string CurrentSessionId { get; private set; }

        public void Begin()
        {
            CurrentSessionId = Guid.NewGuid().ToString("N");
        }
    }
}
