$().ready(function () {

    // Dialog Link
    $('.openableDialogContenerButton').click(function () {
        var URL = $(this).attr("href");
        $('#openableDialogContener').load(URL).dialog();
        return false;
    });

    $('.confirmButton').click(function () {
        var Button = $(this);
        if (Button.hasClass("bGreen")) {
            return false;
        }
        $.ajax({
            url: this.href,
            cache: false,
            success: function (html) {
                var cl = "bGreen";
                if (html == "Error") {
                    cl = "bRed";
                }
                Button.removeClass("bDefault").addClass(cl).text(html);
            }
        });
        return false;
    });

    $("#createTeam").bind("click", function () {
        var URL = "/team/create";
        var data = $(document.forms[2]).serialize()
        $.post(URL, data, function (dt) {
            $('#openableDialogContener').html(dt);
        });
        return false;
    });
});