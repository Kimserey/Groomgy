module rating {
    export class ProfileValidation {
        IsNameValid: boolean;
        IsBirthdayValid: boolean;
        IsGenderValid: boolean;
        IsInterestValid: boolean;
    }

    export interface IProfileValidatorService {
        validate(name: string, birthday: Date, gender: number, interest: number): ProfileValidation;
        validateBirthday(birthday: Date): boolean;
    }

    class ProfileValidatorService {
        isGenderUnspecified(gender: number) {
            return gender == 0;
        }

        isInterestUnspecified(interest: number) {
            return interest == 0;
        }

        matchDefaultName(name: string) {
            return (/^Groomgy-user/).test(name);
        }

        isUnderAge(date: Date) {
            return moment(new Date()).diff(moment(date), 'year') < 18;
        }

        validate(name: string, birthday: Date, gender: number, interest: number) {
            return <ProfileValidation>{
                IsNameValid: !this.matchDefaultName(name),
                IsBirthdayValid: !this.isUnderAge(birthday),
                IsGenderValid: !this.isGenderUnspecified(gender),
                IsInterestValid: !this.isInterestUnspecified(interest)
            };
        }

        validateBirthday(birthday: Date) {
            return !this.isUnderAge(birthday);
        }
    }

    app.service("profilevalidator", ProfileValidatorService);
}
