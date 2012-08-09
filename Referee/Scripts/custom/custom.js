$().ready(function () {

    // Dialog Link
    $('.openableDialogContenerButton').click(function () {
        var URL = $(this).attr("href");
        $('#openableDialogContener').load(URL).dialog();
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