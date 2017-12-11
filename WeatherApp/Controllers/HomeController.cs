using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WeatherApp2.Models;

namespace WeatherApp2.Controllers
{
    public class HomeController : ApiController
    {


        public IHttpActionResult GetWeatherUpdates(string zipcode, string tempscale)
        {
            try
            {
                bool isTempScaleCelsius= true;
                if (!string.IsNullOrEmpty(tempscale) && tempscale == "Fahrenheit")
                {
                    isTempScaleCelsius = false;
                }

                Weather data = GetWeatherData(zipcode);


                if (string.IsNullOrEmpty(zipcode))
                {
                    zipcode = "10001";
                }
                frames currentTemp;
                if(isTempScaleCelsius)
                {
                    currentTemp = new frames() { chartData = null, index = null };
                    currentTemp.text = " "+ Math.Round(data.CrntTemp.Temperature - 273.15,1) + " Current Temp";
                    currentTemp.icon = "i7066";
                }
                else
                {
                    currentTemp = new frames() { chartData = null, index = null };
                    currentTemp.text = " " + Math.Round(1.8 * (data.CrntTemp.Temperature - 273), 1) + 32 + " Current Temp";
                    currentTemp.icon = "12465";
                    
                }
                string details = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase( data.CrntTemp.Details.ToLower());              
            
                int[] _chartdata = data.Readings.Select(x => Convert.ToInt32(x.Temperature)).ToArray();

                lametric lametric = new Models.lametric();
                lametric.frames = new frames[4];
                lametric.frames[0] = new frames() { icon = "i2272", text = "Weather in " + data.City };
                lametric.frames[1] = currentTemp;
                lametric.frames[2] = new frames() { icon = "i2272", text = " " + details, chartData = null, index = null };
                lametric.frames[3] = new frames() { index = 3, chartData = _chartdata };
                return Json(lametric);
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public static Weather GetWeatherData(string zipcode)
        {

            Weather forcastdata = GetForcastDataExternalService(zipcode);
            TemperatureRead current = GetCurrentDataExternalService(zipcode);

            Weather output = new Weather();
            output.CrntTemp = current;
            output.Readings = forcastdata.Readings;
            output.City = forcastdata.City;

            return output;
        }


        public static Weather GetForcastDataExternalService(string zipcode)
        {
            Weather weather = new Weather();
            string URLForcast = "http://api.openweathermap.org/data/2.5/forecast";
            string urlParameters = "?zip={zip}&APPID=62cbe2991520379376b00aecd963ea3a";

            urlParameters = urlParameters.Replace("{zip}", zipcode);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URLForcast);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var result = response.Content.ReadAsStringAsync().Result;
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(result);

                List<TemperatureRead> temperatureReadList = new List<TemperatureRead>();
                weather.City = obj["city"]["name"];
                foreach (dynamic reading in obj["list"])
                {
                    // First make a System.DateTime equivalent to the UNIX Epoch.
                    System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                    // Add the number of seconds in UNIX timestamp to be converted.
                    string utcunixdate = reading["dt"];
                    dateTime = dateTime.AddSeconds(double.Parse(utcunixdate));

                    TemperatureRead temperatureRead = new TemperatureRead();

                    temperatureRead.ReadTime = dateTime;
                    temperatureRead.Temperature = reading["main"]["temp"];
                    temperatureRead.Details = reading["weather"][0]["description"];
                    temperatureRead.Icon = reading["weather"][0]["icon"];
                    temperatureReadList.Add(temperatureRead);
                }
                weather.Readings = temperatureReadList.OrderBy(x => x.ReadTime).ToArray();

            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return weather;

        }


        public static TemperatureRead GetCurrentDataExternalService(string zipcode)
        {
            TemperatureRead temperatureRead = new TemperatureRead();
            string URLCurrentWeather = "http://api.openweathermap.org/data/2.5/weather";

            string urlParameters = "?zip={zip}&APPID=62cbe2991520379376b00aecd963ea3a";

            urlParameters = urlParameters.Replace("{zip}", zipcode);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URLCurrentWeather);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var result = response.Content.ReadAsStringAsync().Result;
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(result);


                // First make a System.DateTime equivalent to the UNIX Epoch.
                System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                // Add the number of seconds in UNIX timestamp to be converted.
                string utcunixdate = obj["dt"];
                dateTime = dateTime.AddSeconds(double.Parse(utcunixdate));

                temperatureRead.ReadTime = dateTime;
                temperatureRead.Temperature = obj["main"]["temp"];
                temperatureRead.Details = obj["weather"][0]["description"];
                temperatureRead.Icon = obj["weather"][0]["icon"];
                return temperatureRead;

            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return temperatureRead;

        }



    }
}
