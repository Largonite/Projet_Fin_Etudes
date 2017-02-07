$(function main() {

    //Change to admin connection view
    $("#AdminConnection").on("click", function () {
        showById("tView");
        hideById("AdminConnection");
        showById("StudentConnection");
    });

    $("#StudentConnection").on("click", function () {
        hideById("tView");
        showById("AdminConnection");
        hideById("StudentConnection");
    });

});