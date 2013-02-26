using System;

namespace Vroemmm.Logic
{
    public interface IDistanceCalculator
    {
        /// <summary>
        /// Calculates the distance in meters.
        /// </summary>
        /// <param name="origin">street numer city.</param>
        /// <param name="destination">street numer city.</param>
        int CalculateDistance(string origin, string destination);
    }
}
