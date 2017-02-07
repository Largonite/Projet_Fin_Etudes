$(function () {
    typeOfUserChanged("Guest");
    $("#typeOfUser").change(function () {
        typeOfUserChanged($(this).val());
    });
});

function typeOfUserChanged(type) {
    hideById("section");
    hideById("year");
    hideById("regNumber");
    if (type==="Student") {
        showById("section");
        showById("year");
        showById("regNumber");
    }

    if (type==="Admin") {
        showById("regNumber");
    }
}
