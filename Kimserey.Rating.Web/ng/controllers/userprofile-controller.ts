module rating {
    class UserProfileController {
        userId: string;
        profile: ProfileView;
        showHighlight: boolean;
        albumPhotoRows: Photo[][];

        isSendMessageBusy: boolean;
        isPhotoUploadBusy: boolean;

        constructor(private $state: ng.ui.IStateService,
            private $http: ng.IHttpService,
            private $scope: ng.IScope,
            private $modal: ng.ui.bootstrap.IModalService,
            private conversation: IConversationService) {

            this.userId = $state.params["id"];

            this.$http.get<ProfileView>("app/Profile/ProfileData/" + this.userId)
                .success(profile => {
                    this.profile = profile;
                    this.profile.Birthday = moment(profile.Birthday).toDate();
                    this.profile.Age = moment().diff(moment(this.profile.Birthday), 'years') + " years old";

                    profile.AlbumPhotos.forEach(photo => {
                        this.addNextPhotoToGallery(photo);
                    });

                    this.$scope.$watch(() => this.profile.Birthday, (newVal: any) => {
                        this.profile.Age = moment().diff(moment(newVal), 'years') + " years old";
                    });

                    this.showHighlight = this.$state.params["highlight"] == "true" && this.profile.CanEdit;
                });

            $scope.$on("profilePhotoChanged", (e: any, userId: string, photo: any) => {
                if (userId == this.userId) {
                    this.profile.ProfilePhoto = photo;
                    $scope.$apply();
                }
            });

            $scope.$on("galleryPhotoUploaded", (e: any, userId: string, photo: any) => {
                if (userId == this.userId) {
                    this.profile.AlbumPhotos.push(photo);
                    this.addNextPhotoToGallery(photo);
                    $scope.$apply();
                }
            });
        }

        saveProfile() {
            this.$http.post("app/Profile/UpdateProfile", this.profile)
                .success(() => {
                    toastr.success("Saved!", null, toastrOptions);
                    this.stopShowWelcome();
                })
                .error(err => {
                    toastr.error("Please try again.", null, toastrOptions);
                });
        }

        startConversation() {
            this.isSendMessageBusy = true;
            this.conversation.startConversation(this.userId)
                .finally(() => { this.isSendMessageBusy = false; });
        }

        uploadPhoto(files: any, action: string) {
            this.isPhotoUploadBusy = true;

            if (files.length < 1
                || (<string>files[0].type).indexOf("image/") < 0) {
                toastr.error("Please check the format of the file.", null, toastrOptions);
                this.isPhotoUploadBusy = false;
                return;
            }

            var formData = new FormData();
            formData.append('file', files[0]);

            this.$http.post<any>("app/Profile/" + action, formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })
                .success(res => {
                    if (res.success) {
                        toastr.success("Success!", null, toastrOptions);
                        this.stopShowWelcome();

                    } else {
                        toastr.error(res.errorMessage, null, toastrOptions);
                    }
                }).error(() => {
                    toastr.error("Please try again.", null, toastrOptions);
                }).finally(() => {
                    this.isPhotoUploadBusy = false;
                });
        }

        addNextPhotoToGallery(photo: Photo) {
            if (!this.albumPhotoRows || this.albumPhotoRows.length == 0) {
                this.albumPhotoRows = [[photo]];
                return;
            }
            if (this.albumPhotoRows[this.albumPhotoRows.length - 1].length < 4) {
                this.albumPhotoRows[this.albumPhotoRows.length - 1].push(photo);
                return;
            }
            this.albumPhotoRows[this.albumPhotoRows.length].push(photo);
        }

        stopShowWelcome() {
            if (this.profile.Gender == 0) {
                return;
            }
            this.$http.post("app/Profile/StopShowingWelcome", null);
            this.showHighlight = false;
        }

        showDeleteGalleryPhoto() {
            this.$modal.open({
                resolve: {
                    pictures: () => this.profile.AlbumPhotos
                },
                controller: ["pictures", "$scope", function (pictures: Photo[], $scope: ng.IScope) {
                    var photos: any[] = pictures.map(p => <any>{
                        url: p.Small,
                        selected: false,
                        relative: p.Relative
                    });

                    $scope["pictures"] = photos;

                    $scope["delete"] = () => {
                        if (confirm("Are you sure you want to delete the selected items?")) {
                            $scope["$close"](photos.filter(p => p.selected).map(p => p.relative));
                        }
                    };
                }],
                templateUrl: "app/Profile/DeleteFromGallery"
            }).result
                .then(res => {
                    this.$http.post("app/Profile/DeletePhotosFromAlbum", {
                        userId: this.userId,
                        photoUrls: res
                    }).success(() => {
                            this.albumPhotoRows = null;
                            this.profile.AlbumPhotos = this.profile
                                .AlbumPhotos
                                .filter(p => !res.some(url => url == p.Relative));
                            this.profile.AlbumPhotos
                                .forEach(p => {
                                    this.addNextPhotoToGallery(p);
                                });
                        });
                });
        }
    }

    app.controller("UserProfileController", ["$state", "$http", "$scope", "$modal", "conversation", UserProfileController]);
}