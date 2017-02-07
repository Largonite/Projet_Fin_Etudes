$(function main() {

    //Change to admin connection view
    $("#AdminConnectionLink").on("click", function () {
        showByClass("AdminConnection");
        hideByClass("StudentConnection");
    });

    $("#StudentConnectionLink").on("click", function () {
        hideByClass("AdminConnection");
        showByClass("StudentConnection");
    });

});