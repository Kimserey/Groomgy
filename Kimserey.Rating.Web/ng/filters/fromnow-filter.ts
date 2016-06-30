module rating {
    app.filter("fromNow", function () {
        return function (dateString) {
            return moment(dateString).fromNow();
        };
    });
}