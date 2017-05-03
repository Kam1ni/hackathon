using System;

namespace Hackaton
{
    public static class Constants
    {
        /// <summary>
        /// The UUID for the Advertising service of a Bean-device.
        /// </summary>
        public static Guid BeanServiceAdvertisingUuid = new Guid("a495ff10-c5b1-4b44-b512-1370f02d74de");
        /// <summary>
        /// The UUID for the Scratch Data service of a Bean-device.
        /// </summary>
        public static Guid BeanServiceScratchDataUuid = new Guid("a495ff20-c5b1-4b44-b512-1370f02d74de");
        public static Guid BeanCharacteristicScratchDataAccelerometerUuid = new Guid("a495ff21-c5b1-4b44-b512-1370f02d74de");
    }
}
