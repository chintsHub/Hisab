$(document).ready(function () {

    
    var $toggleEventCreateButton = $("#toggleEvent");

   
    var $createEventForm = $("#CreateEventForm");

   
   

    $toggleEventCreateButton.on("click",
        function () {


            $createEventForm.toggle(1000);

        });

});