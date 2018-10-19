(function () {
    axios.defaults.baseURL = 'api/';

    function getData(path) {
        return axios.get(path).then(function (response) {
            return response.data;
        }).catch(function (error) {
            return Promise.reject(error);
        });
    }

    var nextImage;
    var getImage = function () {
        getData('lastImageCaptured').then(function (image) {
            $('#lastImage').attr('src', image);
            nextImage = window.setTimeout(getImage, 250);
        });
    };

    getImage();

    getData('settings').then(function (settings) {
        for (var i = 0; i < settings.People.length; i++) {
            var person = settings.People[i];
            $('<img/>')
                .attr('src', 'data:image/jpg;base64,' + person.Image)
                .appendTo($('#people'));
        }
    });
})();