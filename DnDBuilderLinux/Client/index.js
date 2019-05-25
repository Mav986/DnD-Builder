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
    location.href = path;
}

/*
 * get an XHR object to send a 'type' request to the endpoint
 */
async function getHttpRequestObject(type, endpoint, async=true){
    let base_url = window.location.protocol + '//' + window.location.host + '/';
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

    await Promise.all([ updateRaceList(), updateClassList() ]);
    await getSpellcaster();
}

/*
 * Initialize the edit character page
 */
async function initEditCharacterPage(){    
    await initCreateCharacterPage();
    
    let characterName = await getFromCache(await selectedKey());
    if(characterName !== null) {
        document.title = 'Editing: ' + characterName;
        await loadCharacterData(characterName);
        await loadForm(await charDataKey(characterName));
    }
    else{
        await loadPage('/Client/view.html');
    }
}

/*
 * Initialize the view characters page
 */
async function initViewCharacterPage(){
    await updateCharacterList();
    await storeSelectedCharacter();
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
 * Load the edit character page
 */
async function editCharacter(){
    let selectedCharacter = await getFromCache(await selectedKey());
    if(selectedCharacter !== null){
        await loadPage('/Client/edit.html')
    }
    else {
        await promptNoSavedCharacter();
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
        let response = await putReq(path, json);
        let name = response['name'];
        let key = await charDataKey(name);
        
        await storeInCache(key, JSON.stringify(response));
        await loadForm(key);
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
        let characterName = await getFromCache(await selectedKey());
        await deleteReq(characterName);
        await removeFromCache(await selectedKey());
        console.log('cache entry cleared..');
        await loadPage('/Client/view.html');
    }
}

/*
 * Download XML from server for selected character
 */
async function downloadCharacter(){
    let characterName = await getFromCache(await selectedKey());
    if(characterName !== null){
        let path = '/character/xml/' + characterName;
        await loadPage(path);
    }
    else{
        await promptNoSavedCharacter();
    }
}

/*
 * Create a character option text for a select box
 */
async function getCharacterSummaries(json){

    let jsonArray = [];
    for(let entry of json) {

        let name = entry['name'] != null ? entry['name'] : entry;
        let race = entry['race'] != null ? entry['race'] : entry;
        let classType = entry['class'] != null ? entry['class'] : entry;
        let level = entry['level'] != null ? entry['level'] : entry;

        jsonArray.push(name + " : " + race + " : " + classType + " : " + level);
    }

    return jsonArray;
}

/*
 * Update the character list from the api
 */
async function updateCharacterList(){
    let listJson = await getReq('character/view/all');

    if(listJson !== null){
        let summaryArray = await getCharacterSummaries(listJson);
        let summaryList = document.getElementById(await charListKey());

        await populateListFromArray(summaryList, summaryArray);
    }
}

/*
 * Load list of races
 */
async function updateRaceList(){
    let raceData = loadRaceData();
    let raceList = document.getElementById(await raceListKey());
    await populateListFromArray(raceList, await raceData);
}

/*
 * Load list of classes
 */
async function updateClassList(){
    let classData = loadClassData();
    let classList = document.getElementById(await classListKey());
    await populateListFromArray(classList, await classData)
}

/*
 * Populate a select list with data from a string array
 */
async function populateListFromArray(list, data){
    try {
        for (let ii = 0; ii < data.length; ii++) {
            list.options[ii] = new Option(data[ii]);
        }
        list.selectedIndex = 0;
    }
    catch (e) {
        console.log(data);
    }
}

/*
 * Set the selected option in a select box based on option text
 */
async function setSelectedOption(select, key){
    for(let option of select.options) {
        if (option.text.toLowerCase() === key.toLowerCase()) {
            option.selected = true;
            return;
        }
    }
}

/*
 * Store selected character in cache
 */
async function storeSelectedCharacter(){
    let charSelect = document.getElementById(await charListKey());
    let characterEntry = charSelect.options[charSelect.selectedIndex].value;
    let characterEntryArray = characterEntry.split(":");
    let characterName = characterEntryArray[0];

    await storeInCache(await selectedKey(), characterName);
    await loadCharacterData(characterName);
}

/*
 * Update the text indicating remaining ability score points
 */
async function updateRemainingScore(){
    
    let abilityList = await getScoreArray();
    
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
async function getScoreArray(){

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
    let isCaster = await getReq('dnd/spellcaster/' + classType);
    
    document.getElementById(await spellInputKey()).value = isCaster ? 'Yes' : 'No';
}

/*
 * Load character data into the cache
 */
async function loadCharacterData(characterName){
    let charData = await getFromCache(await charDataKey(characterName));
    if(!charData){
        let path = 'character/view/' + characterName;
        let json = await getReq(path);
        await storeInCache(await charDataKey(characterName), JSON.stringify(json));
    }

}

/*
 * Load the race data from cache or api
 */
async function loadRaceData(){
    let races;
    let racesString = await getFromCache(await raceListKey());
    
    if(racesString === null) {
        races = await getReq('dnd/races');
        await storeInCache(await raceListKey(), JSON.stringify(races));
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
    let classesString = await getFromCache(await classListKey());
    
    if(classesString === null) {
        classes = await getReq('dnd/classes');
        await storeInCache(await classListKey(), JSON.stringify(classes));
    }
    else{
        classes = JSON.parse(classesString);
    }
    
    return classes;
}

/*
 * Load form with data from json
 */
async function loadForm(key){
    let charData = await getFromCache(key);
    charData = JSON.parse(charData);
    let form = document.getElementById(await formKey());
    for(let element of form.elements){
        if(charData[element.name] != null){
            if(element.type === selectBoxType()){
                await setSelectedOption(element, charData[element.name]);
            }
            else{
                element.value = charData[element.name];
            }
        }
    }
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
        let abilityList = await getScoreArray();
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
 * Add form values to a JSON object
 */
async function convertFormToJson(){
    let form = document.getElementById('submissionForm');
    let json = {};

    for(let element of form.elements){
        if(element.id !== 'hpInput' || element.id !== 'spellInput'){
            json[element.name] = element.value;
        }
    }

    return json;
}

/*
 * A shared error prompt
 */
async function promptNoSavedCharacter(){
    alert("No saved characters. Create a character and try again.");
}

/*
 * Access the cache cache
 */
async function storeInCache(key, value){
    sessionStorage.setItem(key.trim(), value.trim());
}

async function getFromCache(key){
    return sessionStorage.getItem(key.trim());
}

async function removeFromCache(key){
    sessionStorage.removeItem(key)
}

/*
 * Faux constants
 */

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

async function selectedKey(){
    return 'selected';
}

async function charDataKey(characterName){
    return characterName.trim() + 'Data';
}

async function spellInputKey(){
    return 'spellInput';
}