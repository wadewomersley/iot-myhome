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
            nextImage = window.setTimeout(getImage, 200);
        });
    };

    getImage();

    getData('settings').then(function (settings) {
        var peopleContainer = $('#people');

        for (var i = 0; i < settings.People.length; i++) {
            var person = settings.People[i];

            var personContainer = $('<div class="person"/>');

            $('<img class="image"/>')
                .attr('src', 'data:image/png;base64,' + person.Image)
                .appendTo(personContainer);

            $('<div class="name"/>')
                .text(person.SpokeName || person.Name)
                .appendTo(personContainer);

            personContainer.appendTo(peopleContainer);
        }

        $('<div style="clear: both"/>')
            .appendTo(peopleContainer);
    });
})();