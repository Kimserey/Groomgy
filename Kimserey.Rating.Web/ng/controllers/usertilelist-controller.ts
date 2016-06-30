module rating {
    enum Gender {
        NotSpecified,
        Woman,
        Man
    };

    class UserTileListController {
        tileRows: UserView[][] = [[]];
        canLoadMore: boolean;
        loaded: boolean = true;
        filter: string = "Everyone";
        showFilters: boolean;
        isLoggedIn: boolean;

        constructor(private $http: ng.IHttpService,
            private authService: IAuthService,
            $scope: ng.IScope) {

            if (authService.isAuth()) {
                $http.get<ProfileSummary>("app/Profile/ProfileSummaryData")
                    .success(profile => {
                    switch (profile.User.Interest) {
                        case 1: this.changeFilter("Women"); break;
                        case 2: this.changeFilter("Men"); break;
                        default: break;
                    }
                });
            }

            $scope.$on("profilePhotoChanged",(e: any, userId: string, photo: any) => {
                if (this.tileRows.some(row => row.some(u => u.UserId == userId))) {
                    this.getUser(userId).ProfilePhoto = photo;
                    $scope.$apply();
                }
            });

            this.isLoggedIn = authService.isAuth();
            this.getNextUsers();
        }

        flatten(): UserView[] {
            var flatten: UserView[] = [];
            for (var i = 0; i < this.tileRows.length; i++) {
                for (var j = 0; j < this.tileRows[i].length; j++) {
                    flatten.push(this.tileRows[i][j]);
                }
            }
            return flatten;
        }

        getUser(userId: string) {
            var user = this.flatten().filter(u => u.UserId == userId)[0];
            return user;
        }

        getNextUsers() {
            var itemToTake: number = 12;
            this.loaded = false;
            var gender;
            switch (this.filter) {
                case "Women": gender = Gender.Woman; break;
                case "Men": gender = Gender.Man; break;
                default: gender = Gender.NotSpecified; break;
            }

            this.$http.post<UserView[]>("app/UserList/UserTileListData", {
                skipIds: this.flatten().map(u => u.UserId),
                take: itemToTake,
                gender: gender
            }).success(list => {
                this.canLoadMore = list.length < itemToTake;

                list.forEach(user => {
                    if (!this.tileRows) {
                        this.tileRows = [[user]];
                        return;
                    }

                    if (this.tileRows[this.tileRows.length - 1].length < 3) {
                        this.tileRows[this.tileRows.length - 1].push(user);
                        return;
                    }

                    this.tileRows[this.tileRows.length] = [user];
                });

                this.loaded = true;
            });
        }

        changeFilter(filter: string) {
            this.filter = filter;
            this.showFilters = false;
            this.tileRows = [[]];
            this.getNextUsers();
        }
    }

    app.controller("UserTileListController", ["$http", "authService", "$scope", UserTileListController]);
}