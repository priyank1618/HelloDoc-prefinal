$(document).ready(function () {
    FetchBusiness();
    $("#profession").on("change", function () {
        console.log("iiii")
        FetchBusiness();
    });

    $("#textSearch").on("input", function () {
        console.log("iiii")
        FetchBusiness();
    });



    function FetchBusiness() {

        var Profession = $("#profession").val();
        var searchValue = $("#textSearch").val();
        $.ajax({
            method: "GET",
            url: "/AdminDash/GetBusinessInfo",
            data: { Profession: Profession, searchValue: searchValue },
            success: function (response) {
                if (response.code == 401) {

                    location.reload();
                }
                else {

                    $('#PartnerPartial').html(response)

                }

            },
            error: function () {
                alert("fail")
            }
        })
    }
});
