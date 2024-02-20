

/* This Function for Unique Target toggles */
document.addEventListener('DOMContentLoaded', () => {
    let toggles = document.querySelectorAll('.event-toggle');

    for (let i = 0; i < toggles.length; i++) {

        toggles[i].addEventListener('click', (e) => {
            let targetid = e.target.dataset.target;
            if (!targetid) {
                targetid = e.target.parentElement.dataset.target;
            }

            ToggleExpanded(targetid);
        });
    }

    CreateMiniMapSelectors();

});
function ToggleExpanded(elementId) {
    let container = document.getElementById(elementId);
    container.classList.toggle('expanded');
};


//Discord Link Copy
function copyToClipboard(discord_id, buttonElement) {
    // search for the target target by it's id
    const targetElement = document.querySelector('#' + discord_id);
    // Get the DataValue can be Copied.
    //This is because I wanted to the discord url to be defined at the top of the page.
    const textToCopy = targetElement.getAttribute('data-value');
    //console.log(discordid);
    // Copy the text to the clipboard
    navigator.clipboard.writeText(textToCopy).then(function () {
        // Update the button text
        buttonElement.textContent = 'Copied!';
        buttonElement.style.opacity = 1;
        // Restore the original button text after a delay
        setTimeout(() => {
            buttonElement.textContent = textToCopy;
        }, 3000);
    }).catch(function (err) {
        console.error('Failed to copy text: ', err);
    });
}

function CreateMiniMapSelectors() {

    //Load Cached Values
    let ids = {
        CategoryID: 0,
        LibraryID: 0,
        SubjectID: 0,
        TopicID: 0
    }

    fetch("/api/librarylink/InitializeMiniMapSelectors", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(ids)
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

function MapOptionsChangedDuplicate(targetid, index) {


    let container = document.getElementById(targetid);
    let children = container.querySelectorAll('select');

    let model = {
        CategoryID: children[0].value,
        LibraryID: children[1].value,
        SubjectID: children[2].value,
        TopicID: 0,
    };
    if (index == 0) {
        model.LibraryID = 0;
        model.SubjectID = 0;
    }
    else if (index == 1) {
        model.SubjectID = 0;
    }

    fetch("/api/librarylink/UpdateLibraryOptions", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(model)
    })
        .then(response => response.text())
        .then(data => {
            //console.log(data);

            container.innerHTML = data;

        });
}

function RetrieveRazorPage() {
    
    let Category = document.getElementById('mini-category').value;
    let Library = document.getElementById('mini-library').value;
    let Subject = document.getElementById('mini-subject').value;
    let Topic = document.getElementById('mini-topic').value;


let model = {
        CategoryID: Category,
        LibraryID: Library,
        SubjectID: Subject,
        TopicID: Topic,
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
            console.log(data);
            document.getElementById("mini-page").src = data
        })
        .catch(error => {
            console.log("Failed to Load Page");
        });
}