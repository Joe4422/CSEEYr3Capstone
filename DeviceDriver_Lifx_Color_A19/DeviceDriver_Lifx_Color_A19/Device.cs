﻿using DeviceDriverPluginSystem.Generics;
using Newtonsoft.Json.Linq;
using System;
using DeviceDriverPluginSystem;

namespace DeviceDriver_Lifx_Color_A19
{
    public class Device : GenericDevice
    {
        /// <summary>
        ///     The Lifx API ID of the device.
        /// </summary>
        private readonly string LifxID;

        /// <summary>
        ///     Label of the device as provided by Lifx JSON.
        ///     Cannot be changed by public API so invoking set method does nothing.
        /// </summary>
        public new string Label
        {
            get
            {
                return DeviceDriver.GetJsonByID(LifxID)["label"].ToString();
            }
            set
            {
                return;
            }
        }

        /// <summary>
        ///     Creates a new Device object with unique ID provided by the JSON data from Lifx.
        /// </summary>
        /// <param name="LifxID">
        ///     The unique Lifx API of the device.
        /// </param>
        internal Device(string LifxID)
        {
            this.LifxID = LifxID;
            PopulateDeviceVariables();
        }

        private void PopulateDeviceVariables()
        {
            AddDeviceVariable("Powered", IsBulbOn, SetBulbOn);
            AddDeviceVariable("Hue", GetBulbHue, SetBulbHue, new Range<int>(0, 360));
            AddDeviceVariable("Saturation", GetBulbSaturation, SetBulbSaturation, new Range<double>(0d, 1d));
            AddDeviceVariable("Brightness", GetBulbBrightness, SetBulbBrightness, new Range<double>(0d, 1d));
            AddDeviceVariable("Warmth", GetBulbWarmth, SetBulbWarmth, new Range<int>(1500, 9000));
        }

        public void AddDeviceVariable<VariableType>(string label, Func<VariableType> getter, Action<VariableType> setter) where VariableType : IComparable
        {
            DeviceVariables.Add(new DeviceVariable<VariableType>(getter, setter, label));
        }
        public void AddDeviceVariable<VariableType>(string label, Func<VariableType> getter, Action<VariableType> setter, Range<VariableType> valueRange) where VariableType : IComparable
        {
            DeviceVariables.Add(new DeviceVariableRangeChecked<VariableType>(getter, setter, label, valueRange));
        }

        private bool IsBulbOn() => 
            DeviceDriver.GetJsonByID(LifxID)["power"].ToString() == "on";
        private int GetBulbHue() => 
            DeviceDriver.GetJsonByID(LifxID)["color"]["hue"].Value<int>();
        private double GetBulbSaturation() => 
            DeviceDriver.GetJsonByID(LifxID)["color"]["saturation"].Value<double>();
        private double GetBulbBrightness() =>
            DeviceDriver.GetJsonByID(LifxID)["brightness"].Value<double>();
        private int GetBulbWarmth() =>
            DeviceDriver.GetJsonByID(LifxID)["color"]["kelvin"].Value<int>();

        private void SetBulbOn(bool newState) => 
            DeviceDriver.SetDeviceProperty(LifxID, "power", newState ? "on" : "off");
        private void SetBulbHue(int newHue) =>
            DeviceDriver.SetDeviceProperty(LifxID, "color", "hue:" + newHue.ToString());
        private void SetBulbSaturation(double newSaturation) =>
            DeviceDriver.SetDeviceProperty(LifxID, "color", "saturation:" + newSaturation.ToString());
        private void SetBulbBrightness(double newBrightness) =>
            DeviceDriver.SetDeviceProperty(LifxID, "brightness", newBrightness.ToString());
        private void SetBulbWarmth(int newWarmth) =>
            DeviceDriver.SetDeviceProperty(LifxID, "color", "warmth:" + newWarmth.ToString());
    }
}
