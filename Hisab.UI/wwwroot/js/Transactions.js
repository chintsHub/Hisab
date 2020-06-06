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
    });

    $('#tranDeleteModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var transDesc = button.data('trandescription'); // Extract info from data-* attributes
        var transAmount = button.data('tranamount');
        var id = button.data('id');
        var eventId = button.data('eventid');

        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        var modal = $(this);

        modal.find('#transDesc')[0].innerHTML = transDesc;
        modal.find('#transAmount')[0].innerHTML = transAmount;
        modal.find('#DeleteTransactionVm_TransactionId').val(id);
        modal.find('#DeleteTransactionVm_EventId').val(eventId);




        var savebutton = modal.find('#deleteTransButton');


        savebutton.click(function OnSaveClick(e) {
            e.preventDefault();
            
            $.post(this.baseURI +"?handler=DeletTransaction", $("#deleteTransactionForm").serialize()).done(function (data) {
                //hack to prevent event firing multiple times
                var old_element = document.getElementById("deleteTransButton");
                var new_element = old_element.cloneNode(true);
                old_element.parentNode.replaceChild(new_element, old_element);

                //remove the transaction from UI
                var transId = document.getElementById("DeleteTransactionVm_TransactionId").value;
                var transDiv = document.getElementById(transId);
                transDiv.remove();
                $('#tranDeleteModal').modal('toggle');

            });

            removeEventListener('click', this, false);
        });


    })
});