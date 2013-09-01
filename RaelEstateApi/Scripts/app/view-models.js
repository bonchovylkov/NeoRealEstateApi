/// <reference path="../libs/_references.js" />

window.vmFactory = (function () {
	var data = null;


	
	return {
		
		setPersister: function (persister) {
			data = persister
		}
	};
}());