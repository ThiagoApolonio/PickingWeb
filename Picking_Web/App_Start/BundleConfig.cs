using System.Web;
using System.Web.Optimization;

namespace Picking_Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Content/bootstrap/assets/js/vendor.min.js",
                        "~/Content/bootstrap/assets/js/app.min.js",
                        "~/Content/bootstrap/assets/js/vendor/apexcharts.min.js",
                        "~/Content/bootstrap/assets/js/vendor/jquery-jvectormap-1.2.2.min.js",
                        "~/Content/bootstrap/assets/js/vendor/jquery-jvectormap-world-mill-en.js",
                        "~/Content/bootstrap/assets/js/pages/dashboard.js",
                        "~/Content/bootstrap/assets/js/vendor/jquery.dataTables.min.js",
                        "~/Content/bootstrap/assets/js/vendor/dataTables.bootstrap5.js",
                        "~/Content/bootstrap/assets/js/vendor/dataTables.responsive.min.js",
                        "~/Scripts/bootbox.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/toastr.js",
                        "~/Scripts/Site/Globals.js"                     
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/assets/css/vendor/dataTables.bootstrap5.css",
                      "~/Content/bootstrap/assets/css/vendor/responsive.bootstrap5.css",
                      "~/Content/bootstrap/assets/css/vendor/jquery-jvectormap-1.2.2.css",             
                      "~/Content/toastr.css",
                      "~/Content/site.css"
                    ));



        }
    }
}
