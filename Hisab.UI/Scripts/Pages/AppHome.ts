class HomePage {

    getModalUrl: string;
    postCreateEvent: string;

    constructor(public tgetModalUrl: string, public tpostCreateEvent: string) {
        this.getModalUrl = tgetModalUrl;
        this.postCreateEvent = tpostCreateEvent;
    }

    public Init() {
        //var eventModalButton = document.getElementById("eventModalButton");
        //eventModalButton.addEventListener("click", this.handleClick);

        var placeholder = document.getElementById("eventModalPlaceHolder");
        if (placeholder.innerHTML.length === 0) {

            var xhr = new XMLHttpRequest();
            //xhr.open("Get", "/Hisab/App/Events?handler=EventModalLoad");
            xhr.open("Get", this.getModalUrl);

            xhr.onload = (evt)=> {
                
                if (xhr.status === 200) {
                    placeholder.innerHTML = xhr.response;
                    var saveButton = document.getElementById("eventSaveButton");

                   

                    saveButton.addEventListener("click", (evt)=>{
                        event.preventDefault();

                        var frmData = new FormData(document.forms.namedItem("newEventForm"));

                        var url = this.postCreateEvent;
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
            }
            xhr.send();
        }


    }

    

     
    
}

//document.addEventListener('DOMContentLoaded', function () {
//    var page = new HomePage();
//    page.Init();
    
    
//}, false);