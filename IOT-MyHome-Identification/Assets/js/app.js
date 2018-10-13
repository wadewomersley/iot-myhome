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
        });
        nextImage = window.setTimeout(getImage, 250);
    };

    getImage();
})();