/*
Name: 			Maps / Basic - Examples
Written by: 	Okler Themes - (http://www.okler.net)
Theme Version: 	1.4.0
*/

(function( $ ) {

	'use strict';

	var initBasic = function() {
		new GMaps({
			div: '#gmap-basic',
			lat: 33.639687,
			lng: -111.994878,
            zoom:18,
            markers: [{
                lat: 33.639687,
                lng: -111.994878,
                infoWindow: {
                    content: '<p>Basic</p>'
                }
            }]
		});
        map.addMarker({
            lat: 33.639687,
            lng: -111.994878,
            infoWindow: {
                content: '<p>Example</p>'
            }
        });
	};

	var initBasicWithMarkers = function() {
		var map = new GMaps({
			div: '#gmap-basic-marker',
            lat: 33.639687,
            lng: -111.994878,
			markers: [{
                lat: 33.639687,
                lng: -111.994878,
				infoWindow: {
					content: '<p>Basic</p>'
				}
			}]
		});

		map.addMarker({
            lat: 33.639687,
            lng: -111.994878,
			infoWindow: {
				content: '<p>Example</p>'
			}
		});
	};

	var initStatic = function() {
		var url = GMaps.staticMapURL({
			size: [725, 500],
            lat: 33.639687,
            lng: -111.994878,
			scale: 1
		});

		$('#gmap-static')
			.css({
				backgroundImage: 'url(' + url + ')',
				backgroundSize: 'cover'
			});
	};

	var initContextMenu = function() {
		var map = new GMaps({
			div: '#gmap-context-menu',
            lat: 33.639687,
            lng: -111.994878
		});

		map.setContextMenu({
			control: 'map',
			options: [
				{
					title: 'Add marker',
					name: 'add_marker',
					action: function(e) {
						this.addMarker({
							lat: e.latLng.lat(),
							lng: e.latLng.lng(),
							title: 'New marker'
						});
					}
				},
				{
					title: 'Center here',
					name: 'center_here',
					action: function(e) {
						this.setCenter(e.latLng.lat(), e.latLng.lng());
					}
				}
			]
		});
	};

	var initStreetView = function() {
		var gmap = GMaps.createPanorama({
			el: '#gmap-street-view',
            lat: 33.639687,
            lng: -111.994878
		});

		$(window).on( 'sidebar-left-toggle', function() {
			google.maps.event.trigger( gmap, 'resize' );
		});
	};

	// auto initialize
	$(function() {

		initBasic();
		initBasicWithMarkers();
		initStatic();
		initContextMenu();
		initStreetView();

	});

}).apply(this, [ jQuery ]);