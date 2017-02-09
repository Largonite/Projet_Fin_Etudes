$(function () {
    $(document).ready(function () {
        $('.dt').DataTable();
        $(".dataTables_filter").addClass("pull-right");

    });

    

    typeOfUserChanged("Guest");
    $("#typeOfUser").change(function () {
        typeOfUserChanged($(this).val());
    });
   
    $(".sectionTableRowData").on("click", function () {
        $(this).parent().find(":input").each(function () {
            $(this).attr("checked", "checked");
        });
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
