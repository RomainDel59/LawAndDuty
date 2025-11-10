using GTA;
using GTA.UI;
using LawAndDuty.Constants;
using LawAndDuty.Core;
using Waldhari.Core.Localization;
using Waldhari.Core.Persistence;

namespace LawAndDuty.Scripts
{
    /// <summary>
    /// One-time script that sends Dave Norton's initial message to Michael when he's near his poolside.
    /// This script is instantiated on-demand and terminates itself after sending the message.
    /// </summary>
    public class InitialMessageScript : Script
    {
        private readonly GameState _gameState;
        private readonly IPersistenceService _persistence;
        private readonly ILanguageService _language;
        private bool _messageSent;

        /// <summary>
        /// Initializes a new instance of the InitialMessageScript.
        /// </summary>
        /// <param name="gameState">The current game state</param>
        /// <param name="persistence">Persistence service for saving game state</param>
        /// <param name="language">Language service for localized messages</param>
        public InitialMessageScript(GameState gameState, IPersistenceService persistence, ILanguageService language)
        {
            _gameState = gameState;
            _persistence = persistence;
            _language = language;
            _messageSent = false;

            Waldhari.Core.Core.Logger.Info("InitialMessageScript instantiated");

            Tick += OnTick;
        }

        /// <summary>
        /// Checks Michael's position every tick and sends the message when conditions are met.
        /// </summary>
        private void OnTick(object sender, System.EventArgs e)
        {
            // Safety check: abort if message was somehow already sent
            if (_messageSent || _gameState.HasReceivedDaveInitialMessage)
            {
                Abort();
                return;
            }

            // Get Michael's current position
            Ped player = Game.Player.Character;
            if (player == null || !player.IsAlive)
                return;

            // Check if Michael is near his poolside
            float distance = player.Position.DistanceTo(Locations.MichaelPoolside);
            if (distance <= Locations.ProximityThreshold)
            {
                SendDaveMessage();
            }
        }

        /// <summary>
        /// Sends Dave Norton's initial message as an SMS notification.
        /// </summary>
        private void SendDaveMessage()
        {
            string message = _language.GetMessage("dave_initial_message");

            // Send SMS notification (simulates in-game phone message)
            Notification.Show("~h~Dave Norton~s~", message, "CHAR_DAVE");

            Waldhari.Core.Core.Logger.Info("Dave's initial message sent to Michael");

            // Update and save game state
            _gameState.HasReceivedDaveInitialMessage = true;
            _persistence.Save("LawAndDuty", _gameState);
            _messageSent = true;

            Waldhari.Core.Core.Logger.Info("Game state saved: HasReceivedDaveInitialMessage = true");

            // Terminate this script as its job is done
            Abort();
        }
    }
}
