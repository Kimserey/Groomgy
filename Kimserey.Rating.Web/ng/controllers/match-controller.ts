module rating {
    class MatchController {
        matches: any[] = [];
        isBusy: boolean;
        isAuth: boolean;

        constructor($http: ng.IHttpService,
            private conversation: IConversationService,
            private vote: IVoteService,
            private $state: ng.ui.IStateService,
            authService: IAuthService) {
            if (authService.isAuth()) {
                $http.get<ProfileView[]>("app/Match/GetMatch")
                    .success(data => {
                    this.matches = data;
                    this.isAuth = true;
                });
            }
        }

        startConversation(userId: string) {
            this.conversation.startConversation(userId);
        }

        voteForUser(userId: string) {
            this.isBusy = true;
            var draftVote = new VoteDraft(userId);
            draftVote.Comment = "~ From random match.";
            draftVote.VotedOn = new Date();
            draftVote.Rate = 5;
            this.vote.sendVote(draftVote)
                .then(data => {
                this.matches
                    .filter(m => m.UserId == userId)
                    .forEach(m => m.Liked = true);
            }).finally(() => this.isBusy = false);
        }
    }

    app.controller("MatchController", ["$http", "conversation", "vote", "$state", "authService", MatchController]);
}