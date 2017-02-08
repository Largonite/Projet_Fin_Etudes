$(function main() {
    $(".edit").on("click", function () {
        var tr = $(this).closest("tr");
        var td = $(this).closest("td");
        $(td).find("button").each(function () {
            if ($(this).hasClass("hidden")) {
                $(this).removeClass("hidden");
            } else {
                $(this).addClass("hidden");
            }
        })

        $(tr).children().each(function () {
            var temp = this.innerHTML;
            if (!$(this).hasClass("noInput")) {
                if ($(this).attr("isid") == "true") {
                    this.innerHTML = '<input name="' + $(this).attr("name") + '" class="form-control" type="hidden" value="' + temp + '" size=' + temp.length + '>' + temp + '</input>'
                } else {
                    this.innerHTML = '<div class="form-group"><input name="' + $(this).attr("name") + '" class="form-control" type="text" value="' + temp + '" size=' + temp.length + '></input></div>'
                }
            }
        });

        $(".edit").attr("disabled", "disabled");
        $(".remove").attr("disabled", "disabled");
    });

    $(".cancel").on("click", function () {
        var tr = $(this).closest("tr");
        var td = $(this).closest("td");
        $(td).find("button").each(function () {
            if ($(this).hasClass("hidden")) {
                $(this).removeClass("hidden");
            } else {
                $(this).addClass("hidden");
            }
        })


        $(tr).children().each(function () {
            var input = $(this).find("input")[0];
            if (input) {
                var temp = input.value;
                if (!$(this).hasClass("noInput")) {
                    this.innerHTML = temp;
                }
            }
        });
        $(".edit").removeAttr("disabled");
        $(".remove").removeAttr("disabled");
    });

    $(".submitButton").on("click", function () {
        save($(this));
    });
});

function confirmDelete(msg, location) {
    if (confirm(msg) === true) {
        document.location.href = location;
    }
}

function save(button) {
    var tr = button.closest("tr");
    var data = {};
    $(tr).find("input").each(function () {
        data[$(this).attr("name")] = $(this).val();
    });

    var url = button.attr("submitUrl");
    console.log(url);

    $.ajax({
        type: "POST",
        url: url,
        dataType: "html",
        data: data,
        success: function (e) {
            console.log("success : \n" + e);
            window.location = e;
        },
        error: function (e) {
            console.log("error edit : " + e);
        }
    });
}