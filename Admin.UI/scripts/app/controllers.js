/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/angular.js" />

var serverURL = "http://localhost:38338/api/";

function LoginAdminController($scope, $http) {
    $scope.loginUser = {};

    $scope.update = function (user) {
        $scope.loginUser = angular.copy(user);
    };

    $http.post("scripts/data/users.js")
    //$http.post(serverURL + 'login/' + loginUser + '.json')
}



function AdvertsController($scope, $http) {

    $scope.newAdvert = {
		headline: "",
		text: "",
		pictures: "",
		town: "",
		address: "",
		postDate: new Date(),
		tags: "",
		price: ""
	};

    $http.get("scripts/data/adverts.js")
    //$http.get(serverURL + 'adverts' + '.json')
	.success(function (adverts) {
		$scope.adverts = adverts;
		$scope.towns = _.uniq(_.pluck(adverts, "town"));
		$scope.selectedPicture = $scope.pictures[0];
		$scope.selectedTown = $scope.towns[0];
		$scope.selectedDate = $scope.postDate;
	});

    $scope.addAdvert = function () {
        debugger;
	    $scope.adverts.push($scope.newAdvert);
		var town = $scope.newAdvert.town;
		if (!_.contains($scope.town, town)) {
		    $scope.town.push(town);
		}

		$scope.newAdvert = {
			headline: "",
			text: "",
			pictures: "",
			town: "",
			address: "",
			postDate: new Date(),
			tags: "",
			price: ""
		};
	}
	$scope.orderBy = "postDate";
}

function SingleAdvertController($scope, $http, $routeParams) {
	var id = $routeParams.id;
	$http.get("scripts/data/adverts.js")
    //$http.get(serverURL + 'adverts/' + $routeParams.id + '.json')
	.success(function (advert) {
	    var advert = _.first(_.where(advert, { "id": parseInt(id) }));
	    debugger;
	    $scope.advert = advert;
	});
}

function UsersController($scope, $http) {
    $scope.newUser = {
        username: "",
        fullName: "",
        authCode: "",
        role: "",
        adverts: "",
    };

    $http.get("scripts/data/users.js")
    //$http.get(serverURL + 'users' + '.json')
	.success(function (users) {
	    $scope.users = users;
	    $scope.adverts = _.uniq(_.pluck(users, "adverts"));
	    $scope.selectedAdverts = $scope.adverts[0];
	    $scope.selectedUsername = $scope.username;
	});
    $scope.addUser = function () {
        $scope.User.push($scope.newUser);
        var adverts = $scope.newUser.adverts;
        if (!_.contains($scope.adverts, adverts)) {
            $scope.adverts.push(adverts);
        }

        $scope.newUser = {
            username: "",
            fullName: "",
            authCode: "",
            role: "",
            adverts: "",
        };
    }
    $scope.orderBy = "username";
}

function SingleUserController($scope, $http, $routeParams) {
    var id = $routeParams.id;
    $http.get("scripts/data/users.js")
    //$http.get(serverURL + 'users/' + $routeParams.id + '.json')
	.success(function (user) {
	    var user = _.first(_.where(user, { "id": parseInt(id) }));
	    debugger;
	    $scope.user = user;
	})
}




function PicturesController($scope, $http, $routeParams) {
	var pictureName = $routeParams.name;

	$http.get("scripts/data/advert.js")
	.success(function (adverts) {
		var picture = {
			name: pictureName,
			adverts: []
		};
		_.chain(adverts)
		.where({ "picture": pictureName })
		.each(function (post) {
			picture.adverts.push({
				id: adverts.id,
				title: adverts.headline,
				content: adverts.text
			});
		})
		$scope.picture = picture;
	})
};