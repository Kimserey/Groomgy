module rating {
    export interface IConversationService {
        startConversation(userId: string): ng.IHttpPromise<any>;
    }

    class ConversationService implements IConversationService {
        constructor(private $http: ng.IHttpService, private $state: ng.ui.IStateService) { }

        startConversation(userId: string) {
            return this.$http.post<any>("app/Message/StartConversation/" + userId, null)
                .success(res => {
                this.$state.go("messages", { conversation: res.conversationId });
            });
        }
    }

    app.service("conversation", ["$http", "$state", ConversationService]);
} 