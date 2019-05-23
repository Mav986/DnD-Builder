const base_url = "http://localhost:5000/";

function init()
{
    GetRaces();
    GetClasses();
}

function GetRaces()
{
    return new Promise(function(resolve, reject){
        let req = GetHttpRequestObject("GET", "dnd/races");
        req.onload = function() {
            const retVal = JSON.parse(req.responseText);
            let raceSelect = document.getElementById("raceSelect");

            for (let ii = 0; ii < retVal.length; ii++) {

                let raceName = escapeString(retVal[ii]);
                raceSelect.options[ii] = new Option(raceName);
            }
            resolve();
        };
        req.send();
    });
}

function GetClasses()
{
    return new Promise(function(resolve, reject){
        let req = GetHttpRequestObject("GET", "dnd/classes");
        req.onload = function(){
            const retVal = JSON.parse(req.responseText);
            let classSelect = document.getElementById("classSelect");
            
            for(let ii = 0; ii < retVal.length; ii++)
            {
                let className = escapeString(retVal[ii]);
                classSelect.options[ii] = new Option(className);
            }
            resolve();
        };
        req.send();
    })
}

function GetHttpRequestObject(type, endpoint, async=true){
    let req = new XMLHttpRequest();
    req.open(type, base_url + endpoint, async);
    
    return req;
}

/*
 * Code snippet sourced from: "https://stackoverflow.com/questions/12799539/javascript-xss-prevention"
 * User: "Martin Janeček"
 */
function escapeString(stringToEscape){
    return stringToEscape.replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#x27')
        .replace(/\//g, '&#x2F');
}