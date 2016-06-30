module rating {
    class ForgotPasswordController {
        email: string;
        successMessage: string;
        failMessage: string;
        isSending: boolean;

        constructor(private $http: ng.IHttpService) {
        }

        sendResetPassword() {
            this.isSending = true;
            this.$http.post<any>("app/Account/SendResetPasswordEmail", {
                email: this.email
            }).success(resp => {
                    if (resp.success) {
                        this.successMessage = "An email has been sent to you to reset your password.";
                        this.failMessage = null;
                    } else {
                        this.failMessage = "Sorry, something prevented us from sending the email. Please try again."
                        this.successMessage = null;
                    }
                }).error(() => {
                    this.failMessage = "Sorry, something prevented us from sending the email. Please try again."
                    this.successMessage = null;
                }).finally(() => {
                    this.isSending = false;
                });
        }
    }

    app.controller("ForgotPasswordController", ["$http", ForgotPasswordController]);
}