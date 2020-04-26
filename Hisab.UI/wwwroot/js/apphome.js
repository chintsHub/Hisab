var HomePage = /** @class */ (function () {
    function HomePage() {
    }
    HomePage.prototype.Init = function () {
        var eventModalButton = document.getElementById("eventModalButton");
        eventModalButton.addEventListener("click", this.handleClick);
    };
    HomePage.prototype.handleClick = function () {
        var placeholder = document.getElementById("eventModalPlaceHolder");
        if (placeholder.innerHTML.length === 0) {
            var xhr = new XMLHttpRequest();
            xhr.open("Get", "/Hisab/App/Events?handler=EventModalLoad");
            xhr.onload = function () {
                if (xhr.status === 200) {
                    placeholder.innerHTML = xhr.response;
                    var saveButton = document.getElementById("eventSaveButton");
                    saveButton.addEventListener("click", function () {
                        event.preventDefault();
                        var frmData = new FormData(document.forms.namedItem("newEventForm"));
                        var url = "/Hisab/App/Events"; //this.baseURI;
                        var xhr = new XMLHttpRequest();
                        xhr.onreadystatechange = function () {
                            if (this.readyState === 4 && this.status === 200) {
                                alert('Posted using XMLHttpRequest');
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
document.addEventListener('DOMContentLoaded', function () {
    var page = new HomePage();
    page.Init();
}, false);
//# sourceMappingURL=AppHome.js.map