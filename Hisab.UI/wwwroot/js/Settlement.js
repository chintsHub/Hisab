$(document).ready(function () {

        

    $('#settlementModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var friendName = button.data('friendname'); // Extract info from data-* attributes
        var friendId = button.data('friendid');
        var userid = button.data('id');
        var eventId = button.data('eventid');
        var netAmount = button.data('tranamount');

        // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
        // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
        var modal = $(this);

        modal.find('#paymentTo')[0].innerHTML = friendName;
        modal.find('#transAmount')[0].innerHTML = netAmount;
        
        modal.find('#SettlementTransaction_EventId').val(eventId);
        modal.find('#SettlementTransaction_PaidToUserId').val(friendId);
        modal.find('#SettlementTransaction_Amount').val(netAmount);


        var savebutton = modal.find('#settlementTransButton');
        var closebutton = modal.find('#settlementCloseTransButton');

        savebutton.click(function OnSaveClick(e) {
            e.preventDefault();

            $.post(this.baseURI + "?handler=SettlementTransaction", $("#settlementTransactionForm").serialize()).done(function (data) {
                //hack to prevent event firing multiple times
                var old_element = document.getElementById("settlementTransButton");
                var new_element = old_element.cloneNode(true);
                old_element.parentNode.replaceChild(new_element, old_element);

                
                if (data.success) {
                    var tabDiv = document.getElementById(data.friendId);
                    tabDiv.innerHTML = '<div class="alert alert-success" role="alert">' +  data.responseText + '</div>  ';
                    
                    var settlebutton = document.getElementById("button_" + data.friendId);
                    settlebutton.style.display = "none";

                }
               
                $('#settlementModal').modal('toggle');

            });

            removeEventListener('click', this, false);
        });

        closebutton.click(function OnSaveClick(e) {
            e.preventDefault();

            
                //hack to prevent event firing multiple times
                var old_element = document.getElementById("settlementTransButton");
                var new_element = old_element.cloneNode(true);
                old_element.parentNode.replaceChild(new_element, old_element);


                

                $('#settlementModal').modal('toggle');

            

            removeEventListener('click', this, false);
        });


    })
});