$(document).ready(function () {

    var avatarId = document.getElementById('UserSettingsVm_SelectedAvatarId').value;
    var li = document.getElementById(avatarId);
    var selectionDiv = $('#comboSelection');
    //Set div tag text/image
    selectionDiv.html($(li).html());

    $("#customCombobox1").click(function () {
        //Get ul tag
        var dropDwn = $('#ulcustomCombobox1');
        //Show Dropdown
        if (dropDwn.is(":visible"))
            dropDwn.hide();
        else
            dropDwn.show();
    })

    $("#ulcustomCombobox1 li").click(function (e) {

        //Get div tag
        var selectionDiv = $('#comboSelection');

        //Set div tag text/image
        selectionDiv.html($(this).html());

        //Hide Dropdown
        var dropDwn = $('#ulcustomCombobox1');
        dropDwn.hide();

        var sel = $(this).attr("id");
        document.getElementById('UserSettingsVm_SelectedAvatarId').value = sel;
        

    });
});