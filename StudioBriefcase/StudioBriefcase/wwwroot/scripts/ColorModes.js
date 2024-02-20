// Code Writtent By: Tyler Reber on 01/06/2024
// Licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
// Work Done for Video
//StudioBriefcase - ASP.NET Core MVC Web Application Development
//Episode 3 - Color Management System, Real Time Color Updates


const LocalStorage_Mode_Name = 'StudioBriefCase_Color_Mode';
const LocalStorage_Custom_Mode_Name = 'StudioBriefCase_Custom_Color_Mode';

//Handle Local Storage for Color Modes on Page Load.
document.addEventListener("DOMContentLoaded", () => {
    let mode = localStorage.getItem(LocalStorage_Mode_Name);
    let customcolorset = JSON.parse(localStorage.getItem(LocalStorage_Custom_Mode_Name));

    //If a Local Storage Exists, load colors.    
    if (mode != null) {
        let button = document.getElementById('color_mode_toggle');
        if (mode == 'Dark') {
            toggleColorScheme(button);
        }
        //Null is Default settings in _colors.cshtml, (light mode)
        else if (customcolorset != null) {
            button.textContent = "Custom";

            ApplyColors(customcolorset);
            
        }
    }
    //If a custom color set is saved, load custom colors into the color picker.


    if (customcolorset != null) {

       // UpdateColorPickerColors(customcolorset);
        //let colorpicker = document.querySelectorAll('input[type="color"]');
        //for (let i = 0; i < customcolorset.length; i++) {
        //    colorpicker[i].value = customcolorset[i].value;
        //}
    }//else if not saved


})

// Constant Color Mode Data
const lightmodeSet = [
    { variable: '--background', value: '#eeeeee' },
    { variable: '--foreground', value: '#cccccc' },
    { variable: '--text-color1', value: '#000000' },
    { variable: '--text-color2', value: '#000000' },
    { variable: '--shadow1', value: '#548BC0' },
    { variable: '--shadow2', value: '#942E5A' }, 
    { variable: '--offset', value: '#777777' },
    { variable: '--menu-background', value: '#aaaaaa' }
];

const darkmodeSet = [
    { variable: '--background', value: '#1a1e22' },
    { variable: '--foreground', value: '#393939' },
    { variable: '--text-color1', value: '#f6f6f6' },
    { variable: '--text-color2', value: '#ffffff' },
    { variable: '--shadow1', value: '#F7F3DF' },
    { variable: '--shadow2', value: '#F0DEAD' },
    { variable: '--offset', value: '#777777' },
    { variable: '--menu-background', value: '#444444' }
]

function UpdateColorPickerColors(colorSet) {

    let colorpicker = document.querySelectorAll('input[type=color]');
    for (let i = 0; i < colorpicker.length; i++) {
        colorpicker[i].value = colorSet[i].value;
    }

}

//Toggles Color Mode between Light and Dark mode. Will also revert Custom color scheme to Light mode.
function toggleColorScheme(buttonelement) {

    let modestring = buttonelement.textContent;

    if (modestring == "Light") {
        buttonelement.textContent = "Dark";
        ApplyColors(darkmodeSet);
        localStorage.setItem(LocalStorage_Mode_Name, 'Dark');
    }
    else if (modestring == "Dark" || modestring == "Custom") {
        buttonelement.textContent = "Light";

        ApplyColors(lightmodeSet);
        //localStorage.setItem(LocalStorage_Mode_Name, 'Light');
        // Using Default Colors defined in Shared/_colors.cshtml
        localStorage.removeItem(LocalStorage_Mode_Name);
    }

}

//Function for Advanced Color in Settings
//Applies single color values to the cshtml style for real time updating
function ApplyColor(buttonElement) {
    let newColor = buttonElement.value;
    let buttonName = buttonElement.name;
    let styleelement = document.getElementById('colorscheme');
    styleelement.sheet.cssRules[0].style.setProperty(buttonName, newColor);
}

//Applies Colors Updates the cshtml style with new color values and is the reason we can apply the changes without reloading the page.
function ApplyColors(ColorSet) {
    let styleelement = document.getElementById('colorscheme');
    //loop over Constant Color variable names and give respective colorscheme names its values
    for (let pair of ColorSet) {

        styleelement.sheet.cssRules[0].style.setProperty(pair.variable, pair.value);
    }
    UpdateColorPickerColors(ColorSet);
}

// Saves Custom Color Scheme to Local Storage
function SaveCustomColorScheme() {

    let colorpicker = document.querySelectorAll('input[type="color"]');

    let CustomColorSet = Array.from({ length: colorpicker.length });
    for (let i = 0; i < colorpicker.length; i++) {

        let colorObject = { variable: colorpicker[i].name, value: colorpicker[i].value };
        CustomColorSet[i] = colorObject;
    }

    //let CustomColorSet = [
    //    { variable: colorpicker[0].name, value: colorpicker[0].value },
    //    { variable: colorpicker[1].name, value: colorpicker[1].value },
    //    { variable: colorpicker[2].name, value: colorpicker[2].value },
    //    { variable: colorpicker[3].name, value: colorpicker[3].value },
    //    { variable: colorpicker[4].name, value: colorpicker[4].value },
    //    { variable: colorpicker[5].name, value: colorpicker[5].value },
    //    { variable: colorpicker[6].name, value: colorpicker[6].value },
    //    { variable: colorpicker[7].name, value: colorpicker[7].value }
    //];

    //let bg = document.getElementById('color_background');
    //let fg = document.getElementById('color_foreground');
    //let text1 = document.getElementById('color_text-color1');
    //let text2 = document.getElementById('color_text-color2');
    //let shadow1 = document.getElementById('color_shadow1');
    //let shadow2 = document.getElementById('color_shadow2');
    //let offset = document.getElementById('color_offset');
    //let menu_bg = document.getElementById('color_menu_background');
    //let CustomColorSet = [
    //    { variable: bg.name, value: bg.value },
    //    { variable: fg.name, value: fg.value },
    //    { variable: text1.name, value: text1.value },
    //    { variable: text2.value, value: text2.value },
    //    { variable: shadow1.name, value: shadow1.value },
    //    { variable: shadow2.name, value: shadow2.value },
    //    { variable: offset.name, value: offset.value },
    //    { variable: menu_bg.name, value: menu_bg.value }
    //];

    let jsonSetstring = JSON.stringify(CustomColorSet);

    localStorage.setItem(LocalStorage_Custom_Mode_Name, jsonSetstring);
    let maintogglebutton = document.getElementById('color_mode_toggle');

    maintogglebutton.textContent = "Custom";
    localStorage.setItem(LocalStorage_Mode_Name, 'Custom');

    ApplyColors(CustomColorSet);
}
//Clears All Local Storage related to color modes.
// Incase custom colors is too slow.
function ClearCustomColorLocalStorage() {
    let maintogglebutton = document.getElementById('color_mode_toggle');
    maintogglebutton.textContent = "Light";

    localStorage.removeItem(LocalStorage_Custom_Mode_Name);
    localStorage.removeItem(LocalStorage_Mode_Name);

    let colorpicker = document.querySelectorAll('input[type="color"]');
    for (let i = 0; i < colorpicker.length; i++) {
        colorpicker[i].value = lightmodeSet[i].value;

    }

    ApplyColors(lightmodeSet);
}

