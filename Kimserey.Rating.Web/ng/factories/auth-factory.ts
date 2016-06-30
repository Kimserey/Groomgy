module rating {
    export interface IAuthService {
        login(email: string, password: string): ng.IPromise<any>;
        logout(): ng.IPromise<any>;
        isAuth(): boolean;
        getToken(): string;
        register(data: Registration): ng.IPromise<any>;
        setToken(token: string);
    }

    export class AuthorizationData {
        token: string;
    }

    app.factory("authService", ["$http", "$q", "localStorageService", function ($http: ng.IHttpService,
        $q: ng.IQService,
        localStorageService: ng.local.storage.ILocalStorageService<AuthorizationData>) {

        var login = function (email: string, password: string) {
            var defer = $q.defer();

            var data = "grant_type=password&username=" + email + "&password=" + password;
            $http.post<any>("/token", data, {
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).success(res => {
                    setToken(res.access_token);
                    defer.resolve(res);
                })
                .error(err => { defer.reject(err); });

            return defer.promise;
        }

        var logout = function () {
            var defer = $q.defer();
            $http.post("app/Account/LogOff", null)
                .success(() => {
                    defer.resolve();
                }).error(() =>
                    defer.reject())
                .finally(() => {
                    localStorageService.remove("AuthorizationData");
                });

            return defer.promise;
        }

        var isAuth = function () {
            return localStorageService.get("AuthorizationData") != null;
        }

        var getToken = function () {
            return localStorageService.get("AuthorizationData").token;
        }

        var register = function (data: Registration) {
            var defer = $q.defer();

            $http.post("app/Account/Register", data)
                .success(res => defer.resolve(res));

            return defer.promise;
        }

        var setToken = function (token: string) {
            localStorageService.set("AuthorizationData", <AuthorizationData> {
                token: token
            });
        }

        return <IAuthService> {
            login: login,
            logout: logout,
            isAuth: isAuth,
            register: register,
            getToken: getToken,
            setToken: setToken
        }
    }]);
} 