
//https://www.youtube.com/watch?v=la-0HOaNL10
//When the text editors are openened, onlt then is the text editor created and stored.
const OpenEditors = new Map();

document.addEventListener('DOMContentLoaded', () => {
    Event_AddEditBasicSegments();

    //this.typefaces = EditorButtons.querySelector('#typeface');

    //fontlist.forEach(font => {
    //    let option = document.createElement('option');
    //    option.value = font;
    //    option.innerHTML = font;
    //    this.typeface.appendChild(option);
    //});


});

function Event_AddEditBasicSegments() {
    let toggles = document.querySelectorAll('.event-edit-basicsegment');

    for (let i = 0; i < toggles.length; i++) {

        toggles[i].addEventListener('click', (e) => {

            let targetid = e.target.dataset.target;
            let container = document.getElementById(targetid);
            container.classList.toggle('expanded');

            if (container.classList.contains('expanded')) {
                const editor = new RichTextEditorObject(targetid);
            }
            else {

                const editor = OpenEditors.get(targetid);
                editor.destroy();

            }


        });
    }
}

const fontlist = [
    "Arial", "Verdana", "Times New Roman", "Georgia", "Trebuchet MS",
    "Tahoma", "Courier New", "Lucida Console", "Impact", "Comic Sans MS",
    "Palatino Linotype", "Book Antiqua", "Lucida Sans Unicode", "Garamond", "MS Serif",
    "MS Sans Serif", "Lucida Grande", "Geneva", "Helvetica", "Courier", "Monaco", "Optima",
    "Hoefler Text", "Baskerville", "Big Caslon", "American Typewriter", "Andale Mono", "Copperplate",
    "Papyrus", "Brush Script MT", "Snell Roundhand", "Zapf Chancery", "Zapfino"
];

const BoldFormat = { id: "bold", tag: "B", text: "&#914" };
const ItalicFormat = { id: "italic", tag: "I", text: "&#1030" };

const EditorStyles = [
    BoldFormat,
    ItalicFormat

]


class RichTextEditorObject {

    constructor(id) {
        this.id = id;
        OpenEditors.set(this.id, this);

        this.xtarget = [];
        this.ytarget = [];

        this.titleArea = document.getElementById('title-' + this.id);
        this.titleArea.contentEditable = true;

        this.textArea = document.getElementById('description-' + this.id);
        this.textArea.contentEditable = true;

        let EditorButtons = document.getElementById(id);

        this.style = this.CreationStyleButtons(EditorButtons);


        //Test Code
        this.titleArea.addEventListener('input', (e) => {
            console.log(e.target.value);
        });
        this.textArea.addEventListener('input', (e) => {
            let data = e.data;


        });


        //this.textArea.addEventListener('focusin', (e) => {
        //    console.log('focused');

        //});
        //this.textArea.addEventListener('focusout', (e) => {
        //    console.log('unfocused');
        //    this.ClearTargetSelections();
        //});

        //this.textArea.addEventListener('select', this.modifyText, false);

        //Timeout used to resolve a Race Condition from the browser

        this.textArea.addEventListener('mousedown', () => {
            this.ClearTargetSelections();
        });

        this.textArea.addEventListener("mouseup", () => {
            //this.ClearTargetSelections();
            setTimeout(() => this.RebuildTargetSelections(), 100);
        });
    }

    CreationStyleButtons(containerid) {
        let styles = {
            bold: this.NewStyleButton(containerid, 'B', '&#914'),
            italic: this.NewStyleButton(containerid, 'I', '&#1030'),
            underline: this.NewStyleButton(containerid, 'U', '&#9078'),
            strikethrough: this.NewStyleButton(containerid, 'STRIKE', 'S'),
            superscript: this.NewStyleButton(containerid, 'SUP', 'X<sup>2</sup>'),
            subscript: this.NewStyleButton(containerid, 'SUB', 'X<sub>2</sub>'),
        }
        return styles;
    }


    NewStyleButton(container, tag, text) {
        let button = document.createElement('button');
        button.classList = "Editor-btn";
        button.innerHTML = text;
        container.appendChild(button);

        button.addEventListener('click', this.HandleStyleButtonClick.bind(this, tag));

        return button;
    }

    HandleStyleButtonClick(tag) {
        this.ToggleTextFormat(tag);
    }


    FindContentEditableChild(node) {

        while (node.parentNode !== this.textArea) {
            node = node.parentElement;
        }
        return node;
    }

    ClearEmptyStrings_ReverseTraverse(lastchild_node) {
        while (lastchild_node.previousSibling != null) {
            let temp = lastchild_node.previousSibling;
            if (lastchild_node.textContent === "") {
                if (lastchild_node.tagName != 'BR') {
                    lastchild_node.parentNode.removeChild(lastchild_node);
                }
            }
            lastchild_node = temp;
        };
    }

    ClearEmptyStrings_ForwardTraverse(firstchild_node) {

        while (firstchild_node != null) {
            let temp = firstchild_node.nextSibling;

            if (firstchild_node.textContent === "") {
                if (firstchild_node.tagName != 'BR') {
                    firstchild_node.parentNode.removeChild(firstchild_node);
                }
            }
            firstchild_node = temp;
        }

    }

    AbsorbImmediateNeighborTexts(NewlyTaggedElement) {

    }

    AbsorbImmediateNeighbors(NewlyTaggedElement) {

        this.MergeTextToRight(NewlyTaggedElement, NewlyTaggedElement.nextSibling);
        this.AbsorbTagToRight(NewlyTaggedElement, NewlyTaggedElement.nextSibling);

        this.MergeTextToLeft(NewlyTaggedElement, NewlyTaggedElement.previousSibling);
        this.AbsorbTagToLeft(NewlyTaggedElement, NewlyTaggedElement.previousSibling);

        this.ClearNestedElementsByTagName(NewlyTaggedElement, NewlyTaggedElement.tagName);
        //this.MergeNestedTags(NewlyTaggedElement);

    }

    ClearNestedElementsByTagName(rootnode, targetType) {
        rootnode.querySelectorAll(targetType).forEach((node) => {
            this.RemoveTagOnThisElement(node);
        });
    }

    MergeNestedTags(rootnode) {
        let child = rootnode.firstChild;

        while (child != null) {
            if (child.nodeType !== 3) {
                if (child.tagName === rootnode.tagName) {

                    this.RemoveTagOnThisElement(child);
                    //child.insertAdjacentHTML('afterend', child.innerHTML);
                    //rootnode.removeChild(child);
                }
            }
            if (child.nodeType !== 3) {
                this.MergeNestedTags(child);
            }

            child = child.nextSibling;
        }
        this.MergeConsecutiveNodeTypes(rootnode.firstChild);
    }

    MergeConsecutiveNodeTypes(firstchild_node) {

        while (firstchild_node.nextSibling !== null) {

            if (firstchild_node.nodeType === firstchild_node.nextSibling.nodeType || firstchild_node.nextSibling.textContent === " ") {

                if (firstchild_node.nodeType === 3) { //Text Node MergeText method?
                    firstchild_node.textContent += firstchild_node.nextSibling.textContent;
                    firstchild_node.parentElement.removeChild(firstchild_node.nextSibling);
                }
                else {
                    if (firstchild_node.nextSibling.textContent === " ") { //Absorb when manually highlighting text
                        this.MergeText(firstchild_node, firstchild_node.nextSibling);
                    }
                    else if (firstchild_node.tagName === firstchild_node.nextElementSibling.tagName) {

                        this.AbsorbNodeToRight(firstchild_node, firstchild_node.nextSibling);
                        //this.MergeNestedTags(firstchild_node);
                    }
                    else { // Traverse next node if obscure tags are present, such as <br>
                        firstchild_node = firstchild_node.nextSibling;
                    }
                }
            }
            else {
                firstchild_node = firstchild_node.nextSibling;
            }
        };
    }
    AbsorbNodeToRight(savenode, node_to_merge) {
        savenode.innerHTML += node_to_merge.innerHTML;
        savenode.parentElement.removeChild(node_to_merge);
    }

    AbsorbTagToRight(TagNode, node_to_merge) {
        console.log(TagNode);
        if (node_to_merge != null) {
            if (TagNode.tagName === node_to_merge.tagName) {

                let temp = node_to_merge.firstChild;
                while (temp != null) {
                    TagNode.append(temp);
                    temp = temp.nextSibling;
                }
                console.log(node_to_merge);
                TagNode.parentNode.removeChild(node_to_merge);
                this.AbsorbTagToRight(TagNode, TagNode.nextSibling);
            }
        }
        //Testing Node Absorption
        //Highest Common Ambiguous Container
        //let temp = TagNode;

        //while (temp.parentNode.textContent.length === TagNode.textContent.length) {
        //    temp = temp.parentNode;

        //    if (temp.nextSibling.tagName === TagNode.tagName) {
        //        console.log(temp.nextSibling);

        //        temp.nextSibling.prepend(temp);
        //        temp.nextSibling.parentNode.removeChild(temp);
        //        temp = temp.nextSibling;
        //        console.log(temp);
        //    }


        //    console.log(temp);
        //}

        //temp = temp.nextSibling
        //while (temp != null) {
        //    console.log(temp);
        //    if (temp.tagName === TagNode.tagName) {
        //        let iter = temp.firstChild;
        //        console.log(iter);
        //        while (iter != null) {
        //            console.log('Merging Next First ${temp}');
        //            TagNode.append(iter);
        //            iter = iter.nextSibling;
        //        }
        //        temp.parentNode.removeChild(temp);
        //    }
        //    temp = temp.firstChild;
        //}


    }

    AbsorbTagToLeft(TagNode, node_to_merge) {

        if (node_to_merge != null) {
            if (TagNode.tagName === node_to_merge.tagName) {
                let temp = node_to_merge.lastChild;
                while (temp != null) {
                    TagNode.prepend(temp);
                    temp = temp.previousSibling;
                }

                TagNode.parentNode.removeChild(node_to_merge);
                this.AbsorbTagToLeft(TagNode, TagNode.previousSibling);
            }

        }

    }


    AbsorbNodeToLeft(savenode, node_to_merge) {
        savenode.prepend(node_to_merge.firstChild);
        savenode.parentElement.removeChild(node_to_merge);
    }

    MergeTextToRight(savenode, node_to_merge) {

        if (node_to_merge !== null) {
            if (node_to_merge.nodeType === Node.TEXT_NODE) {
                if (node_to_merge.textContent === " " || node_to_merge.textContent === "") {
                    savenode.innerHTML += node_to_merge.textContent;
                    savenode.parentElement.removeChild(node_to_merge);
                    this.MergeTextToRight(savenode, savenode.nextSibling);
                }
            }
        }
    }

    MergeTextToLeft(savenode, node_to_merge) {

        if (node_to_merge !== null) {
            if (node_to_merge.nodeType === 3) {
                if (node_to_merge.textContent === " " || node_to_merge.textContent === "") {
                    savenode.innerHTML += node_to_merge.textContent;
                    savenode.parentElement.removeChild(node_to_merge);
                    this.MergeTextToRight(savenode, savenode.previousSibling);
                }
            }
        }
    }

    MergeText(savenode, node_to_merge) {
        savenode.innerHTML += node_to_merge.textContent;
        savenode.parentElement.removeChild(node_to_merge);
    }

    //NOT USED
    //Recursive loop to remove all instances of style inside a container. 
    //ClearContainerOfTargetStyle(targetNode, targetType) {
    //    let iter = targetNode;
    //    while (iter != null) {

    //        if (iter.tagName === targetType) {
    //            console.log(iter);
    //            this.RemoveTagOnThisElement(iter);
    //            console.log(iter);
    //            //With a tag removed, it changes the potential similarities with nearby nodes.
    //            this.MergeConsecutiveNodeTypes(iter);
    //        }
    //        iter = iter.nextSibling;
    //    }
    //    if (iter != null) {
    //        this.ClearContainerOfTargetStyle(iter.firstchild_node, targetType);
    //    }

    //}

    //Takes an elements innerHTML and inserts it after the after element before deleteing the target element
    RemoveTagOnThisElement(element) {
        let parent = element.parentElement;
        let inner = element.innerHTML;

        element.insertAdjacentHTML('afterend', inner);
        //parent.outerHTML = inner;
        parent.removeChild(element);
        this.MergeConsecutiveNodeTypes(parent.firstChild);
    }

    //seeks the highest Common Container of target Style
    FindHighestCommonContainerByStyle(CommonContainer, targetStyle) {
        let iter = CommonContainer.parentNode;
        while (iter != this.textArea) {
            if (iter.tagName === targetStyle) {
                if (CommonContainer.tagName === targetStyle) {//Test Condition
                    this.RemoveTagOnThisElement(CommonContainer);
                }
                CommonContainer = iter;
            }
            iter = iter.parentElement;
        }
        return CommonContainer;
    }

    NormalizeSelectionRange(range, targetTag) {

        range.commonAncestorContainer;

        let iter = range.startContainer;
        while (iter != range.commonAncestorContainer || iter.tagName != targetTag) {
            if (iter.tagName === targetTag) {
                range.setStart(iter, 0);
            }
            iter = iter.parentElement;
        };


    }



    //Selection is ambiguous to the tagStyles, so I need to determine if the tag styles are already applied
    ToggleTextFormat(StyleTag) {
        //let tagType = 'B'; //Potential function parameter
        let selection = document.getSelection();
        let range = selection.getRangeAt(0);


        // Lazy Select Toggle - Create a class boolean to enable user to toggle this feature on and off.
        // Selection Type of Caret is converted into a Range if the Caret is not at the end of a word.
        if (selection.type === 'Caret') {
            let initial = range.startOffset;
            selection.modify('move', 'forward', 'word');
            let deltaToEnd = selection.getRangeAt(0).cloneRange().startOffset - initial;
            selection.modify('move', 'backward', 'word');
            let deltaToStart = initial - selection.getRangeAt(0).cloneRange().startOffset;

            //DeltaToEnd is number of characters the cart moves to the right
            //DeltaToStart is number of characters the caret moves to the left from initial
            if (deltaToEnd <= 1 || deltaToStart == 0) {
                selection.setPosition(range.startContainer, range.startOffset);
            }
            else {
                //selection.modify('move', 'backward', 'word');
                selection.modify('extend', 'forward', 'word');
                range = selection.getRangeAt(0);
            }

        }
        console.log(selection.type);

        //if in range, alter text style, else do nothing.
        if (selection.type === 'Range') {
            let lowestCommonContainer = range.commonAncestorContainer;
            let HighestCommonContainer = this.FindHighestCommonContainerByStyle(lowestCommonContainer, StyleTag);
            //console.log(lowestCommonContainer);
            //console.log(HighestCommonContainer);
            if (lowestCommonContainer === HighestCommonContainer) {
                console.log('Same Container');
            }
            else {
                console.log('Different Container');
            }

            let isAlreadyStyled = HighestCommonContainer.tagName === StyleTag;

            //TODO
            //Normalize the Range to Encompass the Entire Tag

            console.log(`${range.startContainer.parentNode}, ${range.endContainer.parentNode}`);



            //If the Start Container and End Container are Same, Then no Partially Selected Tags are present
            if (true) {// ((lowestCommonContainer !== HighestCommonContainer) || (range.startContainer === range.endContainer)) {

                //4 Selection cases but only the full select, beginning selection matter
                // The goal is figure out the order I reassemble the extracted content
                // the cases where the first letter is no selected are assembled the same way, except one will check for a null secdon child
                if (isAlreadyStyled) {
                    let count = selection.toString().length;
                    if (HighestCommonContainer.textContent.length === count) {
                        console.log('Full Selection');
                        console.log(HighestCommonContainer);
                        let parent = HighestCommonContainer.parentNode;
                        //this.ClearEmptyStrings_ForwardTraverse(lowestCommonContainer.firstChild);
                        this.RemoveTagOnThisElement(HighestCommonContainer);
                        //this.MergeConsecutiveNodeTypes(parent.firstChild);
                    }
                    else if (selection.anchorOffset === 0 || selection.focusOffset === 0) {
                        console.log('TODO Front Selection');
                        console.log(HighestCommonContainer);
                    }
                    else {
                        console.log('TODO end Selection - Remove Tag');
                        console.log(HighestCommonContainer);
                    }


                }
                else {
                    console.log('Surrounding Selection');
                    let newElement = document.createElement(StyleTag);
                    selection.getRangeAt(0).surroundContents(newElement);
                    this.ClearEmptyStrings_ForwardTraverse(newElement);
                    this.AbsorbImmediateNeighbors(newElement);
                    //this.ClearEmptyStrings_ReverseTraverse(selection.anchorNode.lastChild);
                    //this.MergeConsecutiveNodeTypes(newElement.parentElement.firstChild);
                }
            }
            else {
                console.log('Partial Selection Block');
                let selectionType = 'none';

                if (HighestCommonContainer.textContent.trim().length === selection.toString().trim().length) {
                    selectionType = 'full';

                    console.log('PARTIAL SELECTION - FULL SELECTION');
                    let newElement = document.createElement(StyleTag);
                    selection.getRangeAt(0).surroundContents(newElement);
                    this.AbsorbImmediateNeighbors(newElement);

                    //this.RemoveTagOnThisElement(commonContainer);

                }
                else if (selection.anchorOffset === 0 || selection.focusOffset === 0) {
                    selectionType = 'front';

                    let clone = new CloneWithRange(HighestCommonContainer, range);

                    let element = clone.CreateCloneElement_FrontHalf(StyleTag);
                    HighestCommonContainer.prepend(element);
                    range.deleteContents();
                    this.AbsorbImmediateNeighbors(element);
                    //this.ClearEmptyStrings_ForwardTraverse(HighestCommonContainer.firstChild);
                    //this.MergeConsecutiveNodeTypes(HighestCommonContainer.firstChild);

                }
                else {
                    //end means children 1 and 2 are at end, 2 may be null

                    console.log(HighestCommonContainer);
                    selectionType = 'end';
                    let leftside = new CloneWithRange(HighestCommonContainer, range);
                    let MiddleClone = new CloneWithRange(HighestCommonContainer, range);
                    let lastEnd = range.endContainer;
                    let lastEndOffset = range.endOffset;
                    range.selectNodeContents(HighestCommonContainer);
                    range.setEnd(lastEnd, lastEndOffset);
                    //range.deleteContents();

                    leftside.CreateCloneElement_FrontHalf(StyleTag);
                    //HighestCommonContainer.prepend(leftside);

                }
                console.log(selectionType);


            }

        }


        ////this.classList.toggle('c-offset');
    };



    HighLightToggles(node) {

        while (node.parentNode !== this.textArea) { //textArea is acting Root Node

            //if (node.nodeType === 3) {

            //    this.ytarget.push(node);
            //}

            if (node.tagName === "B") {

                node.classList.add('c-offset');
                this.ytarget.push(node);

                if (this.style.bold.classList.contains('c-offset') === false) {
                    this.style.bold.classList.add('c-offset');
                }
            }

            if (node.tagName === "I") {

                if (node.parentElement.parentElement === this.textArea) {
                    node.classList.add('c-offset');
                    this.ytarget.push(node)
                }
                if (this.style.italic.classList.contains('c-offset') === false) {
                    this.style.italic.classList.add('c-offset');
                }
            }

            if (node.parentElement.tagName === "U") {

            }

            node = node.parentElement;
        };

        return node;

    }


    RebuildTargetSelections() {

        let caretEvent = document.getSelection();
        let range = caretEvent.getRangeAt(0);
        console.log(`${range.startOffset}, ${range.endOffset}`);


        if (caretEvent.type === "Caret") {
            let element = this.HighLightToggles(caretEvent.anchorNode.parentElement);
            this.xtarget.push(element);

            //TODO FEATURE TOGGLE - Automatically Trim whitesapce from the end of a paragraph
            let node = caretEvent.anchorNode;
            let offset = caretEvent.anchorOffset;
            let iter = this.xtarget[0].lastChild;
            while (iter.nodeType !== Node.TEXT_NODE) {
                iter = iter.lastChild;
            }

            iter.textContent = iter.textContent.replace(/\s+$/, '');
            let newlength = caretEvent.anchorNode.textContent.length;

            if (caretEvent.anchorNode === iter) {
                if (offset > newlength) {
                    caretEvent.setPosition(node, newlength);
                }
                else {
                    caretEvent.setPosition(node, offset);
                }
            }
            else {
                caretEvent.setPosition(node, offset);
            }


        }
        else {
            //let anchor = this.FindParagraphParent(caretEvent.anchorNode);
            //let focus = this.FindParagraphParent(caretEvent.focusNode);
            let anchor = this.FindContentEditableChild(caretEvent.anchorNode);
            let focus = this.FindContentEditableChild(caretEvent.focusNode);

            let direction = anchor.compareDocumentPosition(caretEvent.focusNode);
            let isForwards = true;
            if (!direction && caretEvent.anchorOffset > caretEvent.focusOffset ||
                direction === Node.DOCUMENT_POSITION_PRECEDING)
                isForwards = false;

            if (anchor === focus) {
                this.xtarget.push(anchor);
            }
            else {
                if (isForwards) {
                    //this.HighLightToggles(caretEvent.anchorNode);
                    //this.HighLightToggles(caretEvent.focusNode);
                    this.xtarget.push(anchor);
                    this.xtarget.push(focus);
                }
                else {
                    //this.HighLightToggles(caretEvent.focusNode);
                    //this.HighLightToggles(caretEvent.anchorNode);
                    this.xtarget.push(focus);
                    this.xtarget.push(anchor);
                }
            }
        }

        this.AddTargetHighLights();
    };

    AddTargetHighLights() {

        this.xtarget[0].classList.add('c-menu');

        if (this.xtarget.length > 1) {
            let iter = this.xtarget[0];
            while (iter !== this.xtarget[1]) {
                iter = iter.nextElementSibling;
                iter.classList.add('c-menu');

            }
        }

        Array.from(this.ytarget).forEach((node) => {
            node.classList.add('c-offset');
        });
    }

    ClearTargetSelections() {
        //console.log('clearing targets');
        let buttons = document.getElementById(this.id);
        let allbuttons = buttons.querySelectorAll('button');

        allbuttons.forEach((btn) => {
            btn.classList.remove('c-offset');
        });


        if (this.xtarget.length > 0) {
            this.xtarget[0].classList.remove('c-menu');
            if (this.xtarget.length > 1) {
                let iter = this.xtarget[0];
                while (iter !== this.xtarget[1]) {
                    iter = iter.nextElementSibling;
                    iter.classList.remove('c-menu');
                }
            }
        }


        this.xtarget = [];

        Array.from(this.ytarget).forEach((node) => {
            node.classList.remove('c-offset');
        });
        this.ytarget = [];

    };

    destroy() {

        //this.ClearToggles();

        this.titleArea.contentEditable = false;
        this.textArea.contentEditable = false;

        //this.bold.removeEventListener('click', this.HighLighter(this.bold), false);
        //this.bold.removeEventListener('click', this.ApplyBoldTags, false);
        // this.italic.removeEventListener('click', this.HighLighter, false);

        OpenEditors.delete(this.id);
        console.log(OpenEditors);

    }
}


//This is a class to complete a full clone of a range and its container
class CloneWithRange {
    constructor(OriginalContainer, OriginalRange) {
        console.log(OriginalContainer);
        this.range = OriginalRange.cloneRange();
        this.fragment = document.createDocumentFragment();

        let SearchForStart = true;
        Array.from(OriginalContainer.childNodes).forEach((child) => {
            let clonedChild = child.cloneNode(true);
            this.fragment.appendChild(clonedChild);

            //Traverse the OriginalContainer to find map the cloned start and end to the fragment

            //A Forward Traversal means the Start Container is found first
            //A bool is used to reduce unnecessary iterations
            if (SearchForStart) {
                if (child === OriginalRange.startContainer) {
                    this.range.setStart(clonedChild, OriginalRange.startOffset);
                    SearchForStart = false;
                }
                else if (child.contains(OriginalRange.startContainer)) {
                    let iter = child;
                    let clonedIter = clonedChild;
                    while (iter != OriginalRange.startContainer) {

                        for (let i = 0; i < iter.childNodes.length; i++) {
                            let temp = iter.childNodes[i];

                            if (temp === OriginalRange.startContainer) {
                                iter = temp;
                                this.range.setStart(clonedIter.childNodes[i], OriginalRange.startOffset);
                                SearchForStart = false;
                                break;
                            }

                            if (temp.contains(OriginalRange.startContainer)) {
                                iter = temp;
                                clonedIter = clonedIter.childNodes[i];
                                break;
                            }
                        }
                    }

                }
            }

            if (child === OriginalRange.endContainer) {
                this.range.setEnd(clonedChild, OriginalRange.endOffset);
            }
            else if (child.contains(OriginalRange.endContainer)) {
                let iter = child;
                let clonedIter = clonedChild;
                while (iter != OriginalRange.endContainer) {
                    console.log(iter);
                    for (let i = 0; i < iter.childNodes.length; i++) {
                        let temp = iter.childNodes[i];

                        if (temp === OriginalRange.endContainer) {
                            console.log('End Container Found');
                            iter = temp;
                            this.range.setEnd(clonedIter.childNodes[i], OriginalRange.endOffset);
                            break;
                        }

                        if (temp.contains(OriginalRange.endContainer)) {
                            iter = temp;
                            clonedIter = clonedIter.childNodes[i];
                            break;
                        }

                    }
                }
            }

        });



        //The Cloned Range's Start and End Containers may not exist in the container its referencing.
        // This code to to ensure that the objects range is initialized to the objects fragment
        // The starts and ends are intialized to the first or last character of the container.
        if (this.range.startContainer === OriginalRange.startContainer) {
            console.log('No Start Found');
            let startInit = this.fragment.firstChild;
            while (startInit.nodeType != 3) {
                startInit = startInit.firstChild;
            }
            this.range.setStart(startInit, 0);
        }

        if (this.range.endContainer === OriginalRange.endContainer) {
            console.log('No End Found');
            let endInit = this.fragment.lastChild;
            while (endInit.nodeType != 3) {
                endInit = endInit.lastChild;
            }
            this.range.setEnd(endInit, endInit.textContent.length);
        }
    }

    CreateCloneElement(ElementTag) {

        let element = document.createElement(ElementTag);
        let start = this.range.startContainer;
        let startOffset = this.range.startOffset;
        let end = this.range.endContainer;
        let endOffset = this.range.endOffset;

        this.range.setStart(start, startOffset);
        this.range.setEnd(end, endOffset);

        return element;
    }

    CreateCloneElement_FrontHalf(ElementTag) {

        let element = document.createElement(ElementTag);
        //let start = this.range.startContainer;
        //let startOffset = this.range.startOffset;
        let end = this.range.endContainer;
        let endOffset = this.range.endOffset;
        element.appendChild(this.fragment);
        this.range.selectNodeContents(element);
        this.range.setStart(end, endOffset);

        this.range.deleteContents();

        return element;
    }

    KeepCloneElement_FrontHalf(OuterElement) {
        let element = document.createElement(OuterElement);


        //let start = this.range.startContainer;
        //let startOffset = this.range.startOffset;
        let end = this.range.endContainer;
        let endOffset = this.range.endOffset;
        element.append(this.fragment);
        this.range(selectNodeContents(element));
        this.range.setStart(end, endOffset);
        this.range.deleteContents();

        return element;
    }
}


