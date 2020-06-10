$(document).ready(function () {

    
    $.ajax({
        type: "GET",
        url: this.baseURI + "?handler=MyExpense",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            var spinner = document.getElementById("expenseSpinner");
            var expenseContent = document.getElementById("expenseContent");
            spinner.style.display = "none";
            expenseContent.innerHTML = response.balance;
        },
        failure: function (response) {
            //handle the error
        }
    });

    $.ajax({
        type: "GET",
        url: this.baseURI + "?handler=AllExpense",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            var spinner = document.getElementById("AllexpenseSpinner");
            var expenseContent = document.getElementById("AllexpenseContent");
            spinner.style.display = "none";
            expenseContent.innerHTML = response.totalExpense;
        },
        failure: function (response) {
            //handle the error
        }
    });

    $.ajax({
        type: "GET",
        url: this.baseURI + "?handler=AmountIOweToFriends",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            var spinner = document.getElementById("AmountIOweToFriendsSpinner");
            var expenseContent = document.getElementById("AmountIOweToFriendsContent");
            spinner.style.display = "none";
            expenseContent.innerHTML = response.amountIOwe;
        },
        failure: function (response) {
            //handle the error
        }
    });

    $.ajax({
        type: "GET",
        url: this.baseURI + "?handler=AmountFriendsOweToMe",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            var spinner = document.getElementById("AmountFriendsOweToMeSpinner");
            var expenseContent = document.getElementById("AmountFriendsOweToMeContent");
            spinner.style.display = "none";
            expenseContent.innerHTML = response.amountFriendsOwe;
        },
        failure: function (response) {
            //handle the error
        }
    });

    $.ajax({
        type: "GET",
        url: this.baseURI + "?handler=MyContributions",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            var spinner = document.getElementById("MyContributionsSpinner");
            var expenseContent = document.getElementById("MyContributionsContent");
            spinner.style.display = "none";
            expenseContent.innerHTML = response.myContributions;
        },
        failure: function (response) {
            //handle the error
        }
    });

    $.ajax({
        type: "GET",
        url: this.baseURI + "?handler=EventAccountBalance",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            var spinner = document.getElementById("EventAccountBalanceSpinner");
            var expenseContent = document.getElementById("EventAccountBalanceContent");
            spinner.style.display = "none";
            expenseContent.innerHTML = response.eventBalance;
        },
        failure: function (response) {
            //handle the error
        }
    });
});