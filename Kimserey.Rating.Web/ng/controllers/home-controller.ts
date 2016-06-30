module rating {
    class HomeController {
        isAuth: boolean;
        name: string;
        photoUrl: string;
        userId: string;
        hasNewMessages: boolean;

        showNameInput: boolean;
        showBirthdayInput: boolean;
        showGenderInput: boolean;

        constructor(private $http: ng.IHttpService,
            private authService: IAuthService,
            private $window: ng.IWindowService,
            private $state: ng.ui.IStateService,
            private $modal: ng.ui.bootstrap.IModalService,
            profilevalidator: IProfileValidatorService,
            $scope: ng.IScope) {
            if (authService.isAuth()) {
                this.isAuth = true;
                this.$http.get<ProfileSummary>("app/Profile/ProfileSummaryData")
                    .success(data => {
                    this.name = data.User.Name;
                    this.photoUrl = data.User.ProfilePhoto.Small;
                    this.userId = data.User.UserId;
                    this.hasNewMessages = data.HasNewMessages;

                    var validation = profilevalidator.validate(data.User.Name, data.User.Birthday, data.User.Gender, data.User.Interest);
                    if (!validation.IsNameValid
                        || !validation.IsBirthdayValid
                        || !validation.IsGenderValid
                        || !validation.IsInterestValid) {
                        this.$modal.open({
                            backdrop: "static",
                            templateUrl: "app/Home/Welcome",
                            controller: "WelcomeController as welcomeCtrl",
                            resolve: {
                                userId: () => data.User.UserId
                            }
                        });
                    }
                }).error(() => {
                    this.logOut();
                });
            }

            $scope.$on("profilePhotoChanged",(e: any, userId: string, photo: any) => {
                if (userId == this.userId) {
                    this.photoUrl = photo.Small;
                    $scope.$apply();
                }
            });

            $scope.$on("profileUpdated",(e: any, userId: string, name: string) => {
                if (userId == this.userId) {
                    this.name = name;
                    $scope.$apply();
                }
            });

            $scope.$on("hasNewMessages",(e: any, userId: string) => {
                if (this.userId == userId) {
                    this.hasNewMessages = true;
                    $scope.$apply();
                }
            });

            $scope.$on("hasNoMoreNewMessages",(e: any) => {
                this.hasNewMessages = false;
            });
        }


        logOut() {
            this.authService.logout()
                .then(() => {
                this.$window.location.href = this.$state.href("home", {}, { absolute: true });
            });
        }
    }

    app.controller("HomeController", ["$http", "authService", "$window", "$state", "$modal", "profilevalidator", "$scope", HomeController]);
}