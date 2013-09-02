var templateLoader = (function ($, host) {
    return {
        loadExtTemplate: function (name, path) {
            $.ajax({
                async: false,
                url: path,
                cache: false,
                success: function (result) {
                    $("body").append(result);
                },
                error: function (result) {
                    alert("Error Loading View/Template -- TODO: Better Error Handling");
                }
            });
        }
    };
})(jQuery, document);

$(function () {
    var views = {};
    templateLoader.loadExtTemplate("layout", "/content/views/layout.html");
    var layout = new kendo.Layout($('#layout').html());
    layout.render($("#app"));

    var router = new kendo.Router();
    var addRoute = function (route, name, path, forceRemoteLoad) {
        forceRemoteLoad = typeof forceRemoteLoad !== "undefined" ? forceRemoteLoad : false;
        router.route(route, function () {
            kendo.fx($("#body")).slideInRight().reverse().then(function () { // transition, slide view left
                var isRemotelyLoaded = false;
                if (views[name] == null || forceRemoteLoad) {   // check if we have already loaded in cache, could store this in browser local storage for larger apps
                    isRemotelyLoaded = true;
                    templateLoader.loadExtTemplate(name, path); // load the view
                    views[name] = new kendo.View(('#' + name)); // add the view to cache
                }
                layout.showIn("#body", views[name]); // switch view
                $(document).trigger("viewSwtichedEvent", { route: route, name: name, path: path, isRemotelyLoaded: isRemotelyLoaded }); // publish event view has been loaded (EventAggregator pattern)
                kendo.fx($("#body")).slideInRight().play(); // transition, slide view back to the right (center)
            });
        });
    };

    addRoute("/", "home", "/content/views/home.html");
    addRoute("/about", "about", "/content/views/about.html");
    addRoute("/contact", "contact", "/content/views/contact.html");
    addRoute("/categories", "categories", "/content/views/categories.html");
    addRoute("/customers", "customers", "/content/views/customers.html");
    addRoute("/products", "products", "/Scripts/partials/products.html");
    addRoute("/other-products", "other-products", "/Scripts/partials/other-products.html");
    addRoute("/productEdit/:id", "productEdit", "/Scripts/partials/productEdit.html");

    router.start();
});