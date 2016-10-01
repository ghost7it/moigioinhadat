using System.Web;
using System.Web.Optimization;
namespace Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.FileSetOrderList.Clear();
            #region AdminV1 (Admin Version One)
            //Start Script
            //IMPORTANT! Load jquery-ui-1.10.3.custom.min.js before bootstrap.min.js to fix bootstrap tooltip conflict with jquery ui tooltip
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ie").Include(
                "~/Scripts/AdminV1/respond.min.js",
                "~/Scripts/AdminV1/excanvas.min.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/jquery").Include(
                "~/Scripts/AdminV1/jquery-1.11.0.min.js",
                "~/Scripts/AdminV1/jquery-migrate-1.2.1.min.js",
                "~/Scripts/AdminV1/jquery-ui-1.10.3.custom.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/bootstrap").Include(
                "~/Scripts/AdminV1/bootstrap.min.js",
                "~/Scripts/AdminV1/bootstrap-hover-dropdown.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/slimscroll").Include("~/Scripts/AdminV1/jquery.slimscroll.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/blockui").Include("~/Scripts/AdminV1/jquery.blockui.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/cokie").Include("~/Scripts/AdminV1/jquery.cokie.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/uniform").Include("~/Scripts/AdminV1/jquery.uniform.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/bootstrap-switch").Include("~/Scripts/AdminV1/bootstrap-switch.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/init").Include(
                "~/Scripts/AdminV1/adminv1.js",
                "~/Scripts/AdminV1/layout.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/demo").Include("~/Scripts/AdminV1/demo.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/flot").Include(
                "~/Scripts/AdminV1/jquery.flot.min.js",
                "~/Scripts/AdminV1/jquery.flot.resize.min.js",
                "~/Scripts/AdminV1/jquery.flot.categories.min.js"
                ));
            

            bundles.Add(new ScriptBundle("~/bundles/adminv1/select2").Include("~/Scripts/AdminV1/select2.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/datatable").Include(
                "~/Scripts/AdminV1/jquery.dataTables.min.js",
                "~/Scripts/AdminV1/dataTables.bootstrap.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/datatable-customize").Include("~/Scripts/AdminV1/datatable.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/datatable-ajax-source").Include("~/Scripts/AdminV1/datatable-ajax-source.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/datatable-none-ajax-source").Include("~/Scripts/AdminV1/datatable-none-ajax-source.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/adminv1/datepicker").Include("~/Scripts/AdminV1/bootstrap-datepicker.js"));            

            bundles.Add(new ScriptBundle("~/bundles/adminv1/datetimepicker").Include("~/Scripts/AdminV1/bootstrap-datetimepicker.min.js"));
            //Đoạn tùy chỉnh
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ecommerce-index").Include("~/Scripts/AdminV1/ecommerce-index.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ecommerce-orders").Include("~/Scripts/AdminV1/ecommerce-orders.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ecommerce-orders-view").Include("~/Scripts/AdminV1/ecommerce-orders-view.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ecommerce-products").Include("~/Scripts/AdminV1/ecommerce-products.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ecommerce-products-edit").Include("~/Scripts/AdminV1/ecommerce-products-edit.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ui-general").Include("~/Scripts/AdminV1/ui-general.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ui-tree").Include("~/Scripts/AdminV1/ui-tree.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/ui-blockui").Include("~/Scripts/AdminV1/ui-blockui.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/login").Include("~/Scripts/AdminV1/login.js"));
            //End đoạn tùy chỉnh

            bundles.Add(new ScriptBundle("~/bundles/adminv1/maxlength").Include("~/Scripts/AdminV1/bootstrap-maxlength.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/touchspin").Include("~/Scripts/AdminV1/bootstrap.touchspin.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/fancybox").Include("~/Scripts/AdminV1/jquery.fancybox.pack.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/plupload").Include("~/Scripts/AdminV1/plupload/js/plupload.full.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/pulsate").Include("~/Scripts/AdminV1/jquery.pulsate.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/bootpag").Include("~/Scripts/AdminV1/jquery.bootpag.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminv1/holder").Include("~/Scripts/AdminV1/holder.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/jstree").Include("~/Scripts/AdminV1/jstree.min.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/adminv1/validate").Include("~/Scripts/AdminV1/jquery_validate_113.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/jqueryval").Include(
                        "~/Scripts/adminv1/jquery.unobtrusive*",
                        "~/Scripts/adminv1/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/bootstrap-fileinput").Include("~/Scripts/AdminV1/bootstrap-fileinput.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminv1/moment").Include("~/Scripts/AdminV1/moment.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/adminv1/xxx").Include("~/Scripts/AdminV1/xxx.js"));
            //bundles.Add(new ScriptBundle("~/bundles/adminv1/xxx").Include("~/Scripts/AdminV1/xxx.js"));
            //bundles.Add(new ScriptBundle("~/bundles/adminv1/xxx").Include("~/Scripts/AdminV1/xxx.js"));
            //bundles.Add(new ScriptBundle("~/bundles/adminv1/xxx").Include("~/Scripts/AdminV1/xxx.js"));
            //bundles.Add(new ScriptBundle("~/bundles/adminv1/xxx").Include("~/Scripts/AdminV1/xxx.js"));
            //bundles.Add(new ScriptBundle("~/bundles/adminv1/xxx").Include("~/Scripts/AdminV1/xxx.js"));
            //bundles.Add(new ScriptBundle("~/bundles/adminv1/xxx").Include("~/Scripts/AdminV1/xxx.js"));

            //End Script
            //Start CSS
            bundles.Add(new StyleBundle("~/Content/adminv1/font-awesome").Include("~/Content/AdminV1/font-awesome.min.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/simple-line-icons").Include("~/Content/AdminV1/simple-line-icons.min.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/bootstrap").Include("~/Content/AdminV1/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/uniform").Include("~/Content/AdminV1/uniform.default.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/bootstrap-switch").Include("~/Content/AdminV1/bootstrap-switch.min.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/components").Include("~/Content/AdminV1/components.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/plugins").Include("~/Content/AdminV1/plugins.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/layout").Include("~/Content/AdminV1/layout.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/default").Include("~/Content/AdminV1/default.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/custom").Include("~/Content/AdminV1/custom.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/select2").Include("~/Content/AdminV1/select2.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/datatable").Include("~/Content/AdminV1/dataTables.bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/adminv1/datepicker").Include("~/Content/AdminV1/datepicker.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/datetimepicker").Include("~/Content/AdminV1/datetimepicker.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/fancybox").Include("~/Content/AdminV1/jquery.fancybox.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/style").Include("~/Content/AdminV1/style.min.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/login3").Include("~/Content/AdminV1/login3.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/bootstrap-fileinput").Include("~/Content/AdminV1/bootstrap-fileinput.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/profile").Include("~/Content/AdminV1/profile.css"));

            bundles.Add(new StyleBundle("~/Content/adminv1/error").Include("~/Content/AdminV1/error.css"));

            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //bundles.Add(new StyleBundle("~/Content/adminv1/xxx").Include("~/Content/AdminV1/xxx.css"));
            //End CSS
            #endregion
            #region Dùng chung
            bundles.Add(new ScriptBundle("~/bundles/noty").Include("~/Scripts/noty/packaged/jquery.noty.packaged.js"));
            #endregion
            #region Trang chủ
            //Start Script
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/FrontEndV1/jquery-1.11.0.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bxslider").Include("~/Scripts/FrontEndV1/jquery.bxslider.min.js", "~/Scripts/FrontEndV1/bxslider.js"));
            bundles.Add(new ScriptBundle("~/bundles/slider").Include("~/Scripts/FrontEndV1/slider.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/smoothdivscroll/js/jquery-ui-1.10.3.custom.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/smoothdivscroll").Include(
                "~/Scripts/smoothdivscroll/js/jquery.mousewheel.min.js",
                "~/Scripts/smoothdivscroll/js/jquery.kinetic.min.js",
                "~/Scripts/smoothdivscroll/js/jquery.smoothdivscroll-1.3-min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/FrontEndV1/jquery.unobtrusive*",
                        "~/Scripts/FrontEndV1/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/FrontEndV1/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/additional").Include("~/Scripts/FrontEndV1/additional-methods.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-wizard").Include("~/Scripts/FrontEndV1/jquery.bootstrap.wizard.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-fileinput").Include("~/Scripts/FrontEndV1/bootstrap-fileinput.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval2").Include("~/Scripts/FrontEndV1/jquery.validate.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include("~/Scripts/FrontEndV1/bootstrap-datepicker.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include("~/Scripts/FrontEndV1/scripts.js"));            
            //
            //End Script
            //Start CSS
            bundles.Add(new StyleBundle("~/Content/style").Include("~/Content/FrontEndV1/style.css"));
            bundles.Add(new StyleBundle("~/Content/bxslider").Include("~/Content/FrontEndV1/jquery.bxslider.css"));
            bundles.Add(new StyleBundle("~/Content/font").Include("~/Content/FrontEndV1/fonts/font.css"));
            bundles.Add(new StyleBundle("~/Content/slider").Include("~/Content/FrontEndV1/slider.css"));
            bundles.Add(new StyleBundle("~/Content/smoothdivscroll").Include("~/Scripts/smoothdivscroll/css/smoothDivScroll.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/FrontEndV1/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrap-fileinput").Include("~/Content/FrontEndV1/bootstrap-fileinput.css"));
            bundles.Add(new StyleBundle("~/Content/datepicker").Include("~/Content/FrontEndV1/datepicker.css"));
            bundles.Add(new StyleBundle("~/Content/font-awesome").Include("~/Content/FrontEndV1/font-awesome.min.css"));
            bundles.Add(new StyleBundle("~/Content/components").Include("~/Content/FrontEndV1/components.css"));
            bundles.Add(new StyleBundle("~/Content/plugins").Include("~/Content/FrontEndV1/plugins.css"));
            bundles.Add(new StyleBundle("~/Content/custom").Include("~/Content/FrontEndV1/custom.css"));
            bundles.Add(new StyleBundle("~/Content/category1").Include("~/Content/FrontEndV1/category1.css"));
            bundles.Add(new StyleBundle("~/Content/category2").Include("~/Content/FrontEndV1/category2.css"));
            bundles.Add(new StyleBundle("~/Content/category3").Include("~/Content/FrontEndV1/category3.css"));
            bundles.Add(new StyleBundle("~/Content/category4").Include("~/Content/FrontEndV1/category4.css"));
            bundles.Add(new StyleBundle("~/Content/category5").Include("~/Content/FrontEndV1/category5.css"));
            //End CSS
            #endregion
        }
    }
}
