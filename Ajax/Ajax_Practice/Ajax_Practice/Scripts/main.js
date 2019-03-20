$(document).ready(function () {

    // Jquery Ajax yazılışı

    $("select[name='MarkaId']").change(function () {
        if ($(this).val() != "0") {
            $.ajax({
                url: "/home/modelsjson/" + $(this).val(),
                type: "get",
                dataType: "json",
                success: function (data) {
                    $("select[name='ModelId']").empty();
                    $.each(data, function (index, item) {
                        $("select[name='ModelId']").append(`<option value="${item.Id}">${item.Name}</option>`)
                    });
                },
                error: function () {
                    console.error("Response qayitmadi");
                }
            });
        } else {
            $("select[name='ModelId']").empty();
        }
    })

});