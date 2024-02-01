//
let TargetDescription = null; //Target Description is the expanded Description
function ToggleDescription(description, section) {


    if (TargetDescription != null) { //If something Expanded

        if (TargetDescription != description) { //Clicked preview is not currently expanded
            TargetDescription.classList.toggle('expanded'); //Swap toggles.
            description.classList.toggle('expanded');
            TargetDescription = description;
            document.getElementById("sectionInspectorInput" + section).value = description.dataset.url;
        }
        else { //if same preview is clicked, toggle, 
            description.classList.toggle('expanded');
            TargetDescription = null;
            document.getElementById("sectionInspectorInput" + section).value = "";
        }

    }
    else { //If NOthing is Expanded, Expand the clicked preview.
        description.classList.toggle('expanded');
        TargetDescription = description;
        document.getElementById("sectionInspectorInput" + section).value = description.dataset.url;
    }

};


//Test Function to Retrieve a video preview from a static link value
function VideoButtonTest(button) {
    //document.getElementById("videobuttontest").addEventListener("click", function () {
    let val = button.dataset.section;
    console.log(val);
    let url = "https://youtu.be/90MeC-PTj50"
    //let url = "https://www.youtube.com/watch?v=90MeC-PTj50";

    fetch("/api/librarylink/GetPreviewVideo", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            videolink: url,
            section: val
        })
    })
        .then(response => response.text())
        .then(data => {
            //console.log(data);

            // let newdocument = document.implementation.createHTMLDocument();
            // newdocument.write(data);
            //document.getElementById("viewport2").innerHTML. = "<p>Test Content</P>";

            const main = document.getElementById("viewport" + val);
            const fragment = document.createDocumentFragment();
            const block = document.createElement("div");
            block.innerHTML = data;

            fragment.append(block);
            main.appendChild(fragment);


        });


    //document.getElementsByClassName("section-viewport")[0].src = "https://www.youtube.com/embed/6n3pFFPSlW4";
    //});
}

//A Function that retrieves a Form from the server to Insert or Edit a Post
function OpenPostDetailsForm(pagesection) {
    console.log(pagesection);

    //"https://youtu.be/90MeC-PTj50"

    let clientModel = {
        weblink: document.getElementById('sectionInspectorInput' + pagesection).value,
        sectionValue: pagesection,
        topicName: document.getElementById('PageTopic').value,
        subjectName: document.getElementById('PageSubject').value,
        libraryName: document.getElementById('PageLibrary').value,
        categoryName: document.getElementById('PageCategory').value,
        //language: document.getElementById('submissionForm-language0').value, //0

        //posttype: document.getElementById('LinkType' + pagesection).value,
        //GitId: 0,
        //OS: 0, //0
        //IDE: 0, //0
        //tag1: 0, //0
        //tag2: 0, //0
        //tag3: 0 //0
    }


    fetch("/api/librarylink/GetPostDetailsForm", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(clientModel)
    })
        .then(response => response.text())
        .then(data => {
            //console.log(data);

            // let newdocument = document.implementation.createHTMLDocument();
            // newdocument.write(data);
            const sublayoutlocation = document.getElementById("UserPostManagerLocation");
            sublayoutlocation.innerHTML = data;

        });

}

function InsertLinkToDatabase() {

    //Tags language and from OS - tag3 are subfixed with 1. The location in navigation bar is subfixed with 0
    let LibraryMapModel = {
        sectionValue: document.getElementById('submissionForm-section').value,
        topicName: document.getElementById('submissionForm-topic').value,
        subjectName: document.getElementById('submissionForm-subject').value,
        libraryName: document.getElementById('submissionForm-library').value,
        categoryName: document.getElementById('submissionForm-category').value,
        language: document.getElementById('submissionForm-language').value,
        weblink: document.getElementById('submissionForm-weblink').value,
        posttype: document.getElementById('submissionForm-posttype').value,
        GitId: 0,
        OS: document.getElementById('submissionForm-os1').value,
        IDE: document.getElementById('submissionForm-ide1').value,
        tag1: document.getElementById('submissionForm-tag11').value,
        tag2: document.getElementById('submissionForm-tag21').value,
        tag3: document.getElementById('submissionForm-tag31').value

    }

    fetch("/api/librarylink/InsertLink", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(LibraryMapModel)
    })
        .then(response => response.text())
        .then(data => {
            //console.log(data);

            // let newdocument = document.implementation.createHTMLDocument();
            // newdocument.write(data);
            const sublayoutlocation = document.getElementById("logresult-container");
            sublayoutlocation.textContent = data;


        });

}

function DeletePost(button) {
    let postidModel = { uinttype: button.getAttribute("data-postID") }

    fetch("/api/librarylink/DeletePost", {
        method: 'POST',
        headers: {
            'content-Type': 'application/json'
        },
        body: JSON.stringify(postidModel)
    })
        .then(response => response.text())
        .then(data => {
            const sublayoutlocation = document.getElementById("logresult-container");
            sublayoutlocation.textContent = data;
        })
        .catch(error => {
            console.log("Failed to Delete Post Exception");
        });


}