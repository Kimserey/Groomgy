module rating {
    class UserSettingsController {
        email: string;
        emailNotificationEnabled: boolean;
        constructor(private $http: ng.IHttpService, $state: ng.ui.IStateService, $scope: ng.IScope) {
            this.email = $state.params["email"];

            this.$http.get<any>("app/Home/GetEmailNotificationEnabled", {
                params: {
                    email: this.email
                }
            }).success(res => {
                    this.emailNotificationEnabled = res == "True";
                });
        }

        updateEmailNotifications(isEnabled: boolean) {
            this.$http.post("app/Home/EnableEmailNotification", {
                email: this.email,
                isEnabled: isEnabled
            }).success(() => {
                    toastr.success("Settings updated!", null, toastrOptions);
                }).error(() => {
                    toastr.error("Failed. Please contact support.", null, toastrOptions);
                });
        }
    }

    app.controller("UserSettingsController", ["$http", "$state", "$scope", UserSettingsController]);
} 