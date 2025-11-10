using NUnit.Framework;
using LawAndDuty.Core;

namespace LawAndDuty.Tests.Core
{
    /// <summary>
    /// Unit tests for GameState class.
    /// Tests cover initialization, property behavior, and edge cases.
    /// </summary>
    [TestFixture]
    [TestOf(typeof(GameState))]
    public class GameStateTests
    {
        [Test]
        public void Constructor_DefaultValues_InitializesCorrectly()
        {
            // Act
            var gameState = new GameState();

            // Assert
            Assert.IsFalse(gameState.HasReceivedDaveInitialMessage, 
                "HasReceivedDaveInitialMessage should be false by default");
        }

        [Test]
        public void HasReceivedDaveInitialMessage_SetToTrue_StoresCorrectly()
        {
            // Arrange
            var gameState = new GameState();

            // Act
            gameState.HasReceivedDaveInitialMessage = true;

            // Assert
            Assert.IsTrue(gameState.HasReceivedDaveInitialMessage, 
                "HasReceivedDaveInitialMessage should be true after being set");
        }

        [Test]
        public void HasReceivedDaveInitialMessage_SetToFalse_StoresCorrectly()
        {
            // Arrange
            var gameState = new GameState
            {
                HasReceivedDaveInitialMessage = true
            };

            // Act
            gameState.HasReceivedDaveInitialMessage = false;

            // Assert
            Assert.IsFalse(gameState.HasReceivedDaveInitialMessage, 
                "HasReceivedDaveInitialMessage should be false after being reset");
        }

        [Test]
        public void GameState_MultipleInstances_AreIndependent()
        {
            // Arrange & Act
            var gameState1 = new GameState();
            var gameState2 = new GameState();
            
            gameState1.HasReceivedDaveInitialMessage = true;

            // Assert
            Assert.IsTrue(gameState1.HasReceivedDaveInitialMessage, 
                "First instance should have property set to true");
            Assert.IsFalse(gameState2.HasReceivedDaveInitialMessage, 
                "Second instance should remain independent with default value");
        }

        [Test]
        public void GameState_PropertyToggling_WorksCorrectly()
        {
            // Arrange
            var gameState = new GameState();

            // Act & Assert - Toggle multiple times
            gameState.HasReceivedDaveInitialMessage = true;
            Assert.IsTrue(gameState.HasReceivedDaveInitialMessage);

            gameState.HasReceivedDaveInitialMessage = false;
            Assert.IsFalse(gameState.HasReceivedDaveInitialMessage);

            gameState.HasReceivedDaveInitialMessage = true;
            Assert.IsTrue(gameState.HasReceivedDaveInitialMessage);
        }
    }
}
