using System;


namespace SmartHomeManagementEB
{
    // The value type stored in the dictionary, keyed by DeviceId.
    public class SmartDevice
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }   // e.g. Light, Thermostat, Lock, Camera
        public string Status { get; set; } // e.g. On, Off

        public SmartDevice(string deviceId, string name, string type, string status)
        {
            DeviceId = deviceId;
            Name = name;
            Type = type;
            Status = status;
        }
    }
}