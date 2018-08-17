$(document).ready(function () {
    var table = $("#transactions").DataTable();

    $("#transactions").on("click", ".js-change-status",
        function () {
            var button = $(this);

            bootbox.confirm("Are you sure you want to change status?",
                function(result) {
                    if (result) {
                        $.ajax({
                            url: "/api/transactions/UpdateTransactionValidationStatus?id=" + button.attr("data-transaction-id") + "&isValid=true",
                            method: "PUT",
                            success: function () {
/*
                                var row = table.row(button.parents("tr"));
                                var data = row.data();
                                console.log(data);
                                data[6] = "True";
                                data[6].html("Updated");
                                row.invalidate();
                                table.draw();
                                console.log(data);

                                table
                                    .row(button.parents("tr"))
                                    .invalidate()
                                    .draw()
*/
                                ///MMMM non sono sicuro sia la cosa migliore.... Non vado a modificare if dati sotto, ma in quel modo non funziona...
                                //Per il momento lo tengo così
                                button.html("True");
                                
                                table
                                    .row(button.parents("tr"))
                                    .invalidate()
                                    .draw();
                            }
                        });
                    }
                });
        });
});