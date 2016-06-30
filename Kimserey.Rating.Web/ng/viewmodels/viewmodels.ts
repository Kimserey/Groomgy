module rating {
    export class Photo {
        Relative: string;
        Small: string;
        Medium: string;
        Large: string;
    }

    export class ProfileSummary {
        Settings: UserSettings;
        User: UserView;
        HasNewMessages: boolean;
    }

    export class UserSettings {
        ShowWelcome: boolean;
    }

    export class UserView {
        ProfilePhoto: Photo;
        Gender: number;
        Interest: number;
        Name: string;
        UserId: string;
        Birthday: Date;
    }

    export class UserNameView {
        Gender: number;
        Name: string;
        UserId: string;
        IsOnline: boolean;
    }

    export class ProfileView {
        UserId: string;
        Name: string;
        ProfilePhoto: Photo;
        Description: string;
        Location: string;
        Birthday: Date;
        AlbumPhotos: Photo[];
        AlbumPhotoRows: Photo[][];
        Age: string;
        Gender: number;
        Interest: number;
        CanEdit: boolean;
        IsOnline: boolean;
    }

    export class VoteListView {
        CanVote: boolean;
        Votes: VoteView[];
    }

    export class VoteView {
        VoteId: string;
        Rate: number;
        Comment: string;
        VotedByUser: UserView;
        RatedOn: Date;
    }

    export class VoteDraft {
        Rate: number;
        Comment: string;
        RatedOnUserId: string;
        VotedOn: Date;
        constructor(ratedOnUserId: string) {
            this.Rate = 5;
            this.RatedOnUserId = ratedOnUserId;
        }
    }

    export class ConversationPreviewView {
        ConversationId: string;
        User: UserView;
        Message: MessageView;
        IsActive: boolean;
        HasNewMessages: boolean;
    }

    export class MessageView {
        SentByUserId: string;
        Text: string;
        SentOn: Date;
        IsSentByUser: boolean;
    }

    export class ConversationView {
        Title: string;
        ConversationId: string;
        Users: UserView[];
        Messages: MessageView[];
        IsInterlocutorOnline: boolean;
        InterlocutorId: string;
    }

    export class ActiveConversation {
        Title: string;
        ConversationId: string;
        Users: UserView[];
        Messages: MessageWithPhoto[];
        IsInterlocutorOnline: boolean;
        InterlocutorId: string;
    }

    export class MessageWithPhoto {
        UserId: string;
        ProfilePhotoUrl: string;
        Text: string;
        SentOn: Date
        SentOnDisplay: string;
        IsUserMessage: boolean;

        constructor(userId: string,
            profilePhotoUrl: string,
            text: string,
            sentOn: Date,
            isUserMessage: boolean) {
            this.UserId = userId;
            this.ProfilePhotoUrl = profilePhotoUrl;
            this.Text = text;
            this.SentOn = sentOn;
            this.SentOnDisplay = moment(sentOn).format("llll");
            this.IsUserMessage = isUserMessage;
        }
    }

    export class Registration {
        Email: string;
        Password: string;
        ConfirmPassword: string;
    }

    export class LoginCredentials {
        Email: string;
        Password: string;
    }

    export class UserRankView {
        User: UserView;
        Value: number
    }

    export class ResetPassword {
        Email: string;
        Password: string;
        ConfirmPassword: string;
        Token: string;
        constructor(email: string, password: string, confirmPassword: string, token: string) {
            this.Email = email;
            this.Password = password;
            this.ConfirmPassword = confirmPassword;
            this.Token = token;
        }
    }

    export class MessagePosted {
        UserId: string;
        Text: string;
        SentOn: Date;
    }
}