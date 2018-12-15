function showhide(id) {
            var e = document.getElementById(id);
            e.style.display = (e.style.display == 'block') ? 'none' : 'block';
        }

$('.btn').click(function () {
    var $this = $(this);
    $this.toggleClass('btn');
    if ($this.hasClass('btn')) {
        $this.text('In');
        $.ajax({
            type: "POST",
            url: '@Url.Action("Update", "Attendance")'
        });
    } else {
        $this.text('Out');
        $.ajax({
            type: "POST",
            url: '@Url.Action("Create", "Attendance")'
        });
    }
});

$(document).ready(function () {
    $('#example').DataTable();
});


function getVal(sel) {
    if (sel.value == 'Day') {
        //alert(sel.value);
        showhide('wizard');
    }
    else if (sel.value == 'Week') {
        showhide('wizard');
    }
    else if (sel.value == 'Month') {
        showhide('wizard');
    }

}