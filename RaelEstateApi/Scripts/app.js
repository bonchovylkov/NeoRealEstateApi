/// <reference path="libs/_references.js" />


var application= (function () {
    var appLayout =
		new kendo.Layout('<div id="main-content"></div>');
    var data = persisters.get("http://localhost:38338/api/");
    vmFactory.setPersister(data);

    var router = new kendo.Router();
   

    router.route("/", function () {
        if (data.users.currentUser()) {
            router.navigate("/getaccounts");
        }
        else{
            viewsFactory.getLoginView()
                .then(function (loginViewHtml) {
                    var loginVm = vmFactory.getLoginVM(
                        function () {
                            //not sure what for, was like that "/"
                            router.navigate("/getaccounts");
                        });
                    var view = new kendo.View(loginViewHtml,
                        { model: loginVm });
                    appLayout.showIn("#main-content", view);
                });
        }
    });

    router.route("/login", function () {
        debugger;
        if (data.users.currentUser()) {
            router.navigate("/getaccounts");
        }
        else {
            viewsFactory.getLoginView()
				.then(function (loginViewHtml) {
				    var loginVm = vmFactory.getLoginVM(
						function () {
						    router.navigate("/");
						});
				    var view = new kendo.View(loginViewHtml,
						{ model: loginVm });
				    appLayout.showIn("#main-content", view);
				});
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