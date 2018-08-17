$(document).ready(function () {
    $("#transactions .js-change-status").on("click",
        function () {
            bootbox.confirm("Are you sure you want to change status?",
                function(result) {
                    if (result) {
                        $.ajax({
                            url: "/api/transactions/UpdateTransactionValidationStatus?id=" + $(this).attr("data-transaction-id") + "&isValid=true",
                            method: "PUT",
                            success: function () {
                                console.log("Changed status");
                            }
                        });
                    }
                });
        });
});