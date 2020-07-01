$(document).ready(function () {


    var $hamburgerBttn = $("#hamburgerBtn");
    var $hamburgerMenu = $("#hamburgerMenu");

    

    $hamburgerBttn.on("click",
        function () {

            $hamburgerMenu.toggle(1000);
            

        });

});