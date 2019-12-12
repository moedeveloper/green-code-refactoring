using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace greenbyte
{
    static class TurbineCurtailmentTypes
    {
        static readonly Dictionary<TurbineCurtailment, double> TurbineTypes = new Dictionary<TurbineCurtailment, double>
        {
            { TurbineCurtailment.Default, 0.0},
            { TurbineCurtailment.Noise, 0.25},
            { TurbineCurtailment.Bats, 0.15 },
            { TurbineCurtailment.Shadow, 0.1 },
            { TurbineCurtailment.BoatAction, 0.5},
            { TurbineCurtailment.Technical, 0.5},
            { TurbineCurtailment.Grid, 0.5}
        };

        public static double GetTurbineTypes(TurbineCurtailment turbineCurtailment)
        {
            return TurbineTypes[turbineCurtailment];
        }
    }
    /// <summary>
    /// Implements a turbine curtailment provider that has standard levels for Vestas turbines.
    /// TODO: We think there are a few bugs in the code below, since the calculations look messed up every now and then.
    ///       There are also a number of things that are yet to be implemented.
    /// TODO: The code could likely be more readable and maintainable somehow.
    /// </summary>
    public class VestasCurtailmentProvider : ITurbineCurtailmentProvider
    {
        //* **** ***** *//
        // I think Dictionary isn't the best practice for memory management, but for complexity perspective it costs o(1) to reach the key, value and that's ok
        // I don't prefer to use Tuple .. I prefer to use instance of data-structure like sorted dict of objects
        // Dictionary contains the following keys [Default, Noise, Bats, Shadow, BoatAction, Technical, Grid] 
        // And each key refers to sorted dict of object [CreatedAt:DateTime, Level:double] .. 
        // TODO: Replacing the current dict with dict of instances of objects [Class Measurements] 
        //       OR just these keys could refer to JSON objects with many measurements...
        //* **** ***** *//

        static Dictionary<TurbineCurtailment, SortedDictionary<long, double>> _customLevels;
        public VestasCurtailmentProvider()
        {
            _customLevels = new Dictionary<TurbineCurtailment, SortedDictionary<long, double>>()
            {
                { TurbineCurtailment.Bats, new SortedDictionary<long, double>() },
                { TurbineCurtailment.BoatAction, new SortedDictionary<long, double>() },
                { TurbineCurtailment.Default, new SortedDictionary<long, double>() },
                { TurbineCurtailment.Grid, new SortedDictionary<long, double>() },
                { TurbineCurtailment.Noise, new SortedDictionary<long, double>() },
                { TurbineCurtailment.Shadow, new SortedDictionary<long, double>() },
                { TurbineCurtailment.Technical, new SortedDictionary<long, double>() },
            };
        }
        public double GetStandardLevel(TurbineCurtailment curtailment)
        {
            //TODO: please refactor these ugly if statements somehow...
            // PLEASE NOTE: THESE ARE THE ACTUAL CURTAILMENT LEVELS THAT SHOULD APPLY FOR VESTAS TURBINES, WE JUST GOT THEM FROM THE OPERATOR!
            // Default = 0%
            // Noise curtailment = 25%
            // Bats curtailment = 15%
            // Shadow curtailment = 10%
            // Boat action curtailment = 5%
            // Technical curtailment = 5%
            // Grid curtailment = 5%

            //* **** ***** *//
            // There are many ways to refactor if statments:
            // 1) polymorphism [with business logic]
            // 2) Dictionary 
            // 3) Switch, it is a simple solution if there is no business logic and it just returns values .. we can use it in this case. [optimized]
            // TODO: Returning fixed values isn't the best practice from maintainbility and SE design perspective, since those vaules could be changed .. 
            //       it is a good practice to save those values in config file at least or in DB with a specific class to manage
            //* **** ***** *//
            return TurbineCurtailmentTypes.GetTurbineTypes(curtailment);
        }

        /// <remarks>
        /// When set, the method saves the curtailment level information for the current timestamp in UTC for later retrieval.
        /// 
        /// Each instance of this object supports a different set of custom levels since we run one thread per customer.
        /// For now we just save it in memory but will use a database later.
        /// </remarks>

        //* **** ***** *//
        // The business logic isn't completely clear, I will suppose a scenario and will continue working on it:
        // Turbine is an IOT device which sends and returns data to-from cloud based on timestamp ..
        // Each turbine saves the recent values of TurbineCurtailment{Default, Noise, Bats, Shadow, BoatAction, Technical, Grid} in cloud and it just returns the recent values based on DateTime
        // TODO: Adding log is a good practice to track all the data communications
        //* **** ***** *//
        public void SetCustomLevel(TurbineCurtailment curtailment, double level)
        {
            //*** ***//
            // The used data structure for this case is Dictionary where each key refers to sorted dictionary of datetime and level pairs
            // we used Ticks to get an accurate Datetime and that will help also in get the level later.
            //*** ***//
            //TODO: support saving multiple custom levels for different combinations of TurbineCurtailment/DateTime
            //*** ***//
            // There is no redundant in this data structure since it is sorted dictionary and keys should be uniqued!
            //*** ***//
            //TODO: make sure we never save duplicates, in case of e.g. clock resets, DST etc - overwrite old values if this 
            try
            {
                // This case should not be happened except if there is Reset or DST so the Datetime could be repeated again
                // As usual case add a new item to sorted dictioanry 
                if (!_customLevels[curtailment].ContainsKey(DateTime.Now.Ticks))
                    _customLevels[curtailment].Add(DateTime.Now.Ticks, level);
                // if there is any problem and date is existed before just updating the value
                else
                    _customLevels[curtailment][DateTime.Now.Ticks] = level;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public double GetLevel(TurbineCurtailment curtailment, DateTime timestamp)
        {
            try
            {
                if(_customLevels[curtailment].ContainsKey(timestamp.Ticks)) return _customLevels[curtailment][timestamp.Ticks]; else return 0;
            }
            catch (Exception e)
            {
                // TODO: Throw a correct exception message 
                throw e;
            }
           
        }

        public double GetCurrentLevel(TurbineCurtailment curtailment)
        {
            try
            {
                return _customLevels[curtailment].Values.Last();
            }
            catch (Exception e)
            {
                // TODO: Throw a correct exception message 
                throw e;
            }
        }
    }
}
