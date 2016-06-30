module rating {


    app.factory("authHttpInterceptor", ["$q", "$injector", "$location", "$window", "localStorageService",
        ($q: ng.IQService,
            $injector: any,
            $location: ng.ILocationService,
            $window: ng.IWindowService,
            localStorageService: ng.local.storage.ILocalStorageService<AuthorizationData>) => {
        return {
                request: function (config) {
                    config.headers = config.headers || {};

                    var authData = localStorageService.get('AuthorizationData');
                    if (authData) {
                        config.headers.Authorization = 'Bearer ' + authData.token;
                    }

                    return config;
                },
                responseError: (rejection) => {
                    if (rejection.status === 401) {
                        localStorageService.remove("AuthorizationData");
                        $window.location.href = (<ng.ui.IStateService>$injector.get('$state'))
                            .href("account", { returnUrl: $location.path() }, { absolute: true });;
                    }
                    return $q.reject(rejection);
                }
            }
    }]);
}