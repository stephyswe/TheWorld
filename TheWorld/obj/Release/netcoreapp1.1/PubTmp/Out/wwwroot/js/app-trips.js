// app-trips.js
(function () {
	"use strict";

	// Define the Module
	angular.module("app-trips", ["simpleControls", "ngRoute"])
		.config(function ($routeProvider) {

			// Root of client-side router
			$routeProvider.when("/", {
				controller: "tripsController",
				controllerAs: "vm",
				templateUrl: "/views/tripsView.html"
			}); 

			$routeProvider.when("/editor/:tripName", { 
				controller: "tripEditorController",
				controllerAs: "vm",
				templateUrl: "/views/tripEditorView.html"
			});

			// If no Routes match 
			$routeProvider.otherwise({ redirectTo: "/" });
		});
})();