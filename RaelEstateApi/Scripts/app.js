/// <reference path="libs/_references.js" />


var application = (function () {
    var appLayout =
		new kendo.Layout('<div id="main-content" class="span7"></div><div id="advert-details" class="span5 offset1"></div>');
    var data = persisters.get("http://neodymiumestates.apphb.com/api/");
    vmFactory.setPersister(data);

    var router = new kendo.Router();
    
    router.route("/special", function () {
        debugger;
        router.start();
        router.navigate("/");

    });
    
    router.route("/", function () {
        var grid = $("#body").parent("div");
        if (grid) {
            grid.hide();
        }
        if (data.users.currentUser()) {
            router.navigate("/adverts");
        }
        else {
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
        var grid = $("#body").parent("div");
        if (grid) {
            grid.hide();
        }
        
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
        var grid = $("#body").parent("div");
        if (grid) {
            grid.hide();
        }
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

    router.route("/advertsadmin", function () {
        if (data.users.currentUser()) {
            data.users.isAdmin().then(function (isAdmin) {
                if (isAdmin) {
                    debugger;
                    $("#main-content").hide();
                    $("#advert-details").hide();
                    $("#tb-logout").show();
                    grid(router);
                }
                else {
                    alert("You don't have permissions");
                    router.navigate("/");
                }
            })
        }
        else {
            router.navigate("/");
        }
    });

    router.route("/admin", function () {
        if (data.users.currentUser()) {
            debugger;
            viewsFactory.getAdminView()
                .then(function (advertsViewHtml) {
                    var view = new kendo.View(advertsViewHtml);

                    appLayout.showIn("#main-content", view);
                });
        }
        else {

            var view = new kendo.View("<div id='error-message' style='margin-top: 50px;font-size:large;'>You don't have rights to access this page!</div>");
            appLayout.showIn("#main-content", view);
            // $("#adverts-template").show();
            //setInterval(function() {
            //    router.navigate("/");
            //}, 4000);

        }
    });

    router.route("/create", function () {
        viewsFactory.getCreateView()
        .then(function (createViewHtml) {
            var createVM = vmFactory.getCreateVM(function () {
                router.navigate("/adverts");
            });
            debugger;
            var view = new kendo.View(createViewHtml,
                    { model: createVM });

            //$("#advert-details").hide();
            appLayout.showIn("#advert-details", view);
            $("#advert-details").fadeIn(500);
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