using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherApp2.Models
{
    public class frames
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string text { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string icon { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? index { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int[] chartData { get; set; }
    }

    public class lametric
    {
        public frames[] frames { get; set; }
    }

    public class Weather
    {
        public string City { get; set; }

        private TemperatureRead _CurrentTemperature;
        public TemperatureRead CrntTemp
        {
            get
            {
                try
                {
                    return _CurrentTemperature;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            set
            {
                _CurrentTemperature = value; 
            }
        }


        private TemperatureRead[] _Readings;
        public TemperatureRead[] Readings
        {
            get
            {
                try
                {                
                    return _Readings;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            set
            {
                _Readings = value;
            }
        }


    }

    public class TemperatureRead
    {
        public DateTime ReadTime { get; set; }
        public double Temperature { get; set; }
        public string Details { get; set; }
        public string Icon { get; set; }
    }



}