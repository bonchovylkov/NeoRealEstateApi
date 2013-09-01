/// <reference path="../libs/_references.js" />

window.vmFactory = (function () {

    var data = null;

    function getLoginViewModel(successCallback) {

        var viewModel = {
            username: "a",
            fullname: "b",
            password: "c",
            login: function () {
                data.users.login(this.get("username"), this.get("password"))
                .then(function () {
                    if (successCallback) {
                        successCallback();
                    }
                }, function () {
                    if (errorCallback) {
                        errorCallback();
                    }
                })
            },

            register: function () {
                data.users.register(this.get("username"), this.get("fullname"), this.get("password"))
                .then(function () {
                    if (successCallback) {
                        successCallback();
                    }
                });
            }
        }
        return kendo.observable(viewModel);
    }

    function getAllAdvertsViewModel() {
        return data.adverts.all()
        .then(function (adverts) {
            var viewModel = {
                adverts: adverts
            };

            return kendo.observable(viewModel);
        })
    }

    function getSingleAdvertViewModel(id) {
        return data.adverts.getById(id)
        .then(function (advert) {
            var model = {
                detailedAdvert: advert
            };
            return kendo.observable(model);
        })
    }

    
	
	return {
		
		setPersister: function (persister) {
			data = persister
		},

		getLoginVM: getLoginViewModel,
		getAllAdvertsVM: getAllAdvertsViewModel,
		getSingleAdvertVM: getSingleAdvertViewModel
	};
}());