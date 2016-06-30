module rating {
    class Response {
        success: boolean;
        error: string;
        token: AccessToken;
    }
    class AccessToken {
        userName: string;
        access_token: string;
        token_type: string;
        expires_in: string;
    }

    class ExternalLoginController {
        constructor(private $state: ng.ui.IStateService, private $modalInstance: ng.ui.bootstrap.IModalServiceInstance,
            private $http: ng.IHttpService, private $window: ng.IWindowService, private authService: rating.IAuthService) {
            $http.get<Response>("app/Account/ExternalLoginGetToken", {
                params: {
                    provider: $state.params["provider"],
                    accessToken: $state.params["accessToken"]
                }
            }).success(res => {
                if (res.success) {
                    this.goToHome(res.token.access_token);
                } else {
                    this.goBack(res.error);
                }
            });
        }

        goBack(error: string) {
            this.$modalInstance.dismiss();
            this.$state.go("account");
            toastr.error(error, null, toastrOptions);
        }

        goToHome(token: string) {
            this.authService.setToken(token);
            this.$window.location.href = this.$state.href("home", {}, { absolute: true });
        }
    }

    app.controller("ExternalLoginController", ["$state", "$modalInstance", "$http", "$window", "authService", ExternalLoginController]);
}