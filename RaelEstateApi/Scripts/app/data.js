/// <reference path="../libs/_references.js" />

window.persisters = (function () {
	var sessionKey = "";
	var bashUsername = "";
	function clearLocalStorage() {
	    localStorage.removeItem("displayName");
	    localStorage.removeItem("sessionKey");
	    sessionKey = "";
	    bashUsername = "";
	}

	function saveSessionKey(sessionKeyNew,displayName) {
	    localStorage.setItem("displayName", displayName);
	    localStorage.setItem("sessionKey", sessionKeyNew);
	    bashUsername = displayName;
	    sessionKey = sessionKeyNew;
	}

	var DataPersister = Class.create({
	    init: function (apiUrl) {
	        this.apiUrl = apiUrl;
	        this.users = new UsersPersister(apiUrl + "users/");
	        this.adverts = new AdvertsPersister(apiUrl +"adverts");
	    }
	});

	var UsersPersister = Class.create({
		init: function (apiUrl) {
			this.apiUrl = apiUrl;
		},
		login: function (username, password) {
			var user = {
			    Username: username,
                //for demo purpose
			    AuthCode: CryptoJS.SHA1(password).toString()
			};
			console.log(httpRequester);
			return httpRequester.postJSON(this.apiUrl + "login", user)
				.then(function (data) {
				    saveSessionKey(data.sessionKey, data.fullname);
				    sessionKey = data.sessionKey;
				    bashUsername = data.fullname;
				    //return data;
				}, function (err) {
				    console.log(err);
				});
		},
		register: function (username, fullname, password) {
		    debugger;
		    var user = {
		        username: username,
		        fullname: fullname,
		        authCode: CryptoJS.SHA1(password).toString()
		    };
            
			return httpRequester.postJSON(this.apiUrl + "register", user)
				.then(function (data) {
				    saveSessionKey(data.sessionKey, data.fullname);
				    debugger;
				    sessionKey = data.sessionKey;
				    bashUsername = data.fullname;
				    //return data.fullname;
				});
		},
		logout: function () {
		    if (localStorage.getItem("sessionKey")) {
                
		        return httpRequester.putJSON(this.apiUrl + "logout?sessionKey=" + localStorage.getItem("sessionKey"))
		    .then(function () {
		        clearLocalStorage();
		        console.log("in succes logut");
		        var router = application.router();
		        router.navigate("/");

		    }, function () {
		        clearLocalStorage();
		        var router = application.router();
		        $("#advert-details").html("");
		        router.navigate("/");
		    });
		    }
		    else {
		        console.log("It's allready logged out");
		    }

			
		},
		currentUser: function () {
		    return localStorage.getItem("displayName");
		}
	});

	var AdvertsPersister = Class.create({
		init: function (apiUrl) {
		    this.apiUrl = apiUrl;
		},
		all: function () {
		    return httpRequester.getJSON(this.apiUrl);
		},
		getById: function (id) {
		    return httpRequester.getJSON(this.apiUrl + "/" +id+ "?sessionKey=" + localStorage.getItem("sessionKey"));
		},
		create: function (createModel) {
		    debugger;
		    return httpRequester.postJSON(this.apiUrl + "?sessionKey=" + localStorage.getItem("sessionKey"), createModel);
		},
		//deposit: function (sum,id) {
		//    return httpRequester.putJSON(this.apiUrl + id + "?depositSum=" + sum + "&sessionKey=" + localStorage.getItem("sessionKey"))
        //        .then(function () {
        //            alert("in success");
        //        }, function () {
        //            alert("You have just deposid money");
        //            var router = application.router();
        //            router.navigate("/");
        //        });
		//},
		//withraw: function (sum, id) {
		//    return httpRequester.putJSON(this.apiUrl + id + "?withdrawSum=" + sum + "&sessionKey=" + localStorage.getItem("sessionKey"))
        //        .then(function () {
        //            alert("in success");
        //        }, function () {
        //            alert("You have just withraw money");
        //            var router = application.router();
        //            router.navigate("/");
        //        });
		//}

	});




	return {
		get: function (apiUrl) {
			return new DataPersister(apiUrl);
		}
	}
}());