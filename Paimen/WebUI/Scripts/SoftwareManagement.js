$(function main() {
    $(document).ready(function () {
        $('.dt').DataTable({
            "columns": [
                null,
                null,
                { "width": "10%" }
            ]
        });
        $(".dataTables_filter").addClass("pull-right");
    });
});