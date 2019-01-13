﻿using DeviceDriverPluginSystem;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IoT_Hub
{
    public class OutputJsonProducer
    {
        private readonly List<Driver> drivers;
        private readonly string hubName;

        public OutputJsonProducer(List<Driver> drivers, string hubName)
        {
            this.drivers = drivers;
            this.hubName = hubName;
        }

        public JObject GetHubInformation()
        {
            Utility.WriteTimeStamp("Building hub information as JSON string", typeof(OutputJsonProducer));
            JObject outObject = new JObject
            {
                { "label", hubName },
                { "devices", GetAllDevices() }
            };
            return outObject;
        }
        private JArray GetAllDevices()
        {
            Utility.WriteTimeStamp("Building list of all devices in JSON", typeof(OutputJsonProducer));
            Utility.WriteTimeStamp($"Found {drivers.Count} drivers", typeof(OutputJsonProducer));
            JArray devicesArray = new JArray();
            foreach (Driver d in drivers)
            {
                List<DriverDevice> ddList = d.Devices;
                Utility.WriteTimeStamp($"Found {ddList.Count} devices in {d.Name}", typeof(OutputJsonProducer));
                foreach (DriverDevice dd in ddList)
                    devicesArray.Add(GetDevice(d, dd));
            }
            return devicesArray;
        }
        public JObject GetDevice(Driver driver, DriverDevice device)
        {
            JObject outputObject = new JObject()
            {
                { "label", device.GetDynamicDevice().Label },
                { "name", device.GetDynamicDevice().Name },
                { "manufacturer", device.GetDynamicDevice().Manufacturer },
                { "driverId", driver.Id },
                { "deviceId", device.GetDynamicDevice().Id },
                { "attributes", new JArray(device.DeviceBase.DeviceAttributes.Select(x => x.Label)) }
            };
            Utility.WriteTimeStamp($"Found {device.DeviceBase.DeviceAttributes.Count} device attributes for {device.GetDynamicDevice().Name} in {driver.Name}", typeof(OutputJsonProducer));
            return outputObject;
        }
        private JArray GetDeviceAttributes(DriverDevice device)
        {
            return new JArray(device.DeviceBase.DeviceAttributes.Select(x => GetDeviceAttribute(x)));
        }
        public JObject GetDeviceAttribute(DeviceAttribute attribute)
        {
            dynamic devVar = attribute;
            JObject jsonDevVar = new JObject
            {
                { "label", devVar.Label },
                { "type", attribute.AttributeType.FullName },
                { "value", devVar.Get().ToString() }
            };
            try
            {
                jsonDevVar.Add("minValue", devVar.attributeRange.minValue.ToString());
                jsonDevVar.Add("maxValue", devVar.attributeRange.maxValue.ToString());
            }
            catch (Exception) { }
            return jsonDevVar;
        }
    }
}
