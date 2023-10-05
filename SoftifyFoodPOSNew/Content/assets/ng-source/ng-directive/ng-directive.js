

ngApp.directive("select2", function ($timeout, $parse) {
    return {
        restrict: 'AC',
        require: 'ngModel',
        link: function (scope, element, attrs) {
            $timeout(function () {
                element.select2();
                element.select2Initialized = true;
            });

            var refreshSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.trigger('change');
                });
            };

            var recreateSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.select2('destroy');
                    element.select2();
                });
            };

            scope.$watch(attrs.ngModel, refreshSelect);

            if (attrs.ngOptions) {
                var list = attrs.ngOptions.match(/ in ([^ ]*)/)[1];
                // watch for option list change
                scope.$watch(list, recreateSelect);
            }

            if (attrs.ngDisabled) {
                scope.$watch(attrs.ngDisabled, refreshSelect);
            }
        }
    };
});

ngApp.directive('formatsd', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });

            elem.bind('blur', function (event) {
                var plainNumber = elem.val().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                var pla = plainNumber.replace(/,/g, "");
                elem.val($filter(attrs.format)(pla));
            });
        }
    };
}]);
ngApp.directive('enterSubmit', function () {
    return {
        restrict: 'A',
        link: function (scope, elem, attrs) {

            elem.bind('keydown', function (event) {
                var code = event.keyCode || event.which;

                if (code === 13) {
                    if (!event.shiftKey) {
                        event.preventDefault();
                        scope.$apply(attrs.enterSubmit);
                    }
                }
            });
        }
    }
});

//ngApp.directive("datepicker", function () {

//    function link(scope, element, attrs) {
//        // CALL THE "datepicker()" METHOD USING THE "element" OBJECT.

//        $('[datepicker]').each(function () {
//            var $this = $(this),
//                opts = {};

//            var pluginOptions = $this.data('plugin-options');
//            if (pluginOptions)
//                opts = pluginOptions;

//            $this.themePluginDatePicker(opts);
//        });


//    }
//    return {
//        require: 'ngModel',
//        link: link
//    };
//});
ngApp.directive('datepicker', function ($filter) {
    return {
        restrict: 'A',
        require: 'ngModel',
        compile: function () {
            return {
                pre: function (scope, element, attrs, ngModelCtrl) {
                    var format, dateObj;
                    format = (!attrs.dpFormat) ? 'dd-M-yyyy' : attrs.dpFormat;
                    if (!attrs.initDate && !attrs.dpFormat) {
                        dateObj = new Date();
                        scope[attrs.ngModel] = $filter("date")(dateObj, 'dd-MMM-yyyy')

                    } else if (!attrs.initDate) {
                        // Otherwise set as the init date
                        scope[attrs.ngModel] = attrs.initDate;
                    } else {
                    }
                    $(element).datepicker({
                        format: format,
                    }).on('changeDate', function (ev) {
                        scope.$apply(function () {
                            ngModelCtrl.$setViewValue(ev.format(format));
                        });
                    });
                }
            }
        }
    }
});

ngApp.directive("limitmax", function () {
    return {
        link: function (scope, element, attributes) {
            element.on("keydown keyup", function (e) {
                if (Number(element.val()) > Number(attributes.max) &&
                      e.keyCode != 46 // delete
                      &&
                      e.keyCode != 8 // backspace
                    ) {
                    e.preventDefault();
                    element.val(attributes.max);
                }
            });
        }
    };
});

ngApp.directive("limitprevent", function () {
    return {
        link: function (scope, element, attributes) {
            var oldVal = null;
            element.on("keydown keyup", function (e) {
                if (Number(element.val()) > Number(attributes.max) &&
                      e.keyCode != 46 // delete
                      &&
                      e.keyCode != 8 // backspace
                    ) {
                    e.preventDefault();
                    element.val(oldVal);
                } else {
                    oldVal = Number(element.val());
                }
            });
        }
    };
});
ngApp.directive('title', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).hover(function () {
                // on mouseenter
                $(element).tooltip('show');
            }, function () {
                // on mouseleave
                $(element).tooltip('hide');
            });
        }
    };
});

// ==================   TOOL-TIP USE IN ANGULAR JS ==================================

//<input type="text" class="form-control input-sm" ng-model="x.Address" readonly name="TripLocationinfo[{{$index}}].Address" title={{x.Address}} data-placement="top" data-delay="500" data-toggle="tooltip" />

/* ------------  DATE FILTER HERE ---------------------------*/

ngApp.filter("dateFilter", function () {
    return function (x) {
        return new Date(parseInt(x.substr(6)));
    };
});

/*
        ------------  HOW TO USE?  ---------------------------

        {{model.dtEntry | dateFilter | date: 'dd-MMM-yyyy'}}
*/

/*
        ------------  HOW TO USE?  ---------------------------

        {{model.dtEntry | dateFilter | date: 'dd-MMM-yyyy'}}
*/
/*<button class="btn btn-danger btn-wide btn-sm" style="height:45px; width:300px" type="button" name="btnSubmit" ng-show="PFAmount>0" ng-confirm-click="Are you sure to generate this record ?" confirmed-click="submitForm()">
                         Submit <i class="fa fa-paper-plane" aria-hidden="true"></i>
                     </button>
                     
          $scope.submitForm = function () {
          return 1;
      };
                     */

ngApp.directive('ngConfirmClick',
    [
        function() {
            return {
                link: function(scope, element, attr) {
                    var msg = attr.ngConfirmClick || "Are you sure?";
                    var clickAction = attr.confirmedClick;
                    element.bind('click',
                        function(event) {
                            if (window.confirm(msg)) {
                                scope.$eval(clickAction);
                            }
                        });
                }
            };
        }
    ]);
// ======== DUPLICATE ITEM CHECK =================

ngApp.filter('unique', function () {
    // we will return a function which will take in a collection
    // and a keyname
    return function (collection, keyname) {
        // we define our output and keys array;
        var output = [],
            keys = [];
        // we utilize angular's foreach function
        // this takes in our original collection and an iterator function
        angular.forEach(collection, function (item) {
            // we check to see whether our object exists
            var key = item[keyname];
            // if it's not already part of our keys array
            if (keys.indexOf(key) === -1) {
                // add it to our keys array
                keys.push(key);
                // push this item to our final output array
                output.push(item);

            }
        });
        // return our array which should be devoid of
        // any duplicates
        return output;
    };
});

// =========================== Grid Directive ==================================



ngGridApp.directive("select2", function ($timeout, $parse) {
    return {
        restrict: 'AC',
        require: 'ngModel',
        link: function (scope, element, attrs) {
            $timeout(function () {
                element.select2();
                element.select2Initialized = true;
            });

            var refreshSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.trigger('change');
                });
            };

            var recreateSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.select2('destroy');
                    element.select2();
                });
            };

            scope.$watch(attrs.ngModel, refreshSelect);

            if (attrs.ngOptions) {
                var list = attrs.ngOptions.match(/ in ([^ ]*)/)[1];
                // watch for option list change
                scope.$watch(list, recreateSelect);
            }

            if (attrs.ngDisabled) {
                scope.$watch(attrs.ngDisabled, refreshSelect);
            }
        }
    };
});

ngGridApp.directive('formatsd', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });

            elem.bind('blur', function (event) {
                var plainNumber = elem.val().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                var pla = plainNumber.replace(/,/g, "");
                elem.val($filter(attrs.format)(pla));
            });
        }
    };
}]);
ngGridApp.directive('enterSubmit', function () {
    return {
        restrict: 'A',
        link: function (scope, elem, attrs) {

            elem.bind('keydown', function (event) {
                var code = event.keyCode || event.which;

                if (code === 13) {
                    if (!event.shiftKey) {
                        event.preventDefault();
                        scope.$apply(attrs.enterSubmit);
                    }
                }
            });
        }
    }
});


ngGridApp.directive('datepicker', function ($filter) {
    return {
        restrict: 'A',
        require: 'ngModel',
        compile: function () {
            return {
                pre: function (scope, element, attrs, ngModelCtrl) {
                    var format, dateObj;
                    format = (!attrs.dpFormat) ? 'dd-M-yyyy' : attrs.dpFormat;
                    if (!attrs.initDate && !attrs.dpFormat) {
                        dateObj = new Date();
                        scope[attrs.ngModel] = $filter("date")(dateObj, 'dd-MMM-yyyy');

                    } else if (!attrs.initDate) {
                        // Otherwise set as the init date
                        scope[attrs.ngModel] = attrs.initDate;
                    } else {
                    }
                    $(element).datepicker({
                        format: format,
                    }).on('changeDate', function (ev) {
                        scope.$apply(function () {
                            ngModelCtrl.$setViewValue(ev.format(format));
                        });
                    });
                }
            }
        }
    }
});

ngGridApp.directive("limitmax", function () {
    return {
        link: function (scope, element, attributes) {
            element.on("keydown keyup", function (e) {
                if (Number(element.val()) > Number(attributes.max) &&
                        e.keyCode != 46 // delete
                        &&
                        e.keyCode != 8 // backspace
                ) {
                    e.preventDefault();
                    element.val(attributes.max);
                }
            });
        }
    };
});

ngGridApp.directive("limitprevent", function () {
    return {
        link: function (scope, element, attributes) {
            var oldVal = null;
            element.on("keydown keyup", function (e) {
                if (Number(element.val()) > Number(attributes.max) &&
                        e.keyCode != 46 // delete
                        &&
                        e.keyCode != 8 // backspace
                ) {
                    e.preventDefault();
                    element.val(oldVal);
                } else {
                    oldVal = Number(element.val());
                }
            });
        }
    };
});
ngGridApp.directive('title', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).hover(function () {
                // on mouseenter
                $(element).tooltip('show');
            }, function () {
                // on mouseleave
                $(element).tooltip('hide');
            });
        }
    };
});

// ==================   TOOL-TIP USE IN ANGULAR JS ==================================

//<input type="text" class="form-control input-sm" ng-model="x.Address" readonly name="TripLocationinfo[{{$index}}].Address" title={{x.Address}} data-placement="top" data-delay="500" data-toggle="tooltip" />

/* ------------  DATE FILTER HERE ---------------------------*/

ngGridApp.filter("dateFilter", function () {
    return function (x) {
        return new Date(parseInt(x.substr(6)));
    };
});

/*
        ------------  HOW TO USE?  ---------------------------

        {{model.dtEntry | dateFilter | date: 'dd-MMM-yyyy'}}
*/

/*
        ------------  HOW TO USE?  ---------------------------

        {{model.dtEntry | dateFilter | date: 'dd-MMM-yyyy'}}
*/
/*<button class="btn btn-danger btn-wide btn-sm" style="height:45px; width:300px" type="button" name="btnSubmit" ng-show="PFAmount>0" ng-confirm-click="Are you sure to generate this record ?" confirmed-click="submitForm()">
                         Submit <i class="fa fa-paper-plane" aria-hidden="true"></i>
                     </button>
                     
          $scope.submitForm = function () {
          return 1;
      };
                     */

ngGridApp.directive('ngConfirmClick',
    [
        function () {
            return {
                link: function (scope, element, attr) {
                    var msg = attr.ngConfirmClick || "Are you sure?";
                    var clickAction = attr.confirmedClick;
                    element.bind('click',
                        function (event) {
                            if (window.confirm(msg)) {
                                scope.$eval(clickAction);
                            }
                        });
                }
            };
        }
    ]);
// ======== DUPLICATE ITEM CHECK =================

ngGridApp.filter('unique', function () {
    // we will return a function which will take in a collection
    // and a keyname
    return function (collection, keyname) {
        // we define our output and keys array;
        var output = [],
            keys = [];
        // we utilize angular's foreach function
        // this takes in our original collection and an iterator function
        angular.forEach(collection, function (item) {
            // we check to see whether our object exists
            var key = item[keyname];
            // if it's not already part of our keys array
            if (keys.indexOf(key) === -1) {
                // add it to our keys array
                keys.push(key);
                // push this item to our final output array
                output.push(item);

            }
        });
        // return our array which should be devoid of
        // any duplicates
        return output;
    };
});



