$(document).ready(function () {
    FetchUsers();

    function handleAccountTypeChange() {
        var accountType = $("#accounttypeuser").val();
        var buttonContainer = $("#additionalButtons");

        buttonContainer.empty();


        if (accountType === "1") {
            var createAdminAnchor = $("<a>").text("Create Admin").addClass("btn btn-outline-info");
            createAdminAnchor.attr("href", "/AdminDash/CreateAdminAccount");
            buttonContainer.append(createAdminAnchor);
        }
        else if (accountType === "3") {
            var createProviderAnchor = $("<a>").text("Create Provider").addClass("btn btn-outline-info");
            createProviderAnchor.attr("href", "/AdminDash/CreateProviderAccount");
            buttonContainer.append(createProviderAnchor);
        }
        else if (accountType === "0") {
            var createProviderAnchor = $("<a>").text("Create Provider").css("margin-right","5px").addClass("btn btn-outline-info");
            createProviderAnchor.attr("href", "/AdminDash/CreateProviderAccount");
            buttonContainer.append(createProviderAnchor);

            var createAdminAnchor = $("<a>").text("Create Admin").addClass("btn btn-outline-info");
            createAdminAnchor.attr("href", "/AdminDash/CreateAdminAccount");
            buttonContainer.append(createAdminAnchor);
        }
    }


    $("#accounttypeuser").on("change", handleAccountTypeChange);
    handleAccountTypeChange();
});
$("#accounttypeuser").on("change", function () {
    console.log("iiii")
    FetchUsers();
});
function FetchUsers() {
    var role = $("#accounttypeuser").val();
    $.ajax({
        method: "GET",
        url: "/AdminDash/UserAccessData",
        data: { role: role },
        success: function (response) {
            if (response.code == 401) {

                location.reload();
            }
            else {
                console.log("Function Success")
                $('#UserAccessPartial').html(response)

            }

        },
        error: function () {
            console.log("Function Fail")
        }
    })
}