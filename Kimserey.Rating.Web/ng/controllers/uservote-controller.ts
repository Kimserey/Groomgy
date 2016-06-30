module rating {
    class UserVoteController {
        message: string;
        votes: VoteView[];
        canVote: boolean;

        userId: string;
        draftVote: VoteDraft;
        tempRate: number;

        isVoteBusy: boolean;

        constructor(private $http: ng.IHttpService,
            $state: ng.ui.IStateService,
            $scope: ng.IScope,
            private vote: IVoteService) {
            this.userId = $state.params["id"];
            this.draftVote = new VoteDraft(this.userId);

            this.$http.get<VoteListView>("app/Vote/ListData/" + this.userId)
                .success(voteList => {
                    this.canVote = voteList.CanVote;
                    this.votes = voteList.Votes;
                });

            $scope.$on("voteReceived", (e: any, voteId: string, votedForUserId: string, voteByUser: UserView,
                rate: number, comment: string, date: Date) => {
                if (this.userId == votedForUserId) {
                    this.votes.push({
                        Comment: comment,
                        Rate: rate,
                        RatedOn: date,
                        VotedByUser: voteByUser,
                        VoteId: voteId
                    });
                    $scope.$apply();
                }
            });
        }

        hoveringOverStar(value: number) {
            this.tempRate = value;
        }

        sendVote() {
            this.draftVote.VotedOn = new Date();
            this.isVoteBusy = true;

            this.vote.sendVote(this.draftVote)
                .success(resp => {
                    this.message = null;
                    if (resp.success) {
                        this.draftVote = new VoteDraft(this.userId);
                        this.canVote = false;
                        toastr.success("Thanks for your vote!", null, toastrOptions);
                    } else {
                        this.message = resp.error;
                    }
                })
                .finally(() => {
                    this.isVoteBusy = false;
                });
        }
    }

    app.controller("UserVoteController", ["$http", "$state", "$scope", "vote",  UserVoteController]);
}