/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/angular.js" />

angular.module("advert", [])
	.config(["$routeProvider", function ($routeProvider) {
	    $routeProvider
            //.when("/admin", { templateUrl: "scripts/partials/admin.html", controller: AdminController })
			.when("/admin/adverts", { templateUrl: "scripts/partials/all-advert.html", controller: AdvertsController })
			.when("/admin/adverts/:id", { templateUrl: "scripts/partials/single-advert.html", controller: SingleAdvertController })
			.when("/admin/users", { templateUrl: "scripts/partials/all-users.html", controller: UsersController })
			.when("/admin/users/:id", { templateUrl: "scripts/partials/single-user.html", controller: SingleUserController })
			.otherwise({ redirectTo: "/admin" });
	}]);
