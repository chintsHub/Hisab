$(document).ready(function() {

    var $registerToggleButton = $("#toggleRegister");
    var $toggleForgotPasswordButton = $("#toggleForgotPassword");


    var $registerToggleForm = $("#toggleRegisterForm");
    var $forgotPasswordToggleForm = $("#ForgotPasswordForm");


    var nickName = $("#nickName").val();
    if (nickName.length > 0) {
        $registerToggleForm.toggle(1000);
    }

    $registerToggleButton.on("click",
        function() {
            $forgotPasswordToggleForm.hide();
            $registerToggleForm.toggle(1000);

        });

    $toggleForgotPasswordButton.on("click",
        function () {
            $registerToggleForm.hide();

            $forgotPasswordToggleForm.toggle(1000);

        });



});