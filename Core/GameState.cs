namespace LawAndDuty.Core
{
    /// <summary>
    /// Represents the persistent game state for Law and Duty mod.
    /// This class is serialized and saved to track player progress and story events.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Indicates whether Michael has received the initial message from Dave Norton.
        /// This message triggers when Michael is near his poolside for the first time.
        /// </summary>
        public bool HasReceivedDaveInitialMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of GameState with default values.
        /// </summary>
        public GameState()
        {
            HasReceivedDaveInitialMessage = false;
        }
    }
}
