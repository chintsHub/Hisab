$(document).ready(function () {
   

    $('#SlickSlider').on('afterChange', function (event, slick, currentSlide, nextSlide) {
        if (currentSlide === 2) {
            var totalExp = $('#totalExpense').val();

            var selectedIndex = document.getElementById("paidBySelect").options.selectedIndex;
            var selectedItem = document.getElementById("paidBySelect").options[selectedIndex];
            var paidby = selectedItem.innerHTML;

           
            var html = "<h1> The total expense of <strong>" + totalExp + "</strong> paid by <b>" + paidby + "</b> will be equally split among: </h1>";
            html += "<ul>";


            var checkboxes = document.querySelectorAll('input[type="checkbox"]');
            for (var checkbox of checkboxes) {
                if (checkbox.checked) {
                    var label = checkbox.labels[0];
                    html += "<li>" + label.innerText + "</li>";

                   

                }

            }
            html += "</ul>";

            var confirmDiv = document.getElementById("confirmDiv")
            confirmDiv.innerHTML = html
            
            
        }
        
    });
});