module rating {
    class WelcomeController {
        userId: string;
        redirectingToProfile: boolean;
        profile: ProfileView;
        profilevalidation: ProfileValidation;
        isReady: boolean;

        name: string;
        birthday: Date;
        gender: number;
        interest: number;

        isBirthdayInputValid: boolean;

        constructor(private $http: ng.IHttpService,
            private $modalInstance: ng.ui.bootstrap.IModalServiceInstance,
            $scope: ng.IScope,
            profilevalidator: IProfileValidatorService,
            userId: string) {

            this.$http
                .get<ProfileView>("app/Profile/ProfileData/" + userId)
                .success(profile => {
                this.profile = profile;
                this.profile.Birthday = moment(this.profile.Birthday).toDate();
                this.profilevalidation = profilevalidator.validate(profile.Name, profile.Birthday, profile.Gender, profile.Interest);
                this.isReady = true;
            });

            $scope.$watch(() => this.birthday, val => {
                if (!this.profilevalidation) {
                    return;
                }
                this.isBirthdayInputValid = profilevalidator.validateBirthday(val);
            });
        }

        go() {
            if (!this.isReady) {
                return;
            }

            if (!this.profilevalidation.IsNameValid) {
                this.profile.Name = this.name;
            }

            if (!this.profilevalidation.IsBirthdayValid) {
                this.profile.Birthday = this.birthday;
            }

            if (!this.profilevalidation.IsGenderValid) {
                this.profile.Gender = this.gender;
            }

            if (!this.profilevalidation.IsInterestValid) {
                this.profile.Interest = this.interest;
            }

            this.$http.post("app/Profile/UpdateProfile", this.profile)
                .success(() => this.$modalInstance.close())
                .error(() => toastr.error("Oops we were unable to save your preferences... Please try again."));
        }
    }

    app.controller("WelcomeController", ["$http", "$modalInstance", "$scope", "profilevalidator", "userId", WelcomeController]);
}