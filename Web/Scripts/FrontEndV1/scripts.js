$(document).ready(function () {
    $("#logoParade").smoothDivScroll({
        autoScrollingMode: "always",
        autoScrollingDirection: "endlessLoopRight",
        autoScrollingStep: 1,
        autoScrollingInterval: 25
    });
    $("#logoParade").bind("mouseover", function () {
        $(this).smoothDivScroll("stopAutoScrolling");
    }).bind("mouseout", function () {
        $(this).smoothDivScroll("startAutoScrolling");
    });
    $(document).on('click', '.search-button', function () {
        var keywords = $(this).siblings(":text").val();
        //var keywords = $('.keywords').val();
        //var root = location.protocol + '//' + location.host;
        //var url = ('@Url.RouteUrl("FrontEndSearchIndex", new { keywords = "keywordsReplacement" })').replace("keywordsReplacement", keywords);
        var url = '/tim-kiem/' + keywords;
        window.location = url;
    });
    $(document).on('keyup', '.keywords', function (e) {
        if (e.which == 13) {
            var keywords = $(this).val();
            //var url = ('@Url.RouteUrl("FrontEndSearchIndex", new { keywords = "keywordsReplacement" })').replace("keywordsReplacement", keywords);
            var url = '/tim-kiem/' + keywords;
            window.location = url;
        }
    });
});