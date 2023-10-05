using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace SoftifyFoodPOSNew
{
    public class BundleConfig
    {
        class PassthruBundleOrderer : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }
        public static void RegisterBundles(BundleCollection bundles)
        {
            string vendorUrl = "~/Content/assets/vendor/"; string javaScriptsUrl = "~/Content/assets/javascripts/";
            string gridUrl = "~/Content/assets/javascripts/ui-grid/";

            var commontopJs = new ScriptBundle("~/bundles/commonTopJs").Include(
              
                vendorUrl + "jquery-browser-mobile/jquery.browser.mobile.js",
                vendorUrl + "bootstrap/js/bootstrap.js",
                vendorUrl + "nanoscroller/nanoscroller.js",
                vendorUrl + "magnific-popup/magnific-popup.js",
                vendorUrl + "bootstrap-datepicker/js/bootstrap-datepicker.js",
                vendorUrl + "jquery-placeholder/jquery.placeholder.js",
                vendorUrl + "jquery-ui/js/jquery-ui-1.10.4.custom.js",
                vendorUrl + "jquery-ui-touch-punch/jquery.ui.touch-punch.js",
                vendorUrl + "jquery/jquery.unobtrusive-ajax.min.js",
                vendorUrl + "jquery-appear/jquery.appear.js",
                vendorUrl + "bootstrap-multiselect/bootstrap-multiselect.js");


            var dashboardJs = new ScriptBundle("~/bundles/dashboardJs").Include(
                vendorUrl + "jquery-easypiechart/jquery.easypiechart.js",
                vendorUrl + "flot/jquery.flot.js",
                vendorUrl + "flot-tooltip/jquery.flot.tooltip.js",
                vendorUrl + "flot/jquery.flot.pie.js",
                vendorUrl + "flot/jquery.flot.categories.js",
                vendorUrl + "flot/jquery.flot.resize.js",
                vendorUrl + "jquery-sparkline/jquery.sparkline.js",
                vendorUrl + "raphael/raphael.js",
                vendorUrl + "morris/morris.js",
                vendorUrl + "gauge/gauge.js",
                vendorUrl + "snap-svg/snap.svg.js",
                vendorUrl + "liquid-meter/liquid.meter.js",
                vendorUrl + "chartist/chartist.js",
                vendorUrl + "jquery-easypiechart/jquery.easypiechart.js",
                //ControllerNameP == "Dashboard"
                vendorUrl + "jquery-datatables/media/js/jquery.dataTables.js",
                vendorUrl + "jquery-datatables-bs3/assets/js/datatables.js"
                );

            var createEditIndexReportJs = new ScriptBundle("~/bundles/createEditIndexReportJs").Include(
                vendorUrl + "bootstrap-timepicker/js/bootstrap-timepicker.js",
                //vendorUrl + "select2/select2.js",
                vendorUrl + "summernote/summernote.js",
                vendorUrl + "bootstrap-maxlength/bootstrap-maxlength.js",
                vendorUrl + "bootstrap-confirmation/bootstrap-confirmation.js",
                vendorUrl + "bootstrap-multiselect/bootstrap-multiselect.js",
                //vendorUrl + "jquery/jquery.validate.unobtrusive.min.js",
                vendorUrl + "jquery-validation/jquery.validate.js",
                vendorUrl + "bootstrap-wizard/jquery.bootstrap.wizard.js",
                vendorUrl + "dropzone/dropzone.js",
                //ControllerNameP != "Dashboard" && ActionNameP == "Index" 
                vendorUrl + "bootstrap-timepicker/js/bootstrap-timepicker.js",
                vendorUrl + "bootstrap-confirmation/bootstrap-confirmation.js",
                vendorUrl + "jquery-datatables/media/js/jquery.dataTables.js",
                vendorUrl + "jquery-datatables-bs3/assets/js/datatables.js"
                );

            var commonfooterJs = new ScriptBundle("~/bundles/commonfooterJs").Include(
                vendorUrl + "pnotify/pnotify.custom.js",
                vendorUrl + "fuelux/js/spinner.js",
                javaScriptsUrl + "theme.js",
                javaScriptsUrl + "theme.init.js",
                javaScriptsUrl + "jquery.cookie.js",
                javaScriptsUrl + "flashMessage.js",
                javaScriptsUrl + "tables/examples.datatables.default.js",
                javaScriptsUrl + "ui-elements/modals.js"
                );

                var gridJs = new ScriptBundle("~/bundles/gridJs").Include(
                vendorUrl + "jquery/jquery.js",
                "~/Content/assets/ng-source/angular1.6.js",
                "~/Content/assets/ng-source/ng-vendor/ng-select2/select2.js",
                "~/Content/assets/ng-source/ng-vendor/ng-file-upload.min.js",
                "~/Content/assets/vendor/bootstrap-fileupload/bootstrap-fileupload.min.js",
                gridUrl + "4.4.7/angular/angular-touch.js",
                gridUrl + "4.4.7/angular/angular-animate.js",
                //gridUrl + "4.4.7/angular/angular-aria.js",
                gridUrl + "grunt-scripts/csv.js",
                gridUrl + "grunt-scripts/pdfmake.js",
                gridUrl + "grunt-scripts/vfs_fonts.js",
                gridUrl + "grunt-scripts/lodash.min.js",
                gridUrl + "grunt-scripts/jszip.min.js",
                gridUrl + "grunt-scripts/excel-builder.dist.js",
                gridUrl + "4.4.7/ui-grid.core.min.js",
                gridUrl + "4.4.7/ui-grid.cellnav.min.js",
                gridUrl + "4.4.7/ui-grid.selection.min.js",
                gridUrl + "4.4.7/ui-grid.auto-resize.min.js",
                gridUrl + "4.4.7/ui-grid.exporter.min.js",
                gridUrl + "4.4.7/ui-grid.selection.min.js",
                gridUrl + "4.4.7/autoFitColumns.min.js",
                gridUrl + "4.4.7/ui-grid.js",
                javaScriptsUrl + "ng-common.js"
                );

            commontopJs.Orderer = new PassthruBundleOrderer();
            bundles.Add(commontopJs);
            dashboardJs.Orderer = new PassthruBundleOrderer();
            bundles.Add(dashboardJs);
            createEditIndexReportJs.Orderer = new PassthruBundleOrderer();
            bundles.Add(createEditIndexReportJs);
            commonfooterJs.Orderer = new PassthruBundleOrderer();
            bundles.Add(commonfooterJs);
            gridJs.Orderer = new PassthruBundleOrderer();
            bundles.Add(gridJs);
            //var baseBundlecss = new StyleBundle("~/bundles/templateCSS").Include(
            //    vendorUrl + "_coreui/icons/css/coreui-icons.min.css",
            //    "~/Content/vendors/pace-progress/css/pace.min.css"
            //);
            //baseBundlecss.Orderer = new PassthruBundleOrderer();
            //bundles.Add(baseBundlecss);

            //BundleTable.EnableOptimizations = true;
        }
    }
}
