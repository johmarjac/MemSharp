using System;
using System.Collections.Generic;
using System.Text;

namespace Ether.Network.Client
{
    /// <summary>
    /// Options defining how a <see cref="NetClient"/> will handle a failed connection attempt
    /// </summary>
    public enum NetClientRetryOptions
    {
        /// <summary>
        /// The client will only try to connect one time
        /// </summary>
        OneTime = 0,

        /// <summary>
        /// The client will try to connect a specific amount of times
        /// </summary>
        Limited = 1,

        /// <summary>
        /// The client will try infinitely to connect to the server
        /// </summary>
        Infinite = 2
    }
}
