using System.Web;
using System.Web.Optimization;

namespace Kimserey.Rating.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/components").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/modernizr-*",
                        "~/Components/lightbox/js/lightbox.min.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/jquery.signalR-2.1.2.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-sanitize.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                    "~/Scripts/angular-ui-router.js",
                    "~/Scripts/angular-local-storage.min.js",
                    "~/Scripts/angular-file-upload-all.js",
                    "~/Scripts/scrollglue.js",
                    "~/Scripts/truncate.js",
                    "~/Scripts/angulartics.js",
                    "~/Scripts/angulartics-ga.js"));

            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include("~/ng/viewmodels/*.js")
                .Include("~/ng/*.js")
                .IncludeDirectory("~/ng/hub", "*.js")
                .IncludeDirectory("~/ng/filters", "*.js")
                .IncludeDirectory("~/ng/controllers", "*.js")
                .IncludeDirectory("~/ng/factories", "*.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-social.css",
                      "~/Content/font-awesome.css",
                      "~/Content/Site.css",
                      "~/Content/toastr.css",
                      "~/Components/lightbox/css/lightbox.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
