/*
 * Send a GET request to the server
 */
async function Get(path){
    let req = await GetHttpRequestObject('GET', path);
    return new Promise((resolve, reject) => {
        req.onload = () => {
            if (req.status > 199 && req.status < 300) {
                resolve(req.responseText);
            } else {
                alert('Unable to contact remote server. If the issue persists, contact a server administrator');
                reject();
            }
        };
        req.send();
    });
}

/*
 * Send a POST request to the server
 */
async function Post(path, data){
    let req = await GetHttpRequestObject('POST', path);
    req.setRequestHeader('Accept', 'application/json');
    req.setRequestHeader('Content-Type', 'application/json');
    req.send(data);
}

/*
 * Send an PUT request to the server
 */
async function Put(path, data){
    let req = await GetHttpRequestObject('PUT', path);
    req.setRequestHeader('Content-Type', 'application/json');
    return new Promise((resolve, reject) => {
        req.onload = () => {
            if (req.status > 199 && req.status < 300) {
                resolve(req.responseText);
            } else {
                alert('Unable to contact remote server. If the issue persists, contact a server administrator');
                reject();
            }
        };
        req.send(JSON.stringify(data));
    });
}

/*
 * Send a DELETE request to the server
 */
async function Delete(name){
    let req = await GetHttpRequestObject('DELETE', 'character/delete/' + name);
    return new Promise(() => {
        req.send();
    });
}

/*
 * Get json from cache or server
 */
async function CachedGet(path, storageKey) {
    let data = sessionStorage.getItem(storageKey);
    if(data == null) data = await Get(path, storageKey);
    sessionStorage.setItem(storageKey, data);
    
    return data;
}

/*
 * Load a specified client page
 */
async function LoadPage(path){
    const domain = 'http://localhost:5000';
    location.href = domain + path;
}

/*
 * Populate a select box with data from json or string array
 */
async function PopulateSelectFromArray(selectName, data, generateStringCallback){
    try {
        const retVal = JSON.parse(data);
        let selectBox = document.getElementById(selectName);

        for (let ii = 0; ii < retVal.length; ii++) {
            let selectString = await generateStringCallback(retVal[ii]);
            selectBox.options[ii] = new Option(selectString);
        }
    }
    catch (e) {
        console.log(data);
    }
}

/*
 * Update the text indicating remaining ability score points
 */
async function UpdateAttributesRemaining(){
    
    let abilityList = await GetAbilityScoreList();
    
    let total = 0;
    let remaining = 0;
    const MAX_SCORE = 20;
    
    for(let score of abilityList){
        total = total + parseInt(score.value);
        remaining = MAX_SCORE - total;
        document.getElementById('abilityScoreRem').innerText = remaining.toString();
    }
    
    if(total === 20)
    {
        for(let scoreInp of abilityList){
            scoreInp.max = scoreInp.value;
        }
    }
    else{
        for (let scoreInp of abilityList){
            scoreInp.max = MAX_SCORE;
        } 
    }
}

/*
 * Determine if the selected class is a spellcaster
 */
async function GetSpellcaster(){
    let classType = document.getElementById('classSelect').value;
    let json = await CachedGet('dnd/spellcaster/' + classType, classType + ' is caster?');
    const isCaster = JSON.parse(json);
    document.getElementById('spellInput').value = isCaster ? 'Yes' : 'No';
}

/*
 * Get an XHR object to send a 'type' request to the endpoint
 */
async function GetHttpRequestObject(type, endpoint, async=true){
    let base_url = 'http://localhost:5000/';
    let req = new XMLHttpRequest();
    req.open(type, base_url + endpoint, async);
    
    return req;
}

/*
 * Store the selected character from the 'selectBox' element into session storage
 */
async function StoreSelectedCharacter(){
    let characterOption = document.getElementById('selectBox').value.toString();
    let characterOptionArray = characterOption.split(":");
    let characterName = characterOptionArray[0];
    sessionStorage.setItem('charName', characterName);
}

/*
 * Initialize the create character page
 */
async function InitCreateCharacter(){
    document.getElementById('submissionForm').reset();

    let raceKey = 'raceSelect';
    let raceData = await CachedGet('dnd/races', raceKey);
    PopulateSelectFromArray(raceKey, raceData, CreateRaceOrClassOption);

    let classKey = 'classSelect';
    let classData = await CachedGet('dnd/classes', classKey);
    PopulateSelectFromArray(classKey, classData, CreateRaceOrClassOption);
}

/*
 * Initialize the Edit Character page
 */
async function InitEditCharacter(){
    let characterName = sessionStorage.getItem('charName');
    let path = 'character/view/' + characterName;
    let charData = await CachedGet(path, characterName);
    charData = JSON.parse(charData);
    let form = document.getElementById('submissionForm');
    await InitCreateCharacter();

    for(let element of form.elements){
        if(charData[element.name] != null){
            if(element.type === 'select-one'){
                SetSelected(element, charData);
            }
            else{
                element.value = charData[element.name];
            }
        }
    }
}

/*
 * Initialize the View Characters page
 */
async function InitViewCharacters(){
    let charData = await Get('character/view/all', 'selectBox');
    PopulateSelectFromArray('selectBox', charData, CreateCharacterOption);
    StoreSelectedCharacter();
}

/*
 * Create a new character with current form details
 */
async function CreateCharacter(){
    let path = '/character/add';
    let scoresValid = await ValidateScores();
    if(scoresValid === true){
        let json = await ConvertFormToJson();
        Post(path, JSON.stringify(json));
    }
    else
    {
        alert("Total ability points spent must equal 20");
    }
}

/*
 * Save a character's details
 */
async function SaveCharacter(){
    let json = await ConvertFormToJson();
    let path = 'character/update';
    sessionStorage.setItem(json['name'], JSON.stringify(json));
    await Put(path, json);
    InitEditCharacter();
}

/*
 * Download XML from server for selected character
 */
async function DownloadCharacter(){
    let selectBox = document.getElementById('selectBox');
    let characterName = selectBox.value.toString();
    let path = '/character/xml/' + characterName;
    LoadPage(path);
}

/*
 * Delete the currently selected character
 */
async function DeleteCharacter(){
    let confirmed = confirm("Are you sure you want to delete this character?");

    if(confirmed){
        let selectBox = document.getElementById('selectBox');
        let characterName = selectBox.value.toString();
        await Delete(characterName);
        await GenerateSelectOptions(selectBox);
    }
}

/*
 * Modified from https://stackoverflow.com/a/21209563
 * User: jholster
 * 
 * Convert form data to json object
 */
async function ConvertFormToJson(){
    let form = document.getElementById('submissionForm');
    let json = {};

    for(let element of form.elements){
        if(element.name){
            json[element.name] = element.value;
        }
    }

    return json;
}

/*
 * Set the selected element in a select box to character's class
 */
async function SetSelected(element, charData){
    for(let ii = 0; ii < element.options.length; ii++) {
        if (element.options[ii].text.toLowerCase() === charData[element.name].toLowerCase())
            element.options[ii].selected = true;
    }
}

/*
 * Generate select options from all characters
 * (I don't think this works lol)
 */
async function GenerateSelectOptions(selectBox){
    let charList = await Get('character/view/all', 'charList');
    await PopulateSelectFromArray(selectBox.id, charList, CreateCharacterOption);
    selectBox.selectedIndex = 0;
}

/*
 * Validate that ability scores total 20
 */
async function ValidateScores(){
    let abilityList = await GetAbilityScoreList();
    let total = 0;
    
    for(let score of abilityList){
        total = total + parseInt(score.value);
    }
    
    return total === 20;
}

/*
 * Get a list of ability scores
 */
async function GetAbilityScoreList(){

    let abilityList = [];

    abilityList.push(document.getElementById('conInput'));
    abilityList.push(document.getElementById('dexInput'));
    abilityList.push(document.getElementById('strInput'));
    abilityList.push(document.getElementById('chaInput'));
    abilityList.push(document.getElementById('intInput'));
    abilityList.push(document.getElementById('wisInput'));
    
    return abilityList;
}

/*
 * Create a race/class option text for a select box
 */
async function CreateRaceOrClassOption(json){
    return json;
}

/*
 * Create a character option text for a select box
 */
async function CreateCharacterOption(json){
    let name = json['name'] != null ? json['name'] : json;
    let race = json['race'] != null ? json['race'] : json;
    let classType = json['class'] != null ? json['class'] : json;
    let level = json['level'] != null ? json['level'] : json;

    return name + " : " + race + " : " + classType + " : " + level;
}