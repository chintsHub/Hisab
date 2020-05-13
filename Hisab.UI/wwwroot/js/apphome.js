var HomePage = /** @class */ (function () {
    function HomePage(tgetModalUrl, tpostCreateEvent) {
        this.tgetModalUrl = tgetModalUrl;
        this.tpostCreateEvent = tpostCreateEvent;
        this.getModalUrl = tgetModalUrl;
        this.postCreateEvent = tpostCreateEvent;
    }
    HomePage.prototype.Init = function () {
        //var eventModalButton = document.getElementById("eventModalButton");
        //eventModalButton.addEventListener("click", this.handleClick);
        var _this = this;
        var placeholder = document.getElementById("eventModalPlaceHolder");
        if (placeholder.innerHTML.length === 0) {
            var xhr = new XMLHttpRequest();
            //xhr.open("Get", "/Hisab/App/Events?handler=EventModalLoad");
            xhr.open("Get", this.getModalUrl);
            xhr.onload = function (evt) {
                if (xhr.status === 200) {
                    placeholder.innerHTML = xhr.response;
                    var saveButton = document.getElementById("eventSaveButton");
                    saveButton.addEventListener("click", function (evt) {
                        event.preventDefault();
                        var frmData = new FormData(document.forms.namedItem("newEventForm"));
                        var url = _this.postCreateEvent;
                        //var url = "/Hisab/App/Events";
                        var xhr = new XMLHttpRequest();
                        xhr.onreadystatechange = function () {
                            if (this.readyState === 4 && this.status === 200) {
                                window.location.href = this.responseURL;
                            }
                            if (this.status === 500) {
                                document.getElementById("eventModalPlaceHolder").innerHTML = this.response;
                            }
                        };
                        xhr.open('POST', url, true);
                        xhr.send(frmData);
                    });
                }
                else {
                    alert("failure");
                }
            };
            xhr.send();
        }
    };
    return HomePage;
}());
//document.addEventListener('DOMContentLoaded', function () {
//    var page = new HomePage();
//    page.Init();
//}, false);
//# sourceMappingURL=AppHome.js.map