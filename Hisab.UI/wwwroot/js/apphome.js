var HomePage = /** @class */ (function () {
    function HomePage(tgetModalUrl, tpostCreateEvent) {
        this.tgetModalUrl = tgetModalUrl;
        this.tpostCreateEvent = tpostCreateEvent;
        this.getModalUrl = tgetModalUrl;
        this.postCreateEvent = tpostCreateEvent;
    }
    HomePage.prototype.Init = function () {
        var _this = this;
        var saveButton = document.getElementById("eventSaveButton");
        saveButton.addEventListener("click", function (evt) {
            event.preventDefault();
            var sbutton = evt.target;
            sbutton.setAttribute("disabled", "true");
            sbutton.innerHTML = "<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span>Creating...";
            var frmData = new FormData(document.forms.namedItem("newEventForm"));
            var url = _this.postCreateEvent;
            //var url = "/Hisab/App/Events";
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (this.readyState === 4 && this.status === 200) {
                    var isjson = false;
                    try {
                        var res = JSON.parse(this.response);
                        isjson = true;
                        var errorDiv = document.getElementById("ErrorMessage");
                        errorDiv.innerHTML = res.errorMessage;
                        errorDiv.classList.add("alert-danger");
                        sbutton.removeAttribute("disabled");
                        sbutton.innerHTML = "Save";
                    }
                    catch (e) {
                    }
                    if (!isjson) {
                        window.location.href = this.responseURL;
                    }
                }
            };
            xhr.open('POST', url, true);
            xhr.send(frmData);
        });
    };
    HomePage.prototype.isJson = function (str) {
        try {
            JSON.parse(str);
        }
        catch (e) {
            return false;
        }
        return true;
    };
    return HomePage;
}());
//document.addEventListener('DOMContentLoaded', function () {
//    var page = new HomePage();
//    page.Init();
//}, false);
//# sourceMappingURL=AppHome.js.map