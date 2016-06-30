module rating {
    export interface IVoteService {
        sendVote(vote: VoteDraft): ng.IHttpPromise<any>;
    }

    class VoteService implements IVoteService {
        constructor(private $http: ng.IHttpService) { }

        sendVote(vote: VoteDraft) {
            return this.$http.post<any>("app/Vote/VoteForUser", vote);
        }
    }

    app.service("vote", ["$http", VoteService]);
}