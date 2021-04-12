$(() => {

    let id = $("#like-question").data("question-id");

    $("#like-question").on('click', function () {
        $.post("/questions/Update", { id }, function (id) {
            $("#like-question").attr('class', "oi oi-heart text-danger");
        });
    });

    setInterval(() => {
        $.get("/questions/GetLikes", { id }, function (likes) {
            $("#likes-count").text(likes);
        })
    }, 500);
});