$(document).ready(function () {
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
                                console.log("Changed status");
                            }
                        });
                    }
                });
        });
});