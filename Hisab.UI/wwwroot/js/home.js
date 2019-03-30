$(document).ready(function() {

    var $registerToggle = $("#toggleRegister");
    
    var $popupForm = $(".popupForm");
    

    $registerToggle.on("click",
        function() {

            $popupForm.toggle(1000);

        });

});