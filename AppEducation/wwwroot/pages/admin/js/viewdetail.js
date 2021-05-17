$(document).ready(function(){
    $('a.details').click(function() {
        var id = $(this).attr("id");
        console.log(id);
        $.ajax({
            url: '/Admin/detailasjson', 
            type: "GET", 
            dataType: "json",
            data: {id:id},
            success: function (data) {
                console.log(data);
                content = "<h3>User Detail</h3>";
                content += "<dl class=\"dl-horizontal\">";
                content += "<dt>Email</dt><dd>" + data.Email + "</dd>";
                content += "<dt>BirthDay</dt><dd>" + data.BirthDay + "</dd>";
                content += "<dt>PhoneNumber</dt><dd>" + data.PhoneNumber + "</dd>";
                content += "<dt>Job</dt><dd>" + data.Job + "</dd>";
                content += "<dt>Gender </dt><dd>" + data.Sex + "</dd>";
                content += " </dl>";
                $('#details').html(content);
            },
            error: function (xhr,status,error){
                alert(xhr.responseText);
                alert(status);
                alert(error);
            }
        });

    });
});