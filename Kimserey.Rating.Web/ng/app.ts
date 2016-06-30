declare var siteUrl: string;

module rating {
    export var app: ng.IModule;
    export var toastrOptions: ToastrOptions = {
        timeOut: 3000,
        showMethod: "show",
        hideMethod: "hide",
        extendedTimeOut: 2000
    };

    app = angular.module("app", ["ui.bootstrap",
        "angulartics",
        "angulartics.google.analytics",
        "ui.router",
        "LocalStorageModule",
        "angularFileUpload",
        "luegg.directives",
        "truncate"]);

    app.config(["localStorageServiceProvider", function (localStorageServiceProvider: ng.local.storage.ILocalStorageServiceProvider) {
        localStorageServiceProvider
            .setPrefix("rating");
    }]);

    app.config(["$httpProvider", function ($httpProvider) {
        $httpProvider.interceptors.push("authHttpInterceptor");
    }]);

    app.config(["$stateProvider", "$urlRouterProvider", "$locationProvider", function ($stateProvider: ng.ui.IStateProvider,
        $urlRouterProvider: ng.ui.IUrlRouterProvider,
        $locationProvider: ng.ILocationProvider) {

        $locationProvider.html5Mode(true)
            .hashPrefix('!');

        $urlRouterProvider.otherwise('/home');

        // More information on named view : https://github.com/angular-ui/ui-router/wiki/Multiple-Named-Views
        //
        $stateProvider
            .state({
                name: "home",
                url: "/home",
                views: {
                    "": <ng.ui.IState> {
                        templateUrl: "app/UserList"
                    },
                    "nameList@home": <ng.ui.IState> {
                        templateUrl: "app/UserList/NameList"
                    },
                    "userTileList@home": <ng.ui.IState> {
                        templateUrl: "app/UserList/UserTileList"
                    },
                    "match@home": <ng.ui.IState>{
                        templateUrl: "app/Match"
                    }
                }
            })
            .state({
                name: "terms",
                url: "/terms",
                templateUrl: "/app/Home/TermsAndConditions"
            })
            .state({
                name: "settings",
                url: "/settings?email",
                templateUrl: "/app/Home/Settings"
            })
            .state({
                name: "about",
                url: "/about",
                templateUrl: "/app/Home/About"
            })
            .state({
                name: "profile",
                url: "/profile/:id?highlight",
                views: {
                    "": <ng.ui.IState>{
                        templateUrl: "app/Profile"
                    },
                    "userProfile@profile": <ng.ui.IState> {
                        templateUrl: "app/Profile/UserProfile"
                    },
                    "userVote@profile": <ng.ui.IState> {
                        templateUrl: "app/Vote/List"
                    }
                }
            })
            .state({
                name: "messages",
                url: "/messages?conversation",
                views: {
                    "": <ng.ui.IState>{
                        templateUrl: "app/Message"
                    },
                    "conversationList@messages": <ng.ui.IState>{
                        templateUrl: "app/Message/ConversationList"
                    },
                    "conversation@messages": <ng.ui.IState>{
                        templateUrl: "app/Message/Conversation"
                    }
                }
            })
            .state({
                name: "tops",
                url: "/tops",
                views: {
                    "": <ng.ui.IState>{
                        templateUrl: "app/Top"
                    }
                }
            })
            .state({
                name: "account",
                url: "/account",
                templateUrl: "app/Account/Index",
                controller: "AccountController",
                controllerAs: "accountCtrl"
            })
            .state({
                name: "account.externallogin",
                url: "/external?provider&accessToken",
                onEnter: ["$modal", function ($modal: ng.ui.bootstrap.IModalService) {
                    $modal.open({
                        template:
                        "<div class='well well-lg' style='margin-bottom:0'>"
                        + "<h5>Please wait a moment while we log you in!</h5>"
                        + "</div>",
                        controller: "ExternalLoginController",
                        backdrop: 'static'
                    })
                }]
            })
            .state({
                name: "account.forgotpassword",
                url: "/forgotpassword",
                onEnter: ["$modal", "$state", function ($modal: ng.ui.bootstrap.IModalService, $state: ng.ui.IStateService) {
                    $modal.open({
                        templateUrl: "app/Account/ForgotPassword"
                    }).result.finally(() => {
                            $state.go("^");
                        });
                }]
            })
            .state({
                name: "account.resetpassword",
                url: "/resetpassword?email&token",
                onEnter: ["$modal", function ($modal: ng.ui.bootstrap.IModalService) {
                    $modal.open({
                        templateUrl: "app/Account/ResetPassword"
                    })
                }]
            })
            .state({
                name: "account.verify",
                url: "/verify?email&token",
                onEnter: ['$modal', function ($modal: ng.ui.bootstrap.IModalService) {
                    $modal.open({
                        templateUrl: "app/Account/Confirm",
                        backdrop: 'static'
                    })
                }]
            });
    }]);
}

