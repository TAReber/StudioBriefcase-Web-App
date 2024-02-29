const localStorage_Language = 'user_language';

//Function called when the Menu Language is changed
function UpdateLanguage() {

    //Get all Links to Razor Pages that dynamically load Lanaguge specific content
    let LibraryLinks = document.querySelectorAll("#language-parameter");

    let savedText = localStorage.getItem(localStorage_Language);
    let currentUrl = window.location.href;

    let selection = document.getElementById("menu-language");
    let optionText = selection.options[selection.selectedIndex].text;

    //if Changed to Master, means the links have lanaguage parameters on the end
    if (selection.value == 0) {
        //Clear Local Storage
        localStorage.removeItem(localStorage_Language);
        
        //Remove appended Language
        if (savedText != null) {
            let object = JSON.parse(savedText);


            //If I'm in a area that supports different languages, I can find the correct url and reload the page.
            if (currentUrl.includes("/Library")) {

                let baseurl = currentUrl.split('/' + object.text)[0];

                window.location.href = baseurl;
            }
            else { // else I need to update the links on the page

                LibraryLinks.forEach(function (link) {

                    if (link.href.endsWith(object.text)) {
                        console.log(object);
                        link.href = link.href.slice(0, -('/' + object.text).length);

                    }
                });


            }

        }
        
    }
    else {
        //Reset Local Storage
        let option = {
            val: selection.value,
            text: optionText
        }
        localStorage.setItem(localStorage_Language, JSON.stringify(option));

        //If Saved Text is null, Then was set to master and I can add language Parameter to all links
        if (savedText == null) {

            if (currentUrl.includes("/Library")) {
                let newurl = currentUrl + '/' + optionText;
                window.location.href = newurl;
            }
            else {
                LibraryLinks.forEach(function (link) {
                    link.href = link.href + '/' + optionText;

                });
            }

        }
        else { //Replace language Parameter with new selection


            if (currentUrl.includes("/Library")) {

                if (savedText != null) {
                    let object = JSON.parse(savedText);
                    let baseurl = currentUrl.split('/' + object.text)[0];
                    
                    baseurl = baseurl + '/' + optionText;                   
                    window.location.href = baseurl;
                }

                //let newurl = currentUrl + '/' + optionText;
                //window.open(newurl);
            }
            else {
                LibraryLinks.forEach(function (link) {
                    if (savedText != null) {
                        let object = JSON.parse(savedText);

                        if (link.href.endsWith(object.text)) {
                            link.href = link.href.replace(object.text, option.text);

                        }

                    }

                });
            }         
        }
    } 
}



function AppendLanguageNameParameter() {
    let language_selection = localStorage.getItem(localStorage_Language);
    if (language_selection != null) {
        let option = JSON.parse(language_selection);
        document.getElementById("menu-language").value = option.val;
        
        let LibraryLinks = document.querySelectorAll("#language-parameter");
        LibraryLinks.forEach(function (link) {
            
            if (link.href.endsWith(option.text) == false) {
                link.href = link.href + '/' + option.text;
            }
            
        });

    }
}


document.addEventListener("DOMContentLoaded", () => {

    AppendLanguageNameParameter();

});


function updateUserRole(event) {
    event.stopPropagation();

    var selectedRole = document.getElementById('Menu_RoleDropdown').value;
    var userId = "test";

    fetch('/api/user/UpdateUserClass', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Id: userId,
            userClass: selectedRole
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.json();
        })
        .then(data => {
            console.log(data);
        })
        .catch(error => {
            console.error("Fetch Operation failed in _menuPartial.cshtml:", error);
        });
}




document.addEventListener('DOMContentLoaded', () => {

    CreateMiniMapSelectors();

});

function CreateMiniMapSelectors() {

    //Load Cached Values
    let ids = {
        CategoryID: 0,
        LibraryID: 0,
        SubjectID: 0,
        TopicID: 0
    }

    let object = {
        map: ids,
        LanguageID: document.getElementById('menu-language').value
    }

    fetch("/api/librarylink/InitializeMiniMapSelectors", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(object)
    })
        .then(response => response.text())
        .then(data => {
            //console.log(data);
            document.getElementById("mini-map").innerHTML = data;
        })
        .catch(error => {
            console.log("Failed to Delete Post Exception");
        });


};

function RetrieveRazorPage() {

    //let Category = document.getElementById('mini-category').value;
    //let Library = document.getElementById('mini-library').value;
    //let Subject = document.getElementById('mini-subject').value;
    let Topic = document.getElementById('mini-topic').value;
    

    let model = {
        //CategoryID: Category,
        //LibraryID: Library,
        //SubjectID: Subject,
        id: Topic,
        text: document.getElementById('mini-language').textContent
    };

    fetch("/api/librarylink/RetrieveRazorPage", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(model)
    })
        .then(response => response.text())
        .then(data => {

            document.getElementById("mini-page").src = data
        })
        .catch(error => {
            console.log("Failed to Load Page");
        });
}