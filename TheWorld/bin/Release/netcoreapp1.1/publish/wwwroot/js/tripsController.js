// tripsController.js
(function () {

	"use strict";

	// Reference the Module
	angular.module("app-trips")
		.controller("tripsController", tripsController);

	function tripsController($http) { // Injection 
		var vm = this;
		vm.trips = [];
		vm.newTrip = {};

		vm.errorMessage = "";
		vm.isBusy = true;

		$http.get("/api/trips")
			.then(function (response) {
				// Success
				angular.copy(response.data, vm.trips);
			}, function (error) {
				// Failure
				vm.errorMessage = "Failed to load data: " + error;
			})
			.finally(function () {
				vm.isBusy = false;
			});
		

		vm.addTrip = function () {
			vm.isBusy = true;
			vm.errorMessage = "";

			$http.post("/api/trips", vm.newTrip)
				.then(function (response) {
					// Success
					vm.trips.push(response.data);
					vm.newTrip = {};
				}, function () {
					// Failure
					vm.errorMessage = "Failed to save new trip";
				})
				.finally(function () {
					vm.isBusy = false;
				});

		};
	}

})();