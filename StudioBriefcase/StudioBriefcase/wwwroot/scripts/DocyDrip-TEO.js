﻿// Attribution
// DocyDrip - Text Editor
// Organization - pig booger Studio
// Created By: Tyler Andrew Reber
// Date: 05/01/2024
// Version: 0.1a
// https://github.com/TAReber/DocyDrip

//Alpha Build

//Text Editor Object That Can be Created Created And Deleted Freely
// While remaining Isolated from Adjacent Text Editors


//When the text editors are openened, onlt then is the text editor created and stored.
const OpenEditors = new Map();

document.addEventListener('DOMContentLoaded', () => {
    Event_AddEditBasicSegments();
});

function Event_AddEditBasicSegments() {
    let toggles = document.querySelectorAll('.event-docydrip-teo');

    for (let i = 0; i < toggles.length; i++) {

        toggles[i].addEventListener('click', (e) => {

            let targetid = e.target.dataset.target;
            let container = document.getElementById(targetid);

            if (container.classList.contains(Active) === false) {
                const editor = new RichTextEditorObject(container);
            }
            else {
                const editor = OpenEditors.get(targetid);
                editor.destroy();

            }
        });
    }
}

//Initialization class name 
const Active = 'active';


const ConstTree = 'SPAN';
//Standard Block Tags

const ConstBlock = 'P';
const ConstH1 = 'H1';
const ConstH2 = 'H2';
const ConstH3 = 'H3';
const ConstH4 = 'H4';
const ConstH5 = 'H5';
const ConstH6 = 'H6';
const StandardStructureTags = [ConstBlock, ConstH1, ConstH2, ConstH3, ConstH4, ConstH5, ConstH6];

//List Block Elements
const ConstUList = 'UL';
const ConstOList = 'OL';
const ConstListItem = 'LI';


//CSS CLASS Globals
const ConstEditorBody = '#docydrip-teo-body';
const ConstButtonContainerStyles = 'teo-button-container'
const ConstButtonStyle = 'teo-btn';


//Options
const ClassIndent = 'indent';
const ClassOutdent = 'outdent';
const ClassJustifyLeft = 'justify-left';
const ClassJustifyCenter = 'justify-center';
const ClassJustifyRight = 'justify-right';
const ClassJustifyFull = 'justify-full';
const BlockClasses = [ClassJustifyLeft, ClassJustifyCenter, ClassJustifyRight, ClassJustifyFull];


//Void Element
const ConstBR = 'BR';

const fontlist = [
    "Arial", "Verdana", "Times New Roman", "Georgia", "Trebuchet MS",
    "Tahoma", "Courier New", "Lucida Console", "Impact", "Comic Sans MS",
    "Palatino Linotype", "Book Antiqua", "Lucida Sans Unicode", "Garamond", "MS Serif",
    "MS Sans Serif", "Lucida Grande", "Geneva", "Helvetica", "Courier", "Monaco", "Optima",
    "Hoefler Text", "Baskerville", "Big Caslon", "American Typewriter", "Andale Mono", "Copperplate",
    "Papyrus", "Brush Script MT", "Snell Roundhand", "Zapf Chancery", "Zapfino"
];

const Behavior = {
    Add: 1,
    Remove: 2,
    Toggle: 3
}

class RichTextEditorObject {

    constructor(container) {
        this.Editor = container;
        this.id = container.id;

        OpenEditors.set(this.id, this);

        //this.Header = container.querySelector('#EditorHeader');
        //this.Header.contentEditable = true;
        this.Page = container.querySelector(ConstEditorBody);
        this.Page.contentEditable = true;
        //this.Footer = container.querySelector('#EditorFooter');

        //Targeting Variables Surrounding Selections and clicking
        this.StartBlock = null;
        this.EndBlock = null;
        this.RangeTree_Start = null;
        this.RangeTree_End = null;
        this.XTargetStart = null;
        this.XTargetEnd = null;

        //Event Handler References
        this.boundHandlePageSelectionChange = this.HandlePageSelectionChange.bind(this);

        this.boundHandlePageKeyDown = this.HandlePageKeyDown.bind(this);
        this.boundHandlePageFocusOut = this.HandlePageFocusOut.bind(this);
        this.boundHandlePageMouseUp = this.HandlePageMouseUp.bind(this);
        this.boundHandlePageMouseDown = this.HandlePageMouseDown.bind(this);
        this.boundHandlePageInput = this.HandlePageInputEvent.bind(this);
        this.boundHandlePageDragStart = this.HandlePageDragStart.bind(this);
        this.boundHandlePageDragDrop = this.HandlePageDragDrop.bind(this);
        this.boundHandlePagePaste = this.HandlePagePasteText.bind(this);

        //Text Style Event Handlers
        this.boundHandleBoldClick = this.HandleStyleButtonClick.bind(this, 'B');
        this.boundHandleItalicClick = this.HandleStyleButtonClick.bind(this, 'I');
        this.boundHandleUnderlineClick = this.HandleStyleButtonClick.bind(this, 'U');
        this.boundHandleStrikeClick = this.HandleStyleButtonClick.bind(this, 'STRIKE');
        this.boundHandleSuperscriptClick = this.HandleStyleButtonClick.bind(this, 'SUP');
        this.boundHandleSubscriptClick = this.HandleStyleButtonClick.bind(this, 'SUB');

        this.boundHandleOrderedListClick = this.HandleBlockStyleClick.bind(this, 'OL');
        this.boundHandleUnorderedListClick = this.HandleBlockStyleClick.bind(this, 'UL');

        this.boundHandleBlockOptionsClick = this.HandleStandardBlockSelectorClick.bind(this);

        this.boundHandleIndentClick = this.HandleBlockFormatClick.bind(this, ClassIndent);
        this.boundHandleOutdentClick = this.HandleBlockFormatClick.bind(this, ClassOutdent);
        this.boundHandleJustifyLeftClick = this.HandleBlockFormatClick.bind(this, ClassJustifyLeft);
        this.boundHandleJustifyCenterClick = this.HandleBlockFormatClick.bind(this, ClassJustifyCenter);
        this.boundHandleJustifyRightClick = this.HandleBlockFormatClick.bind(this, ClassJustifyRight);
        this.boundHandleJustifyFullClick = this.HandleBlockFormatClick.bind(this, ClassJustifyFull);

        this.Initialize();
        //let ButtonContainer = container.querySelector('#EditorButtons');
		let ButtonContainer = this.CreateButtonContainer();

        this.styles = this.CreateStyleButtons(ButtonContainer);
        this.blocks = this.CreateBlockButtons(ButtonContainer);
        this.standardBlockOptions = this.CreateStandardBlockOptions(ButtonContainer);
        this.blockFormats = this.CreateBlockFormatButtons(ButtonContainer);


    }
    destroy() {
        //console.log('Destroying Object');
        this.Editor.classList.remove(Active);
        this.Page.classList.remove(Active);

        //this.Header.contentEditable = false;
        this.Page.contentEditable = false;

        document.removeEventListener('selectionchange', this.boundHandlePageSelectionChange);

        this.Page.removeEventListener('keydown', this.boundHandlePageKeyDown);
        this.Page.removeEventListener('focusout', this.boundHandlePageFocusOut);
        this.Page.removeEventListener('mouseup', this.boundHandlePageMouseUp);
        this.Page.removeEventListener('mousedown', this.boundHandlePageDown);
        this.Page.removeEventListener('input', this.boundHandlePageInput);
        this.Page.removeEventListener('dragstart', this.boundHandlePageDragStart);
        this.Page.removeEventListener('drop', this.boundHandlePageDragDrop);
        this.Page.removeEventListener('paste', this.boundHandlePagePaste);


        this.styles.bold.removeEventListener('click', this.boundHandleBoldClick);
        this.styles.italic.removeEventListener('click', this.boundHandleItalicClick);
        this.styles.underline.removeEventListener('click', this.boundHandleUnderlineClick);
        this.styles.strikethrough.removeEventListener('click', this.boundHandleStrikeThroughClick);
        this.styles.subscript.removeEventListener('click', this.boundHandleSubscriptClick);
        this.styles.superscript.removeEventListener('click', this.boundHandleSuperscriptClick);

        this.styles.bold.remove();
        this.styles.italic.remove();
        this.styles.underline.remove();
        this.styles.strikethrough.remove();
        this.styles.subscript.remove();
        this.styles.superscript.remove();

        this.standardBlockOptions.removeEventListener('change', this.boundHandleBlockOptionsClick);

        this.blocks.OL.removeEventListener('click', this.boundHandleOrderedListClick);
        this.blocks.UL.removeEventListener('click', this.boundHandleUnorderedListClick);

        this.blocks.OL.remove();
        this.blocks.UL.remove();

        this.blockFormats.Indent.removeEventListener('click', this.boundHandleIndentClick);
        this.blockFormats.Outdent.removeEventListener('click', this.boundHandleOutdentClick);
        this.blockFormats.JustifyLeft.removeEventListener('click', this.boundHandleJustifyLeftClick);
        this.blockFormats.JustifyCenter.removeEventListener('click', this.boundHandleJustifyCenterClick);
        this.blockFormats.JustifyRight.removeEventListener('click', this.boundHandleJustifyRightClick);
        this.blockFormats.JustifyFull.removeEventListener('click', this.boundHandleJustifyFullClick);

        this.blockFormats.Indent.remove();
        this.blockFormats.Outdent.remove();
        this.blockFormats.JustifyLeft.remove();
        this.blockFormats.JustifyCenter.remove();
        this.blockFormats.JustifyRight.remove();
        this.blockFormats.JustifyFull.remove();

        this.standardBlockOptions.remove();

        OpenEditors.delete(this.id);
        //console.log(OpenEditors);
    }

    HandlePageSelectionChange() {
        if (document.activeElement === this.Page) {
            this.ClearTargetSelections();
            this.RebuildTargetSelections();
        }
    }

    HandlePageKeyDown(events) {
        this.KeyBoardPageEvents(events);
    }
    HandlePageFocusOut() {
        //console.log('Focus Out');
        this.ClearTargetSelections();
    }
    HandlePageMouseUp() {
        //console.log('Mouse Up');
        //this.ClearTargetSelections();
    }
    HandlePageMouseDown() {
        //this.ClearTargetSelections();
    }
    HandlePageInputEvent(events) {
        //this.Selection_KeyInput(events);
    }
    HandlePageDragStart(event) {
        //console.log('start drag');
        event.preventDefault();
        event.stopPropagation();
    }
    HandlePagePasteText(event) {
        event.preventDefault();
        let plaintext = event.clipboardData.getData('text/plain');

        let selection = document.getSelection();
        let range = selection.getRangeAt(0);

        let offset = range.startOffset + plaintext.length;
        if (event.target.tagName === ConstBR) {
            let text = document.createTextNode(plaintext);          
            event.target.parentNode.prepend(text);
            event.target.remove();
        }
        else {
            let textarea = event.target;
            const textBefore = textarea.textContent.substring(0, range.startOffset);
            const textAfter = textarea.textContent.substring(range.endOffset);
            textarea.textContent = textBefore + plaintext + textAfter;           
        }
        selection.setPosition(this.GetTreeNode(this.RangeTree_Start), offset);
    }


    HandlePageDragDrop(event) {
        //console.log(event);
        event.preventDefault();
        event.stopPropagation();
    }

    HandleStyleButtonClick(tag) {
        this.ToggleTextFormat(tag);
    }
    HandleBlockStyleClick(tag) {
        this.ToggleBlockTag(tag);
    }
    HandleBlockFormatClick(style) {
        this.ToggleBlockFormat(style);
    }
    HandleStandardBlockSelectorClick(event) {

        if (event.isTrusted) {
            this.ChangeBlockType(this.standardBlockOptions.value);
        }

    }

    Initialize() {
        this.Editor.classList.add(Active);
        this.Page.classList.add(Active);
			
        if (this.Page.children.length === 0) {
            let paragraph = this.CreateNewBlock();
            this.Page.append(paragraph);
        }
		Array.from(this.Page.childNodes).forEach((child) => {
			if(child.nodeType === Node.TEXT_NODE){
				this.Page.removeChild(child);
			}
		});

        document.addEventListener('selectionchange', this.boundHandlePageSelectionChange);

        this.Page.addEventListener('keydown', this.boundHandlePageKeyDown);
        this.Page.addEventListener('focusout', this.boundHandlePageFocusOut);
        this.Page.addEventListener('mouseup', this.boundHandlePageMouseUp);
        this.Page.addEventListener('mousedown', this.boundHandlePageMouseDown);
        this.Page.addEventListener('input', this.boundHandlePageInput);
        this.Page.addEventListener('dragstart', this.boundHandlePageDragStart);
        this.Page.addEventListener('drop', this.boundHandlePageDragDrop);
        this.Page.addEventListener('paste', this.boundHandlePagePaste);
    }
	
	CreateButtonContainer(){
		let container = document.createElement('div');
		container.classList.add(ConstButtonContainerStyles);

		this.Editor.children[0].append(container);			
		return container;
	}

    CreateStyleButtons(containerid) {
        let styles = {
            bold: this.NewClickButton(containerid, this.boundHandleBoldClick, '&#914', 'Bold'),
            italic: this.NewClickButton(containerid, this.boundHandleItalicClick, '&#1030', 'Italic'),
            underline: this.NewClickButton(containerid, this.boundHandleUnderlineClick, '&#9078', 'Underline'),
            strikethrough: this.NewClickButton(containerid, this.boundHandleStrikeClick, 'S', 'Strike Through'),
            superscript: this.NewClickButton(containerid, this.boundHandleSuperscriptClick, 'X<sup>2</sup>', 'Superscript'),
            subscript: this.NewClickButton(containerid, this.boundHandleSubscriptClick, 'X<sub>2</sub>', 'Subscript'),
        }
        return styles;
    }

    CreateStandardBlockOptions(container) {
        let selector = document.createElement('select');
        container.append(selector);
        selector.appendChild(this.AddNewOption(ConstBlock, 'Paragraph'));
        selector.appendChild(this.AddNewOption(ConstH1, 'Header 1'));
        selector.appendChild(this.AddNewOption(ConstH2, 'Header 2'));
        selector.appendChild(this.AddNewOption(ConstH3, 'Header 3'));
        selector.appendChild(this.AddNewOption(ConstH4, 'Header 4'));
        selector.appendChild(this.AddNewOption(ConstH5, 'Header 5'));
        selector.appendChild(this.AddNewOption(ConstH6, 'Header 6'));

        selector.addEventListener('change', this.boundHandleBlockOptionsClick);
        return selector
    }

    AddNewOption(value, text) {
        let option = document.createElement('option');
        option.value = value;
        option.innerHTML = text;

        return option;
    }

    CreateBlockButtons(containerid) {
        let blocks = {
            OL: this.NewClickButton(containerid, this.boundHandleOrderedListClick, '&#9776', 'Ordered List'),
            UL: this.NewClickButton(containerid, this.boundHandleUnorderedListClick, '&#9783', 'Unordered List')
        }

        return blocks;
    }

    CreateBlockFormatButtons(containerid) {
        let formats = {
            Indent: this.NewClickButton(containerid, this.boundHandleIndentClick, '&#8640', 'Indent'),
            Outdent: this.NewClickButton(containerid, this.boundHandleOutdentClick, '&#8636', 'Outdent'),
            JustifyLeft: this.NewClickButton(containerid, this.boundHandleJustifyLeftClick, '&#8676', 'Justify Left'),
            JustifyCenter: this.NewClickButton(containerid, this.boundHandleJustifyCenterClick, '&#8633', 'Justify Center'),
            JustifyRight: this.NewClickButton(containerid, this.boundHandleJustifyRightClick, '&#8677', 'Justify Right'),
            JustifyFull: this.NewClickButton(containerid, this.boundHandleJustifyFullClick, '&#8700', 'Justify Full'),
        }

        return formats;
    }

    NewClickButton(container, handleBinding, text, description) {
        let button = document.createElement('button');
        button.classList = ConstButtonStyle;
        button.innerHTML = text;
        button.title = description;
        container.appendChild(button);

        button.addEventListener('click', handleBinding);

        return button;
    }

    Selection_KeyInput(event) {
        console.log(event);
    }
    PrintTargets() {
        this.Selection_LazySelector_Word();
		console.log(this.StartBlock);
		console.log(this.EndBlock);
		console.log(document.getSelection().getRangeAt(0));
    }

    KeyBoardPageEvents(event) {
        if (event.keyCode === 192) {
            event.preventDefault();
            event.stopPropagation();
            this.PrintTargets();
        }
        else if (event.code === 'Enter') {
            event.preventDefault();
            event.stopPropagation();
            let selection = document.getSelection();
            let rangeclone = selection.getRangeAt(0).cloneRange();

            if (selection.type === 'Range') {
                rangeclone = this.Selection_Delete();
                rangeclone.setStart(rangeclone.endContainer, rangeclone.endOffset);

                selection.removeAllRanges();
                selection.addRange(rangeclone);
            }
            else { //Selection Type is Caret               
                if (event.shiftKey) {
                    if (this.Block_is_Standard_Structure(this.StartBlock)) {
                        rangeclone = this.Block_Standard_Behavior_ShiftEnterKey();
                    }
                    else {
                        if (this.Block_is_List_Structure(this.StartBlock)) {
                            rangeclone = this.Block_List_Behavior_ShiftEnterKey();
                        }
                    }
                }
                else { //Logic for a Normal Enter Key Press                   
                    if (this.Block_is_Standard_Structure(this.StartBlock)) {
                        rangeclone = this.Block_Standard_Behavior_EnterKey();
                    }
                    else {
                        //SubBlock Level Unique Enter Behaviors
                        if (this.Block_is_List_Structure(this.StartBlock)) {
                            rangeclone = this.Block_List_Behavior_EnterKey();
                        }
                    }
                }
            }
            selection.removeAllRanges();
            selection.addRange(rangeclone);

        }
        else if (event.code === 'Backspace') {

            //Create my Own behaviors to ensure I handle the paragraph creation and deletion for targeting system.
            let selection = document.getSelection();
            let rangeclone = selection.getRangeAt(0).cloneRange();

            if (selection.type === 'Range') {
                event.preventDefault();
                event.stopPropagation();

                rangeclone = this.Selection_Delete();
                rangeclone.setEnd(rangeclone.startContainer, rangeclone.startOffset);

                selection.removeAllRanges();
                selection.addRange(rangeclone);

            }
            else { //Selection Type is Caret

                if (this.Block_is_Standard_Structure(this.StartBlock)) {
                    rangeclone = this.Block_Standard_Behavior_BackspaceKey(event);
                }
                else { //Process Alternate Block Types

                    if (this.Block_is_List_Structure(this.StartBlock)) {
                        rangeclone = this.Block_List_Behavior_BackspaceKey(event);
                    }
                }
            }
        }

        else if (event.code === 'Delete') {
            let selection = document.getSelection();
            let rangeclone = selection.getRangeAt(0).cloneRange();

            if (selection.type === 'Range') {
                event.preventDefault();
                event.stopPropagation();

                rangeclone = this.Selection_Delete();
                rangeclone.setStart(rangeclone.endContainer, rangeclone.endOffset);

                selection.removeAllRanges();
                selection.addRange(rangeclone);

            }
            else { //Selection Type is Caret

                if (this.Block_is_Standard_Structure(this.StartBlock)) {
                    rangeclone = this.Block_Standard_Behavior_DeleteKey(event);
                }
                else {
                    if (this.Block_is_List_Structure(this.StartBlock)) {
                        rangeclone = this.Block_List_Behavior_DeleteKey(event);
                    }
                }

            }
        }
        else { //Default Behavior
            if (document.getSelection().type === 'Range') {

                if (event.key.startsWith('Arrow') === false &&
                    (!event.shiftKey && !event.altKey && !event.ctrlKey)) {
                    let selection = document.getSelection();
                    let range = selection.getRangeAt(0);
                    range = this.Selection_Delete();
                    range.setEnd(range.startContainer, range.startOffset);
                }
            }
        }
    }

    //Merges previous Blocks if its the same type or sets the caret to the end of the previous block
    BackSpace_BlockBehavior(previousBlock) {
        let range = document.createRange();
        //Turn into Function
        if (previousBlock.tagName === this.StartBlock.tagName) {
            //let lasttree = this.GetLastTreeNode(previousBlock);
            range.selectNodeContents(previousBlock);
            this.StartBlock.prepend(range.extractContents());
            this.Page.removeChild(previousBlock);
            let length = this.Core_MergeIdenticalTree_Previous(this.RangeTree_Start);
            let tempTextNode = this.GetLastNode(this.RangeTree_Start);

            if (tempTextNode.nodeType === Node.TEXT_NODE) {
                range.setStart(tempTextNode, length);
                range.setEnd(tempTextNode, length);
            }
            else { //This was a BR Tag, set caret to parent node and offset to position of child index to place caret on BR tags. Technically always 0, but this handles multiple BR Tags in a tree
                range.setStart(tempTextNode.parentNode, tempTextNode.parentNode.childNodes.length - 1);
                range.setEnd(tempTextNode.parentNode, tempTextNode.parentNode.childNodes.length - 1);
            }
        }
        else {
            let previousLast = this.GetLastNode(previousBlock);

            if (previousLast.tagName === ConstBR) {
                range.setStart(previousLast.parentNode, previousLast.parentNode.childNodes.length - 1);
                range.setEnd(previousLast.parentNode, previousLast.parentNode.childNodes.length - 1);
            }
            else {
                range.setStart(previousLast, previousLast.textContent.length);
                range.setEnd(previousLast, previousLast.textContent.length);
            }
            //Deletes
            if (this.StartBlock.textContent.length === 0) {
                this.Page.removeChild(this.StartBlock);
            }
        }

        return range;
    }

    //Merges Next Block if its the same type or sets the caret to the start of the next block
    Delete_BlockBehavior(nextBlock) { //Delete_BlockBehavior
        let rangeclone = document.createRange();
        if (nextBlock.tagName === this.StartBlock.tagName) {
            rangeclone.selectNodeContents(nextBlock);
            let fragment = rangeclone.extractContents();
            this.StartBlock.append(fragment);
            this.Page.removeChild(nextBlock);
            let length = this.RangeTree_Start.textContent.length;
            this.Core_MergeIdenticalTrees(this.RangeTree_Start);
            let savenode = this.GetLastNode(this.RangeTree_Start);

            if (savenode.nodeType === Node.TEXT_NODE) {
                rangeclone.setStart(savenode, length);
                rangeclone.setEnd(savenode, length);
            }
            else {
                rangeclone.setStart(savenode.parentNode, 0);
                rangeclone.setEnd(savenode.parentNode, 0);
            }
        }
        else {
            if (nextBlock.textContent.length === 0 && nextBlock.childNodes.length <= 1) {
                this.Page.removeChild(nextBlock);
            }
            else {
                let nextFirst = this.GetFirstNode(nextBlock);

                if (nextFirst.tagName === ConstBR) {
                    rangeclone.setStart(nextFirst.parentNode, nextFirst.parentNode.childNodes.length - 1);
                    rangeclone.setEnd(nextFirst.parentNode, nextFirst.parentNode.childNodes.length - 1);
                }
                else {
                    rangeclone.setStart(nextFirst, 0);
                    rangeclone.setEnd(nextFirst, 0);
                }
            }
        }
        return rangeclone;
    }
	
	//Deletes the selected Range and returns 
    Selection_Delete() {
        let resultRange = document.getSelection().getRangeAt(0);

		//Range Delete Solution Divides Blocks, deletes every block between the new outer blocks
        let outerend = this.EndBlock.nextSibling;
        if (this.atContainerEnd(this.EndBlock, resultRange) === false) {
			//Extra Check to prevent division of container with 2 or less BR tags, subsequent traversal code will clean up EndBlock
			if(this.EndBlock.textContent.length !== 0 && this.EndBlock.childNodes.length <= 2){
				this.Core_DivideContainer_New_End(this.EndBlock, this.EndBlock.tagName);
				outerend = this.EndBlock.nextSibling;
			}
        }

        if (this.atContainerStart(this.StartBlock, resultRange) === false) {
            this.Core_DivideContainer_New_Front(this.StartBlock, this.StartBlock.tagName);
        }
        else {
            if (this.StartBlock.previousSibling === null) {
                let block = this.CreateNewBlock();
                this.StartBlock.insertAdjacentElement('beforebegin', block);
            }
        }
        let outerstart = this.StartBlock.previousSibling;

        //Delete The Sandwiched Blocks
        let iter = this.StartBlock;
        while (iter !== this.EndBlock) {
            let temp = iter;
            iter = iter.nextSibling;
            temp.parentNode.removeChild(temp);
        }
        this.EndBlock.remove();
		
		//Merge and/or Set the Range
        let lastTree = this.GetLastTreeNode(outerstart);
        if (outerend !== null) {
            let firstTree = this.GetFirstTreeNode(outerend);
			
            if (outerstart.tagName === outerend.tagName) { //Merge Blocks if same
                resultRange.selectNodeContents(outerend);
                outerstart.append(resultRange.extractContents());
                this.Page.removeChild(outerend);
				
                let length = this.Core_MergeIdenticalTree_Previous(firstTree);
				
                let newend = this.GetTreeNode(firstTree);
                if (newend.nodeType === Node.TEXT_NODE) {
                    resultRange.setStart(newend, length);
                    resultRange.setEnd(newend, length);
                }
                else {
                    resultRange.setStart(newend.parentNode, newend.parentNode.childNodes.length - 1);
                    resultRange.setEnd(newend.parentNode, newend.parentNode.childNodes.length - 1);
                }
            }
            else {
                let newfront = this.GetTreeNode(lastTree);
                if (newfront.nodeType === Node.TEXT_NODE) {
                    resultRange.setStart(newfront, newfront.textContent.length);
                }
                else {
                    resultRange.setStart(newfront.parentNode, newfront.parentNode.childNodes.length - 1);
                }
                let newend = this.GetTreeNode(firstTree);
                if (newend.nodeType === Node.TEXT_NODE) {
                    resultRange.setEnd(newend, 0);
                }
                else {
                    resultRange.setEnd(newend.parentNode, newend.parentNode.childNodes.length - 1);
                }
            }
        }
        else {
            let node = this.GetTreeNode(lastTree);
            if (node.nodeType === Node.TEXT_NODE) {
                resultRange.setStart(node, node.textContent.length);
                resultRange.setEnd(node, node.textContent.length);
            }
            else {
                resultRange.setStart(node.parentNode, node.parentNode.childNodes.length - 1);
                resultRange.setEnd(node.parentNode, node.parentNode.childNodes.length - 1);
            }
        }
        return resultRange;
    }

    CreateNewBlock() {
        const element = document.createElement(ConstBlock);
        element.appendChild(this.CreateNewTextTree());
        return element;
    }

    CreateNewTextTree() {
        const texttree = document.createElement(ConstTree);
        const br = document.createElement(ConstBR);
        texttree.appendChild(br);
        return texttree;
    }
    CreateNewListItem() {
        const element = document.createElement('li');
        element.appendChild(this.CreateNewTextTree());
        return element;
    }

    isWhiteSpace(node) {
        return !/[^\t\n\r ]/.test(node.textContent);
    }

    isEmptyTag(node) {
        let isEmpty = false;

        if (node.textContent === "") {
            isEmpty = true;
        }
        else {
            node = this.GetTreeNode(node);
            if (node.nodeType !== Node.TEXT_NODE) {
                if (node.tagName !== ConstBR) {
                    isEmpty = true;
                }
            }
        }
        return isEmpty;
    }

    GetNextBlock(BlockNode) {
        BlockNode = BlockNode.nextSibling;
        return BlockNode;
    }

    GetPreviousBlock(BlockNode) {
        BlockNode = BlockNode.previousSibling;
        return BlockNode;
    }


    Core_MergeIdenticalTrees(treeNode) {
        let length = 0;
        if (treeNode.nextSibling !== null) {
            if (treeNode.nextSibling.textContent.length !== 0) {
                if (this.Core_hasIdentical_Tags(treeNode, treeNode.nextSibling) === true) {
                    this.GetTreeNode(treeNode).textContent += treeNode.nextSibling.textContent;
                    treeNode.nextSibling.remove();
                }
            }
        }
        length = this.Core_MergeIdenticalTree_Previous(treeNode);
        return length;
    }


    Core_MergeIdenticalTree_Previous(treeNode) {
        //let length = treeNode.textContent.length;

        let length = 0;
        if (treeNode.previousSibling !== null) {
            if (treeNode.previousSibling.textContent.length !== 0) {
                if (this.Core_hasIdentical_Tags(treeNode, treeNode.previousSibling) === true) {
                    //console.log('merging previous');
                    if (treeNode.previousSibling === this.RangeTree_Start) {
                        this.RangeTree_Start = treeNode;
                    }
                    let textNode = this.GetTreeNode(treeNode);
                    length = treeNode.previousSibling.textContent.length;

                    textNode.textContent = treeNode.previousSibling.textContent + textNode.textContent;
                    treeNode.previousSibling.remove();
                    treeNode = null;
                }
            }
        }
        return length;
    }

    Core_hasIdentical_Tags(anchorTreeNode, siblingTreeNode) {
        let isEqual = true;

        let left = anchorTreeNode;
        let right = siblingTreeNode;
        while (right.nodeType === Node.ELEMENT_NODE && isEqual) {
            if (this.Core_SearchTreeForTag(left, right)) {
                right = right.firstChild;
            }
            else {
                isEqual = false;
            }
        }

        left = anchorTreeNode;
        right = siblingTreeNode;
        while (left.nodeType === Node.ELEMENT_NODE && isEqual) {
            if (this.Core_SearchTreeForTag(right, left)) {
                left = left.firstChild;
            }
            else {
                isEqual = false;
            }
        }

        return isEqual;
    }

    Core_SearchTreeForTag(containerNode, style) {
        let test = false;
        while (containerNode.firstChild !== null) {
            if (containerNode.tagName === style.tagName) {
                test = true;
            }
            containerNode = containerNode.firstChild;
        }
        return test;
    }

    atContainerStart(container, range) {
        let atStart = false;
        let startNode = this.GetFirstNode(container);
        if (range.startOffset === 0) {
            if (startNode === range.startContainer) {
                atStart = true;
            }
            if (startNode.tagName === ConstBR) {
                if (startNode === range.startContainer.firstChild) {
                    atStart = true;
                }
            }
        }

        return atStart;
    }

    atContainerEnd(container, range) {
        let atEnd = false;
        let endNode = this.GetLastNode(container);

        if (endNode === range.endContainer && range.endOffset === endNode.textContent.length) {
            atEnd = true;
        }
        if (endNode.tagName === ConstBR) {
            if (endNode === range.endContainer.lastChild) {
                atEnd = true;
            }
        }
        return atEnd;
    }

    //TODO: Clean Up these GetNode Functions
    GetTreeNode(containerNode) {

        while (containerNode.firstChild !== null) {
            containerNode = containerNode.firstChild;
        }
        return containerNode;
    }

    GetFirstTreeNode(BlockContainer) {
        while (BlockContainer.tagName !== ConstTree) { //Change null checked BlockContainer
            BlockContainer = BlockContainer.firstChild;
        }
        return BlockContainer;
    }

    GetLastTreeNode(BlockContainer) {
        while (BlockContainer.tagName !== ConstTree) {
            BlockContainer = BlockContainer.lastChild;
        }
        return BlockContainer;
    }

    GetFirstNode(containerToSearch) {
        let result = null
        while (result === null) {

            if (containerToSearch.tagName === ConstBR) {
                result = containerToSearch;
            }
            else {
                if (containerToSearch.nodeType === Node.TEXT_NODE) {
                    result = containerToSearch;
                }
                else {
                    containerToSearch = containerToSearch.firstChild;
                }
            }

        }
        return result;
    }

    GetLastNode(containerToSearch) {
        let result = null;
        while (result === null) {
            if (containerToSearch.tagName === ConstBR) {
                result = containerToSearch;
            }
            else {
                if (containerToSearch.nodeType === Node.TEXT_NODE) {
                    result = containerToSearch;
                }
                else {
                    containerToSearch = containerToSearch.lastChild;
                }
            }
        }

        return result;
    }

    ToggleBlockTag(blocktag) {

        let element = document.createElement(blocktag);
        let item = this.CreateNewListItem();
        element.appendChild(item);
        this.StartBlock.insertAdjacentElement('afterend', element);
        let range = document.createRange();
        range.setStart(item.firstChild, 0);
        range.setEnd(item.firstChild, 0);
        document.getSelection().removeAllRanges();
        document.getSelection().addRange(range);

    }

    Core_DivideContainer_New_End(container, TagString) {
        let range = document.getSelection().getRangeAt(0);
        let deepclone = new CloneWithRange(container, range);
        let clonedelement = deepclone.GetEnd_nonInclusive(TagString);
        container.insertAdjacentElement('afterend', clonedelement);

        let tempRange = document.createRange();
        tempRange.selectNodeContents(container);
        tempRange.setStart(range.endContainer, range.endOffset);
        tempRange.deleteContents();
        tempRange.detach();
        //Potential Edge Case for Empty Tags Caused by deleteContents Function	
		if(this.isEmptyTag(container.lastChild)){
			container.removeChild(container.lastChild);			
		}

    }

    Core_DivideContainer_New_Front(container, TagString) {
        let range = document.getSelection().getRangeAt(0);
        let deepclone = new CloneWithRange(container, range);
        let fronthalf = deepclone.GetFront_nonInclusive(TagString);

        container.insertAdjacentElement('beforebegin', fronthalf);

        let temprange = document.createRange();
        temprange.selectNodeContents(container);
        temprange.setEnd(range.startContainer, range.startOffset);
        temprange.deleteContents();

        if (this.isEmptyTag(container.firstChild)) {
            container.firstChild.remove();
        }
    }

    Core_ImplementTag_Tree(container, tag, behavior) {
        if (container.textContent.length !== 0) {
            let styledelement = container.querySelector(tag);
            if (styledelement === null && (behavior === Behavior.Add || behavior === Behavior.Toggle)) {
                let rangeclone = document.createRange();
                rangeclone.selectNodeContents(container);
                let style = document.createElement(tag);
                style.append(rangeclone.extractContents());
                container.append(style);
                rangeclone.detach();
            }

            if (styledelement !== null && (behavior === Behavior.Remove || behavior === Behavior.Toggle)) {
                styledelement.parentNode.replaceChild(styledelement.firstChild, styledelement);
                //let inner = styledelement.innerHTML;                
                //styledelement.insertAdjacentHTML('afterend', inner);
                //styledelement.remove();
            }
        }
    }

    Core_Set_Range_Selection(startoffset, endoffset) {
        let selectionRange = document.createRange();
        let start = this.GetTreeNode(this.RangeTree_Start);
        if (start.nodeType === Node.TEXT_NODE) {
            selectionRange.setStart(start, startoffset);
        }
        else {
            selectionRange.setStart(start.parentNode, 0);
        }

        let end = this.GetTreeNode(this.RangeTree_End);
        if (end.nodeType === Node.TEXT_NODE) {
            selectionRange.setEnd(end, endoffset);
        }
        else {
            selectionRange.setEnd(end.parentNode, endoffset);
        }

        return selectionRange;
    }

    Block_is_Standard_Structure(container) {
        return StandardStructureTags.includes(container.tagName);
    }
    Block_is_List_Structure(blockLevel_Container) {
        let isListBlock = false;
        if (blockLevel_Container.tagName === ConstOList || blockLevel_Container.tagName === ConstUList) {
            isListBlock = true;
        }
        return isListBlock;
    }

    Block_Traversal_Find_First_Tree_In_NextBlock(block, lastTree) {
        let iter = { block, tree: null };

        if (this.Block_is_Standard_Structure(iter.block)) {
            iter = this.Block_Standard_Traversal_SubRoutine(iter.block);
        }
        else {
            if (this.Block_is_List_Structure(iter.block)) {
                iter = this.Block_List_Traversal_SubRoutine(iter.block, lastTree.parentNode);
            }
        }

        return iter;
    }

    Core_DetermineStyleBehavior(tag) {
        let behavior = Behavior.Add;

        let treecount = 0;
        let stylecount = 0;

        //count the Tail Node - Can be infront to bypass counter
        treecount++;
        if (this.RangeTree_End.querySelector(tag) !== null) {
            stylecount++;
        }

        let iter = { block: this.StartBlock, tree: this.RangeTree_Start };
        let lastiter = this.RangeTree_Start;
        //Only remove if the counts are equal - Break out as soon as a tag is not found in a tree
        while (iter.tree !== this.RangeTree_End && treecount === stylecount) {
            if (iter.tree === null) {//Increment the tempiter
                iter = this.Block_Traversal_Find_First_Tree_In_NextBlock(iter.block, lastiter);
            }
            else {
                // Checking length will ignore trees with BR tags, but will unsync the count logic to determine the behavior
                if (iter.tree.textContent.length !== 0) {
                    treecount++;
                    if (iter.tree.querySelector(tag) !== null) {
                        stylecount++;
                    }
                }
                lastiter = iter.tree;
                iter.tree = iter.tree.nextSibling;
            }
        }


        if (treecount === stylecount) {
            behavior = Behavior.Remove;
        }
        return behavior
    }


    //Lazy Selector Modifies the selection to the exact size of the word if caret is in the middle of a word
    //If Caret is outside of the word, it will do nothing.
    Selection_LazySelector_Word() {
        // Lazy Select Toggle - Create a class boolean to enable user to toggle this feature on and off.
		if(true){
			let selection = document.getSelection();
			let originalrange = selection.getRangeAt(0).cloneRange();

			let offset = originalrange.startOffset;

			selection.modify('move', 'forward', 'word');
			selection.modify('extend', 'backward', 'word');
			let newrange = selection.getRangeAt(0);
			let wordlength = selection.toString().trim().length;
			let wordStart = this.FindTargetStartTree(newrange.startContainer);

			offset -= newrange.startOffset;
			let counter = 0;
			while (this.RangeTree_Start !== wordStart) {
            counter += wordStart.textContent.length;
            wordStart = wordStart.nextSibling;
			}
			offset += counter;


			if (offset <= 0 || offset >= wordlength) {
				selection.setPosition(originalrange.startContainer, originalrange.startOffset);
			}
			else {
				let tree = this.FindTargetStartTree(newrange.startContainer);
				let counter = -newrange.startOffset;
				while (counter < wordlength) {
					if (counter + tree.textContent.length < wordlength) {
						counter += tree.textContent.length;
						tree = tree.nextSibling;
					}
					else {
                    offset = wordlength - counter;
                    counter = wordlength;
					}
				}
				selection.setBaseAndExtent(newrange.startContainer, newrange.startOffset, this.GetTreeNode(tree), offset);
			}
			originalrange.detach();
			newrange.detach();
			
			this.RebuildTargetSelections();
		}
       
    }

    ChangeBlockType(blocktag) {
        if (this.Block_is_Standard_Structure(this.StartBlock) === true) {
            if (this.StartBlock.tagName !== blocktag) {
                let range = document.createRange();
                range.selectNodeContents(this.StartBlock);
                let newBlock = document.createElement(blocktag);
                newBlock.appendChild(range.extractContents());
                this.Page.replaceChild(newBlock, this.StartBlock);
                range.detach();

                let node = this.GetTreeNode(newBlock);
                if (node.nodeType === Node.TEXT_NODE) {                   
                    document.getSelection().setPosition(node, 0);
                }
                else {
                    document.getSelection().setPosition(node.parentNode, 0);
                }
             
            }
        }

    }

    ToggleBlockFormat(classname) {
		let range = document.getSelection().getRangeAt(0).cloneRange();
        if (this.Block_is_Standard_Structure(this.StartBlock) === true) {          
            if (classname === ClassIndent || classname === ClassOutdent) {              
                if (classname === ClassIndent) {
                    this.StartBlock.classList.add(ClassIndent);
                }
                else {
                    this.StartBlock.classList.remove(ClassIndent);
                }
            }
            else {
                BlockClasses.forEach((value) => {
                    if (this.StartBlock.classList.contains(value)) {
                        this.StartBlock.classList.remove(value);
                    }
                });
                this.StartBlock.classList.add(classname);
            }           
        }
		//setBaseAndExtent function is unable to unselect a html selector
		document.getSelection().removeAllRanges();
        document.getSelection().addRange(range);

    }
    //Applies a Style to the selected text
    //If a button is clicked, it changes the focus and selection.
    //Function needs to capture the text node and the offset to put the caret back on the text node.
    ToggleTextFormat(StyleTag) {
        let selection = document.getSelection();
        if (selection.type === 'Caret') {
            this.Selection_LazySelector_Word();
        }
		let range = selection.getRangeAt(0);
		
        //Solution
        //1. Divide Containers from selection to maintain 1 text node per a tree container
        //2. Preserve target container during divide
        //3. Apply Style to the preserved tree container
        //4. Merge Identical sibling Trees
        if (selection.type === 'Range') {
            if (this.RangeTree_Start === this.RangeTree_End) {
                //Possible Selection Patterns
                //1. Entire Tree is Selected - no need to divide the tree
                //2. Front Half of Tree is Selected
                //3. Middle of Tree selected - break off the front and optionally the end into new trees
                if (selection.toString().trim().length !== this.RangeTree_Start.textContent.trim().length) {
                    //Function 1
                    if (this.atContainerStart(this.RangeTree_Start, range) === true) {
                        //Pattern 2
                        this.Core_DivideContainer_New_End(this.RangeTree_Start, ConstTree);
                    }
                    else {
                        //Function 3 - Divides Container into 3 parts, end is optional
                        //Process the End first to preserve the integrity of the original range's startOffset
                        if (this.atContainerEnd(this.RangeTree_End, range) === false) {
                            this.Core_DivideContainer_New_End(this.RangeTree_Start, ConstTree);
                        }
                        //call Pattern 2
                        this.Core_DivideContainer_New_Front(this.RangeTree_Start, ConstTree);
                    }
                }
                let caretPosition = this.RangeTree_Start.textContent.length;
                let offset = 0;
                // All Trees Are full Selection After Divide
                this.Core_ImplementTag_Tree(this.RangeTree_Start, StyleTag, Behavior.Toggle);
                offset = this.Core_MergeIdenticalTrees(this.RangeTree_Start);
				selection.setBaseAndExtent(this.GetTreeNode(this.RangeTree_Start), offset, this.GetTreeNode(this.RangeTree_End), caretPosition + offset);			
				
            }
            else {
                //Figure out if I'm adding or Removing Tags
                let behavior = this.Core_DetermineStyleBehavior(StyleTag);
                let caretStartPosition = 0;
                let caretEndPosition = range.endOffset;

                //List Traversal occurs in a forward direction
                //Only Merge Previous identical trees to preserve the forward structure
                //Possible Selection Patterns for Start Nodes
                //1. Entire Start Tree is Selected
                //2. Back Half of Start Tree is Selected
                if (range.startOffset !== 0) {
                    //Function 1
                    this.Core_DivideContainer_New_Front(this.RangeTree_Start, ConstTree);
                    this.Core_ImplementTag_Tree(this.RangeTree_Start, StyleTag, behavior);
                }
                else { //Handle the RangeTree_Start to calculate the caretStartPosition
                    this.Core_ImplementTag_Tree(this.RangeTree_Start, StyleTag, behavior);
                    caretStartPosition = this.Core_MergeIdenticalTree_Previous(this.RangeTree_Start);
                }

                //Possible Selection Patterns for Middle Nodes
                //1. Entire Tree iter is Selected
                let iter = { block: this.StartBlock, tree: this.RangeTree_Start.nextSibling };
                let tempiter = this.RangeTree_Start;
                while (iter.tree !== this.RangeTree_End) {
                    if (iter.tree === null) {
                        iter = this.Block_Traversal_Find_First_Tree_In_NextBlock(iter.block, tempiter);
                    }
                    else {
                        this.Core_ImplementTag_Tree(iter.tree, StyleTag, behavior);
                        this.Core_MergeIdenticalTree_Previous(iter.tree);
                        tempiter = iter.tree;
                        iter.tree = iter.tree.nextSibling;
                    }
                }

                //Possible Selection Patterns for 
                //1. Entire End Tree is Selected - divide not necessary
                //2. Front Half of End Tree is Selected - Do division
                if (this.atContainerEnd(this.RangeTree_End, range) === false) {
                    this.Core_DivideContainer_New_End(this.RangeTree_End, ConstTree);
                }
                this.Core_ImplementTag_Tree(this.RangeTree_End, StyleTag, behavior);
                caretEndPosition += this.Core_MergeIdenticalTrees(this.RangeTree_End);

                //Set the Range
                let selectionRange = this.Core_Set_Range_Selection(caretStartPosition, caretEndPosition);
                selection.removeAllRanges();
                selection.addRange(selectionRange);
            }

        }
    }

    Target_FindBlock(node) {
        while (node.parentNode !== this.Page) {
            node = node.parentNode;
        }
        return node;
    }

    //Takes in a the Range's startContainer and traverses to the Tree Node
    //If the input is not a text node, it is the parent container of a br tag
    FindTargetStartTree(TextNode) {

        if (TextNode.nodeType === Node.TEXT_NODE) {
            while (TextNode.tagName !== ConstTree) {
                TextNode = TextNode.parentNode;
            }
        }
        else if (TextNode.tagName !== ConstTree) {
            if (TextNode.tagName === ConstBR) {
                TextNode = TextNode.parentNode;
            }
            if (TextNode.tagName !== ConstTree) {
                if (TextNode.firstChild.tagName !== ConstTree) {
                    console.log('Find Target Start Tree: Normalizing Block');
                    TextNode = this.NormalizeBlock(TextNode);
                    document.getSelection().getRangeAt(0).setStart(TextNode, 0);
                }
                else {
                    TextNode = TextNode.firstChild;
                }

            }
        }

        return TextNode;
    }
    //Browsers treat the end differently from the start, the selecting a target has an extra edge case
    FindTargetEndTree(TextNode) {
        if (TextNode.nodeType === Node.TEXT_NODE) {
            while (TextNode.tagName !== ConstTree) {
                TextNode = TextNode.parentNode;
            }
        }
        else if (TextNode.tagName !== ConstTree) {

            if (TextNode.tagName === ConstBR) {
                TextNode = TextNode.parentNode;
            }
            if (TextNode.tagName !== ConstTree) {
                if (TextNode.firstChild.tagName !== ConstTree) {
                    console.log('Find Target End Tree: Normalizing Block');
                    this.NormalizeBlock(TextNode);
                    document.getSelection().getRangeAt(0).setStart(TextNode, 0);
                }
                else {
                    TextNode = TextNode.firstChild;
                }
            }
        }
        return TextNode;
    }

    NormalizeBlock(BlockNode) {

        let element = document.createElement(ConstTree);
        element.appendChild(document.createElement(ConstBR));
        BlockNode.removeChild(BlockNode.firstChild);
        BlockNode.appendChild(element);
        BlockNode = element;
        return element;
    }

    RepairTargetTree() { //Prototype Code - Default Browser Behaviors can mishandle the tree structure
        //Normalize the RangeTree, If a tree has more than 2 children, adding a character wasn't added in a new tree
        let range = document.getSelection().getRangeAt(0);
        if (this.RangeTree_Start !== null) {
            if (this.RangeTree_Start.childNodes.length >= 2) {
                if (this.RangeTree_Start.lastChild.tagName === ConstBR) {
                    let tree = this.CreateNewTextTree();
                    this.RangeTree_Start.insertAdjacentElement('afterend', tree);
                    this.RangeTree_Start.removeChild(this.RangeTree_Start.lastChild);
                }
                else {
                    console.log('Repair Target Tree: Normalizing Block');
                    let clone = this.RangeTree_Start.lastChild.cloneNode(true);
                    this.RangeTree_Start.lastChild.remove();
                    let element = document.createElement(ConstTree);
                    element.appendChild(clone);
                    this.RangeTree_Start.insertAdjacentElement('afterend', element);
                    range.setStart(this.GetTreeNode(element), element.textContent.length);
                    range.setEnd(range.startContainer, range.startOffset);
                }
            }
        }

        return range;
    }

    RebuildTargetSelections() {
        this.UpdateTargets();
        this.ToggleInterfaceButtons();
        this.AddTargetHighLights();
    };

    UpdateTargets() {
        let range = this.RepairTargetTree();
        //console.log(`${range.startOffset}, ${range.endOffset}`);

        this.StartBlock = this.Target_FindBlock(range.startContainer);
        this.EndBlock = this.Target_FindBlock(range.endContainer);

        this.RangeTree_Start = this.FindTargetStartTree(range.startContainer);
        if (range.startContainer === range.endContainer) {
            this.RangeTree_End = this.RangeTree_Start;
        }
        else {
            this.RangeTree_End = this.FindTargetEndTree(range.endContainer);
        }
		
		
        this.XTargetStart = this.RangeTree_Start;
        this.XTargetEnd = this.RangeTree_End;
    }

    ToggleInterfaceButtons() {
        this.standardBlockOptions.value = this.StartBlock.tagName;
    }

    // CHanges the background Color of selected styles
    AddTargetHighLights() {
        if (this.StartBlock === this.EndBlock) {
            this.PlayHighLightAnimation_1(this.StartBlock);
        }
        else {
            let iter = this.StartBlock;
            do {
                this.PlayHighLightAnimation_1(iter);
                iter = iter.nextSibling;
                //iter.classList.add('c-menu');
            }
            while (iter !== this.EndBlock);
        }

        //Todo: highLight inner attributes
        this.XTargetStart.classList.add('text-bg');
        this.XTargetEnd.classList.add('text-bg');

        this.PlayHighLightAnimation_2(this.RangeTree_Start);
        this.PlayHighLightAnimation_2(this.RangeTree_End);
    }

    PlayHighLightAnimation_1(node) {
        if (node.nodeType !== Node.TEXT_NODE) {
            node.classList.add('text-highlight-1');
            setTimeout(() => {
                node.classList.remove('text-highlight-1');
            }, 1500);
        }
    }

    PlayHighLightAnimation_2(node) {
        if (node.nodeType !== Node.TEXT_NODE) {
            node.classList.add('text-highlight-2');
            setTimeout(() => {
                node.classList.remove('text-highlight-2');
            }, 1500);
        }
    }

    ClearTargetSelections() {
        //Error: Cannon Read classList of TextNodes
        if (this.XTargetStart !== null) {
            this.XTargetStart.classList.remove('text-bg');
        }
        if (this.XTargetEnd !== null) {
            this.XTargetEnd.classList.remove('text-bg');
        }

    };

    Block_Standard_Behavior_ShiftEnterKey() {
        let rangeclone = document.getSelection().getRangeAt(0).cloneRange();

        if (this.atContainerEnd(this.RangeTree_End, rangeclone)) {
            
            if (this.GetTreeNode(this.RangeTree_End).nodeType === Node.TEXT_NODE) {
                if (this.RangeTree_Start.nextSibling === null) {
                    
                    let firstTree = this.CreateNewTextTree();
                    this.RangeTree_Start.insertAdjacentElement('afterend', firstTree);
                    this.RangeTree_Start = firstTree;
                }
                else {
                    this.RangeTree_Start = this.RangeTree_Start.nextSibling;
                }
            }
            
            let newTree = this.CreateNewTextTree();
            this.RangeTree_Start.insertAdjacentElement('afterend', newTree);
            rangeclone.setStart(newTree, 0);
            rangeclone.setEnd(newTree, 0);
        }
        else {
            
            if (this.atContainerStart(this.RangeTree_Start, rangeclone)) {
                
                let newtree = this.CreateNewTextTree();
                this.RangeTree_Start.insertAdjacentElement('beforebegin', newtree);
                rangeclone.setStart(newtree, 0);
                rangeclone.setEnd(newtree, 0);
            }
            else {
                
                this.Core_DivideContainer_New_Front(this.RangeTree_Start, ConstTree);
                this.RangeTree_Start.insertAdjacentElement('beforebegin', this.CreateNewTextTree());
                let text = this.GetTreeNode(this.RangeTree_Start);
                rangeclone.setStart(text, 0);
                rangeclone.setEnd(text, 0);
            }
        }
        return rangeclone;
    }

    Block_Standard_Behavior_EnterKey() {
        let rangeclone = document.getSelection().getRangeAt(0).cloneRange();
        let newparagraph = null;
        if (this.atContainerEnd(this.EndBlock, rangeclone)) {
            newparagraph = this.CreateNewBlock();
            this.StartBlock.insertAdjacentElement('afterend', newparagraph);
            newparagraph = newparagraph.firstChild;
        }
        else if (this.atContainerStart(this.StartBlock, rangeclone)) {
            newparagraph = this.CreateNewBlock();
            this.EndBlock.insertAdjacentElement('beforebegin', newparagraph);
            newparagraph = newparagraph.firstChild;
        }
        else {
            this.Core_DivideContainer_New_Front(this.StartBlock, this.StartBlock.tagName);
            newparagraph = this.GetTreeNode(this.StartBlock);
        }
        rangeclone.setStart(newparagraph, 0);
        rangeclone.setEnd(newparagraph, 0);
        return rangeclone;
    }

    Block_Standard_Behavior_BackspaceKey(event) {
        let selection = document.getSelection();
        let rangeclone = selection.getRangeAt(0).cloneRange();

        if (this.atContainerStart(this.RangeTree_Start, rangeclone) === true || this.RangeTree_Start.textContent.length === 1) {

            if (this.atContainerStart(this.StartBlock, rangeclone) === true) {
                event.preventDefault();
                event.stopPropagation();
                let previousBlock = this.GetPreviousBlock(this.StartBlock);
                if (this.Block_is_Standard_Structure(this.StartBlock)) {
                    if (previousBlock !== null) {
                        rangeclone = this.BackSpace_BlockBehavior(previousBlock);
                        selection.removeAllRanges();
                        selection.addRange(rangeclone);
                    }
                }
            }
            else {
                let previousTree = this.RangeTree_Start.previousSibling;
                if (this.RangeTree_Start.textContent.length === 1) {
                    if (previousTree !== null) {
                        event.preventDefault();
                        event.stopPropagation();
                        let previous = this.GetTreeNode(previousTree);
                        if (previous.nodeType === Node.TEXT_NODE) {
                            this.RangeTree_Start.remove();
                            let length = previous.textContent.length;
                            this.Core_MergeIdenticalTrees(previousTree);
                            rangeclone.setStart(previous, length);
                            rangeclone.setEnd(previous, length);
                        }
                        else {

                            let newTree = this.CreateNewTextTree();
                            this.RangeTree_Start.insertAdjacentElement('beforebegin', newTree);
                            this.RangeTree_Start.remove();
                            rangeclone.setStart(newTree, 0);
                            rangeclone.setEnd(newTree, 0);
                        }
                        selection.removeAllRanges();
                        selection.addRange(rangeclone);
                    }
                    else {
                        if (this.StartBlock.previousSibling === null) {//!IMPORTANT Recreate the First Block in the Document
                            event.preventDefault();
                            event.stopPropagation();
                            let block = this.CreateNewBlock();
                            this.StartBlock.insertAdjacentElement('beforebegin', block);
                            this.Page.replaceChild(block, this.StartBlock);
                            rangeclone.setStart(block.firstChild, 0);
                            rangeclone.setEnd(block.firstChild, 0);
                            selection.removeAllRanges();
                            selection.addRange(rangeclone);
                        }
                    }

                }
                else if (this.GetTreeNode(previousTree).nodeType !== Node.TEXT_NODE) { //Remove BR Tree and Try to Merge
                    event.preventDefault();
                    event.stopPropagation();
                    previousTree.remove();
                    let length = this.Core_MergeIdenticalTree_Previous(this.RangeTree_Start);
                    rangeclone.setStart(this.GetTreeNode(this.RangeTree_Start), length);
                    rangeclone.setEnd(this.GetTreeNode(this.RangeTree_Start), length);
                    selection.removeAllRanges();
                    selection.addRange(rangeclone);
                }
            }
        }
        return rangeclone;
    }

    Block_Standard_Behavior_DeleteKey(event) {
        let selection = document.getSelection();
        let rangeclone = selection.getRangeAt(0).cloneRange();
        if (this.atContainerEnd(this.RangeTree_End, rangeclone)) {
            if (this.atContainerEnd(this.StartBlock, rangeclone)) {
                event.preventDefault();
                event.stopPropagation();
                let nextBlock = this.GetNextBlock(this.StartBlock);
                if (nextBlock !== null) {
                    rangeclone = this.Delete_BlockBehavior(nextBlock);
                    selection.removeAllRanges();
                    selection.addRange(rangeclone);
                }
            }
            else {
                let nextTree = this.RangeTree_End.nextSibling;               //TextContent Length 1 is testing for a single character, Stable?
                if (this.GetTreeNode(nextTree).nodeType !== Node.TEXT_NODE || nextTree.textContent.length === 1) {
                    event.preventDefault();
                    event.stopPropagation();
                    nextTree.remove();
                    let length = this.RangeTree_Start.textContent.length;
                    this.Core_MergeIdenticalTrees(this.RangeTree_Start);
                    rangeclone.setStart(this.GetTreeNode(this.RangeTree_Start), length);
                    rangeclone.setEnd(this.GetTreeNode(this.RangeTree_Start), length);
                    selection.removeAllRanges();
                    selection.addRange(rangeclone);
                }
            }
        }
        return rangeclone;
    }

    // Editor Text Traversal - Finds the Next Block and returns the new block and tree iters
    Block_Standard_Traversal_SubRoutine(block) {
        let tree = null;
        block = block.nextSibling;
        tree = block.firstChild;
        //Search depth for next blocks of different types.
        while (tree.tagName !== ConstTree) {
            tree = tree.firstChild;
        }
        return { block, tree };
    }

    Block_List_Behavior_ShiftEnterKey() {
        let rangeclone = document.createRange();
        let li = document.createElement('li');
        let span = document.createElement(ConstTree);
        let br = document.createElement('br');
        li.appendChild(span);
        span.appendChild(br);
        this.RangeTree_Start.parentNode.insertAdjacentElement('afterend', li);
        rangeclone.setStart(span, 0);
        rangeclone.setEnd(span, 0);
        return rangeclone;
    }

    Block_List_Behavior_EnterKey() {
        let rangeclone = document.getSelection().getRangeAt(0).cloneRange();

        let currentListItem = this.RangeTree_Start.parentNode;
        if (this.RangeTree_Start.textContent.length === 0) {
            // First Enter Creates a list item, second enter deletes and exits the list block into a new paragraph
            if (currentListItem === this.StartBlock.firstChild) {
                let li = this.CreateNewListItem();
                currentListItem.insertAdjacentElement('afterend', li);
                rangeclone.setStart(li.firstChild, 0);
                rangeclone.setEnd(li.firstChild, 0);
            }
            else {
                this.StartBlock.removeChild(currentListItem);
                let block = this.CreateNewBlock();
                this.StartBlock.insertAdjacentElement('afterend', block);
                rangeclone.setStart(block.firstChild, 0);
            }
        }
        else {
            if (this.atContainerStart(currentListItem, rangeclone)) {
                let listitem = this.CreateNewListItem();
                currentListItem.insertAdjacentElement('beforebegin', listitem);
                rangeclone.setStart(listitem.firstChild, 0);
                rangeclone.setEnd(listitem.firstChild, 0);

            }
            else if (this.atContainerEnd(currentListItem, rangeclone)) {
                let listitem = this.CreateNewListItem();
                currentListItem.insertAdjacentElement('afterend', listitem);
                rangeclone.setStart(listitem.firstChild, 0);
                rangeclone.setEnd(listitem.firstChild, 0);
            }
            else {
                this.Core_DivideContainer_New_Front(currentListItem, ConstListItem);
                rangeclone.setStart(this.GetTreeNode(currentListItem), 0);
                rangeclone.setEnd(rangeclone.startContainer, 0);
            }
        }
        return rangeclone;
    }

    //Back Space Only executes if character is at the very start of the list item text.
    Block_List_Behavior_BackspaceKey(event) {
        let selection = document.getSelection();
        let rangeclone = selection.getRangeAt(0).cloneRange();

        let targetList = this.RangeTree_Start.parentNode;
        if (this.atContainerStart(targetList, rangeclone)) { //Handle Backspace at ListItem Level
            event.preventDefault();
            event.stopPropagation();
            if (this.StartBlock.firstChild === targetList) {
                let previousBlock = this.GetPreviousBlock(this.StartBlock);
                if (previousBlock !== null) {
                    rangeclone = this.BackSpace_BlockBehavior(previousBlock);
                }
            }
            else {

                if (targetList.previousSibling.textContent.length === 0) {
                    targetList.previousSibling.remove();
                }
                else if (targetList.textContent.length === 0) {
                    let last = this.GetLastNode(targetList.previousSibling);
                    if (last.nodeType === Node.TEXT_NODE) {
                        rangeclone.setStart(last, last.textContent.length);
                        rangeclone.setEnd(last, last.textContent.length);
                    }
                    else {
                        rangeclone.setStart(last.parentNode, 0);
                        rangeclone.setEnd(last.parentNode, 0);
                    }
                    targetList.remove();
                }
                else {
                    let tempclone = document.createRange();
                    tempclone.selectNodeContents(targetList.previousSibling);
                    targetList.prepend(tempclone.extractContents());
                    tempclone.detach();
                    let offset = this.Core_MergeIdenticalTree_Previous(this.RangeTree_Start);
                    targetList.previousSibling.remove();
                    rangeclone.setStart(this.GetTreeNode(this.RangeTree_Start), offset);
                    rangeclone.setEnd(this.GetTreeNode(this.RangeTree_Start), offset);
                }


            }
            selection.removeAllRanges();
            selection.addRange(rangeclone);
        }
        return rangeclone;
    }

    Block_List_Behavior_DeleteKey(event) {
        let selection = document.getSelection();
        let rangeclone = selection.getRangeAt(0).cloneRange();
        let targetList = this.RangeTree_Start.parentNode;
        if (this.atContainerEnd(targetList, rangeclone)) {
            event.preventDefault();
            event.stopPropagation();
            if (this.StartBlock.lastChild === targetList) {
                let nextBlock = this.GetNextBlock(this.StartBlock);
                if (nextBlock !== null) {
                    rangeclone = this.Delete_BlockBehavior(nextBlock);
                }
            }
            else {
                if (targetList.nextSibling.textContent.length === 0) {
                    targetList.nextSibling.remove();
                }
                else if (targetList.textContent.length === 0) {
                    let first = this.GetFirstNode(targetList.nextSibling);
                    if (first.nodeType === Node.TEXT_NODE) {
                        rangeclone.setStart(first, 0);
                        rangeclone.setEnd(first, 0);
                    }
                    else {
                        rangeclone.setStart(first.parentNode, 0);
                        rangeclone.setEnd(first.parentNode, 0);
                    }
                    targetList.remove();
                }
                else {
                    let tempclone = document.createRange();
                    tempclone.selectNodeContents(targetList.nextSibling);
                    targetList.append(tempclone.extractContents());
                    tempclone.detach();
                    let offset = this.RangeTree_Start.textContent.length;
                    targetList.nextSibling.remove();
                    this.Core_MergeIdenticalTrees(this.RangeTree_Start);
                    rangeclone.setStart(this.GetTreeNode(this.RangeTree_Start), offset);
                    rangeclone.setEnd(this.GetTreeNode(this.RangeTree_Start), offset);
                }
            }
            selection.removeAllRanges();
            selection.addRange(rangeclone);
        }
        return rangeclone;
    }

    /* List Traversal - Has an Extra Step to traverse to the next list item before traversing to the next Block */
    Block_List_Traversal_SubRoutine(block, LINode) {
        let nextlistitem = LINode.nextSibling;
        let tree = null;
        if (nextlistitem === null) {
            block = block.nextSibling;
            tree = block.firstChild;
            while (tree.tagName !== ConstTree) {
                tree = treeiter.firstChild;
            }
        }
        else {
            tree = nextlistitem.firstChild;
        }
        return { block, tree };
    }
}


//This is a class to complete a full clone of a range and its container
class CloneWithRange {
    constructor(OriginalContainer, OriginalRange) {
        //console.log(OriginalContainer);
        this.range = OriginalRange.cloneRange();
        this.fragment = document.createDocumentFragment();
        this.StartNode = OriginalContainer.startContainer;
        this.StartOffset = OriginalContainer.startOffset;
        this.EndNode = OriginalContainer.endContainer;
        this.EndOffset = OriginalContainer.endOffset;

        let SearchForStart = true;
        Array.from(OriginalContainer.childNodes).forEach((child) => {
            let clonedChild = child.cloneNode(true);
            this.fragment.appendChild(clonedChild);
            //console.log(child);
            //Traverse the OriginalContainer to find map the cloned start and end to the fragment

            //A Forward Traversal means the Start Container is found first
            //A bool is used to reduce unnecessary iterations
            if (SearchForStart) {
                if (child === OriginalRange.startContainer) {
                    //console.log(`Tree Mystery - From CloneWithRange Contstructor, ${child}, ${OriginalContainer}`);
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

                    for (let i = 0; i < iter.childNodes.length; i++) {
                        let temp = iter.childNodes[i];

                        if (temp === OriginalRange.endContainer) {

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
        if (this.StartNode === OriginalRange.startContainer) {
            console.log('No Start Found');
            let startInit = this.fragment.firstChild;
            while (startInit.nodeType !== Node.TEXT_NODE) {
                startInit = startInit.firstChild;
            }
            this.range.setStart(startInit, 0);
        }
        if (this.range.endContainer === OriginalRange.endContainer) {
            console.log('No End Found');
            let endInit = this.fragment.lastChild;
            while (endInit.lastChild !== null) {
                endInit = endInit.lastChild;
            }
            if (endInit.nodeType === Node.TEXT_NODE) {
                this.range.setEnd(endInit, endInit.textContent.length);
            }
            else {
                this.range.setEnd(endInit.parentNode, 0);
            }

        }

        this.StartNode = this.range.startContainer;
        this.StartOffset = this.range.startOffset;
        this.EndNode = this.range.endContainer;
        this.EndOffset = this.range.endOffset;

    }

    Destroy() {
        this.range.detache();
    }

    PrintFragment() {
        console.log(this.fragment.childNodes);
    }


    isElementEmpty(childNode) {
        let isEmpty = false;

		if(childNode.textContent === ""){
			isEmpty = true;
		}
		else {
			while (childNode.lastChild !== null) {
            childNode = childNode.lastChild;
			}
			if (childNode.nodeType !== Node.TEXT_NODE) {
				if (childNode.tagName !== ConstBR) {
                isEmpty = true;
				}
			}
		}
        return isEmpty;
    }

    GetFront_nonInclusive(ElementTag) {

        let element = document.createElement(ElementTag);
        element.appendChild(this.fragment);
        //Select Unwanted Text and Delete
        this.range.selectNodeContents(element);
        this.range.setStart(this.StartNode, this.StartOffset);
        this.range.deleteContents();

        if (this.isElementEmpty(element.lastChild)) {
            element.removeChild(element.lastChild);
        }
        return element;
    }

    GetFront_Inclusive(ElementTag) {
        let element = document.createElement(ElementTag);
        element.appendChild(this.fragment);

        this.range.selectNodeContents(element);
        this.range.setStart(this.EndNode, this.EndOffset);
        this.range.deleteContents();

        return element;
    }

    GetEnd_nonInclusive(ElementTag) {
        let element = document.createElement(ElementTag);
        element.appendChild(this.fragment);

        this.range.selectNodeContents(element);
        this.range.setEnd(this.EndNode, this.EndOffset);
        this.range.deleteContents();

        if (this.isElementEmpty(element.firstChild)) {
            element.removeChild(element.firstChild);
        }

        return element;
    }

    GetEnd_Inclusive(ElementTag) {
        let element = document.createElement(ElementTag);
        element.appendChild(this.fragment);
        this.PrintFragment();

        this.range.selectNodeContents(element);
        this.range.setEnd(this.StartNode, this.StartOffset);

        this.range.deleteContents();
        return element;
    }

    GetMiddle_TrimInverseSelections(ElementTag) {
        let element = document.createElement(ElementTag);
        element.appendChild(this.fragment);

        //Delete BackSide
        this.range.selectNodeContents(element);
        this.range.setStart(this.EndNode, this.EndOffset);
        this.range.deleteContents();
        //delete FrontInverse
        this.range.selectNodeContents(element);
        this.range.setEnd(this.StartNode, this.StartOffset);
        this.range.deleteContents();
        return element;
    }
}


