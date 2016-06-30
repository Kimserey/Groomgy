module rating {
    class TopsController {
        mostViewedMan: UserRankView[];
        mostVotedMan: UserRankView[];
        voteAddictsMan: UserRankView[];
        messageAddictsMan: UserRankView[];

        mostViewedLady: UserRankView[];
        mostVotedLady: UserRankView[];
        voteAddictsLady: UserRankView[];
        messageAddictsLady: UserRankView[];

        constructor($http: ng.IHttpService) {
            //Most votes
            $http.get<UserRankView[]>("app/Top/GetMostViewedManData")
                .success(data => {
                    this.mostViewedMan = data;
                });
            $http.get<UserRankView[]>("app/Top/GetMostViewedLadyData")
                .success(data => {
                    this.mostViewedLady = data;
                });

            //Most votes
            $http.get<UserRankView[]>("app/Top/GetMostVotesManData")
                .success(data => {
                    this.mostVotedMan = data;
                });
            $http.get<UserRankView[]>("app/Top/GetMostVotesLadyData")
                .success(data => {
                    this.mostVotedLady = data;
                });

            //Vote addicts
            $http.get<UserRankView[]>("app/Top/GetVoteAddictsManData")
                .success(data => {
                    this.voteAddictsMan = data;
                });
            $http.get<UserRankView[]>("app/Top/GetVoteAddictsLadyData")
                .success(data => {
                    this.voteAddictsLady = data;
                });

            //Message addicts
            $http.get<UserRankView[]>("app/Top/GetMessageAddictsManData")
                .success(data => {
                    this.messageAddictsMan = data;
                });
            $http.get<UserRankView[]>("app/Top/GetMessageAddictsLadyData")
                .success(data => {
                    this.messageAddictsLady = data;
                });
        }
    }

    app.controller("TopsController", ["$http", TopsController]); 
} 