/// <reference path="libs/_references.js" />


var application= (function () {
    var appLayout =
		new kendo.Layout('<div id="main-content"></div><div id="advert-details"></div>');
    var data = persisters.get("http://localhost:38338/api/");
    vmFactory.setPersister(data);

    var router = new kendo.Router();
   

    router.route("/", function () {
        if (data.users.currentUser()) {
            router.navigate("/adverts");
        }
        else{
            viewsFactory.getLoginView()
                .then(function (loginViewHtml) {
                    var loginVm = vmFactory.getLoginVM(
                        function () {
                            //not sure what for, was like that "/"
                            router.navigate("/adverts");
                        });
                    var view = new kendo.View(loginViewHtml,
                        { model: loginVm });
                    appLayout.showIn("#main-content", view);

                    $("#tabstrip").kendoTabStrip({
                        animation: {
                            open: {
                                effects: "fadeIn"
                            }
                        }
                    });
                });
        }
    });

    router.route("/adverts", function () {
        viewsFactory.getAdvertsView()
        .then(function (advertsViewHtml) {
            vmFactory.getAllAdvertsVM()
            .then(function (advertsVM) {
                var view = new kendo.View(advertsViewHtml,
                    { model: advertsVM });
                appLayout.showIn("#main-content", view);

            })
        })
    });

    router.route("/adverts-:id", function (id) {
        viewsFactory.getSingleAdvertView()
        .then(function (advertViewHtml) {
            vmFactory.getSingleAdvertVM(id)
            .then(function (advertVM) {
                var view = new kendo.View(advertViewHtml,
                    { model: advertVM });
                console.log(advertVM)
                appLayout.showIn("#advert-details", view);

                $("#myCarousel").carousel();
            })
        })
    });

    $(function () {
        appLayout.render("#app");
        router.start();
    });

    return {

        router: function () {
            return router;
        }
    }
}());