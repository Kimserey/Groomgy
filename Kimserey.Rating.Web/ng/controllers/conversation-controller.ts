module rating {
    class ConversationController {
        isLoaded: boolean;

        conversationIdFromParams: string;
        draftMessage: string;
        isSendMessageBusy: boolean;

        conversationPreviews: ConversationPreviewView[] = [];
        activatedConversation: ActiveConversation;

        constructor(private $http: ng.IHttpService,
            private $state: ng.ui.IStateService,
            private $scope: ng.IScope,
            private $timeout: ng.ITimeoutService) {
            this.conversationIdFromParams = this.$state.params["conversation"];


            this.$http.get<ConversationPreviewView[]>("app/Message/ConversationListData")
                .success(list => {
                this.conversationPreviews = list;

                if (this.conversationPreviews.length > 0) {
                    this.selectConversation();
                }
            }).finally(() => {
                this.isLoaded = true;
            });

            $scope.$on("messageReceived",(e: any, conversationId: string, user: UserView, text: string, sentOn: Date) => {
                if (this.activatedConversation && this.activatedConversation.ConversationId == conversationId) {
                    this.addMessageToConversation(user.UserId, text, sentOn, false);
                    $timeout(() => {
                        this.markConversationAsRead(conversationId);
                    }, 2000);
                }

                if (!this.conversationPreviews.some(preview => preview.ConversationId == conversationId)) {
                    this.conversationPreviews.push(<ConversationPreviewView> {
                        ConversationId: conversationId,
                        User: user
                    });
                }

                this.conversationPreviews
                    .filter(c => c.ConversationId == conversationId)
                    .forEach(c => {
                    c.HasNewMessages = true;
                    c.Message = <MessageView> {
                        IsSentByUser: false,
                        SentByUserId: user.UserId,
                        SentOn: sentOn,
                        Text: text
                    };
                });

                $scope.$apply();
            });

            $scope.$on("markedAsRead",(e: any, conversationId: string) => {
                this.markConversationAsRead(conversationId);
                $scope.$apply();
            });

            $scope.$on("userConnected",(e: any, userId: string) => {
                if (this.activatedConversation && this.activatedConversation.InterlocutorId == userId) {
                    this.activatedConversation.IsInterlocutorOnline = true;
                    $scope.$apply();
                }
            });

            $scope.$on("userDisconnected",(e: any, userId: string) => {
                if (this.activatedConversation && this.activatedConversation.InterlocutorId == userId) {
                    this.activatedConversation.IsInterlocutorOnline = false;
                    $scope.$apply();
                }
            });
        }

        markConversationAsRead(conversationId: string) {
            this.conversationPreviews
                .filter(c => c.ConversationId == conversationId)
                .forEach(c => {
                c.HasNewMessages = false;
            });

            if (!this.conversationPreviews.some(preview => preview.HasNewMessages)) {
                this.$scope.$emit("hasNoMoreNewMessages");
            }
        }

        selectConversation(conversationId?: string) {
            if (!conversationId && !this.conversationIdFromParams) {
                return;
            }

            this.draftMessage = null;
            this.conversationPreviews.forEach(c => c.IsActive = false);
            var activatedPreview = this.conversationPreviews.filter(c => conversationId
                ? c.ConversationId == conversationId
                : c.ConversationId == this.conversationIdFromParams)[0];
            activatedPreview.IsActive = true;

            this.$http.get<ConversationView>("app/Message/ConversationData/" + activatedPreview.ConversationId)
                .success(data => {
                this.activatedConversation = <ActiveConversation> {
                    Messages: [],
                    Title: data.Title,
                    ConversationId: data.ConversationId,
                    Users: data.Users,
                    IsInterlocutorOnline: data.IsInterlocutorOnline,
                    InterlocutorId: data.InterlocutorId
                };
                data.Messages.forEach(m => {
                    this.addMessageToConversation(m.SentByUserId, m.Text, m.SentOn, m.IsSentByUser);
                });
            })
                .then(() => this.$http.post("app/Message/MarkConversationAsRead", {
                conversationId: this.activatedConversation.ConversationId
            }));
        }

        send() {
            this.isSendMessageBusy = true;
            this.$http.post<MessagePosted>("app/Message/Send",
                {
                    ConversationId: this.activatedConversation.ConversationId,
                    Text: this.draftMessage,
                    SentOn: new Date()
                })
                .success(messagePosted => {
                this.addMessageToConversation(messagePosted.UserId, messagePosted.Text, messagePosted.SentOn, true);
            }).then(() => {
                this.draftMessage = null;
            }).finally(() => {
                this.isSendMessageBusy = false;
                this.$timeout(() => {
                    angular.element("#sendInput").focus();
                });
            });
        }

        addMessageToConversation(userId: string, text: string, sentOn: Date, isUser: boolean) {
            var photoUrl = this.activatedConversation.Users.filter(u => u.UserId == userId)[0].ProfilePhoto.Small;
            this.activatedConversation.Messages.push(new MessageWithPhoto(userId, photoUrl, text, sentOn, isUser));
        }

        deleteActivatedConversation() {
            var conversationId = this.activatedConversation.ConversationId;
            this.$http
                .post("app/Message/Delete", {
                conversationId: conversationId
            })
                .success(() => {
                this.conversationPreviews.forEach(c => c.IsActive = false);
                this.conversationPreviews = this.conversationPreviews.filter(c => c.ConversationId != conversationId);
                this.activatedConversation = null;
            });
        }
    }

    app.controller("ConversationController", ["$http", "$state", "$scope", "$timeout", ConversationController]);
} 