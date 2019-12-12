using System;
using System.Collections.Generic;
using System.Text;

namespace greenbyte
{
    /// <summary>
    /// Enum for different kinds of turbine curtailment. Curtailment occurs when the power plant
    /// is not allowed to output energy and can imply total shutdown or a reduced power output.
    /// </summary>

    public enum TurbineCurtailment
    {
        Default,
        Noise,
        Bats,
        Shadow,
        BoatAction,
        Technical,
        Grid
    }
    /// <summary>
    /// Interface for a type that provides turbine curtailment levels over time as well as standard levels.
    /// </summary>
    public interface ITurbineCurtailmentProvider
    {
        /// <summary>
        /// Get the standard curtailment level for the specified curtailment type.
        /// </summary>
        /// <param name="curtailment">The curtailment to get the standard level for.</param>
        double GetStandardLevel(TurbineCurtailment curtailment);

        /// <summary>
        /// Sets custom curtailment levels for the given curtailment type.
        /// </summary>
        /// <param name="curtailment">The curtailment type to set level for.</param>
        /// <param name="level">The curtailment level to set to.</param>
        void SetCustomLevel(TurbineCurtailment curtailment, double level);

        /// <summary>
        /// Gets the curtailment level that is active for a specific timestamp (in UTC). A custom level is seen as
        /// the active level for a period from its starting timestamp until a new custom level is set. If there
        /// is no custom curtailment level for the specified timestamp, the standard level is used.
        /// </summary>
        /// <param name="curtailment">The curtailment type to get the level of.</param>
        /// <param name="timestamp">The UTC timestamp to get the level of.</param>
        double GetLevel(TurbineCurtailment curtailment, DateTime timestamp);

        /// <summary>
        /// Gets the curtailment level that is active for the current point in time. A custom level is seen as the
        /// active level for a period from its starting timestamp until a new custom level is set. If there is
        /// no custom curtailment level currently active, the standard level is used.
        /// </summary>
        double GetCurrentLevel(TurbineCurtailment curtailment);
    }
}
