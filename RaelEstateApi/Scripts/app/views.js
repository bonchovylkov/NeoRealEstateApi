/// <reference path="../libs/_references.js" />


window.viewsFactory = (function () {
	var rootUrl = "Scripts/partials/";

	var templates = {};

	function getTemplate(name) {
		var promise = new RSVP.Promise(function (resolve, reject) {
			if (templates[name]) {
				resolve(templates[name])
			}
			else {
				$.ajax({
					url: rootUrl + name + ".html",
					type: "GET",
					success: function (templateHtml) {
						templates[name] = templateHtml;
						resolve(templateHtml);
					},
					error: function (err) {
						reject(err)
					}
				});
			}
		});
		return promise;
	}

	function getLoginView() {
	    return getTemplate("login-form");
	}

	function getAdvertsView() {
	    return getTemplate("adverts-list");
	}

	function getSingleAdvertView() {
	    return getTemplate("single-advert");
	}
	function getAdvertsViewWithLogOut() {
	    return getTemplate("adverts-list-with-logout");
	}

	function getCreateView() {
	    return getTemplate("create-view");
	}

	return {
	    getLoginView: getLoginView,
	    getAdvertsView: getAdvertsView,
	    getSingleAdvertView: getSingleAdvertView,
	    getAdvertsViewWithLogOut: getAdvertsViewWithLogOut,
	    getCreateView: getCreateView
	};
}());