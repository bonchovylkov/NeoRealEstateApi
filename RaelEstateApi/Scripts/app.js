﻿/// <reference path="libs/_references.js" />


var application= (function () {
    var appLayout =
		new kendo.Layout('<div id="main-content" class="span7"></div><div id="advert-details" class="span5 offset1"></div>');
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
                    
                   // $("#main-content").hide();
                    appLayout.showIn("#main-content", view);
                   // $("main-content").fadeIn(500);

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
        if (data.users.currentUser()) {
            viewsFactory.getAdvertsViewWithLogOut()
            .then(function (advertsViewHtml) {
                vmFactory.getAllAdvertsVM()
                .then(function (advertsVM) {
                    var view = new kendo.View(advertsViewHtml,
                        { model: advertsVM });
                    appLayout.showIn("#main-content", view);

                })
            })
        }
        else {
            viewsFactory.getAdvertsView()
            .then(function (advertsViewHtml) {
                vmFactory.getAllAdvertsVM()
                .then(function (advertsVM) {
                    var view = new kendo.View(advertsViewHtml,
                        { model: advertsVM });
                    appLayout.showIn("#main-content", view);

                })
            })
        }
    });

    router.route("/adverts-:id", function (id) {
        if (data.users.currentUser()) {
            viewsFactory.getSingleAdvertView()
            .then(function (advertViewHtml) {
                vmFactory.getSingleAdvertVM(id)
                .then(function (advertVM) {
                    var view = new kendo.View(advertViewHtml,
                        { model: advertVM });
                    console.log(advertVM)

                    $("#advert-details").hide();
                    appLayout.showIn("#advert-details", view);
                    $("#advert-details").fadeIn(500);
                    $("#myCarousel").carousel();
                })
            })
        }
        else {
            router.navigate("/");
        }
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