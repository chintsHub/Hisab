$(document).ready(function () {

  

    $("#paidByMeCheckbox").click(function (e) {
       

        // user wants to see only my transactions
        if (e.currentTarget.checked) {

            for (var transaction of document.querySelectorAll(".transaction")) {

                //if its not my transaction then set it to none
                if (transaction.dataset.ismytransaction === "False") {
                    transaction.style.display = "none";


                }

            }
        }
        else {
            for (var transaction of document.querySelectorAll(".transaction")) {

                //if its not my transaction then set it to visible
                if (transaction.dataset.ismytransaction === "False") {
                    transaction.style.display = "block";


                }

            }
        }
    })

    
});