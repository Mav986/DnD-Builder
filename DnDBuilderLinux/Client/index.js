/*
 * Define a new Validation error to throw
 */
function ValidationError(message){
    this.name = 'ValidationError';
    this.message = "" || message;
}
ValidationError.prototype = Error.prototype;

/*
 * Load the specified client page
 */
async function loadPage(path){
    const domain = 'http://localhost:5000';
    location.href = domain + path;
}

/*
 * get an XHR object to send a 'type' request to the endpoint
 */
async function getHttpRequestObject(type, endpoint, async=true){
    let base_url = 'http://localhost:5000/';
    let req = new XMLHttpRequest();
    req.open(type, base_url + endpoint, async);

    return req;
}

/*
 * Send a GET request to the server
 */
async function getReq(path){
    let req = await getHttpRequestObject('GET', path);
    
    return new Promise((resolve, reject) => {
       req.onload = () => {
           if(req.readyState === XMLHttpRequest.DONE) {
               let response = JSON.parse(req.responseText);
               if (req.status > 199 && req.status < 300) {
                   resolve(response);
               } else {
                   reject();
                   alert(response["Message"]);
               }
           }
       };
       
       req.send();
    });
}

/*
 * Send a POST request to the server
 */
async function postReq(path, data){
    let req = await getHttpRequestObject('POST', path);
    req.setRequestHeader('Accept', 'application/json');
    req.setRequestHeader('Content-Type', 'application/json');
    
    return new Promise((resolve, reject) => {
        req.onload = () => {
            if (req.readyState === XMLHttpRequest.DONE) {
                let response = JSON.parse(req.responseText);
                if (req.status > 199 && req.status < 300) {
                    resolve(response);
                } else {
                    reject();
                    alert(response["Message"]);
                }
            }
        };
        
        req.send(data);
    });
}

/*
 * Send an PUT request to the server
 */
async function putReq(path, data){
    let req = await getHttpRequestObject('PUT', path);
    req.setRequestHeader('Content-Type', 'application/json');
    
    return new Promise((resolve, reject) => {
        req.onload = () => {
            if (req.readyState === XMLHttpRequest.DONE) {
                let response = JSON.parse(req.responseText);
                if (req.status > 199 && req.status < 300) {
                    resolve(response);
                } else {
                    reject();
                    alert(response["Message"]);
                }
            }
        };
        
        req.send(JSON.stringify(data));
    });
}

/*
 * Send a DELETE request to the server
 */
async function deleteReq(name){
    let req = await getHttpRequestObject('DELETE', 'character/delete/' + name);
    return new Promise((resolve, reject) => {
        req.onload = () => {
            if(req.readyState === XMLHttpRequest.DONE){
                if(req.status > 199 && req.status < 300){
                    resolve();
                }
                else {
                    reject();
                    let response = JSON.parse(req.responseText);
                    alert(response["Message"]);                
                }
            }
        };

        req.send();
    });
}

/*
 * Initialize the create character page
 */
async function initCreateCharacterPage(){
    document.getElementById(await formKey()).reset();

    await loadSelects();
    await getSpellcaster();
}

/*
 * Initialize the edit character page
 */
async function initEditCharacterPage(){
    await initCreateCharacterPage();
    let charSelected = await getFromCache(await selectedCharNameKey());
    document.title = charSelected;

    if(charSelected !== null){
        console.log(charSelected);
        await cacheCharacterDataFor(charSelected);
        let charData = await getFromCache(await charDataKey(charSelected));

        charData = JSON.parse(charData);
        await loadFormWith(charData);
    }
    else{
        await loadPage('/Client/view.html');
    }
}

/*
 * Initialize the view characters page
 */
async function initViewCharacterPage(){
    let listJson = await getReq('character/view/all');

    await populateSelectFromArray(await charListKey(), listJson, createCharacterOption);
    await loadSelected();
}

/*
 * Create a new character with current form details
 */
async function createCharacter(){
    try {
        await validateForm();
        let path = '/character/add';
        let json = await convertFormToJson();
        let response = await postReq(path, JSON.stringify(json));
        alert(response);
        await loadPage('/Client/view.html');
    }
    catch(error){
        alert(error.message);
    }
}

/*
 * Save a character's details
 */
async function updateCharacter(){
    try {
        await validateForm();
        let json = await convertFormToJson();
        let path = 'character/update';

        console.log("Sending update request...");
        let response = await putReq(path, json);
        let name = response['name'];
        await storeInCache(await charDataKey(name), JSON.stringify(response));
        await loadFormWith(response);
        alert("Character successfully updated");
    }
    catch(e){
        if(e instanceof ValidationError){
            alert(e.message);
        }
    }
}

/*
 * deleteReq the currently selected character
 */
async function deleteCharacter(){
    let confirmed = confirm("Are you sure you want to delete this character?");

    if(confirmed){
        let characterName = await getFromCache(await selectedCharNameKey());
        await deleteReq(characterName).then(async () => {
            console.log("attempting to load page");
            await loadPage('/Client/view.html');
            console.log("page loaded...");
        });
    }
}

/*
 * Download XML from server for selected character
 */
async function downloadCharacter(){
    let characterName = await getFromCache(await selectedCharNameKey());
    let path = '/character/xml/' + characterName;
    await loadPage(path);
}

/*
 * Populate a select box with data from json or string array
 */
async function populateSelectFromArray(selectName, data, generateStringCallback){
    try {
        let selectBox = document.getElementById(selectName);

        for (let ii = 0; ii < data.length; ii++) {
            let selectString = await generateStringCallback(data[ii]);
            selectBox.options[ii] = new Option(selectString);
        }
        selectBox.selectedIndex = 0;
    }
    catch (e) {
        console.log(data);
    }
}

/*
 * Set the selected element in a select box to character's class
 */
async function setSelected(element, charData){
    for(let option of element.options) {
        if (option.text.toLowerCase() === charData[element.name].toLowerCase())
            option.selected = true;
    }
}

/*
 * Load race and class select boxes
 */
async function loadSelects(){
    let races = await loadRaceData();
    let classes = await loadClassData();
    let raceKey = await raceListKey();
    let classKey = await classListKey();

    let racePop = populateSelectFromArray(raceKey, races, createRaceOrClassOption);
    let classPop = populateSelectFromArray(classKey, classes, createRaceOrClassOption);

    await Promise.all([racePop, classPop]);
}

/*
 * Update the text indicating remaining ability score points
 */
async function updateScoreRemaining(){
    
    let abilityList = await getScoreList();
    
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
 * get a list of ability scores
 */
async function getScoreList(){

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
 * Determine if the selected class is a spellcaster
 */
async function getSpellcaster(){
    let classType = document.getElementById(await classListKey()).value;
    let caster = await getFromCache(await isCasterKey(classType));
    let isCaster = JSON.parse(caster);
    
    if(isCaster === null){
        isCaster = await getReq('dnd/spellcaster/' + classType);
        await storeInCache(await isCasterKey(classType), JSON.stringify(isCaster));
    }
    
    document.getElementById(await spellInputKey()).value = isCaster ? 'Yes' : 'No';
}

/*
 * Store selected character in cache
 */
async function loadSelected(){
    let charSelect = document.getElementById(await charListKey());
    let characterEntry = charSelect.options[charSelect.selectedIndex].value;
    let characterEntryArray = characterEntry.split(":");
    let characterName = characterEntryArray[0];

    await storeInCache(await selectedCharNameKey(), characterName);
    await cacheCharacterDataFor(characterName);
}

/*
 * Load the race data from cache or api
 */
async function loadRaceData(){
    let races;
    let racesString = await getFromCache(await allRacesKey());
    
    if(racesString === null) {
        races = await getReq('dnd/races');
        await storeInCache(await allRacesKey(), JSON.stringify(races));
    }
    else{
        races = JSON.parse(racesString);
    }
    
    return races;
}

/*
 * Load the class data from cache or api
 */
async function loadClassData(){
    let classes;
    let classesString = await getFromCache(await allClassesKey());
    
    if(classesString === null) {
        classes = await getReq('dnd/classes');
        await storeInCache(await allClassesKey(), JSON.stringify(classes));
    }
    else{
        classes = JSON.parse(classesString);
    }
    
    return classes;
}

/*
 * Validate all form details
 */
async function validateForm() {
    function validateRange(min, max, value){
        return min <= value && value <= max;
    }

    // Name is valid if it has some text
    async function validateName() {
        let name = document.getElementById('nameInput').value;
        if(!name || !name.trim()){
            throw new ValidationError(`Name is required`);
        }
    }

    // Age is valid if it's an integer between 0 and 500
    async function validateAge() {
        let age = document.getElementById('ageInput').value;
        const minAge = 0;
        const maxAge = 500;

        if(!age || !validateRange(minAge, maxAge, age)){
            throw new ValidationError(`Age should be between ${minAge} and ${maxAge}`);
        }
    }

    // Level is valid is it's an integer between 1 and 20
    async function validateLevel() {
        let level = document.getElementById('levelInput').value;
        const minLevel = 1;
        const maxLevel = 20;

        if(!level || !validateRange(minLevel, maxLevel, level)){
            throw new ValidationError(`Level should be between ${minLevel} and ${maxLevel}`);
        }
    }

    // Ability scores are valid if they're integers and add up to 20
    async function validateScores() {
        const required = 20;
        let abilityList = await getScoreList();
        let total = 0;

        for(let score of abilityList){
            total = total + parseInt(score.value);
        }

        if(total !== required){
            throw new ValidationError(`Ability score total must add up to ${required}`);
        }
    }

    let validation = [];
    validation.push(validateName());
    validation.push(validateAge());
    validation.push(validateLevel());
    validation.push(validateScores());

    return Promise.all(validation);
}

/*
 * Modified from https://stackoverflow.com/a/21209563
 * User: jholster
 * 
 * Convert form data to json object
 */
async function convertFormToJson(){
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
 * Load form with data from json
 */
async function loadFormWith(charData){
    let form = document.getElementById(await formKey());
    for(let element of form.elements){
        if(charData[element.name] != null){
            if(element.type === selectBoxType()){
                await setSelected(element, charData);
            }
            else{
                element.value = charData[element.name];
            }
        }
    }
}

/*
 * Create a race/class option text for a select box
 */
async function createRaceOrClassOption(json){
    return json;
}

/*
 * Create a character option text for a select box
 */
async function createCharacterOption(json){
    let name = json['name'] != null ? json['name'] : json;
    let race = json['race'] != null ? json['race'] : json;
    let classType = json['class'] != null ? json['class'] : json;
    let level = json['level'] != null ? json['level'] : json;

    return name + " : " + race + " : " + classType + " : " + level;
}

/*
 * Store and retrieve from cache
 */
async function storeInCache(key, value){
    sessionStorage.setItem(key.trim(), value.trim());
}

async function getFromCache(key){
    return sessionStorage.getItem(key.trim());
}

async function cacheCharacterDataFor(characterName, overwrite=false){
    let charData;

    console.log("overwrite == " + overwrite);
    if(overwrite === false){
        console.log("retrieving data for " + characterName + " from cache...");
        charData = await getFromCache(await charDataKey(characterName));
    }

    if(!charData){
        console.log("retrieving data for " + characterName + " from api...");
        let path = 'character/view/' + characterName;
        await getReq(path).then(async (result) => {
            console.log("storing data for " + characterName + " in the cache...");
            await storeInCache(await charDataKey(characterName), JSON.stringify(result));
        }).catch(error => {
            console.log(error.message);
        });
    }

}

/*
 * Faux constants
 */
async function allRacesKey(){
    return 'allRaces';
}

async function allClassesKey(){
    return 'allClasses';
}

async function formKey(){
    return 'submissionForm';
}

async function selectBoxType(){
    return 'select-one';
}

async function charListKey(){
    return 'charList';
}

async function raceListKey(){
    return 'allRaces';
}

async function classListKey(){
    return 'allClasses';
}

async function selectedCharNameKey(){
    return 'selected';
}

async function charDataKey(characterName){
    return characterName.trim() + 'Data';
}

async function isCasterKey(classType) {
    return classType.trim() + '-caster';
}

async function spellInputKey(){
    return 'spellInput';
}