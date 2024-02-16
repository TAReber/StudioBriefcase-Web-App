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
    let posttype = button.dataset.type;

    var tagList = [];

    // Add tags to the array only if the value is not 0
    if (document.getElementById('PageTag-OS').value !== "0") {
        tagList.push(document.getElementById('submissionForm-os0').value);
    }

    if (document.getElementById('PageTag-IDE').value !== "0") {
        tagList.push(document.getElementById('submissionForm-ide0').value);
    }

    if (document.getElementById('PageTag-1').value !== "0") {
        tagList.push(document.getElementById('submissionForm-tag10').value);
    }

    if (document.getElementById('PageTag-2').value !== "0") {
        tagList.push(document.getElementById('submissionForm-tag20').value);
    }

    if (document.getElementById('PageTag-3').value !== "0") {
        tagList.push(document.getElementById('submissionForm-tag30').value);
    }

    let datamap = {
        sectionValue: val,
        topicID: document.getElementById('PageTopic').dataset.id,
        posttype: posttype,
        //topicName: document.getElementById('PageTopic').value,
        //subjectName: document.getElementById('PageSubject').value,
        //libraryName: document.getElementById('PageLibrary').value,
        //categoryName: document.getElementById('PageCategory').value,
        language: document.getElementById('menu-language').value,
        tags: tagList
    }

    //FUTURE PERFORMANCE DEVELOPMENT FOR DATABASE AND SERVER TRAFFIC
    //LIMIT FETCH AMOUNT TO 25 LINKS AT A TIME AND LAZY LOAD THEM IN INCREMENTS

    fetch("/api/librarylink/GetVideoPostList", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(datamap)

    })
        .then(response => response.json())
        .then(data => {
            for (let i = 0; i < data.length; i++) {
                CreateVideoPreview(val, data[i]);
            }
        })
        .catch(error => {
            console.error('Error', error);
        });


    //document.getElementsByClassName("section-viewport")[0].src = "https://www.youtube.com/embed/6n3pFFPSlW4";
    //});
}

function CreateVideoPreview(sectionVal, url) {

    let pair = {
        id: sectionVal,
        text: url
    }


    fetch("/api/librarylink/GetPreviewVideo", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(pair)
    })
        .then(response => response.text())
        .then(data => {
            //console.log(data);

            // let newdocument = document.implementation.createHTMLDocument();
            // newdocument.write(data);
            //document.getElementById("viewport2").innerHTML. = "<p>Test Content</P>";

            const main = document.getElementById("viewport" + sectionVal);
            const fragment = document.createDocumentFragment();
            const block = document.createElement("div");
            block.innerHTML = data;

            fragment.append(block);
            main.appendChild(fragment);


        });
}



//A Function that retrieves a Form from the server to Insert or Edit a Post
function OpenPostDetailsForm(pagesection) {
    //console.log(pagesection);

    //"https://youtu.be/90MeC-PTj50"

    let clientData = {
        topicID: document.getElementById('PageTopic').dataset.id,
        sectionID: pagesection,
        url: document.getElementById('sectionInspectorInput' + pagesection).value,
        language: document.getElementById('menu-language').value
    }



    fetch("/api/librarylink/GetPostDetailsForm", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(clientData)
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
        sectionID: document.getElementById('PostInspect-section').value,
        topicID: document.getElementById('PostInspect-topic').value,
        language: document.getElementById('menu-language').value,
        url: document.getElementById('PostInspect-link').value,
        post_type: document.getElementById('PostInspect-posttype').value,


        tags: [
            document.getElementById('PostInspect-Tag-OS').value,
            document.getElementById('PostInspect-Tag-IDE').value,
            document.getElementById('PostInspect-Tag-1').value,
            document.getElementById('PostInspect-Tag-2').value,
            document.getElementById('PostInspect-Tag-3').value
        ]

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