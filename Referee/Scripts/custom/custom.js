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
    $.jGrowl.defaults.position = 'bottom-right';
    $("a.saveConfiguration").bind("click", function () {
        var formData = $(this).parents("form").serialize();
        var URL = this.href;
        $.post(URL, formData, function (dt) {
            var ErrorMessage = 'Nieznany b³¹d';
            if (dt.Error == 0) {
                ErrorMessage = 'Konfiguracja poprawnie zapisana';
            } else if (dt.Error == 1) {
                ErrorMessage = dt.Message;
            }
            $.jGrowl(ErrorMessage);
        });
        return false;
    });
});