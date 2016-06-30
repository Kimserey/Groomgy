module rating {
    class AccountController {
        registration: Registration;
        registrationErrors: string[];
        verificationEmailSent: string;
        canResendConfirmation: boolean;

        loginCredentials: LoginCredentials;
        loginErrors: string[] = [];

        isLoginBusy: boolean;
        isRegisterBusy: boolean;

        constructor(private authService: IAuthService,
            private $window: ng.IWindowService,
            private $http: ng.IHttpService,
            private $state: ng.ui.IStateService) {
        }

        register() {
            this.isRegisterBusy = true;
            this.authService.register(this.registration)
                .then(data => {
                    this.registrationErrors = [];
                    if (!data.success) {
                        this.registrationErrors = data.errors;
                        this.canResendConfirmation = data.canResendConfirmation;
                    } else {
                        this.showVerificationEmailSentAlert();
                    }
                }).finally(() => {
                    this.isRegisterBusy = false;
                });
        }

        login() {
            this.loginErrors = [];

            if (!this.loginCredentials.Email || !this.loginCredentials.Password) {
                this.loginErrors.push("The email or password is incorrect.");
                return;
            }

            this.isLoginBusy = true;
            this.authService
                .login(this.loginCredentials.Email, this.loginCredentials.Password)
                .then(() => {
                    this.loginErrors = [];
                    this.$window.location.href = this.$state.href("home", {}, { absolute: true }); 
                }, err => {
                    this.loginErrors = [];
                    this.loginErrors.push("The email or password is incorrect.");
                }).finally(() => {
                    this.isLoginBusy = false;
                });
        }

        sendConfirmationEmail(email: string) {
            this.$http.post<any>("app/Account/SendConfirmEmail", {
                email: this.registration.Email
            }).success(resp => {
                    if (resp.success) {
                        this.registrationErrors = null;
                        this.showVerificationEmailSentAlert();
                    }
                });
        }

        showVerificationEmailSentAlert() {
            this.verificationEmailSent = "A verification email has been sent to your email address.";
        }
    }

    app.controller("AccountController", ["authService", "$window", "$http", "$state", AccountController]);
} 