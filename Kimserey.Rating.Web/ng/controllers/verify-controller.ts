module rating {
    class VerifyController {
        constructor($http: ng.IHttpService, $state: ng.ui.IStateService, $scope: ng.IScope) {
            var email = $state.params["email"];
            var token = $state.params["token"];

            if (email && token) {
                $http.post<any>("app/Account/ConfirmEmail", {
                    email: email,
                    token: token
                }).then(resp => {
                    $scope["$dismiss"]();
                    if (resp.data.success) {
                        toastr.success("Email verification succeeded. You can now log in!", null, toastrOptions);
                    } else {
                        toastr.error("Email verification failed. Please try again...", null, toastrOptions);
                    }
                });
            }
        }
    }

    app.controller("VerifyController", ["$http", "$state", "$scope", VerifyController]);
} 