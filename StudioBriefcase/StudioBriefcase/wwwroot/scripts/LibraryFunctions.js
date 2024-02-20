
//This function has a Duplicate function for the mini-map selectors in site.js. I'm not familiar with javascript.
function MapOptionsChanged(targetid, index) {
    
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

    fetch("/api/librarylink/GetLibraryOptions", {
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
