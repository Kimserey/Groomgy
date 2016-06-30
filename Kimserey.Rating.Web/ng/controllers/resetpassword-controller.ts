module rating {
    class ResetPasswordController {
        token: string;
        email: string;
        newPassword: string;
        confirmPassword: string;
        failMessage: string;
        isSending: boolean;

        constructor(private $http: ng.IHttpService, private $scope: ng.IScope, $state: ng.ui.IStateService) {
            this.token = $state.params["token"];
            this.email = $state.params["email"];

            if (!this.token || !this.email) {
                $state.go("account");
            }
        }

        resetPassword() {
            this.isSending = true;
            this.$http.post<any>("app/Account/ResetPassword", new ResetPassword(this.email,
                this.newPassword,
                this.confirmPassword,
                this.token))
                .success(resp => {
                    if (resp.success) {
                        this.$scope["$close"]();
                        toastr.success("Password updated! You can now log in.", null, toastrOptions);
                    } else {
                        if (resp.errors && resp.errors.length > 0) {
                            this.failMessage = resp.errors[0];
                        } else {
                            this.failMessage = "Sorry, something went wrong. Please try again.";
                        }
                    }
                }).finally(() => {
                    this.isSending = false;
                });
        }
    }

    app.controller("ResetPasswordController", ["$http", "$scope", "$state", ResetPasswordController]);
} 