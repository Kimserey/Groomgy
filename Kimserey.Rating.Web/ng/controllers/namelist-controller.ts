module rating {
    class NameListController {
        users: UserNameView[] = [];

        constructor($scope: ng.IScope, $http: ng.IHttpService) {
            $http.get<UserNameView[]>("app/UserList/NameListData")
                .success(list => {
                if (!list) return;
                list.filter(u => u.Gender == 1)
                    .forEach(u => this.users.push(u));
                list.filter(u => u.Gender == 2)
                    .forEach(u => this.users.push(u));
                list.filter(u => u.Gender == 0)
                    .forEach(u => this.users.push(u));
            });

            $scope.$on("userConnected",(e: any, userId: string) => {
                this.users
                    .filter(user => user.UserId == userId)
                    .forEach(user => {
                    user.IsOnline = true;
                });
                $scope.$apply();
            });

            $scope.$on("userDisconnected",(e: any, userId: string) => {
                this.users
                    .filter(user => user.UserId == userId)
                    .forEach(user => {
                    user.IsOnline = false;
                });
                $scope.$apply();
            });
        }
    }

    app.controller("NameListController", ["$scope", "$http", NameListController]);
}