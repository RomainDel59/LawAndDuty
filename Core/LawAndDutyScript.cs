using System;
using GTA;
using LawAndDuty.Core;
using LawAndDuty.Scripts;
using Waldhari.Core.Localization;
using Waldhari.Core.Logging;
using Waldhari.Core.Persistence;

namespace LawAndDuty
{
    /// <summary>
    /// Main script for the Law and Duty mod.
    /// Handles initialization, state management, and on-demand script instantiation.
    /// </summary>
    public class LawAndDutyScript : Script
    {
        private const string ModName = "LawAndDuty";
        
        private readonly IPersistenceService _persistence;
        private readonly ILanguageService _language;
        private GameState _gameState;

        /// <summary>
        /// Initializes the Law and Duty mod script.
        /// Sets up Waldhari.Core services and loads game state.
        /// </summary>
        public LawAndDutyScript()
        {
            try
            {
                // Configure Waldhari.Core logger for this mod
                var logger = new TsvLogService(ModName);
                Waldhari.Core.Core.SetLogger(logger);
                Waldhari.Core.Core.Logger.Info($"{ModName} script initializing...");

                // Initialize persistence service
                _persistence = new XmlPersistenceService();

                // Initialize localization service with French language
                _language = new CsvLanguageService(ModName, "fr-FR");
                _language.Load(ModName, "fr-FR");

                Waldhari.Core.Core.Logger.Info("Waldhari.Core services initialized");

                // Load or create game state
                LoadGameState();

                // Check and instantiate necessary scripts based on game state
                CheckAndInstantiateScripts();

                Waldhari.Core.Core.Logger.Info($"{ModName} script initialized successfully");
            }
            catch (Exception ex)
            {
                Waldhari.Core.Core.Logger.Error($"Failed to initialize {ModName} script", ex);
                throw;
            }
        }

        /// <summary>
        /// Loads the game state from persistent storage or creates a new one if none exists.
        /// </summary>
        private void LoadGameState()
        {
            try
            {
                _gameState = _persistence.Load<GameState>(ModName);

                if (_gameState == null)
                {
                    Waldhari.Core.Core.Logger.Info("No existing save found, creating new GameState");
                    _gameState = new GameState();
                    _persistence.Save(ModName, _gameState);
                }
                else
                {
                    Waldhari.Core.Core.Logger.Info("GameState loaded successfully");
                    Waldhari.Core.Core.Logger.Debug($"HasReceivedDaveInitialMessage: {_gameState.HasReceivedDaveInitialMessage}");
                }
            }
            catch (Exception ex)
            {
                Waldhari.Core.Core.Logger.Error("Failed to load GameState", ex);
                throw;
            }
        }

        /// <summary>
        /// Checks game state and instantiates necessary scripts on-demand.
        /// This method is called once during initialization.
        /// </summary>
        private void CheckAndInstantiateScripts()
        {
            // Check if Dave's initial message needs to be sent
            if (!_gameState.HasReceivedDaveInitialMessage)
            {
                Waldhari.Core.Core.Logger.Info("Initial message not yet sent, instantiating InitialMessageScript");
                InstantiateScript<InitialMessageScript>(_gameState, _persistence, _language);
            }
        }
    }
}
