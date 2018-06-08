(function () {
    var playlistRowTemplate = '<div class="row"><div class="col-sm-9 fileContainer"></div><div class="col-sm-offset-0 col-sm-3 text-right optionsContainer"></div></div>';
    var serverSettings = {};
    var $playlist;

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

    function playLink(e) {
        e.preventDefault();

        sendChange('play', { Term: decodeURIComponent(this.title) });
    }

    function changeStartupFile(e) {
        e.preventDefault();

        var inputs = $('input[type="checkbox"].startupFile');
        var displayName = this.value;

        inputs.not(this).attr('checked', false);

        sendChange('startupFile', { term: this.checked ? displayName : null });
    }

    function addPlaylistItem(file) {
        var $row = $(playlistRowTemplate);
        var $fileContainer = $row.find('.fileContainer');
        var $optionsContainer = $row.find('.optionsContainer');

        var $link = $('<a/>');
        var $input = $('<input type="checkbox" class="startupFile" />');
        var $label = $('<label class="float-right"></label>');

        $input.val(file.DisplayName);

        $link.attr('href', file.FileName);
        $link.attr('title', file.DisplayName);
        $link.text(file.SearchTerms.join(' / '));

        $input.attr('checked', file.FileName === serverSettings.StartupFilename);

        $input.appendTo($label);
        $link.appendTo($fileContainer);
        $label.appendTo($optionsContainer);
        $row.appendTo($playlist);

        $link.on('click', playLink);
        $input.on('change', changeStartupFile);
    }

    $(document).ready(function () {
        $playlist = $('#playlist');
        var $volume = $('#volume');

        getData('settings').then(function (settings) {
            serverSettings = settings;

            if (settings.Volume !== null) {
                $volume.val(settings.Volume);
            }

            getData('playlist').then(function (playlist) {
                var files = playlist.Files;
                files.forEach(addPlaylistItem);

            });
        }).catch(function (error) {
            return;
        });

        $volume.on('change', function (e) {
            sendChange('volume', { Volume: parseInt($(this).val()) });
        });
    });
})();