! function (o) {
	
	
	function e() {
		this.$body = o("body"), this.charts = []
	}
	e.prototype.initCharts = function () {
		window.Apex = {
			chart: {
				parentHeightOffset: 0,
				toolbar: {
					show: !1
				}
			},
			grid: {
				padding: {
					left: 0,
					right: 0
				}
			},
			colors: ["#727cf5", "#0acf97", "#fa5c7c", "#ffbc00"]
		};
		var e = ["#727cf5", "#0acf97", "#fa5c7c", "#ffbc00"],
			t = o("#revenue-chart").data("colors");
		t && (e = t.split(","));
		var r = {
			chart: {
				height: 364,
				type: "line",
				dropShadow: {
					enabled: !0,
					opacity: .2,
					blur: 7,
					left: -7,
					top: 7
				}
			},
			dataLabels: {
				enabled: !1
			},
			stroke: {
				curve: "smooth",
				width: 4
			},
			series: [{
				name: "Mes Anterior",
				data: [0, 0, 24, 7]
			}, {
				name: "Proximo Mes",
				data: [7, 24, 35,45 ]
			}],
			colors: e,
			zoom: {
				enabled: !1
			},
			legend: {
				show: !1
			},
			xaxis: {
				type: "string",
				categories: ["Aguardando", "Futura", "Pendente", "Ordens"],
				tooltip: {
					enabled: !1
				},
				axisBorder: {
					show: !1
				}
			},
			yaxis: {
				labels: {
					formatter: function (e) {
						return e + "k"
					},
					offsetX: -15
				}
			}
		};
		$(document).ready = (function countPendente() {
			
			
			
		});
		new ApexCharts(document.querySelector("#revenue-chart"), r).render();
		e = ["#727cf5", "#e3eaef"];
		(t = o("#high-performing-product").data("colors")) && (e = t.split(","));
		r = {
			chart: {
				height: 257,
				type: "bar",
				stacked: !0
			},
			plotOptions: {
				bar: {
					horizontal: !1,
					columnWidth: "20%"
				}
			},
			dataLabels: {
				enabled: !1
			},
			stroke: {
				show: !0,
				width: 2,
				colors: ["transparent"]
			},
			series: [{
				name: "Atual",
				data: [0,0, 24, 7]
			}],
			zoom: {
				enabled: !1
			},
			legend: {
				show: !1
			},
			colors: e,
			xaxis: {
				categories: ["Aguardando", "Futura", "Pendente", "Ordens"],
				axisBorder: {
					show: !1
				}
			},
			yaxis: {
				labels: {
					formatter: function (e) {
						return e + "k"
					},
					offsetX: -10
				}
			},
			fill: {
				opacity: 1
			},
			tooltip: {
				y: {
					formatter: function (e) {
						return "" + e + " - Registros"
					}
				}
			}
		};
		new ApexCharts(document.querySelector("#high-performing-product"), r).render();
		e = ["#727cf5", "#0acf97", "#fa5c7c", "#ffbc00"];
		(t = o("#average-sales").data("colors")) && (e = t.split(","));
		r = {
			chart: {
				height: 208,
				type: "donut"
			},
			legend: {
				show: !1
			},
			stroke: {
				colors: ["transparent"]
			},
			series: [44, 55, 41, 17],
			labels: ["Direct", "Affilliate", "Sponsored", "E-mail"],
			colors: e,
			responsive: [{
				breakpoint: 480,
				options: {
					chart: {
						width: 200
					},
					legend: {
						position: "bottom"
					}
				}
			}]
		};
		new ApexCharts(document.querySelector("#average-sales"), r).render()
	}, e.prototype.initMaps = function () {
		0 < o("#world-map-markers").length && o("#world-map-markers").vectorMap({
			map: "world_mill_en",
			normalizeFunction: "polynomial",
			hoverOpacity: .7,
			hoverColor: !1,
			regionStyle: {
				initial: {
					fill: "#e3eaef"
				}
			},
			markerStyle: {
				initial: {
					r: 9,
					fill: "#727cf5",
					"fill-opacity": .9,
					stroke: "#fff",
					"stroke-width": 7,
					"stroke-opacity": .4
				},
				hover: {
					stroke: "#fff",
					"fill-opacity": 1,
					"stroke-width": 1.5
				}
			},
			backgroundColor: "transparent",
			markers: [{
				latLng: [40.71, -74],
				name: "New York"
			}, {
				latLng: [37.77, -122.41],
				name: "San Francisco"
			}, {
				latLng: [-33.86, 151.2],
				name: "Sydney"
			}, {
				latLng: [1.3, 103.8],
				name: "Singapore"
			}],
			zoomOnScroll: !1
		})
	}, e.prototype.init = function () {
		o("#dash-daterange").daterangepicker({
			singleDatePicker: !0
		}), this.initCharts(), this.initMaps()
	}, o.Dashboard = new e, o.Dashboard.Constructor = e
}(window.jQuery),
	function (t) {
		"use strict";
		t(document).ready(function (e) {
			t.Dashboard.init()
		})
	}(window.jQuery);