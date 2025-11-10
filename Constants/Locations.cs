using GTA.Math;

namespace LawAndDuty.Constants
{
    /// <summary>
    /// Defines important location coordinates in the game world.
    /// </summary>
    public static class Locations
    {
        /// <summary>
        /// Michael's poolside location where the initial Dave Norton message is triggered.
        /// TODO: Define the exact Vector3 coordinates for Michael's poolside area.
        /// </summary>
        public static readonly Vector3 MichaelPoolside = Vector3.Zero;

        /// <summary>
        /// Proximity threshold in meters for location-based triggers.
        /// </summary>
        public const float ProximityThreshold = 5f;
    }
}
