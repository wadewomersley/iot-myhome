(function () {
    var weatherSample = {
        "Temperature": 12.06,
        "TemperatureMin": 12.06,
        "TemperatureMax": 12.63,
        "PressureSea": 1032.17,
        "PressureGround": 1023.13,
        "Humidity": 75,
        "WeatherTitle": "Clouds",
        "WeatherDescription": "broken clouds",
        "CloudCover": 56,
        "WindSpeed": 2.07,
        "WindDirection": 32.5002,
        "DateTime": "2018-06-08T22:00:00+01:00"
    };

    var userPosition = {};
    var serverSettings = {};
    var latestWeather = {};

    var windDirections = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N"];

    axios.defaults.baseURL = 'api/';

    function sendChange(path, data) {
        return axios.put(path, data).then(function (response) {
            return response.data;
        }).catch(function (error) {
            return Promise.reject(error);
        });
    }

    function getData(path) {
        return axios.get(path).then(function (response) {
            return response.data;
        }).catch(function (error) {
            return Promise.reject(error);
        });
    }

    /**
     * @param {jQuery} $container
     * @param {weatherSample} weatherInfo
     */
    function createWeatherBox($container, weatherInfo) {
        $container.empty();

        var dateTime = moment(weatherInfo.DateTime).local();
        var times = SunCalc.getTimes(dateTime.toDate(), userPosition.latitude, 0);
        var dayOrNight = dateTime > times.sunrise && dateTime < times.sunset ? 'day' : 'night';
        
        $('<time datetime="' + weatherInfo.DateTime + '">' + dateTime.format(serverSettings.ForecastDateTimeFormat) + '</time>')
            .appendTo($container);

        var desc = weatherInfo.WeatherDescription;
        var icon = null;
        if (desc.indexOf('fog') > -1) {
            icon = 'fog';
        } else if (desc.indexOf('cloud') > -1) {
            icon = dayOrNight + '-cloudy';
        } else if (desc.indexOf('thunder') > -1) {
            icon = dayOrNight + '-thunderstorm';
        } else if (desc.indexOf('rain') > -1) {
            icon = dayOrNight + '-rain';
        } else if (desc.indexOf('snow') > -1) {
            icon = dayOrNight + '-snow';
        } else if (desc.indexOf('volcan') > -1) {
            icon = 'volcano';
        } else if (desc.indexOf('tornado') > -1) {
            icon = 'tornado';
        } else if (desc.indexOf('sand') > -1) {
            icon = 'sandstorm';
        } else if (desc.indexOf('fog') > -1) {
            icon = dayOrNight + '-fog';
        } else if (desc.indexOf('clear') > -1) {
            icon = dayOrNight === 'day' ? 'day-sunny' : 'night-clear';
        }

        $('<div class="primary"><i class="wi wi-' + icon + '"/></div>')
            .appendTo($container);

        $('<div class="temperature"><div class="low">' + Math.round(weatherInfo.TemperatureMin) + '&deg;<small><sup>C</sup></small></div><div class="high">' + Math.round(weatherInfo.TemperatureMax) + '&deg;<small><sup>C</sup></small></div></div>')
            .appendTo($container);

        var windDirection = weatherInfo.WindDirection % 360;
        windDirection = Math.round(windDirection / 22.5, 0);
        var compassDir = windDirections[windDirection];
        $('<div class="wind"><i class="wi wi-wind wi-from-' + compassDir.toLowerCase() + '"/><span class="direction">' + Math.round(weatherInfo.WindDirection) + '&deg;</span><span class="knots">' + Math.round(weatherInfo.WindSpeed) + 'kts</span></div>')
            .appendTo($container);

        $('<div class="pressure">' + Math.round(weatherInfo.PressureGround, 0) + 'hPa</div>')
            .appendTo($container);

        $('<div class="humidity">' + weatherInfo.Humidity + ' <i class="wi wi-humidity"/></div>')
            .appendTo($container);
    }

    function updateWeather() {
        getData('latest').then(function (weather) {
            latestWeather = weather;

            var keys = Object.keys(latestWeather.Items);

            createWeatherBox($('#forecastAhead0'), latestWeather.Items[keys[0]]);
            createWeatherBox($('#forecastAhead1'), latestWeather.Items[keys[1]]);
            createWeatherBox($('#forecastAhead2'), latestWeather.Items[keys[2]]);
            createWeatherBox($('#forecastAhead3'), latestWeather.Items[keys[3]]);
        });
    }

    function positionKnown(position) {
        userPosition = position.coords;

        getData('settings').then(function (settings) {
            serverSettings = settings;

            updateWeather();
            window.setInterval(updateWeather, 60000);
        }).catch(function (error) {
            return;
        });
    }

    $(function () {
        navigator.geolocation.getCurrentPosition(positionKnown);
    });
})();