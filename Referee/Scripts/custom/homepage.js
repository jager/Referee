$().ready(function () {


    $("a.openSaveScorePopup").live("click", function () {
        var URL = $(this).attr("href").toString();
        $.ajax({
            url: URL,
            success: function (data) {
                $("#openableDialogContener").html(data).dialog();
            }
        });
        return false;
    });



    /* Zaciąganie nowych nominacji w czasie rzeczywistym tzn co 10 minut*/
    var NominationTable = $("table#NominationTable");


    function UpdateNominationTable() {
        $.getJSON('/Nomination/GetCurrentNominations', function (data) {
            var items = [];
            $.each(data, function (key, val) {
                var row = '<td>' + val.Ident + '</td><td>' + val.Name + '</td><td>' + val.Venue + '</td><td>' + val.Date + '</td><td>' + val.Time + '</td>';
                if (val.Type == "game") {
                    if (val.Score != "") {
                        row += '<td>' + val.Score + '</td>';
                    } else {
                        row += '<td><a href="/Game/SaveScore/' + val.Ident + '" class="buttonS bBlue openSaveScorePopup">Dodaj wynik</a></td>';
                    }
                } else {
                    row += '<td></td>';
                }

                items.push('<tr>' + row + '</tr>');
            });
            NominationTable.children("tbody").replaceWith("<tbody>" + items.join('') + "</tbody>");
        });
    }

    if (NominationTable) {
        UpdateNominationTable();
        setInterval(function () {
            UpdateNominationTable()
        }, 600000);
    }

    


});