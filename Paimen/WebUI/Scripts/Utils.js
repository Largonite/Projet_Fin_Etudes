function hideByClass(c) {
    $("." + c).addClass("hidden");
}

function hideById(i) {
    $("#" + i).addClass("hidden");
}

function showByClass(c) {
    $("." + c).removeClass("hidden");
}

function showById(i) {
    $("#" + i).removeClass("hidden");
}