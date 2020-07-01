class HomePage {

    getModalUrl: string;
    postCreateEvent: string;

    constructor(public tgetModalUrl: string, public tpostCreateEvent: string) {
        this.getModalUrl = tgetModalUrl;
        this.postCreateEvent = tpostCreateEvent;
    }

    public Init() {
        
                   
            var saveButton = document.getElementById("eventSaveButton");
            saveButton.addEventListener("click", (evt) => {
                event.preventDefault();
                var sbutton = evt.target as HTMLElement;
                sbutton.setAttribute("disabled", "true");
                sbutton.innerHTML = "<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span>Creating...";

                var frmData = new FormData(document.forms.namedItem("newEventForm"));

                var url = this.postCreateEvent;
                //var url = "/Hisab/App/Events";
                var xhr = new XMLHttpRequest();
                xhr.onreadystatechange = function () {
                    if (this.readyState === 4 && this.status === 200) {
                        var isjson: boolean = false;

                        try {
                            var res: any = JSON.parse(this.response);
                            isjson = true;
                            var errorDiv = document.getElementById("ErrorMessage") as HTMLElement;
                            errorDiv.innerHTML = res.errorMessage;
                            errorDiv.classList.add("alert-danger");
                            sbutton.removeAttribute("disabled");
                            sbutton.innerHTML = "Save";
                        } catch (e) {

                        }

                        if (!isjson) {
                            window.location.href = this.responseURL;
                        }





                    }
                    
                };
                xhr.open('POST', url, true);
                xhr.send(frmData);



            });
        


    }

    
    isJson(str: string) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}
     
    
}

//document.addEventListener('DOMContentLoaded', function () {
//    var page = new HomePage();
//    page.Init();
    
    
//}, false);