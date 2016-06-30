module rating {
/* Hub configuration
* All comments are the name of the events
*/
    app.run(["$rootScope", "authService", ($rootScope: ng.IRootScopeService, authService: IAuthService) => {
        if (authService.isAuth()) {
           $.signalR["ajaxDefaults"].headers = { Authorization: "Bearer " + authService.getToken() };
        }

        /**** start UserLisHub ****/
        var userLisHub = $.connection.userListHub;

//userConnected
        userLisHub.client.UserConnected = function (userId: string) {
            $rootScope.$broadcast("userConnected", userId);
        }
//userDisconnected
        userLisHub.client.UserDisconnected = function (userId: string) {
            $rootScope.$broadcast("userDisconnected", userId);
        }
        /**** end UserLisHub ****/

        if (authService.isAuth()) {
            /**** start Conversationhub ****/
            var conversationhub = $.connection.conversationHub;

//messageReceived
            conversationhub.client.MessageReceived = function (conversationId: string,
                user: any, text: string, sentOn: Date) {
                $rootScope.$broadcast("messageReceived", conversationId, user, text, sentOn);
            };
//hasNewMessages
            conversationhub.client.HasNewMessages = function (userId: string) {
                $rootScope.$broadcast("hasNewMessages", userId);
            };
//markedAsRead
            conversationhub.client.MarkedAsRead = function (conversationId: string) {
                $rootScope.$broadcast("markedAsRead", conversationId);
            }
            /**** end Conversationhub ****/
        }

        /**** start ProfileHub ****/
        var profileHub = $.connection.profileHub;

//profileUpdated
        profileHub.client.ProfileUpdated = function (userId: string, name: string, description: string, location: string, birthday: Date, gender: number) {
            $rootScope.$broadcast("profileUpdated", userId, name, description, location, birthday, gender);
        }

//profilePhotoChanged
        profileHub.client.ProfilePhotoChanged = function (userId: string, photo: any) {
            $rootScope.$broadcast("profilePhotoChanged", userId, photo);
        }
//galleryPhotoUploaded
        profileHub.client.GalleryPhotoUploaded = function (userId: string, photo: any) {
            $rootScope.$broadcast("galleryPhotoUploaded", userId, photo);
        }
        /**** end ProfileHub ****/

        /**** start VoteHub ****/
        var voteHub = $.connection.voteHub;

//voteReceived
        voteHub.client.VoteReceived = function (voteId: string, votedForUserId: string, voteByUser: any,
            rate: number, comment: string, date: Date) {
            $rootScope.$broadcast("voteReceived", voteId, votedForUserId, <UserView>voteByUser, rate, comment, date);
        }
        /**** end ProfileHub ****/

        $.connection.hub.start();
    }]);
} 