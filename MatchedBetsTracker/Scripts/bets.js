$(document).ready(function() {
    var table = $("#bets").DataTable();

    $("#bets").on("click", ".js-change-status",
            function () {
                var button = $(this);

                bootbox.prompt({
                    title: "Choose bet status:",
                    inputType: 'checkbox',
                    inputOptions: [
                        {
                            text: 'Open',
                            value: '1',
                        },
                        {
                            text: 'Won',
                            value: '2',
                        },
                        {
                            text: 'Loss',
                            value: '3',
                        }
                    ],
                    callback: function (result) {
                        if (result) {
                            $.ajax({
                                url: "/api/bets/UpdateBetStatus?id=" + button.attr("data-bet-id") + "&status=" + result,
                                method: "PUT",
                                success: function () {

                                    button.html("????");

                                    table
                                        .row(button.parents("tr"))
                                        .invalidate()
                                        .draw();
                                }
                            });
                        }
                        
                    }
                });

            });


});


$(document).ready(function () {
    var table = $("#sportEvents").DataTable();   
});