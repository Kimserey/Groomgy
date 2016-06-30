interface SignalR {
    userListHub: UserListHub;
    conversationHub: ConversationHub;
    profileHub: ProfileHub;
    voteHub: VoteHub;
}

/**** start UserList hub ****/
interface UserListHub {
    client: UserListHubClient;
}

interface UserListHubClient {
    UserConnected(userId: string): void;
    UserDisconnected(userId: string): void;
}
/**** end UserList hub ****/

/**** start ConversationHub hub ****/
interface ConversationHub {
    client: ConversationHubClient;
}

interface ConversationHubClient {
    MessageReceived(conversationId: string,
        user: any,
        text: string,
        sentOn: Date);
    HasNewMessages(userId: string);
    MarkedAsRead(conversationId: string);
}
/**** end ConversationHub hub ****/

/**** start ProfileHub hub ****/
interface ProfileHub {
    client: ProfileHubClient;
}

interface ProfileHubClient {
    ProfileUpdated(userId: string, name: string, description: string, location: string, birthday: Date, gender: number);
    ProfilePhotoChanged(userId: string, photo: any): void;
    GalleryPhotoUploaded(userId: string, photo: any): void;
}
/**** end ProfileHub hub ****/

/**** start VoteHub hub ****/
interface VoteHub {
    client: VoteHubClient;
}

interface VoteHubClient {
    VoteReceived(voteId: string,
        votedForUserId: string,
        voteByUser: any,
        rate: number,
        comment: string,
        date: Date);
}
/**** end ProfileHub hub ****/
