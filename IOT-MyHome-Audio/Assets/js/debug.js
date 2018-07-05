// Used for local testing
if (location.search.indexOf('DEBUG') > -1) {
    $.ajax = function(args) {
        console.log('AJAX', args);
        return new Promise((resolve, reject) => {
            switch (args.type) {
                case 'get': 
                    if (args.url.indexOf('/settings') > -1) {
                        resolve([{
                            Volume: 75,
                            ApiKeySaved: false,
                            ApiKey: "TESTTESTTEST"
                        }]);
                    } else if (args.url.indexOf('/playlist')) {
                        resolve([{
                            Files: [
                                {
                                    FileName: "File One.mp3",
                                    DisplayName: "File One"
                                },
                                {
                                    FileName: "Another File.mp3",
                                    DisplayName: "Another File"
                                }
                            ]
                        }]);
                    }
                    break;
            }
        });
    };
}